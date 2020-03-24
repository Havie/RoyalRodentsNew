using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class sRodentData
{
    public int[] _IDs;
    public int[] _Type;
    public int[] _team;
    public int[] _BuildingID;
    public float[] _position;

    private int index = 0;

    public sRodentData(List<Rodent> rodents)
    {
        _IDs = new int[rodents.Count];
        _Type = new int[rodents.Count];
        _team = new int[rodents.Count];
        _BuildingID = new int[rodents.Count];
        _position = new float[rodents.Count];

        foreach (Rodent r in rodents)
        {
            //Also an Option   b.GetHashCode();
            _IDs[index] = r.getID();
            _Type[index] = (int)r.GetRodentType(); // get all stats from rodent Type
            _team[index] = r.getTeam();

            _position[index] = r.transform.position.x;
            //Dont think we need a Y if levels are set up right.

            GameObject go= r.getPlaceOfWork();
            if (go != null)
            {
                if (go.GetComponent<BuildableObject>())
                {
                    _BuildingID[index] = go.GetComponent<BuildableObject>().getID();
                }
                else if(go.GetComponent<PlayerMovement>())
                {
                    _BuildingID[index] = -2;
                }
                else
                    _BuildingID[index] = -1;
            }
            else
            {
                _BuildingID[index] = -1;

            }
            //Job? doesn't seem to be used

            ++index;
        }
    }
}

