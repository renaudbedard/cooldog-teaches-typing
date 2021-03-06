﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;


[Serializable]
class RectTransformValues
{
	public string Name;
	public Vector2 OffsetMin;
	public Vector2 OffsetMax;
	public Vector2 AnchorMin;
	public Vector2 AnchorMax;
	public TextAnchor ChildAlignment;
	public float TransitionDuration;
}


public class SpeechPosition : MonoBehaviour {

	[SerializeField] CameraAnimator MainCamera;
	[SerializeField] RectTransformValues[] Viewpoints;
	[SerializeField] HorizontalLayoutGroup m_HorizontalLayoutGroup;
	RectTransform m_RectTransform;
	int CurrentViewpoint = 0;
	float transitionDuration = 1f;

	float sinceTransitionStarted = 0;

	Vector2 originOffsetMax;
	Vector2 originOffsetMin;
	Vector2 originAnchorMax;
	Vector2 originAnchorMin;


	// Use this for initialization
	void Start () {
		MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraAnimator>();
		m_RectTransform = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
		if (CurrentViewpoint != MainCamera.CurrentViewpoint) {
			CurrentViewpoint = MainCamera.CurrentViewpoint;
			transitionDuration = Viewpoints[CurrentViewpoint].TransitionDuration;
			sinceTransitionStarted = 0;

			originOffsetMax = m_RectTransform.offsetMax;
			originOffsetMin = m_RectTransform.offsetMin;
			originAnchorMax = m_RectTransform.anchorMax;
			originAnchorMin = m_RectTransform.anchorMin;

			m_HorizontalLayoutGroup.childAlignment = Viewpoints[CurrentViewpoint].ChildAlignment;
		}

		float step = Mathf.Clamp01(sinceTransitionStarted / transitionDuration);

		if (step < 1) {
			sinceTransitionStarted += Time.deltaTime;
		} else {
			sinceTransitionStarted = transitionDuration;
		}


	
		m_RectTransform.offsetMax = Viewpoints[CurrentViewpoint].OffsetMax;
		m_RectTransform.offsetMin = Viewpoints[CurrentViewpoint].OffsetMin;
		m_RectTransform.anchorMax = Viewpoints[CurrentViewpoint].AnchorMax;
		m_RectTransform.anchorMin = Viewpoints[CurrentViewpoint].AnchorMin;
	}
}
