/***************************************************************************\
Project:      Mancala
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Niobium
{
    /**
     *  The board class represents the Board GameObject
     **/
    public class Board : MonoBehaviour
    {
        public Font textHudFont;
        public Color textHudColor;
        public int textHudFontSize;
        public MaterialType materialType;

        void Start()
        {
            Slot[] slots = transform.GetComponentsInChildren<Slot>();
            foreach (Slot slot in slots)
            {
                Text textFollow = slot.hudText;
                if (textFollow != null)
                {
                    textFollow.text = "0";
                }
                else
                {
                    Debug.LogWarning("Add a Text object with to the slot if you want to show the hud");
                }
            }
        }

    }
}