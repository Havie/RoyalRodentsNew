using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sSaveableObjects : MonoBehaviour
{
    // Singleton
    private static sSaveableObjects _instance;

    [SerializeField]
    PlayerStats _playerStats;
    [SerializeField]
    PlayerMovement _playerMovement;
    [SerializeField]
    BuildingSlotManager _BuildingSlots;


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
        sSaveSystem.SavePlayer(_playerStats, _playerMovement);
        sSaveSystem.SaveBuildings(_BuildingSlots.getBuildings());
        sSaveSystem.SaveRodents(GameManager.Instance.getAllRodents());

    }

    public void Load()
    {
        _playerStats.LoadData();
        _playerMovement.LoadData();
        _BuildingSlots.LoadData();
        GameManager.Instance.LoadData();
    }
}
