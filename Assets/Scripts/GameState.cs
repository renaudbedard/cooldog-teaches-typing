using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameState : MonoBehaviour {

	public static GameState Instance;

	IntroScene m_Intro;
	IEnumerator IntroRoutine;
	IEnumerator IntroTransition;
	[SerializeField] float TitleFadeTime = 1.8f;
	[SerializeField] CanvasRenderer[] Renderers;
	
	public PipAnimator m_CooldogPip;
	public CameraAnimator m_CamAnimator;
	public UIParts m_UILayer;

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
			IntroTransition = StateOneTransition();
			StartCoroutine(IntroTransition);
			CurrentState = 2;
			break;
		case 2:
			// Skip intro on keypress ~
			if (Input.GetKeyDown(KeyCode.Escape)) {
				StopCoroutine(IntroTransition);
				SetIntroRenderersAlpha( 0f );
				if (IntroRoutine != null)
					StopCoroutine( IntroRoutine );
				CurrentState = 3;
			}
			break;
		default:
			if (Input.GetKeyDown("up"))
				m_CamAnimator.CurrentViewpoint = Mathf.Max(m_CamAnimator.CurrentViewpoint - 1, 0);
			if (Input.GetKeyDown("down"))
				m_CamAnimator.CurrentViewpoint = Mathf.Min(m_CamAnimator.CurrentViewpoint + 1, m_CamAnimator.Viewpoints.Length - 1);
			break;
		}
	}

	void SetIntroRenderersAlpha(float alpha) {
		foreach (CanvasRenderer c in Renderers) {
			c.SetAlpha(alpha);
		}
	}

	IEnumerator StateOneTransition() {
		float fadeEndTime = Time.fixedTime + TitleFadeTime;
		while (Time.fixedTime < fadeEndTime) {
			SetIntroRenderersAlpha(Mathf.Clamp01( 1f - (Time.fixedTime - fadeEndTime + TitleFadeTime) / TitleFadeTime ));
			yield return false;
		}

		m_Intro = new IntroScene();
		IntroRoutine =  m_Intro.Play();
		yield return StartCoroutine( IntroRoutine );
		CurrentState = 3;
	}
}
