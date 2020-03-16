using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sSaveManager : MonoBehaviour
{
    // Singleton
    private static sSaveManager _instance;

    [SerializeField]
    PlayerStats _playerStats;
    [SerializeField]
    PlayerMovement _playerMovement;
    [SerializeField]
    BuildingSlotManager _BuildingSlots;

    private bool setUp = false;

    public static sSaveManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<sSaveManager>();
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
            return;
        }
    }


    private void Start()
    {
        this.transform.SetParent(GameManager.Instance.gameObject.transform);
        SetUpObjects();


    }
    private void SetUpObjects()
    {
        if(_playerStats==null)
            _playerStats = GameObject.FindObjectOfType<PlayerStats>();
        if(_playerMovement==null)
            _playerMovement = GameObject.FindObjectOfType<PlayerMovement>();
        if(_BuildingSlots==null)
            _BuildingSlots = GameObject.FindObjectOfType<BuildingSlotManager>();

        setUp = true;
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Save();
        }
        else
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
    }
    public void Save()
    {
        if (!setUp)
            SetUpObjects();
        sSaveSystem.SavePlayer(_playerStats, _playerMovement);
        sSaveSystem.SaveBuildings(_BuildingSlots.getBuildings());
        sSaveSystem.SaveRodents(GameManager.Instance.getAllRodents());
        sSaveSystem.SaveResources(ResourceManagerScript.Instance);

    }

    public void Load()
    {
        if (!setUp)
            SetUpObjects();
        _playerStats.LoadData();
        _playerMovement.LoadData();
        _BuildingSlots.LoadData();
        GameManager.Instance.LoadData();

        //To:Do Implement on RM script once ethan pushes
       sResourceData rd= sSaveSystem.LoadResources();
        Debug.Log("Food=" + rd._food);
    }
}
