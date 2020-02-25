using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManagerScript : MonoBehaviour
{
	//Make a singleton
	private static ResourceManagerScript _instance;

	//create resource variables
	private int _food, _trash, _wood, _metal, _shiny;

	//resource icon sprites
	public Sprite _foodIcon;
	public Sprite _trashIcon;
	public Sprite _woodIcon;
	public Sprite _metalIcon;
	public Sprite _shinyIcon;

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

	//setters and getters for resource variable properties
	public int Food
    {
        get
        {
            return _food;
        }
        set
        {
			_food = value;
        }
    }
    public int Trash
    {
        get
        {
            return _trash;
        }
        set
        {
			_trash = value;
        }
    }

    public int Wood
    {
        get
        {
            return _wood;
        }
        set
        {
			_wood = value;
        }
    }

    public int Metal
    {
        get
        {
            return _metal;
        }
        set
        {
			_metal = value;
        }
    }

    public int Shiny
    {
        get
        {
            return _shiny;
        }
        set
        {
			_shiny = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
		_food = 0;
		_trash = 0;
		_wood = 0;
		_metal = 0;
		_shiny = 0;
    }

	
}
