using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaphragmaticBreathing : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D other) {
		Application.LoadLevel ("Diaphragmatic Breathing");
	}
}
