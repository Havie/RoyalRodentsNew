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
        _trash = rm.GetResourceCount(ResourceManagerScript.ResourceType.Trash);
        _wood = rm.GetResourceCount(ResourceManagerScript.ResourceType.Wood);
        _stone = rm.GetResourceCount(ResourceManagerScript.ResourceType.Stone);
        _shiny = rm.GetResourceCount(ResourceManagerScript.ResourceType.Shiny);
        _food = rm.GetResourceCount(ResourceManagerScript.ResourceType.Food);

        //To-Do:
        _crowns = 0;
    }
}
