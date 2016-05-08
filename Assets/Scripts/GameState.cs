using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameState : MonoBehaviour {

	public static GameState Instance;

	IntroScene m_Intro;
	[SerializeField] float TitleFadeTime = 2.2f;
	[SerializeField] CanvasRenderer[] Renderers;
	
	public PipAnimator m_CooldogPip;
	public CameraAnimator m_CamAnimator;

	void Awake() {
		Instance = this;
	}

	public int CurrentState = 0;

	void Restart() {
		CurrentState = 0;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		switch(CurrentState) {
		case 0:
			if (Input.anyKeyDown && !Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
				CurrentState = 1;
			}
			break;
		case 1:
			StartCoroutine(StateOneTransition());
			CurrentState = 2;
			break;
		case 2:
			break;
		default:
			if (Input.GetKeyDown("up"))
				m_CamAnimator.CurrentViewpoint = Mathf.Max(m_CamAnimator.CurrentViewpoint - 1, 0);
			if (Input.GetKeyDown("down"))
				m_CamAnimator.CurrentViewpoint = Mathf.Min(m_CamAnimator.CurrentViewpoint + 1, m_CamAnimator.Viewpoints.Length - 1);
			break;
		}
	}

	IEnumerator StateOneTransition() {
		float fadeEndTime = Time.fixedTime + TitleFadeTime;
		float alpha = 1;
		while (Time.fixedTime < fadeEndTime) {
			foreach (CanvasRenderer c in Renderers) {
				alpha = Mathf.Clamp01( 1f - (Time.fixedTime - fadeEndTime + TitleFadeTime) / TitleFadeTime );
				c.SetAlpha(alpha);
			}
			yield return false;
		}

		m_Intro = new IntroScene();
		yield return StartCoroutine( m_Intro.Play() );
		CurrentState = 3;
	}
}
