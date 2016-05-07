using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
class Viewpoint
{
	public string Name;
	public Vector3 Position;
	public float Pitch;
	public float TransitionDuration;
}

class Camera : MonoBehaviour
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

		if (Input.GetKeyDown("up"))
			CurrentViewpoint = Mathf.Max(CurrentViewpoint - 1, 0);
		if (Input.GetKeyDown("down"))
			CurrentViewpoint = Mathf.Min(CurrentViewpoint + 1, Viewpoints.Length - 1);
	}
}
