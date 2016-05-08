using UnityEngine;
using System.Collections;

public class IntroScene {

	Cooldog m_Cooldog;
	DogBarker m_DogBarker;
	PipAnimator m_CoolPip;
	CameraAnimator m_CameraAnim;
	GameObject m_UILayer;

	public IntroScene() {
		m_Cooldog = Cooldog.Instance;
		m_DogBarker = m_Cooldog.GetComponent<DogBarker>();
		m_CoolPip = GameState.Instance.m_CooldogPip;
		m_CameraAnim = GameState.Instance.m_CamAnimator;
		m_UILayer = GameState.Instance.m_UILayer;
	}

	public IEnumerator Play() {

		m_CoolPip.TogglePip(true);

		yield return m_DogBarker.Play(0f, new string[] {
			"hey, its me cooldog.", 
			"welcome to my typing tutorial game where im going to get you really good at typing", 
			"lets start by getting you to type your name"
		});

		// show name input UI
		yield return new WaitForSeconds(0.2f);

		yield return m_DogBarker.Play(0f, new string[] {
			"wow u do not seem good at typing right now.",
			"ill just call you... student"
		});

		yield return m_DogBarker.Play(0f, new string[] {
			"lets instead check how fast you can keyboard",
			"your typing per second will show up here"
		});

		// Show huge number
		yield return new WaitForSeconds(0.2f);

		yield return m_DogBarker.Play(0f, new string[] {
			"ready.",
			"go."
		});

		// 5 seconds of input.
		yield return new WaitForSeconds(5f);

		yield return m_DogBarker.Play(0f, new string[] {
			"and stop.",
		});

		//hide ui stuff

		yield return m_DogBarker.Play(0f, new string[] {
			"okay " + ((Random.Range(0f,1f) > 0.5f) ? "good" : "bad") + ". you scored " + 5 + " typing.",
			"we’ll start with that and check to see how much better youve gotten later.",
			"lets head over to the main menu\b\b\b\b\b\b\b\b\bmy office"
		});

		m_CoolPip.TogglePip(false);
		m_CameraAnim.CurrentViewpoint = 2;
		yield return new WaitForSeconds(0.3f);

		yield return m_DogBarker.Play(0f, new string[] {
			"this is it. many typing legends grew here including me.\n maybe you will be one of them",
			"lessons can be picked from the board here."
		});

		//highlight chalkboard.

		yield return m_DogBarker.Play(0f, new string[] {
			"if you want to take a typing break, you can check out some of the other stuff",
			"make yourslf at home"
		});
	}
}
