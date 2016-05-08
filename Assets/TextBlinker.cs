using UnityEngine;
using System.Collections;

public class TextBlinker : MonoBehaviour {

	CanvasRenderer m_CanvasRenderer;

	// Use this for initialization
	void Start () {
		m_CanvasRenderer = GetComponent<CanvasRenderer>();
	}

	// Update is called once per frame
	void Update () {
		m_CanvasRenderer.SetAlpha((int)Time.time % 2);
	}
}
