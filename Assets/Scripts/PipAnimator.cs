using UnityEngine;
using System.Collections;
using System;

public class PipAnimator : MonoBehaviour {

	[SerializeField] CameraAnimator m_CameraAnimator;
	RectTransform m_RectTransform;
	Vector2 TargetPosition;
	Vector2 OffscreenPosition;
	Vector2 OnscreenPosition;

	Vector3 BigScale;
	Vector3 OriginalScale;

	int LastViewpoint = 0;

	// Use this for initialization
	void Start () {
		m_RectTransform = GetComponent<RectTransform>();

		OriginalScale = m_RectTransform.localScale;
		BigScale = OriginalScale * 2f;

		OnscreenPosition = m_RectTransform.anchoredPosition;
		OffscreenPosition = m_RectTransform.anchoredPosition;
		OffscreenPosition.x -= 200f;

		TargetPosition = OffscreenPosition;
		m_RectTransform.anchoredPosition = OffscreenPosition;

		LastViewpoint = m_CameraAnimator.CurrentViewpoint;
	}

	public void TogglePip(bool state) {
		if (state) {
			TargetPosition = OnscreenPosition;
		} else {
			TargetPosition = OffscreenPosition;
		}
	}

	public void ToggleBigdog(bool state) {
		if (state) {
			m_RectTransform.localScale = BigScale;
		} else {
			m_RectTransform.localScale = OriginalScale;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (m_CameraAnimator.CurrentViewpoint != LastViewpoint) {
			if (m_RectTransform.anchoredPosition.x < OffscreenPosition.x + 1f ) {
				ToggleBigdog(false);
			}
			TogglePip(m_CameraAnimator.CurrentViewpoint == 3);
		}

		m_RectTransform.anchoredPosition += (TargetPosition - m_RectTransform.anchoredPosition) * 0.2f;
	}
}
