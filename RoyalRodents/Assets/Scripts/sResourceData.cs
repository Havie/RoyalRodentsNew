using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class sResourceData 
{
    int _pop; // MIGHT auto update when Rodents are loaded back into game
    int _popCap; //MIGHT auto update when Buildings are loaded back into game
    int _trash;
    int _wood;
    int _stone;
    int _shiny;
    int _food;
    int _crowns;

    public sResourceData (ResourceManagerScript rm)
    {

    }
}
