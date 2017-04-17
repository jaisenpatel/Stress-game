using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour {
	void OnMouseDown() {
		Application.LoadLevel("Scenes/Stress Game");
	}


	// Update is called once per frame
//	void Update() {
//		if(Input.GetMouseButton(0)) {
//			Application.LoadLevel("Scenes/Stress Game");
//		}
//	}
}
