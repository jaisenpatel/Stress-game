/***************************************************************************\
Project:      Mancala
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using UnityEngine;

namespace Niobium
{
    /*
    The Stone class represents a stone GameObject
    */
    public class Stone : MonoBehaviour
    {
        private AudioSource audioSource;
        public GameObject hightlight;
        public MaterialType materialType;

        void Awake()
        {
            audioSource = FindObjectOfType<AudioSource>();
        }

        // Plays a sound according to the material type of both colliders
        void OnCollisionEnter(Collision col)
        {
            // Ignore small collisions
            if (col.relativeVelocity.magnitude < 1)
            {
                return;
            }

            var materialType = MaterialType.WOOD;

            if (col.gameObject.tag == "Stone")
            {
                Stone stone = col.gameObject.GetComponent<Stone>();
                materialType = stone.materialType;
            }

            if (col.gameObject.tag == "Board")
            {
                Board board = col.gameObject.GetComponentInParent<Board>();
                materialType = board.materialType;
            }

            switch (materialType)
            {
                case MaterialType.WOOD:
                    switch (materialType)
                    {
                        case MaterialType.WOOD:
                            audioSource.clip = ResourcesManager.instance.woodWood;
                            break;
                        case MaterialType.STONE:
                            audioSource.clip = ResourcesManager.instance.woodStone;
                            break;
                        case MaterialType.GLASS:
                            audioSource.clip = ResourcesManager.instance.glassWood;
                            break;
                    }
                    break;
                case MaterialType.STONE:
                    switch (materialType)
                    {
                        case MaterialType.WOOD:
                            audioSource.clip = ResourcesManager.instance.woodStone;
                            break;
                        case MaterialType.STONE:
                            audioSource.clip = ResourcesManager.instance.stoneStone;
                            break;
                        case MaterialType.GLASS:
                            audioSource.clip = ResourcesManager.instance.glassStone;
                            break;
                    }
                    break;
                case MaterialType.GLASS:
                    switch (materialType)
                    {
                        case MaterialType.WOOD:
                            audioSource.clip = ResourcesManager.instance.glassWood;
                            break;
                        case MaterialType.STONE:
                            audioSource.clip = ResourcesManager.instance.glassStone;
                            break;
                        case MaterialType.GLASS:
                            audioSource.clip = ResourcesManager.instance.glassGlass;
                            break;
                    }
                    break;
            }
            audioSource.Play();
        }

        // Highlights the Stone to show selection
        public void Highlight(bool show, bool canSelect)
        {
            if (canSelect)
            {
                // Green
                hightlight.GetComponent<Renderer>().material.SetColor("_OutlineColor", new Color(0, 255, 0));
            }
            else
            {
                // Red
                hightlight.GetComponent<Renderer>().material.SetColor("_OutlineColor", new Color(255, 0, 0));
            }
            hightlight.SetActive(show);
        }
    }
}