using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DogBarker : MonoBehaviour {
	[SerializeField]
	Text dialogueBox;

	[SerializeField]
	GameObject hideAlso;

	[SerializeField]
	AudioClip[] coolBark;

	Cooldog cooldog;

	float speed;
	AudioSource	talkingSpeaker;

	public bool typing = false;
	string targetText = "";
	public bool busy = false;

	public float SinceIdle { get; private set; }

	Queue<string> currentParts = new Queue<string>(); 

	// Use this for initialization
	void Start () {
		dialogueBox.enabled = false;
		hideAlso.SetActive(false);
		busy = false;
		talkingSpeaker = GetComponent<AudioSource>();
		cooldog = GetComponent<Cooldog>();
	}

	public IEnumerator Play(float delay, params string[] parts) {
		var wasBusy = busy;
		busy = true;
		foreach (var p in parts)
			currentParts.Enqueue(p);
		if (!wasBusy)
			yield return StartCoroutine(PlayInternal(delay));
	}

	IEnumerator PlayInternal(float delay = 0f) {
		yield return new WaitForSeconds(delay);
		while (currentParts.Count > 0)
		{
			var part = currentParts.Dequeue();
			targetText = part;
			speed = targetText.Length / 17.0f;

			dialogueBox.text = "";
			dialogueBox.enabled = true;
			hideAlso.SetActive(true);

			bool skip = false;

			foreach (char letter in targetText.ToCharArray()) {
				if (targetText == "") {
					break;
				}
				if (letter == '\b') {
					dialogueBox.text = dialogueBox.text.Remove(dialogueBox.text.Length - 1);
				} else {
					dialogueBox.text += letter;
				}

				if (letter == ' ' || letter == '0') {
					cooldog.CloseMouth();
				}
				if (".aeiou?!1".Contains(letter.ToString())) {
					if (!skip)
						talkingSpeaker.PlayOneShot(coolBark[Random.Range(0, coolBark.Length)]);
					skip = !skip;

					cooldog.OpenMouth();
				}

				yield return new WaitForSeconds(speed / (float)targetText.Length);
			}
			cooldog.CloseMouth();
			yield return new WaitForSeconds(currentParts.Count > 0 ? 0.5f : 1.5f);
		}
		busy = false;
		Hide();
	}

	void Update()
	{
		if (busy)
			SinceIdle = 0;
		else
			SinceIdle += Time.deltaTime;
	}

	public void Hide() {
		dialogueBox.enabled = false;
		hideAlso.SetActive(false);
		dialogueBox.text = "";
		targetText = "";
		typing = false;
		StopCoroutine("PlayInternal");
	}
}
