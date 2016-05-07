using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Camera : MonoBehaviour
{
	void Update()
	{
		transform.position += Vector3.up * Input.GetAxis("Vertical") * 0.5f;
	}
}
