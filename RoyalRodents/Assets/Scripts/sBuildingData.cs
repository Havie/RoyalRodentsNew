using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class sBuildingData {

    public int[] _IDs;
    public int[] _Type;
    public int[] _State;
    public int[] _level;
    public float[] _hp;
    public float[] _hpMax;

    int index = 0;

    public sBuildingData(BuildableObject[] bs)
    {
        _IDs = new int[bs.Length];
        _Type = new int[bs.Length];
        _State = new int[bs.Length];
        _level = new int[bs.Length];
        _hp = new float[bs.Length];
        _hpMax = new float[bs.Length];

        foreach ( BuildableObject b in bs)
        {
            //Also an Option   b.GetHashCode();
            _IDs[index] = b.getID();
            _Type[index] = (int) b.getType();
            _State[index] = (int)b.getState();
            _level[index] = b.getLevel();
            _hp[index] = b.getHP();
            _hpMax[index] = b.getHPMax();
            //Team?

            ++index;
        }
    }



}
