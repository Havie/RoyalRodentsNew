using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class sResourceData 
{
    public int _pop; // MIGHT auto update when Rodents are loaded back into game
    public int _popCap; //MIGHT auto update when Buildings are loaded back into game
    public int _trash;
    public int _wood;
    public int _stone;
    public int _shiny;
    public int _food;
    public int _crowns;

    public sResourceData (ResourceManagerScript rm)
    {
        _trash = rm.Trash;
        _wood = rm.Wood;
        _stone = rm.Metal;
        _shiny = rm.Shiny;
        _food = rm.Food;

        //To-Do:
        _crowns = 0;
    }
}
