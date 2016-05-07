using System;
using System.Collections.Generic;
using UnityEngine;

public class Scratch : MonoBehaviour
{
	Cooldog Cooldog;

	bool draggingWindow;
	Vector2 startPosition;
	Vector2 lastPosition;
	float totalDistance;
	bool lovingIt;

	public bool Scratching { get; private set; }

	public void Start()
	{
		Cooldog = transform.GetComponent<Cooldog>();
	}

	void OnMouseDown()
	{
		lastPosition = startPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		draggingWindow = false;
	}
	void OnMouseUp()
	{
		totalDistance = 0;
		Cooldog.SetScratching(false, false);
		Scratching = false;
		lovingIt = false;
	}

	void OnMouseDrag()
	{
		var thisPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

		totalDistance += (lastPosition - thisPosition).magnitude;
		lastPosition = thisPosition;

		if (totalDistance > 750)
		{
			if (!Scratching)
			{
				Cooldog.SetScratching(true, false);
			}

			if (!lovingIt && totalDistance > 2000)
			{
				lovingIt = true;
				Cooldog.SetScratching(true, true);
			}
			Scratching = true;
		}
	}
}
