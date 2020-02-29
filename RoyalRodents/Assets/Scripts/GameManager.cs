using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private List<Rodent> _PlayerRodents = new List<Rodent>();
    private List<Rodent> _AllRodents = new List<Rodent>();
    public Transform _PlayerRodentDummy;

	private bTownCenter _TownCenter;

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
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Temp way to give the player a TownCenter at start.
        GameObject.FindGameObjectWithTag("TownCenter").GetComponent<bTownCenter>().StartingBuildComplete();

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
        // Any rodent set up this way should have PlayerRodent Tag in Inspector
        GameObject[] go = GameObject.FindGameObjectsWithTag("PlayerRodent");
        foreach (GameObject g in go)
        {
            if (g.GetComponent<Rodent>())
            {
                _PlayerRodents.Add(g.GetComponent<Rodent>());
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

        if (!_firstClick)
        {
            if (Input.anyKeyDown)
            {
                _SplashScreen.gameObject.SetActive(false);
                _firstClick = true;
            }
        }
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

	public bTownCenter getTownCenter()
	{
		return _TownCenter;
	}
    public void addToPlayerRodents(Rodent r)
    {
        //can Lists add duplicates? should we check against this?
        _PlayerRodents.Add(r);

        //Keep organized in hierarchy 
        r.gameObject.transform.SetParent(_PlayerRodentDummy);
    }
    public void AddtoRodents(Rodent r)
    {
        _AllRodents.Add(r);
    }
}
