using System;
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
	public Text Reference;

	InputField typed;
	bool ignoreNextEvent;
	int lastSelectionPosition = -1;

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

		LoadLesson("lesson1");
	}

	void Update()
	{
		if (lastSelectionPosition == typed.selectionAnchorPosition)
			return;
		lastSelectionPosition = typed.selectionAnchorPosition;

		typed.caretPosition = typed.text.Length;

		// detect scrolling and move reference text view accordingly

		textGenerator.Populate(typed.text.Substring(0, typed.selectionAnchorPosition), settings);
		var typedLinesCount = textGenerator.lineCount;

		if (typedLinesCount >= 6)
		{
			textGenerator.Populate(referenceText, settings);

			int firstLine = Mathf.Clamp(typedLinesCount - 5, 0, textGenerator.lineCount - 1);
			Reference.text = referenceText.Substring(textGenerator.lines[firstLine].startCharIdx);
		}
		else
			Reference.text = referenceText;
	}

	public void LoadLesson(string name)
	{
		var lessonText = Resources.Load<TextAsset>(name);
		referenceText = Reference.text = lessonText.text;
	}

	public void OnValueChanged(string value)
	{
		if (ignoreNextEvent)
		{
			ignoreNextEvent = false;
			return;
		}

		// clean up value
		if (value.EndsWith("/color", StringComparison.InvariantCulture))
			value = value.Substring(0, value.LastIndexOf("<color", StringComparison.InvariantCulture));

		cleaner.Remove(0, cleaner.Length);
		cleaner.Append(value);
		cleaner.Replace("<color=red>", "");
		cleaner.Replace("</color>", "");
		value = cleaner.ToString();
			
		// clear
		builder.Remove(0, builder.Length);

		bool inBracket = false;

		// per line
		var lines = value.Split('\n');
		var referenceLines = referenceText.Split('\n');

		for (int l = 0; l < lines.Length; l++)
		{
			var line = lines[l];
			var referenceLine = l < referenceLines.Length ? referenceLines[l] : null;
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
					builder.AppendFormat("<color=red>{0}</color>", c);

				position++;
			}

			if (l != lines.Length - 1)
				builder.Append('\n');
		}

		ignoreNextEvent = true;

		//var lastTyped = typed.text;
		typed.text = builder.ToString();

		typed.caretPosition = typed.text.Length;

		//Debug.Log(value.Replace('<', '{').Replace('>', '}') + " => " + typed.text.Replace('<', '{').Replace('>', '}'));
	}
}
