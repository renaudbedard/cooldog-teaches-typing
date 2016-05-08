using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class Viewpoint
{
	public string Name;
	public Vector3 Position;
	public float Pitch;
	public float TransitionDuration;
}

public class CameraAnimator : MonoBehaviour
{
	Vector3 originPosition;
	Quaternion originRotation;

	float sinceTransitionStarted;
	float transitionDuration;
	int lastViewpoint = -1;

	public Viewpoint[] Viewpoints;
	public Vector3 StartPosition;

	public int CurrentViewpoint;

	void Start()
	{
		transform.position = StartPosition;
		transform.rotation = Quaternion.identity;
	}

	void Update()
	{
		if (lastViewpoint != CurrentViewpoint)
		{
			//var lastDuration = 0.0f;
			//if (lastViewpoint != -1)
			//	lastDuration = Viewpoints[lastViewpoint].TransitionDuration;

			//transitionDuration = Viewpoints[CurrentViewpoint].TransitionDuration + (lastDuration - sinceTransitionStarted);

			if (lastViewpoint == 0 || lastViewpoint == Viewpoints.Length - 1)
				transitionDuration = Viewpoints[lastViewpoint].TransitionDuration;
			else
				transitionDuration = Viewpoints[CurrentViewpoint].TransitionDuration;

			sinceTransitionStarted = 0;
			lastViewpoint = CurrentViewpoint;

			originPosition = transform.position;
			originRotation = transform.rotation;

			if (CurrentViewpoint == Viewpoints.Length - 1)
			{
				if (!ScreenTyping.Instance.BootedUp)
					StartCoroutine(ScreenTyping.Instance.BootUp());
				else
					ScreenTyping.Instance.GetComponentInChildren<InputField>().Select();
			}
			if (CurrentViewpoint == 1)
				EventSystem.current.SetSelectedGameObject(null);
		}

		float step = Mathf.Clamp01(sinceTransitionStarted / transitionDuration);
		var easedStep = Easing.EaseInOut(step, EasingType.Sine);

		var viewpoint = Viewpoints[CurrentViewpoint];
		Vector3 destinationPosition = Vector3.Lerp(originPosition, viewpoint.Position, easedStep);
		Quaternion destinationRotation = Quaternion.Slerp(originRotation, Quaternion.AngleAxis(viewpoint.Pitch, Vector3.right), easedStep);

		if (step < 1)
			sinceTransitionStarted += Time.deltaTime;

		transform.position = Vector3.Lerp(transform.position, destinationPosition, 1 - Mathf.Pow(0.005f, Time.deltaTime));
		transform.rotation = Quaternion.Slerp(transform.rotation, destinationRotation, 1 - Mathf.Pow(0.005f, Time.deltaTime));
	}
}
