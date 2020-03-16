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
    private int _food, _trash, _wood, _metal, _shiny;
    private int _currentPopulation, _currentCapacity;

    //TopPanel UI Resource Bar Text
    public TextMeshProUGUI _TrashText;
    public TextMeshProUGUI _WoodText;
    public TextMeshProUGUI _MetalText;
    public TextMeshProUGUI _ShinyText;
    public TextMeshProUGUI _FoodText;

    public TextMeshProUGUI _PopulationText;

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

    // Start is called before the first frame update
    void Start()
    {
        _currentCapacity = 5;
        UpdateCurrentPopulation();
        _food = 10;
		_trash = 10;
		_wood = 10;
		_metal = 10;
		_shiny = 10;

        UpdateAllResourcesText();
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

    public void UpdatePopulationText()
    {
        if (_PopulationText)
        {
			_PopulationText.text = _currentPopulation.ToString() + "/" + _currentCapacity.ToString();
        }
    }
    public void UpdateAllResourcesText()
    {
        UpdateResourceText(ResourceType.Food);
        UpdateResourceText(ResourceType.Trash);
        UpdateResourceText(ResourceType.Wood);
        UpdateResourceText(ResourceType.Stone);
        UpdateResourceText(ResourceType.Shiny);
        UpdatePopulationText();
    }
}
