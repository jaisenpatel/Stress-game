/***************************************************************************\
Project:      Mancala
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
This class represents a Slot that contains the stones
*/
public class Slot : MonoBehaviour {

	[Tooltip("The Slot Number")]
	public int number;
	[Tooltip("Is it a house object?")]
	public bool house;
	[Tooltip("Is Player1 the owner of this slot?")]
	public bool p1Owner;
	[Tooltip("The position of the text that shows the seed count")]
	public Text hudText;
	public GameObject slotSelect;
	
	private int mStones;
	public int stones{
		get{return mStones;}
		set{
			mStones = value;
			if(hudText != null) {
				hudText.text = mStones.ToString();
			}
		}
		
	}			 
}
