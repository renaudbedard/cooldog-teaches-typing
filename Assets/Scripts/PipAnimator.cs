using UnityEngine;
using System.Collections;
using System;

public class PipAnimator : MonoBehaviour {

	[SerializeField] CameraAnimator m_CameraAnimator;
	RectTransform m_RectTransform;
	Vector2 TargetPivot;
	Vector2 OffscreenX;
	Vector2 OnscreenX;

	// Use this for initialization
	void Start () {
		m_RectTransform = GetComponent<RectTransform>();
		TargetPivot = m_RectTransform.pivot;
		OnscreenX = TargetPivot;
		OffscreenX = TargetPivot;
		OffscreenX.x += 5f;
		m_RectTransform.pivot = OffscreenX;
	}
	
	// Update is called once per frame
	void Update () {
		if (m_CameraAnimator.CurrentViewpoint == 3) {
			TargetPivot = OnscreenX;
		} else {
			TargetPivot = OffscreenX;
		}

		m_RectTransform.pivot = (m_RectTransform.pivot - TargetPivot) * 0.8f;
	}
}
