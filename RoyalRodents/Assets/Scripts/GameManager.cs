using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //GameManager Instance
    private static GameManager _instance;

    //Other Vars
    public Image _WinImg;
    public Image _LoseImg;
    public Animator _WinAnimator;
    public Animator _LoseAnimator;
    public Button _ButtonQuit;
    public Image _SplashScreen;
    private bool _firstClick;

    //ResourceManagerScript
    public ResourceManagerScript _rm;

    //Rodent Lists
    [SerializeField]
    private List<Rodent> _PlayerRodents = new List<Rodent>();
    private List<Rodent> _AllRodents = new List<Rodent>();
    Dictionary<int, Rodent> _RodentHashTable = new Dictionary<int, Rodent>();
    public Transform _PlayerRodentDummy;
	public Transform _NeutralRodentDummy;
	public Transform _EnemyRodentDummy;

	private bTownCenter _TownCenter;

    private bool _IsMobileMode;

    //Could possibly keep track of all buildings via an array/list?
    private int _buildingIndex=0;
    private int _RodentIndex =0;

    //Create Instance of GameManager
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new GameManager();
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

        DontDestroyOnLoad(gameObject);

        //Set the screen to be correct on mobile
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortraitUpsideDown = false;
    }
    public void LoadData()
    {
        sRodentData data = sSaveSystem.LoadRodentData();
        int[] IDs = data._IDs;
        for (int i = 0; i < data._IDs.Length; ++i)
        {
            int id = IDs[i];
            Rodent r = _RodentHashTable[id];
            r.LoadData(id, data._team[i], data._Type[i], data._BuildingID[i], data._position[i]);

        }
    }

    public void Start()
    {
        //Figure out if were on the Main Menu
        int sceneid= SceneManager.GetActiveScene().buildIndex;
        if (sceneid != 0)
            StartScene();
    }

    public void StartScene()
    {
        //Figure out if on mobile device
        _IsMobileMode = Application.isMobilePlatform;

        //Get ResourceManagerScript from Instance
        _rm = ResourceManagerScript.Instance;


        //Have to Find a New Way Because Loading from mainMenu doesn't have these
        //Set up our animators
        if (_WinAnimator && _LoseAnimator)
        {
            _WinAnimator = _WinImg.GetComponent<Animator>();
            _LoseAnimator = _LoseImg.GetComponent<Animator>();
        }

        // Find any Rodents starting under Players control
        Rodent[] rg = GameObject.FindObjectsOfType<Rodent>();

        foreach (var r in rg)
        {
            if (r.getTeam()==1)
            {
                if(!_PlayerRodents.Contains(r))
                    _PlayerRodents.Add(r);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        //Developer Tools to get resources
        if (Input.GetKeyDown(KeyCode.Z))
            _rm.incrementTrash(1);
        if (Input.GetKeyDown(KeyCode.X))
            _rm.incrementFood(1);
    }
    public void setTownCenter(bTownCenter tc)
    {
        _TownCenter = tc;
        _TownCenter.StartingBuildComplete();
       // StartCoroutine(TownCenterDelay());
    }
    IEnumerator TownCenterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        _TownCenter.StartingBuildComplete();
    }
    public void youWin()
    {
        if (_WinAnimator)
        {
            _WinAnimator.SetTrigger("PlayAnim");
        }
        StartCoroutine(QuitMenu());
    }
    public void youLose()
    {
        if (_LoseAnimator)
        {
            _LoseAnimator.SetTrigger("PlayAnim");
        }

        StartCoroutine(QuitMenu());
    }
    IEnumerator QuitMenu()
    {
        yield return new WaitForSeconds(5);
        if (_ButtonQuit)
            _ButtonQuit.gameObject.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public List<Rodent> getPlayerRodents()
    {
        return _PlayerRodents;
    }
    public List<Rodent> getAllRodents()
    {
        return _AllRodents;
    }
	//Used to update amount of rodents player has
	public int getPlayerRodentsCount()
	{
		return _PlayerRodents.Count;
	}

	public bTownCenter getTownCenter()
	{
		return _TownCenter;
	}
    public int GetBuildingCap()
    {
        if (_TownCenter)
        {
            BuildableObject bo = _TownCenter.transform.GetComponent<BuildableObject>();
            if (bo)
            {
                return (bo.getLevel() * 2);
            }
        }
        Debug.LogWarning("No TownCenter");
        return 0;
    }
    public void addToPlayerRodents(Rodent r)
    {
        if (_PlayerRodents != null)
        {
            //List Should not add Duplicates
            if (_PlayerRodents.Contains(r))
            {
                //Debug.Log("Trying to add a rodent thats already in player List? - happens at start of scene but what ab with Load?"); 
                return;
            }
            else
            {
                _PlayerRodents.Add(r);
                _rm.UpdateCurrentPopulation();

                //Keep organized in hierarchy 
                r.gameObject.transform.SetParent(_PlayerRodentDummy);
            }
        }
    }
	public void RemovePlayerRodent(Rodent r)
	{
        if (_PlayerRodents!=null)
        {
            if (_PlayerRodents.Contains(r))
                _PlayerRodents.Remove(r);
            _rm.UpdateCurrentPopulation();

            //Keep organized in hierarchy 
            r.gameObject.transform.SetParent(_NeutralRodentDummy);
        }
	}
	public void AddtoRodents(Rodent r)
    {
        if (!_AllRodents.Contains(r))
        {
            _AllRodents.Add(r);
            _RodentHashTable.Add(r.getID(), r);
        }
    }
    public bool getMobileMode()
    {
        return _IsMobileMode;
    }
   public int getBuildingIndex()
    {
        return ++_buildingIndex;
    }
    public int getRodentIdex()
    {
        return +_RodentIndex++;
    }
}
