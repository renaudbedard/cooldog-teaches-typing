using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

class ScreenTyping : MonoBehaviour
{
	StringBuilder builder = new StringBuilder();
	StringBuilder cleaner = new StringBuilder();

	public Text Reference;

	InputField typed;
	bool ignoreNextEvent;

	void Start()
	{
		typed = GetComponentInChildren<InputField>();

		LoadLesson("lesson1");
	}

	void Update()
	{
		
	}

	public void LoadLesson(string name)
	{
		var lessonText = Resources.Load<TextAsset>(name);
		Reference.text = lessonText.text;
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
		cleaner.Replace("<color>", "");
		cleaner.Replace("</color>", "");
		value = cleaner.ToString();
			
		// clear
		builder.Remove(0, builder.Length);

		var referenceText = Reference.text;

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
