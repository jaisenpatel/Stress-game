/***************************************************************************\
Project:      Mancala
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using UnityEngine;
using System.Collections;

namespace Niobium
{
    [ExecuteInEditMode]
    public class MultiResolutionCamera : MonoBehaviour
    {
        public float myDesiredHorizontalFov;

        void Start()
        {
            Camera myCam = Camera.main;
            myCam.fieldOfView = myDesiredHorizontalFov / ((float)myCam.pixelWidth / myCam.pixelHeight);
        }
    }
}