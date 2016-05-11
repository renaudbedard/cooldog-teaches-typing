using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class OutboundLinks : MonoBehaviour {

	string[] twitters = new string[5] {
		"https://twitter.com/cooldog__",
		"https://twitter.com/renaudbedard",
		"https://twitter.com/teamstersub",
		"https://twitter.com/onefifth",
		"https://twitter.com/Q__XO"
	};

	public void ClickLink(int id) {
		#if UNITY_WEBGL
		openWindow(twitters[id]);
		#else
		Application.OpenURL( twitters[id] );
		#endif
	}

	#if UNITY_WEBGL
	[DllImport("__Internal")]
	private static extern void openWindow(string url);
	#endif
}
