/***************************************************************************\
Project:      Manacala
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using System;
using UnityEngine;

namespace Niobium
{
    /**
    * The Configuration class that handles all game configuration
    **/
    public class ConfigurationManager : UnitySingleton<ConfigurationManager>
    {
        [Tooltip("Offline Configuration")]
        public GameConfiguration offlineConfiguration;
        [Tooltip("Hard AI Configuration")]
        public GameConfiguration aiConfiguration;

        public GameConfiguration GetGameConfigurationByType(GameConfiguration.GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameConfiguration.GameMode.AGAINST_AI:
                    return aiConfiguration;
                case GameConfiguration.GameMode.OFFLINE:
                    return offlineConfiguration;
            }

            return null;
        }
    }
}