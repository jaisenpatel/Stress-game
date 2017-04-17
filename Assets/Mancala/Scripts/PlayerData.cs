/***************************************************************************\
Project:      Mancala
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using System;
using System.Collections.Generic;

namespace Niobium
{
    /**
    * The Player Data holds all the player parameters
    **/
    [Serializable]
    public class PlayerData
    {
        public GameConfiguration.GameMode gameMode;     // Game configuration serves to pass to the GameManager class
    }
}