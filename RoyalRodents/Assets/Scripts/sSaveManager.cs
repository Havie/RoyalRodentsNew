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

    List<Employee> _portraits = new List<Employee>();

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
        ResourceManagerScript.Instance.LoadData();

        StartCoroutine(PortaitFix());
    }

    public void GatherPortaits(Employee e)
    {
        if (!_portraits.Contains(e))
            _portraits.Add(e);

        //Debug.Log("Added:" + e.gameObject);
    }
    public void RemovePortraits(Employee e)
    {
        if (_portraits.Contains(e))
            _portraits.Remove(e);

       // Debug.Log("Removed:" + e.gameObject);
    }

    IEnumerator PortaitFix()
    {
        yield return new WaitForSeconds(0.1f);
        foreach (var e in _portraits)
            e.LoadDataFix();
    }
}
