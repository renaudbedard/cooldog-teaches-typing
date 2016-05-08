using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

class ScreenTyping : MonoBehaviour
{
	TextGenerator textGenerator;
	TextGenerationSettings settings;

	StringBuilder builder = new StringBuilder();
	StringBuilder cleaner = new StringBuilder();

	string referenceText;
	UILineInfo[] referenceTextLineInfo;
	string[] referenceTextLines;

	public Text Reference;

	InputField typed;
	bool ignoreNextEvent;
	int lastSelectionPosition = -1;
	int lastMistakeCount;

	public AudioClip[] TypingSounds;
	public AudioClip[] AltTypingSounds;

	int lastUsedSpeaker;
	AudioSource[] speakers;

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
			filter.reverbLevel = -1000;
		}

		LoadLesson("lesson1");
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

		if (typedLinesCount >= 5)
		{
			int firstLine = Mathf.Clamp(typedLinesCount - 4, 0, referenceTextLineInfo.Length - 1);
			Reference.text = referenceText.Substring(referenceTextLineInfo[firstLine].startCharIdx);
		}
		else
			Reference.text = referenceText;
	}

	public void LoadLesson(string title)
	{
		var lessonText = Resources.Load<TextAsset>(title);
		referenceText = Reference.text = lessonText.text;

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
		if (ignoreNextEvent)
			return;

		// clean up value ending (if need be)
		if (value.TrimEnd().EndsWith("/color", StringComparison.InvariantCulture))
			value = value.Substring(0, value.LastIndexOf("<color", StringComparison.InvariantCulture));

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

		bool inBracket = false;
		for (int l = 0; l < typedLines.Length; l++)
		{
			var line = typedLines[l];
			var referenceLine = l < referenceTextLines.Length ? referenceTextLines[l] : null;
			int position = 0;

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

				if (referenceLine != null && position < referenceLine.Length && referenceLine[position] == c)
					builder.Append(c);
				else
				{
					mistakeCount++;
					builder.AppendFormat("<color=red>{0}</color>", c);
				}

				position++;
			}

			if (l != typedLines.Length - 1)
				builder.Append('\n');
		}

		ignoreNextEvent = true;
		//var lastTyped = typed.text;
		typed.text = builder.ToString();
		ignoreNextEvent = false;

		typed.caretPosition = typed.text.Length;

		if (mistakeCount > lastMistakeCount)
			speakers[lastUsedSpeaker].PlayOneShot(AltTypingSounds[UnityEngine.Random.Range(0, AltTypingSounds.Length - 1)]);
		else
			speakers[lastUsedSpeaker].PlayOneShot(TypingSounds[UnityEngine.Random.Range(0, TypingSounds.Length - 1)]);

		lastUsedSpeaker = (lastUsedSpeaker + 1) % speakers.Length;

		lastMistakeCount = mistakeCount;

		//Debug.Log(value.Replace('<', '{').Replace('>', '}') + " => " + typed.text.Replace('<', '{').Replace('>', '}'));
	}
}
