using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSlotManager : MonoBehaviour
{
    //Make a singleton
    private static BuildingSlotManager _instance;

    [SerializeField]
     BuildableObject[] _buildings;
    //make a hash map of all the buildings IDs?
     Dictionary<int, BuildableObject> _hashTable = new Dictionary<int, BuildableObject>();

    private  bool _isStarted;



    //Create Instance of GameManager
    public static BuildingSlotManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new BuildingSlotManager();
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            //if not, set instance to this
            _instance = this;
        }
        //If instance already exists and it's not this:
        else if (_instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }

       // DontDestroyOnLoad(gameObject); // cant do because its not a root obj in scene
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!_isStarted)
            StartCoroutine(LoadDelay());
    }

    public void LoadData()
    {
        if (!_isStarted)
        {
            LoadImmediate();
            LoadData();
        }
        else
        {
            //Debug.Log("BeginLoad");

            sBuildingData data = sSaveSystem.LoadBuildingData();
            int[] IDs = data._IDs;
            for (int i = 0; i < data._IDs.Length; ++i)
            {
                int id = IDs[i];
                BuildableObject b = _hashTable[id];
                // Debug.Log("Loading for:" + b + " with hp=" + data._hp[i]);
                if (b)
                    b.LoadData(data._IDs[i], data._Type[i], data._State[i], data._level[i], data._hp[i], data._hpMax[i]);
                else
                    Debug.LogError("ID:" + id + " was not present in the dictionary"); ;
            }
        }
    }

    public BuildableObject[] getBuildings()
    {
        return _buildings;
    }

    IEnumerator LoadDelay()
    {
        yield return new WaitForSeconds(0.2f);
        LoadImmediate();
       // Debug.Log("EndLoadDelay");
    }
    private void LoadImmediate()
    {
        _buildings = this.transform.GetComponentsInChildren<BuildableObject>();
        foreach (var b in _buildings)
        {
            if(!_hashTable.ContainsKey(b.getID()))
                _hashTable.Add(b.getID(), b);
        }
        _isStarted = true;
    }
    public BuildableObject getBuildingFromID(int id)
    {
        if (!_isStarted)
        {
            LoadImmediate();
        }
        return _hashTable[id];
    }

}
