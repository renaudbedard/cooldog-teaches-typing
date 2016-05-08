using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

class ScreenTyping : MonoBehaviour
{
	public static ScreenTyping Instance;

	const string DynamicStringMarker = "559";

	TextGenerator textGenerator;
	TextGenerationSettings settings;

	StringBuilder builder = new StringBuilder();
	StringBuilder cleaner = new StringBuilder();

	string referenceText;
	UILineInfo[] referenceTextLineInfo;
	string[] referenceTextLines;

	public Text Reference;
	public Image LoadingScreen;

	InputField typed;
	bool ignoreNextEvent;
	int lastSelectionPosition = -1;
	public int lastMistakeCount;

	bool hasDynamicText;
	int nonWhitespaceReferenceLength;
	string dynamicTextReplacement;

	public AudioClip[] TypingSounds;
	public AudioClip[] AltTypingSounds;

	public AudioClip TeachesTyping;

	public HandMasher Masher;

	int lastUsedSpeaker;
	AudioSource[] speakers;

	public string NextLesson;

	HashSet<string> seenWords = new HashSet<string>();

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		typed = GetComponentInChildren<InputField>();
		textGenerator = new TextGenerator();

		var size = Reference.rectTransform.rect.size;

		settings = new TextGenerationSettings
		{
			textAnchor = Reference.alignment,
			generationExtents = size,
			pivot = Vector2.zero,
			richText = true,
			font = Reference.font,
			fontSize = Reference.fontSize,
			scaleFactor = Reference.canvas.scaleFactor,
			fontStyle = Reference.fontStyle,
			verticalOverflow = VerticalWrapMode.Overflow
		};

