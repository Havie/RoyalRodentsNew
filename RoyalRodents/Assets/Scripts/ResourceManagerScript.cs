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
   [SerializeField] private int _food, _trash, _wood, _metal, _shiny;
   [SerializeField] private int _currentPopulation, _currentCapacity;

    //TopPanel UI Resource Bar Text
    public TextMeshProUGUI _TrashText;
    public TextMeshProUGUI _WoodText;
    public TextMeshProUGUI _MetalText;
    public TextMeshProUGUI _ShinyText;
    public TextMeshProUGUI _FoodText;

    public TextMeshProUGUI _PopulationText;
    public TextMeshProUGUI _CrownsText;
    private bool _started;

    //Create Instance of GameManager
    public static ResourceManagerScript Instance
    {
        get
        {
            if (_instance == null)
                _instance = new ResourceManagerScript();
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
            _metal = data._stone;
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
        _trash = 10;
        _wood = 10;
        _metal = 10;
        _shiny = 10;

        UpdateAllText();
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

            t = _topPanel.transform.Find("Metal Display");
            if (t)
                _MetalText = t.GetComponent<TextMeshProUGUI>();

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
                    return _metal;
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
                    _metal += amnt;
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
    }

    public void incrementPopulationCapacity(int amnt)
    {
        _currentCapacity += amnt;
        UpdatePopulationText();
       // Debug.Log("Incremented population capacity by " + amnt.ToString());
    }

    //Update Resource Panel UI Text
    private void UpdateResourceText(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Food:
                {
                    if (_FoodText)
                        _FoodText.text = _food.ToString();
                    break;
                }
            case ResourceType.Trash:
                {
                    if (_TrashText)
                        _TrashText.text = _trash.ToString();
                    break;
                }
            case ResourceType.Wood:
                {
                    if (_WoodText)
                        _WoodText.text = _wood.ToString();
                    break;
                }
            case ResourceType.Stone:
                {
                    if (_MetalText)
                        _MetalText.text = _metal.ToString();
                    break;
                }
            case ResourceType.Shiny:
                {
                    if (_ShinyText)
                        _ShinyText.text = _shiny.ToString();
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
    }
    private void UpdatePopulationText()
    {
        if (_PopulationText)
        {
            _PopulationText.text = _currentPopulation.ToString() + "/" + _currentCapacity.ToString();
        }
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
}

