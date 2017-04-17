/***************************************************************************\
Project:      Mancala
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Niobium
{
    /**
    * The Game Configuration class contains every game aspect for offline, AI and Online rooms
    */
    [Serializable]
    public class GameConfiguration
    {
        [Tooltip("Name of the Room")]
        public string name;

        [Tooltip("Game mode. Don't change those")]
        public GameMode gameMode;

        [Tooltip("Time in seconds between each player turn")]
        public int turnTime;

        [Tooltip("Name of the scene")]
        public string sceneName;

        [Tooltip("Board Prefab")]
        public GameObject boardPrefab;

        [Tooltip("Stones Prefab")]
        public List<GameObject> stonesPrefab;

        public enum GameMode
        {
            OFFLINE,
            AGAINST_AI
        }

        public GameObject GetRandomStone()
        {
            return stonesPrefab[UnityEngine.Random.Range(0, stonesPrefab.Count)];
        }
    }
}