		speakers = new AudioSource[10];
		for (int i = 0; i < 10; i++)
		{
			var go = new GameObject("Typing speaker " + i);
			go.transform.parent = transform;
			speakers[i] = go.AddComponent<AudioSource>();
			var filter = go.AddComponent<AudioReverbFilter>();
			filter.reverbLevel = -4000;
		}
	}

	public bool BootedUp;
	public IEnumerator BootUp()
	{
		BootedUp = true;

		yield return new WaitForSeconds(1.0f);

		var c = LoadingScreen.color;

		LoadingScreen.color = new Color(c.r * 0.25f, c.g * 0.25f, c.b, 0.25f);
		yield return new WaitForSeconds(0.25f);
		LoadingScreen.color = new Color(c.r * 0.5f, c.g * 0.5f, c.b, 0.5f);
		yield return new WaitForSeconds(0.25f);
		LoadingScreen.color = new Color(c.r * 0.75f, c.g * 0.75f, c.b, 0.75f);
		yield return new WaitForSeconds(0.25f);
		LoadingScreen.color = new Color(c.r, c.g, c.b, 1.0f);

		speakers[lastUsedSpeaker].PlayOneShot(TeachesTyping);
		lastUsedSpeaker = (lastUsedSpeaker + 1) % speakers.Length;

		yield return new WaitForSeconds(2.0f);

		LoadingScreen.color = new Color(LoadingScreen.color.r, LoadingScreen.color.g, LoadingScreen.color.b, 0.75f);
		yield return new WaitForSeconds(0.25f);
		LoadingScreen.color = new Color(LoadingScreen.color.r, LoadingScreen.color.g, LoadingScreen.color.b, 0.5f);
		yield return new WaitForSeconds(0.25f);
		LoadingScreen.color = new Color(LoadingScreen.color.r, LoadingScreen.color.g, LoadingScreen.color.b, 0.25f);
		yield return new WaitForSeconds(0.25f);
		LoadingScreen.color = new Color(LoadingScreen.color.r, LoadingScreen.color.g, LoadingScreen.color.b, 0f);
		yield return new WaitForSeconds(1.0f);

		GameState.Instance.StartLesson();

		LoadLesson(NextLesson);
		GetComponentInChildren<InputField>().Select();
	}

	public void ShutDown()
	{
		enabled = true;
		ignoreNextEvent = true;

		BootedUp = false;
		referenceText = Reference.text = string.Empty;
		typed.text = string.Empty;

		ignoreNextEvent = false;
	}

	void Update()
	{
		if (lastSelectionPosition == typed.selectionAnchorPosition)
			return;
		lastSelectionPosition = typed.selectionAnchorPosition;

		typed.caretPosition = typed.text.Length;

		// detect scrolling and move reference text view accordingly

		//textGenerator.Populate(typed.text.Substring(0, typed.selectionAnchorPosition), settings);
		textGenerator.Populate(typed.text, settings);

		var typedLinesCount = textGenerator.lineCount;

		var displayedText = referenceText;
		if (hasDynamicText)
			displayedText = displayedText.Replace(DynamicStringMarker, dynamicTextReplacement);

		if (typedLinesCount >= 5)
		{
			int firstLine = Mathf.Clamp(typedLinesCount - 4, 0, referenceTextLineInfo.Length - 1);
			Reference.text = displayedText.Substring(referenceTextLineInfo[firstLine].startCharIdx);
		}
		else
			Reference.text = displayedText;
	}

	public void LoadLesson(string title)
	{
		seenWords.Clear();

		var lessonText = Resources.Load<TextAsset>(title);
		referenceText = Reference.text = lessonText.text;

		hasDynamicText = referenceText.Contains(DynamicStringMarker);
		if (hasDynamicText)
			nonWhitespaceReferenceLength = referenceText.Replace(" ", "").Length;

		textGenerator.Populate(referenceText, settings);
		referenceTextLineInfo = textGenerator.lines.ToArray();

		referenceTextLines = new string[referenceTextLineInfo.Length];
		for (int i = 0; i < referenceTextLineInfo.Length; i++)
		{
			int nextLineStart = i == referenceTextLineInfo.Length - 1
				                    ? referenceText.Length
				                    : referenceTextLineInfo[i + 1].startCharIdx;
			var thisLineStart = referenceTextLineInfo[i].startCharIdx;
			referenceTextLines[i] = referenceText.Substring(thisLineStart, nextLineStart - thisLineStart);
		}
	}

	public void OnValueChanged(string value)
	{
		if (ignoreNextEvent || referenceTextLines == null || !enabled)
			return;

		// clean up value ending (if need be)
		if (value.TrimEnd().EndsWith("/color", StringComparison.InvariantCulture))
			value = value.Substring(0, value.LastIndexOf("<color", StringComparison.InvariantCulture));
		// no tabs!
		value = value.Replace("\t", "");

		// split lines
		textGenerator.Populate(value, settings);
		var typedLineInfo = textGenerator.lines;
		var typedLines = new string[typedLineInfo.Count];
		for (int i = 0; i < typedLines.Length; i++)
		{
			int nextLineStart = i == typedLineInfo.Count - 1
									? value.Length
									: typedLineInfo[i + 1].startCharIdx;
			var thisLineStart = typedLineInfo[i].startCharIdx;
			typedLines[i] = value.Substring(thisLineStart, nextLineStart - thisLineStart).Replace("\n", string.Empty);
		}

		// clear
		builder.Remove(0, builder.Length);

		int mistakeCount = 0;
		int globalPosition = 0;
		char lastChar = '\0';

		bool done = false;

		bool inBracket = false;
		for (int l = 0; l < typedLines.Length; l++)
		{
			var line = typedLines[l];
			var referenceLine = l < referenceTextLines.Length ? referenceTextLines[l] : null;
			int position = 0;

			if (hasDynamicText && referenceLine != null)
				referenceLine = referenceLine.Replace(DynamicStringMarker, dynamicTextReplacement);

			for (int i = 0; i < line.Length; i++)
			{
				var c = line[i];
				if (c == '<')
				{
					inBracket = true;
					continue;
				}
				if (inBracket)
				{
					if (c == '>')
						inBracket = false;
					continue;
				}

				lastChar = c;

				if (referenceLine != null && position < referenceLine.Length && referenceLine[position] == c)
					builder.Append(c);
				else
				{
					mistakeCount++;
					builder.AppendFormat("<color=red>{0}</color>", c);
				}

				position++;
				if (!char.IsWhiteSpace(c))
					globalPosition++;
			}

			if (referenceLine != null && l >= referenceTextLines.Length - 1 && position >= referenceLine.Length)
				done = true;

			if (l != typedLines.Length - 1)
				builder.Append('\n');
			else
			{
				// apppend lf if last character is a space and the above line
				if (referenceLine != null && builder.Length > 0 && char.IsWhiteSpace(builder[builder.Length - 1]) && position >= referenceLine.Length && !Input.GetKey(KeyCode.Backspace))
					builder.Append('\n');
			}
		}

		if (char.IsWhiteSpace(lastChar))
		{
			// check for known costumes
			var trimmed = value.TrimEnd();
			var lastSpace = trimmed.LastIndexOf(' ');
			var lastLF = trimmed.LastIndexOf('\n');
			var lastWord = trimmed.Substring(Math.Max(Math.Max(lastSpace, lastLF), 0)).ToLower().Trim();

			//Debug.Log("Last word : " + lastWord);

			foreach (var key in Cooldog.Instance.Costumes.Keys)
			{
				var lowerKey = key.ToLower();
				if (lowerKey == lastWord && !seenWords.Contains(lastWord))
				{
					seenWords.Add(lastWord);
					//Debug.Log("Changing costume to : " + lastWord);
					StartCoroutine(Cooldog.Instance.ChangeCostume(key));
				}
			}
		}

		if (hasDynamicText)
		{
			var charsLeft = nonWhitespaceReferenceLength - globalPosition;
			dynamicTextReplacement = string.Format("{0:000}", charsLeft);
		}

		ignoreNextEvent = true;
		//var lastTyped = typed.text;
		typed.text = builder.ToString();
		ignoreNextEvent = false;

		typed.caretPosition = typed.text.Length;

		if (mistakeCount > lastMistakeCount) {
			StartCoroutine(Cooldog.Instance.Woah());
			PlayTypingSound(true);
		} else {
			PlayTypingSound(false);
		}

		Masher.Mash(lastChar);

		lastMistakeCount = mistakeCount;

		if (done)
		{
			enabled = false;
			GameState.Instance.EndLesson();
		}

		//Debug.Log(value.Replace('<', '{').Replace('>', '}') + " => " + typed.text.Replace('<', '{').Replace('>', '}'));
	}

	public void PlayTypingSound(bool mistake = false) {
		if (mistake) {
			speakers[lastUsedSpeaker].PlayOneShot(AltTypingSounds[UnityEngine.Random.Range(0, AltTypingSounds.Length - 1)]);
		} else {
			speakers[lastUsedSpeaker].PlayOneShot(TypingSounds[UnityEngine.Random.Range(0, TypingSounds.Length - 1)]);
		}

		lastUsedSpeaker = (lastUsedSpeaker + 1) % speakers.Length;
	}
}
