using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

[Serializable]
public struct AnimatedSprite
{
	public string Frame;
	public float Time;
}

public class Cooldog : MonoBehaviour
{
	bool flipped;
	public bool Flipped
	{
		set
		{
			foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
				sr.flipX = value;
			flipped = value;

			var armPos = Arms.transform.localPosition;
			Arms.transform.localPosition = new Vector3(Mathf.Abs(armPos.x) * (flipped ? 1 : -1), armPos.y, armPos.z);
		}
		get { return flipped; }
	}

	DogPart Body;
	DogPart Face;
	DogPart Headdress;
	DogPart Arms;
	DogPart Eyes;
	DogPart NeckDecoration;
	DogPart Overlay;
	DogPart OtherOverlay;

	[Serializable]
	public struct SpriteMapping
	{
		public AnimatedSprite[] Body;
		public AnimatedSprite[] Face;
		public AnimatedSprite[] Headdress;
		public AnimatedSprite[] Arms;
		public AnimatedSprite[] Eyes;
		public AnimatedSprite[] NeckDecoration;
		public AnimatedSprite[] Overlay;
		public AnimatedSprite[] OtherOverlay;
	}

	public readonly Dictionary<string, SpriteMapping> Costumes = new Dictionary<string, SpriteMapping>
	{
		// Fallback
		{ 
			"Normal", new SpriteMapping
			{
				Body = new [] { new AnimatedSprite { Frame = "Normal" } },
				Face = new[] { new AnimatedSprite { Frame = "Normal" } },
				Headdress = new[] { new AnimatedSprite { Frame = "Ears" } },
				NeckDecoration = new[] { new AnimatedSprite { Frame = "Collar" } },
			} 
		},

		// Keywords
		{ 
			"Batman", new SpriteMapping
			{
				Body = new [] { new AnimatedSprite { Frame = "Batman" } },
				Headdress = new[] { new AnimatedSprite { Frame = "Batman" } },
			} 
		},
		{ 
			"Horoscope", new SpriteMapping
			{
				Headdress = new[] { new AnimatedSprite { Frame = "GipsyHat" } },
				Arms = new [] { new AnimatedSprite { Frame = "CrystalBall" }  },
			} 
		},
		{ 
			"SortingHat", new SpriteMapping
			{
				Headdress = new[] { new AnimatedSprite { Frame = "WizardHat" } },
				NeckDecoration = new[] { new AnimatedSprite { Frame = "Scarf" } },
			} 
		},
		{ 
			"Quiz", new SpriteMapping
			{
				Arms = new[] { new AnimatedSprite { Frame = "Microphone" } },
			} 
		},
		{ 
			"Trivia", new SpriteMapping
			{
				Arms = new[] { new AnimatedSprite { Frame = "Book" } },
				Eyes = new[] { new AnimatedSprite { Frame = "Glasses" } },
			} 
		},
		{ 
			"ELIZA", new SpriteMapping
			{
				Arms = new[] { new AnimatedSprite { Frame = "Pipe" } },
				Eyes = new[] { new AnimatedSprite { Frame = "Glasses" } },
			} 
		},
		{ 
			"Music", new SpriteMapping
			{
				Headdress = new[] { new AnimatedSprite { Frame = "Parappa" } },
			} 
		},
		{ 
			"Picture", new SpriteMapping
			{
				Arms = new[] { new AnimatedSprite { Frame = "Pen" } },
			} 
		},

		// Weather
		{ 
			"Hot", new SpriteMapping
			{
				Face = new[] { new AnimatedSprite { Frame = "TongueOut" } },
				Eyes = new[] { new AnimatedSprite { Frame = "Sunglasses" } },
				Overlay = new[]
				{
					new AnimatedSprite { Frame = "Sweat1", Time = 0.25f },
					new AnimatedSprite { Frame = "Sweat2", Time = 0.25f }
				},
			} 
		},
		{ 
			"Cold", new SpriteMapping
			{
				Headdress = new[] { new AnimatedSprite { Frame = "Toque" } },
				NeckDecoration = new[] { new AnimatedSprite { Frame = "Scarf" } },
			} 
		},
		{ 
			"Rain", new SpriteMapping
			{
				Headdress = new[] { new AnimatedSprite { Frame = "Hoodie" } },
				//TODO : Rain overlay
			} 
		},

		// Time
		{ 
			"Morning", new SpriteMapping
			{
				Arms = new [] { new AnimatedSprite { Frame = "Coffee" }  },
			} 
		},
		{ 
			"Night", new SpriteMapping
			{
				Eyes = new[] { new AnimatedSprite { Frame = "Closed" } },
				Headdress = new[] { new AnimatedSprite { Frame = "Nightcap" } },
			} 
		},

		// State
		{ 
			"Dirty", new SpriteMapping
			{
				Overlay = new[] { new AnimatedSprite { Frame = "Dirt" } },
				OtherOverlay = new[]
				{
					new AnimatedSprite { Frame = "Flies1", Time = 0.25f },
					new AnimatedSprite { Frame = "Flies2", Time = 0.25f }
				},
			} 
		},
		{ 
			"Walk", new SpriteMapping
			{
				NeckDecoration = new[] { new AnimatedSprite { Frame = "Collar" } },
			} 
		},
		{ 
			"Eat", new SpriteMapping
			{
				Face = new[]
				{
					new AnimatedSprite { Frame = "Chewing1", Time = 0.4f },
					new AnimatedSprite { Frame = "Chewing2", Time = 0.275f }
				},
			} 
		},
		{ 
			"Barf", new SpriteMapping
			{
				Eyes = new[] { new AnimatedSprite { Frame = "Buggy" } },
				Face = new[] { new AnimatedSprite { Frame = "TongueOut" } },
			} 
		},
	};

