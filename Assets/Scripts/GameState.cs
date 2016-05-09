using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

[Serializable]
public class Lesson
{
	public string Name;
	public string Path;
}

public class GameState : MonoBehaviour {

	public static GameState Instance;

	IntroScene m_Intro;
	IEnumerator IntroRoutine;
	IEnumerator IntroTransition;
	[SerializeField] float TitleFadeTime = 1.8f;
	[SerializeField] CanvasRenderer[] Renderers;
	
	public PipAnimator m_CooldogPip;
	public CameraAnimator m_CamAnimator;

	public LogoAnimator LogoAnimator;

	public UIParts m_UILayer;

	public Lesson[] Lessons;

	public Func<IEnumerator>[] LessonStartCoroutine = new Func<IEnumerator>[]
	{
		LessonOneStart,
		LessonTwoStart,
		LessonThreeStart
	};
	public Func<IEnumerator>[] LessonEndCoroutine = new Func<IEnumerator>[]
	{
		LessonOneEnd,
		LessonTwoEnd,
		LessonThreeEnd
	};

	public int currentLesson;

	public Canvas Blackboard;
	public GameObject Strikeline;

	void Awake() {
		Instance = this;
	}

	public int CurrentState = 0;

	void Restart() {
		CurrentState = 0;
		currentLesson = 0;
	}

	// Use this for initialization
	void Start () {
		m_UILayer.gameObject.SetActive(true);
		updateLesson();
	}

	void updateLesson()
	{
		if (currentLesson >= Lessons.Length)
			currentLesson = Lessons.Length - 1;

		ScreenTyping.Instance.NextLesson = Lessons[currentLesson].Path;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl)) {
			int scene = SceneManager.GetActiveScene().buildIndex;
			SceneManager.LoadScene(scene, LoadSceneMode.Single);
			CurrentState = 0;
		}
		switch(CurrentState) {
		case 0:
			m_UILayer.m_PressStart.SetAlpha((int)Time.time % 2);
			if (Input.anyKeyDown && !Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
				ScreenTyping.Instance.PlayTypingSound(true);
				CurrentState = 1;
			}
			break;
		case 1:
			LogoAnimator.FadeOutSong();

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

	public void EndLesson()
	{
		StartCoroutine(LessonEndCoroutine[currentLesson]());
		currentLesson++;
		updateLesson();
	}

	public void StartLesson()
	{
		StartCoroutine(LessonStartCoroutine[currentLesson]());
	}

	static IEnumerator LessonOneStart()
	{
		var dogBarker = Cooldog.Instance.GetComponent<DogBarker>();

		yield return dogBarker.Play(0f, new[] {
			"typing can be hard, this is an easy lesson."
		});

		yield return dogBarker.Play(0.25f, new[] {
			"just type what's on the screen and try not to hit extra keys",
			"with your paws, i do that all teh time. dont worry."
		});
	}

	static IEnumerator LessonTwoStart()
	{
		var dogBarker = Cooldog.Instance.GetComponent<DogBarker>();

		yield return dogBarker.Play(0f, new[] {
			"another lesson coming right up. this time we focus on the middle homerow of the keyboard."
		});

		yield return dogBarker.Play(0.25f, new[] {
			"try using your back paws to type more at once."
		});
	}

	static IEnumerator LessonThreeStart()
	{
		var dogBarker = Cooldog.Instance.GetComponent<DogBarker>();

		yield return dogBarker.Play(0f, new[] {
			"alright. after that last lesson i had some computer questions to search.",
			"help me go through my notes."
		});
	}

	static IEnumerator LessonOneEnd()
	{
		var dogBarker = Cooldog.Instance.GetComponent<DogBarker>();

		yield return new WaitForSeconds(0.5f);

		Instance.m_CamAnimator.CurrentViewpoint = 2;
		EventSystem.current.SetSelectedGameObject(null);

		yield return new WaitForSeconds(1.0f);

		ScreenTyping.Instance.ShutDown();

		yield return dogBarker.Play(0f, new [] {
			string.Format("that’s not bad but you still goofed {0} times.", ScreenTyping.Instance.lastMistakeCount)
		});

		yield return dogBarker.Play(0.25f, new [] {
			"dont worry youll get better if you do more lessons."
		});
	}

	static IEnumerator LessonTwoEnd()
	{
		var dogBarker = Cooldog.Instance.GetComponent<DogBarker>();

		yield return new WaitForSeconds(0.5f);

		Instance.m_CamAnimator.CurrentViewpoint = 2;
		EventSystem.current.SetSelectedGameObject(null);

		yield return new WaitForSeconds(1.0f);

		ScreenTyping.Instance.ShutDown();

		yield return dogBarker.Play(0f, new[] {
			"woah okay. i left for a bit to research, but it sounded like you did really fast."
		});

		yield return dogBarker.Play(0.25f, new[] {
			"how about you put those typing skills to the test and help me look some stuff up."
		});
	}

	static IEnumerator LessonThreeEnd()
	{
		var dogBarker = Cooldog.Instance.GetComponent<DogBarker>();

		yield return new WaitForSeconds(0.5f);

		Instance.m_CamAnimator.CurrentViewpoint = 2;
		EventSystem.current.SetSelectedGameObject(null);

		yield return new WaitForSeconds(1.0f);

		ScreenTyping.Instance.ShutDown();

		// TODO: GAME END DIALOG

		yield return dogBarker.Play(0f, new[] {
			"here we are. youre now a typing legend just like me."
		});

		yield return dogBarker.Play(0.25f, new[] {
			"i hope you enjoyed your stay and you learned cool facts. ill see you later"
		});	
	}
}
