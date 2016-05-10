using UnityEngine;
using System.Collections;

public class OutboundLinks : MonoBehaviour {

	string[] twitters = new string[5] {
		"https://twitter.com/cooldog__",
		"https://twitter.com/renaudbedard",
		"https://twitter.com/teamstersub",
		"https://twitter.com/onefifth",
		"https://twitter.com/Q__XO"
	};

	public void ClickLink(int id) {
		Application.OpenURL( twitters[id] );
	}
}
