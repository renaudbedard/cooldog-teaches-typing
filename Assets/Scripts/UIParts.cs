using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIParts : MonoBehaviour {

	public RectTransform m_IntroScreen;
	public RectTransform m_UsernameContainer;
	public RectTransform m_CounterContainer;
	public Image m_CounterFill;

	Vector2 TargetCounterContainerPosition;
	Vector2 OffScreenCounterContainerPosition;
	Vector2 OnScreenCounterContainerPosition;

	void Start() {
		OnScreenCounterContainerPosition = m_CounterContainer.anchoredPosition;
		OffScreenCounterContainerPosition = m_CounterContainer.anchoredPosition;
		OffScreenCounterContainerPosition.y += 300f;
		m_CounterContainer.anchoredPosition = OffScreenCounterContainerPosition;
	}

	public IEnumerator ToggleCounter(bool state) {
		TargetCounterContainerPosition = (state) ? OnScreenCounterContainerPosition : OffScreenCounterContainerPosition;
		Vector2 PositionDifference = TargetCounterContainerPosition - m_CounterContainer.anchoredPosition;

		// While we're not yet at our target
		while (Vector2.SqrMagnitude(PositionDifference) > 0.5f) {
			PositionDifference = m_CounterContainer.anchoredPosition - TargetCounterContainerPosition;
			m_CounterContainer.anchoredPosition -= PositionDifference * 0.1f;
			yield return false;
		}
	}
}