	SpriteMapping CurrentSet;

	public bool Blinking { get; private set; }

	void Start()
	{
		CurrentSet = Costumes["Normal"];

		Body = GameObject.Find("Body").GetComponent<DogPart>();
		Face = GameObject.Find("Face").GetComponent<DogPart>();
		Headdress = GameObject.Find("Headdress").GetComponent<DogPart>();
		Arms = GameObject.Find("Arms").GetComponent<DogPart>();
		Eyes = GameObject.Find("Eyes").GetComponent<DogPart>();
		NeckDecoration = GameObject.Find("NeckDecoration").GetComponent<DogPart>();
		Overlay = GameObject.Find("Overlay").GetComponent<DogPart>();
		OtherOverlay = GameObject.Find("OtherOverlay").GetComponent<DogPart>();

		ApplyCostume();
	}

	public IEnumerator ChangeCostume(string costume) 
	{
		Debug.Log(costume);

		yield return WalkOutOfFrame();

		CurrentSet = Costumes[costume];
		ApplyCostume();

		yield return WalkIntoFrame();
	}

	const float WalkOffset = 17;
	const float WalkSpeed = 20;
	const float BobHeight = 0.75f;
	const float BobSpeed = 3;

	public IEnumerator WalkOutOfFrame()
	{
		float sign = Flipped ? -1 : 1;

		float step = 0;
		while (step < 1)
		{
			var easedStep = Easing.EaseIn(Mathf.Clamp01(step), EasingType.Sine);
			transform.localPosition = new Vector3(easedStep * sign * WalkOffset, Math.Abs(Mathf.Cos(easedStep * BobSpeed * Mathf.PI)) * BobHeight - BobHeight, 0);
			step += Time.deltaTime * WalkSpeed / WalkOffset;
			yield return new WaitForEndOfFrame();
		}
		transform.localPosition = new Vector3(WalkOffset * sign, 0, 0);
	}
	public IEnumerator WalkIntoFrame()
	{
		float sign = Flipped ? -1 : 1;

		float step = 0;
		while (step < 1)
		{
			var easedStep = Easing.EaseOut(Mathf.Clamp01(step), EasingType.Sine);
			transform.localPosition = new Vector3((1 - easedStep) * sign * WalkOffset, Math.Abs(Mathf.Cos(easedStep * BobSpeed * Mathf.PI)) * BobHeight - BobHeight, 0);
			step += Time.deltaTime * WalkSpeed / WalkOffset;
			yield return new WaitForEndOfFrame();
		}
		transform.localPosition = new Vector3(0, 0, 0);
	}

	readonly AnimatedSprite[] BlinkEyes = new[] { new AnimatedSprite { Frame = "Closed" } };
	public IEnumerator Blink()
	{
		if (!hasMouthOpen)
		{
			if (Eyes.CurrentAnimation == null || Eyes.CurrentAnimation[0].Frame == "Buggy")
			{
				Blinking = true;
				Eyes.SetAnimation(BlinkEyes);
				yield return new WaitForSeconds(UnityEngine.Random.Range(0.075f, 0.175f));
				Eyes.SetAnimation(CurrentSet.Eyes ?? Costumes["Normal"].Eyes);
				Blinking = false;
			}
		}
	}

	bool hasMouthOpen;
	readonly AnimatedSprite[] TalkFace = new [] { new AnimatedSprite { Frame = "Talk" } };
	public void OpenMouth()
	{
		if (Face.CurrentAnimation == null || Face.CurrentAnimation[0].Frame == "Normal")
		{
			Face.SetAnimation(TalkFace);
			hasMouthOpen = true;
		}
	}
	public void CloseMouth()
	{
		if (hasMouthOpen)
		{
			Face.SetAnimation(CurrentSet.Face ?? Costumes["Normal"].Face);
			hasMouthOpen = false;
		}
	}

	void ApplyCostume()
	{
		var normal = Costumes["Normal"];

		Body.SetAnimation(CurrentSet.Body ?? normal.Body);
		Face.SetAnimation(CurrentSet.Face ?? normal.Face);
		Headdress.SetAnimation(CurrentSet.Headdress ?? normal.Headdress);
		Arms.SetAnimation(CurrentSet.Arms ?? normal.Arms);
		Eyes.SetAnimation(CurrentSet.Eyes ?? normal.Eyes);
		NeckDecoration.SetAnimation(CurrentSet.NeckDecoration ?? normal.NeckDecoration);
		Overlay.SetAnimation(CurrentSet.Overlay ?? normal.Overlay);
		OtherOverlay.SetAnimation(CurrentSet.OtherOverlay ?? normal.OtherOverlay);
	}

	float toNextBlink;
	void Update()
	{
		toNextBlink -= Time.deltaTime;
		if (toNextBlink <= 0)
		{
			StartCoroutine(Blink());
			toNextBlink = UnityEngine.Random.Range(1, 10);

			// double blink
			if (UnityEngine.Random.value < 0.25)
				toNextBlink = UnityEngine.Random.Range(0.3f, 0.6f);
		}
	}
}
