using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceManagerScript : MonoBehaviour
{
	//Make a singleton
	private static ResourceManagerScript _instance;

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
    // Start is called before the first frame update
    void Start()
    {
       // Debug.Log("Started resource Manager");
        _currentCapacity = 5;
        UpdateCurrentPopulation();
        _food = 10;
        _trash = 10;
        _wood = 10;
        _metal = 10;
        _shiny = 10;

        UpdateAllResourcesText();
    }

    //getters for resource variable properties
    public int Food
    {
        get
        {
            return _food;
        }
    }
    public int Trash
    {
        get
        {
            return _trash;
        }
    }

    public int Wood
    {
        get
        {
            return _wood;
        }
    }

    public int Metal
    {
        get
        {
            return _metal;
        }
    }

    public int Shiny
    {
        get
        {
            return _shiny;
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

    //Increment Resources Methods
    public void incrementTrash(int amnt)
    {
        _trash += amnt;
        UpdateTrashText();
    }
    public void incrementWood(int amnt)
    {
        _wood += amnt;
        UpdateWoodText();
    }
    public void incrementMetal(int amnt)
    {
        _metal += amnt;
        UpdateMetalText();
    }
    public void incrementShiny(int amnt)
    {
        _shiny += amnt;
        UpdateShinyText();
    }
    public void incrementFood(int amnt)
    {
        _food += amnt;
        UpdateFoodText();
    }
    public void incrementPopulationCapacity(int amnt)
    {
        _currentCapacity += amnt;
        UpdatePopulationText();
      // Debug.Log("Incremented population capacity by " + amnt.ToString());
    }

    //Update Resource Panel UI Text
    public void UpdateTrashText()
    {
        if (_TrashText)
        {
            _TrashText.text = _trash.ToString();
        }
    }
    public void UpdateWoodText()
    {
        if (_WoodText)
        {
            _WoodText.text = _wood.ToString();
        }
    }
    public void UpdateMetalText()
    {
        if (_MetalText)
        {
            _MetalText.text = _metal.ToString();
        }
    }
    public void UpdateShinyText()
    {
        if (_ShinyText)
        {
            _ShinyText.text = _shiny.ToString();
        }
    }
    public void UpdateFoodText()
    {
        if (_FoodText)
        {
            _FoodText.text = _food.ToString();
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
        UpdateTrashText();
        UpdateWoodText();
        UpdateMetalText();
        UpdateShinyText();
        UpdateFoodText();
        UpdatePopulationText();
    }
}
