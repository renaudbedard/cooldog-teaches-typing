using UnityEngine;
using System.Collections;
using System;

public class PipAnimator : MonoBehaviour {

	[SerializeField] CameraAnimator m_CameraAnimator;
	RectTransform m_RectTransform;
	Vector2 TargetPivot;
	Vector2 OffscreenPivot;
	Vector2 OnscreenPivot;

	// Use this for initialization
	void Start () {
		m_RectTransform = GetComponent<RectTransform>();

		OnscreenPivot = m_RectTransform.pivot;
		OffscreenPivot = m_RectTransform.pivot;
		OffscreenPivot.x -= 22f;

		TargetPivot = OffscreenPivot;
		m_RectTransform.pivot = OffscreenPivot;
	}
	
	// Update is called once per frame
	void Update () {
		if (m_CameraAnimator.CurrentViewpoint == 3) {
			TargetPivot = OnscreenPivot;
		} else {
			TargetPivot = OffscreenPivot;
		}

		m_RectTransform.pivot += (TargetPivot - m_RectTransform.pivot) * 0.2f;
	}
}
