/***************************************************************************\
Project:      Mancala
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using UnityEngine;

/**
* The Game manager class manages the needed resorces for the gameplay
*/
public class ResourcesManager : UnitySingleton<ResourcesManager>
{
    [Header("Sound Resources")]
    public AudioClip glassWood;                     // Sound when glass collides with wood
    public AudioClip glassStone;                    // Sound when glass collides with stone
    public AudioClip glassGlass;                    // Sound when glass collides with glass
    public AudioClip woodStone;                     // Sound when wood collides with stone
    public AudioClip woodWood;                      // Sound when wood collides with wood
    public AudioClip stoneStone;					// Sound when stone collides with stone
}