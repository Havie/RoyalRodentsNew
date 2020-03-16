using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSlotManager : MonoBehaviour
{
    [SerializeField]
    BuildableObject[] _buildings;
    //make a hash map of all the buildings IDs?
    Dictionary<int, BuildableObject> _hashTable = new Dictionary<int, BuildableObject>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadDelay());
    }

    public void LoadData()
    {
        sBuildingData data=sSaveSystem.LoadBuildingData();
        int[] IDs = data._IDs;
        for(int i=0; i<data._IDs.Length; ++i)
        {
            int id= IDs[i];
            BuildableObject b = _hashTable[id];
           // Debug.Log("Loading for:" + b + " with hp=" + data._hp[i]);
            b.LoadData(data._IDs[i], data._Type[i], data._State[i], data._level[i], data._hp[i], data._hpMax[i]);
        }
    }

    public BuildableObject[] getBuildings()
    {
        return _buildings;
    }

    IEnumerator LoadDelay()
    {
        yield return new WaitForSeconds(0.5f);
        _buildings = this.transform.GetComponentsInChildren<BuildableObject>();
        foreach (var b in _buildings)
        {
            _hashTable.Add(b.getID(), b);
        }
    }
}
