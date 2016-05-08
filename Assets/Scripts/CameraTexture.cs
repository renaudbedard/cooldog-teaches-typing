using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(RawImage))]
public class CameraTexture : MonoBehaviour {

	[SerializeField] Camera m_RenderCam;
	[SerializeField] RenderTexture m_RenderTexture;
	[SerializeField] Material m_Smalldog;
	RawImage m_UIDog; 

	// Use this for initialization
	void Start () {
		m_UIDog = GetComponent<RawImage>();
		m_RenderTexture = new RenderTexture(1024, 1024, 16);
		m_RenderCam.targetTexture = m_RenderTexture;
		m_Smalldog.mainTexture = m_RenderTexture;
		m_UIDog.texture = m_RenderTexture;
	}
}
