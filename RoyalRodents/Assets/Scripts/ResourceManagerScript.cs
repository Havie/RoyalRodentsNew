using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceManagerScript : MonoBehaviour
{
    //Make a singleton
    private static ResourceManagerScript _instance;

    //Set up ResourceType enum
    public enum ResourceType { Food, Trash, Wood, Stone, Shiny };

    //create resource variables
   [SerializeField] private int _food, _trash, _wood, _stone, _shiny;
   [SerializeField] private int _currentPopulation, _currentCapacity;
    private int _crowns;

    [SerializeField]
    private int _buildingSlots;

    //TopPanel UI Resource Bar Text
    public TextMeshProUGUI _TrashText;
    public TextMeshProUGUI _WoodText;
    public TextMeshProUGUI _StoneText;
    public TextMeshProUGUI _ShinyText;
    public TextMeshProUGUI _FoodText;
    public TextMeshProUGUI _PopulationText;
    public TextMeshProUGUI _CrownsText;
    public TextMeshProUGUI _BuildingsText;

    //VFX
    public GameObject _VFXPrefab;
    private ParticleSystem _VFXResourcePop;
    private bool _started;

    //Create Instance of GameManager
    public static ResourceManagerScript Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<ResourceManagerScript>();
            return _instance;
        }
    }

    public void LoadData()
    {
        FindTexts();
        sResourceData data = sSaveSystem.LoadResources();
        if (data != null)
        {
            _trash = data._trash;
            _wood = data._wood;
            _stone = data._stone;
            _shiny = data._shiny;
            _food = data._food;
            _currentPopulation = data._pop;
            _currentCapacity = data._popCap;
            //To-Do: Crowns
            UpdateAllText();
            UpdatePopulationText();

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
        FindTexts();
        // Debug.Log("Started resource Manager");
        _currentCapacity = 5;
        UpdateCurrentPopulation();
        _food = 10;
        _trash = 6;
        _wood = 5;
        _stone =1;
        _shiny = 1;
        _crowns = 0;

        //LoadVFX();
        UpdateAllText();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            incrementCrownCount(1);
        if (Input.GetKeyDown(KeyCode.F))
            incrementResource(ResourceType.Food, 1);
    }

    private GameObject LoadVFX()
    {
        if (_VFXPrefab == null)
            _VFXPrefab = Resources.Load<GameObject>("UI/vfx_ResourcePop");
        var vfx = GameObject.Instantiate(_VFXPrefab, this.transform.position, this.transform.rotation);

        return vfx;
    }
    private void PlayVFX(TextMeshProUGUI obj)
    {
        if (obj == null)
            return;

        Vector3 loc = obj.transform.position;
        var vfx = LoadVFX();
        vfx.transform.SetParent(obj.transform);
        _VFXResourcePop = vfx.GetComponent<ParticleSystem>();

        if (_VFXResourcePop)
        {
            _VFXResourcePop.gameObject.transform.position = loc;
            _VFXResourcePop.Stop();
            _VFXResourcePop.Play();
            StartCoroutine(destroyVFX(vfx));
        }
    }
    private IEnumerator destroyVFX(GameObject vfx)
    {
        yield return new WaitForSeconds(5);
        Destroy(vfx);
    }
    //Needed to Find the Correct Objects when new Scene is loaded - otherwise everythings Null
    public void FindTexts()
    {
        GameObject _topPanel = GameObject.FindGameObjectWithTag("TopPanel");
        if (_topPanel)
        {
             Transform t=_topPanel.transform.Find("Trash Display");
            if (t)
                _TrashText = t.GetComponent<TextMeshProUGUI>();

            t = _topPanel.transform.Find("Wood Display");
            if (t)
                _WoodText = t.GetComponent<TextMeshProUGUI>();

            t = _topPanel.transform.Find("Stone Display");
            if (t)
                _StoneText = t.GetComponent<TextMeshProUGUI>();

            t = _topPanel.transform.Find("Shiny Display");
            if (t)
                _ShinyText = t.GetComponent<TextMeshProUGUI>();

            t = _topPanel.transform.Find("Food Display");
            if (t)
                _FoodText = t.GetComponent<TextMeshProUGUI>();

            t = _topPanel.transform.Find("Population Display");
            if (t)
                _PopulationText = t.GetComponent<TextMeshProUGUI>();
            t = _topPanel.transform.Find("VictoryPoints");
            if (t)
                _CrownsText = t.GetComponent<TextMeshProUGUI>();
            t = _topPanel.transform.Find("Building Display");
            if (t)
                _BuildingsText = t.GetComponent<TextMeshProUGUI>();
        }

    }
    //getters for resource variable properties
    public int GetResourceCount(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Food:
                {
                    return _food;
                }
            case ResourceType.Trash:
                {
                    return _trash;
                }
            case ResourceType.Wood:
                {
                    return _wood;
                }
            case ResourceType.Stone:
                {
                    return _stone;
                }
            case ResourceType.Shiny:
                {
                    return _shiny;
                }
            default:
                {
                    return 0;
                }
        }
    }

    public void UpdateCurrentPopulation()
    {
        _currentPopulation = GameManager.Instance.getPlayerRodentsCount();
        UpdatePopulationText();
    }

    //Population Getters
    public int getPopulationCapacity()
    {
        return _currentCapacity;
    }
    public int getCurrentPopulation()
    {
        return _currentPopulation;
    }

    //Crown Getters
    public int getCrownCount()
    {
        return _crowns;
    }

    //Increment Resource Method
    public void incrementResource(ResourceType type, int amnt)
    {
        switch (type)
        {
            case ResourceType.Food:
                {
                    _food += amnt;
                    UpdateResourceText(type);
                    break;
                }
            case ResourceType.Trash:
                {
                    _trash += amnt;
                    UpdateResourceText(type);
                    break;
                }
            case ResourceType.Wood:
                {
                    _wood += amnt;
                    UpdateResourceText(type);
                    break;
                }
            case ResourceType.Stone:
                {
                    _stone += amnt;
                    UpdateResourceText(type);
                    break;
                }
            case ResourceType.Shiny:
                {
                    _shiny += amnt;
                    UpdateResourceText(type);
                    break;
                }
            default:
                {
                    Debug.LogError("ResourceType not found when incrementing resource");
                    break;
                }
        }
        if(amnt>0)
            SoundManager.Instance.PlayResource();
    }

    public void incrementPopulationCapacity(int amnt)
    {
        _currentCapacity += amnt;
        UpdatePopulationText();
    }

    public void incrementCrownCount(int amnt)
    {
        _crowns += amnt;
        UpdateCrownText();
        SoundManager.Instance.PlayCrown();
        //Check win condition
        if(_crowns>=2)
        {
            GameManager.Instance.youWin();
        }
    }

    //Update Resource Panel UI Text
    private void UpdateResourceText(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Food:
                {
                    if (_FoodText)
                    {
                        playAnim(_FoodText);
                        PlayVFX(_FoodText); // do we want this ti play if subtracting?
                        _FoodText.text = _food.ToString();
                    }
                    break;
                }
            case ResourceType.Trash:
                {
                    if (_TrashText)
                    {
                        playAnim(_TrashText);
                        PlayVFX(_TrashText);
                        _TrashText.text = _trash.ToString();
                    }
                    break;
                }
            case ResourceType.Wood:
                {
                    if (_WoodText)
                    {
                        playAnim(_WoodText);
                        PlayVFX(_WoodText);
                        _WoodText.text = _wood.ToString();
                    }
                    break;
                }
            case ResourceType.Stone:
                {
                    if (_StoneText)
                    {
                        playAnim(_StoneText);
                        PlayVFX(_StoneText);
                        _StoneText.text = _stone.ToString();
                    }
                    break;
                }
            case ResourceType.Shiny:
                {
                    if (_ShinyText)
                    {
                        playAnim(_ShinyText);
                        PlayVFX(_ShinyText);
                        _ShinyText.text = _shiny.ToString();
                    }
                    break;
                }
            default:
                {
                    Debug.LogError("ResourceType not found when updating resource text");
                    break;
                }
        }
    }
    //Needed to update from GM once scene is loaded or stays #
    public void UpdateAllText()
    {
        UpdateResourceText(ResourceType.Food);
        UpdateResourceText(ResourceType.Shiny);
        UpdateResourceText(ResourceType.Stone);
        UpdateResourceText(ResourceType.Trash);
        UpdateResourceText(ResourceType.Wood);
        UpdatePopulationText();
        UpdateCrownText();
    }
    private void playAnim(TextMeshProUGUI text)
    {
        Animator animator = text.GetComponent<Animator>();
        if (animator)
            animator.SetTrigger("doAnim");
    }
    private void UpdatePopulationText()
    {
        if (_PopulationText)
        {
            _PopulationText.text = _currentPopulation.ToString() + "/" + _currentCapacity.ToString();
        }
    }
    private void UpdateBuildingText()
    {
        if (_BuildingsText)
        {
            _BuildingsText.text = _buildingSlots.ToString() + "/" + GameManager.Instance.GetBuildingCap().ToString();
        }
    }
    private void UpdateCrownText()
    {
        if (_CrownsText)
            _CrownsText.text = _crowns.ToString();
    }

    public static string GetIconPath(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Food:
                {
                    return "ResourceIcons/FoodIcon";
                }
            case ResourceType.Trash:
                {
                    return "ResourceIcons/TrashIcon";
                }
            case ResourceType.Wood:
                {
                    return "ResourceIcons/WoodIcon";
                }
            case ResourceType.Stone:
                {
                    return "ResourceIcons/StoneIcon";
                }
            case ResourceType.Shiny:
                {
                    return "ResourceIcons/ShinyIcon";
                }
            default:
                {
                    return "";
                }
        }
    }

    public void IncrementBuildingSlots(int num)
    {
        _buildingSlots += num;
        UpdateBuildingText();
    }
    public int getNoBuildingSlots()
    {
        return _buildingSlots;
    }
}

