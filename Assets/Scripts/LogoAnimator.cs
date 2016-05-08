using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LogoAnimator : MonoBehaviour
{
	public Sprite[] CooldogFrames;
	public Sprite[] TeachesFrames;

	Transform cooldog;
	Transform teachesTyping;

	AudioSource song;

	bool shownCooldog;
	bool shownTeaches;

	bool fadingOut;

	void Awake()
	{
		cooldog = transform.FindChild("Cooldog");
		teachesTyping = transform.FindChild("Teaches Typing");

		song = GetComponent<AudioSource>();

		foreach (var r in GetComponentsInChildren<CanvasRenderer>())
			r.SetAlpha(0);
	}

	public void FadeOutSong()
	{
		fadingOut = true;
	}

	float logoAlpha = 1;
	float lastAngle;
	int lastFrame = 1;

	void Update()
	{
		var beat = (int)(song.time / 0.3f);
		int frame = beat % 2;

		if (!shownCooldog && song.time > 1.3 && !fadingOut)
		{
			cooldog.GetComponent<CanvasRenderer>().SetAlpha(1);
			shownCooldog = true;
		}

		if (!shownTeaches && song.time > 3.7 && !fadingOut)
		{
			teachesTyping.GetComponent<CanvasRenderer>().SetAlpha(1);
			shownTeaches = true;
		}

		if (song.time > 4.8)
		{
			cooldog.GetComponent<Image>().sprite = CooldogFrames[1 - frame];
			teachesTyping.GetComponent<Image>().sprite = TeachesFrames[1 - frame];
		}

		if (song.time >= 6)
		{
			const float Angle = 6;

			float angle = lastAngle;
			if (lastFrame != frame)
			{
				angle = Angle * Mathf.Sqrt(song.volume);
				lastAngle = angle;
				lastFrame = frame;
			}

			cooldog.GetComponent<RectTransform>().localRotation = Quaternion.AngleAxis(frame == 0 ? angle : -angle, Vector3.forward);
			teachesTyping.GetComponent<RectTransform>().localRotation = Quaternion.AngleAxis(frame == 0 ? -angle : angle, Vector3.forward);
		}

		if (fadingOut)
		{
			logoAlpha -= Time.deltaTime * 0.4f;

			song.volume *= 0.975f;
			if (song.volume == 0)
				enabled = false;

			foreach (var cr in GetComponentsInChildren<CanvasRenderer>())
				cr.SetAlpha(Mathf.Clamp01(logoAlpha));
		}
	}
}
