/***************************************************************************\
Project:      Mancala
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using UnityEngine;
using System.Collections;

namespace Niobium
{
    /**
    * The Slot Select handles the player click. Just make sure every other collider above this GameObject is ignored by raycast
    **/
    public class SlotSelect : MonoBehaviour
    {

        private Slot slot;
        private bool isOver = false;                // Check if its over the object
        private GameManager gameManager;

        void Awake()
        {
            slot = transform.GetComponentInParent<Slot>();
            gameManager = GameManager.instance;
        }

        public void OnMouseUp()
        {
            if (isOver)
            {
                GameManager.instance.OnSlotClick(slot.number);
            }
        }

        // highlights on mouse over
        public void OnMouseEnter()
        {
            isOver = true;

            if (gameManager.state == GameManager.GameState.TURN_START)
            {
                // Enables highlight
                Stone[] stones = transform.parent.GetComponentsInChildren<Stone>();
                bool canSelect = gameManager.CanSelectSlot(slot.number, false);
                foreach (Stone stone in stones)
                {
                    stone.Highlight(true, canSelect);
                }
            }
        }

        public void OnMouseExit()
        {
            isOver = false;
            if (gameManager.state == GameManager.GameState.TURN_START)
            {
                // Disables highlight
                Stone[] stones = transform.parent.GetComponentsInChildren<Stone>();
                foreach (Stone stone in stones)
                {
                    stone.Highlight(false, true);
                }
            }
        }
    }
}