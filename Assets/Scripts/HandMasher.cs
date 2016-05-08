using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

class HandMasher : MonoBehaviour
{
	public Transform RightHand;
	public Transform LeftHand;

	public Transform RightHandShadow;
	public Transform LeftHandShadow;

	HashSet<Transform> ownedTransforms = new HashSet<Transform>();

	Vector3 LeftOrigin, RightOrigin;
	Vector3 LeftShadowOrigin, RightShadowOrigin;

	void Start()
	{
		LeftOrigin = LeftHand.localPosition;
		RightOrigin = RightHand.localPosition;

		LeftShadowOrigin = LeftHandShadow.localPosition;
		RightShadowOrigin = RightHandShadow.localPosition;
	}

	bool lastWasRight;
	public void Mash(char c)
	{
		var isRight = !lastWasRight;
		if (Random.value < 0.1)
			isRight = !isRight;

		lastWasRight = isRight;

		StartCoroutine(MashHand(c, isRight ? RightHand : LeftHand, 
			isRight ? RightHandShadow : LeftHandShadow, 
			isRight ? RightOrigin : LeftOrigin,
			isRight ? RightShadowOrigin : LeftShadowOrigin));
	}

	IEnumerator MashHand(char c, Transform t, Transform shadowTransform, Vector3 origin, Vector3 shadowOrigin)
	{
		ownedTransforms.Add(t);

		float hash = Mathf.Sin(c) * 43758.5453123f;
		Vector3 lateralDisplacement = (hash - (float) Math.Truncate(hash)) * Vector3.right * 5;

		float time = 0;

		const float ToDuration = 0.05f;
		const float FromDuration = 0.2f;

		while (time < ToDuration)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();

			var step = Mathf.Clamp01(time / ToDuration);
			var easedStep = Easing.EaseIn(step, EasingType.Quadratic);

			t.localPosition = Vector3.Lerp(origin, new Vector3(0, 0.01f, -0.01f) + lateralDisplacement, easedStep);
			shadowTransform.localPosition = Vector3.Lerp(shadowOrigin, Vector3.zero + lateralDisplacement, easedStep);
		}

		t.localPosition = new Vector3(0, 0.01f, -0.01f) + lateralDisplacement;
		shadowTransform.localPosition = Vector3.zero + lateralDisplacement;

		yield return new WaitForFixedUpdate();

		ownedTransforms.Remove(t);

		time = 0;
		bool cancelled = false;
		while (time < FromDuration)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();

			if (ownedTransforms.Contains(t))
			{
				cancelled = true;
				break;
			}

			var step = Mathf.Clamp01(time / FromDuration);
			var easedStep = Easing.EaseOut(step, EasingType.Sine);

			t.localPosition = Vector3.Lerp(new Vector3(0, 0.01f, -0.01f) + lateralDisplacement, origin, easedStep);
			shadowTransform.localPosition = Vector3.Lerp(Vector3.zero + lateralDisplacement, shadowOrigin, easedStep);
		}

		if (!cancelled)
			t.localPosition = origin;
	}
}
