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

    //local Resource Vars
    public int _gold = 1;
    public int _victoryPoints;

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
            r.LoadData(id, data._team[i], data._Type[i], data._BuildingID[i]);

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Temp way to give the player a TownCenter at start.
        _TownCenter = GameObject.FindGameObjectWithTag("TownCenter").GetComponent<bTownCenter>();
        _TownCenter.StartingBuildComplete();

        _gold = 0;
        _victoryPoints = 0;

        //Get ResourceManagerScript from Instance
        _rm = ResourceManagerScript.Instance;

        //Set up our animators
        _WinAnimator = _WinImg.GetComponent<Animator>();
        _LoseAnimator = _LoseImg.GetComponent<Animator>();
        //Shows the splash screen (TMP till main menu?)
        // _SplashScreen.gameObject.SetActive(true);


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

        //Figure out if on mobile device
       _IsMobileMode= Application.isMobilePlatform;

    }

    // Update is called once per frame
    void Update()
    {
        //Developer Tools to get resources
        if (Input.GetKeyDown(KeyCode.Z))
            _rm.incrementTrash(1);
        if (Input.GetKeyDown(KeyCode.X))
            _rm.incrementFood(1);

        if (!_firstClick)
        {
            if (Input.anyKeyDown)
            {
                _SplashScreen.gameObject.SetActive(false);
                _firstClick = true;
            }
        }
//Wont use this most likely can delete
//         if(Input.GetKeyDown(KeyCode.G))
//             SceneManager.LoadScene(1);
//         if (Input.GetKeyDown(KeyCode.H))
//             SceneManager.LoadScene(0);
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
        //can Lists add duplicates? should we check against this?
        if (_PlayerRodents.Contains(r))
             { Debug.Log("Trying to add a rodent thats already in player List?");return; }

        _PlayerRodents.Add(r);
        _rm.UpdateCurrentPopulation();

        //Keep organized in hierarchy 
        r.gameObject.transform.SetParent(_PlayerRodentDummy);
    }
	public void RemovePlayerRodent(Rodent r)
	{
		if (_PlayerRodents.Contains(r))
			_PlayerRodents.Remove(r);
		_rm.UpdateCurrentPopulation();

		//Keep organized in hierarchy 
		r.gameObject.transform.SetParent(_NeutralRodentDummy);
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
