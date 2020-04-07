using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIButtonCosts : MonoBehaviour
{
	//Button Definition
	public string _type;
	public int _level;

	//Current local dictionary
	public Dictionary<ResourceManagerScript.ResourceType, int> _cost;

	//Cost of Upgrade by Resource Vars
	private int costTrash;
	private int costWood;
	private int costMetal;
	private int costShiny;

	//Current Player Resources Local Vars
	private int currentTrash;
	private int currentWood;
	private int currentMetal;
	private int currentShiny;

	//Get reference to Button Text
	public TextMeshProUGUI textTrashCost; //displays costs of button
	public TextMeshProUGUI textWoodCost;
	public TextMeshProUGUI textMetalCost;
	public TextMeshProUGUI textShinyCost;

	public TextMeshProUGUI textTitle; //displays name of button

	//Set Text Colors
	private Color bad = Color.red;
	private Color good = Color.black;

	//Get Resource Manager Instance
	ResourceManagerScript _rm;

    //shinies
    int _royalGuardSlotCost = 1;

	// Start is called before the first frame update
	void Start()
    {
		//text = this.gameObject.transform.GetComponent<TextMeshProUGUI>();
		_rm = ResourceManagerScript.Instance;
		UpdateButton();
	}

     void Update()
    {
        UpdateButton();
    }

	//updates all variables to update the button
	//***Needs to get called elsewhere from some other system such as the game manager when we increment a resource, not in Update, will be figured out later.*
	public void UpdateButton()
	{
		UpdateCurrentResources();
		UpdateCosts();
		UpdateCostsText();
	}

	//set local vars from resource manager
	void UpdateCurrentResources()
	{
		//update local vars from player resources
		currentTrash = _rm.GetResourceCount(ResourceManagerScript.ResourceType.Trash);
		currentWood = _rm.GetResourceCount(ResourceManagerScript.ResourceType.Wood);
		currentMetal = _rm.GetResourceCount(ResourceManagerScript.ResourceType.Stone);
		currentShiny = _rm.GetResourceCount(ResourceManagerScript.ResourceType.Shiny);
	}

    public void UpdateCosts()
    {
		if (_type.Equals("house"))
		{
			_cost = bHouse.getCost(_level);
		}
		else if (_type.Equals("farm"))
		{
			_cost = bFarm.getCost(_level);
		}
		else if (_type.Equals("banner"))
		{
			_cost = bBanner.getCost(_level);
		}
		else if (_type.Equals("outpost"))
		{
			_cost = bOutpost.getCost(_level);
		}
		else if (_type.Equals("towncenter"))
		{
			_cost = bTownCenter.getCost(_level);
		}
		else if (_type.Equals("garbagecan"))
		{
			_cost = bGarbageCan.getCost(_level);
		}
		else if (_type.Equals("woodpile"))
		{
			_cost = bWoodPile.getCost(_level);
		}
		else if (_type.Equals("stonepile"))
		{
			_cost = bStonePile.getCost(_level);
		}
        else if (_type.Equals("unlockbutton"))
        {
            Dictionary<ResourceManagerScript.ResourceType, int> _costShiny = new Dictionary<ResourceManagerScript.ResourceType, int>();
            _costShiny.Add(ResourceManagerScript.ResourceType.Shiny, 1);
            _cost = _costShiny;

        }
        else
            Debug.LogError("Build button not defined with type or level, or couldn't get cost dictionary from building script");

		//set default costs to zero before recalculating
		costTrash = 0;
		costWood = 0;
		costMetal = 0;
		costShiny = 0;

		//set cost variables from specific cost dictionary
		if (_cost != null)
		{
			foreach (ResourceManagerScript.ResourceType key in _cost.Keys)
			{
				int tmp;
				_cost.TryGetValue(key, out tmp);

				switch (key)
				{
					case ResourceManagerScript.ResourceType.Trash:
						{
							costTrash = tmp;
							break;
						}
					case ResourceManagerScript.ResourceType.Wood:
						{
							costWood = tmp;
							break;
						}
					case ResourceManagerScript.ResourceType.Stone:
						{
							costMetal = tmp;
							break;
						}
					case ResourceManagerScript.ResourceType.Shiny:
						{
							costShiny = tmp;
							break;
						}
				}
			}
		}
    }

	//update UI button text
	private void UpdateCostsText()
	{
		//Set Trash Cost Text
		if (textTrashCost != null)
		{
			if (costTrash == 0) textTrashCost.text = "";
			else
				textTrashCost.text =  costTrash.ToString();

			if (currentTrash < costTrash)
			{
				textTrashCost.color = bad;
			}
			else
				textTrashCost.color = good;
		}
		//Set Wood Cost Text
		if (textWoodCost != null)
		{
			if (costWood == 0) textWoodCost.text = "";
			else
				textWoodCost.text = costWood.ToString();

			if (currentWood < costWood)
			{
				textWoodCost.color = bad;
			}
			else
				textWoodCost.color = good;
		}
		//Set Metal Cost Text
		if (textMetalCost != null)
		{
			if (costMetal == 0) textMetalCost.text = "";
			else
				textMetalCost.text = costMetal.ToString();

			if (currentMetal < costMetal)
			{
				textMetalCost.color = bad;
			}
			else
				textMetalCost.color = good;
		}
		//Set Shiny Cost Text
		if (textShinyCost != null)
		{
			if (costShiny == 0) textShinyCost.text = "";
			else
				textShinyCost.text = costShiny.ToString();

			if (currentShiny < costShiny)
			{
				textShinyCost.color = bad;
			}
			else
				textShinyCost.color = good;
		}
	}
    
    //Makes sure if the button is clicked, we can afford the cost, Then we let the MVC controller know were good to go
    public void ApproveCosts()
    {
        //Debug.LogWarning("Heard UI ApproveCosts");
        UpdateButton();

        if ((currentTrash >= costTrash) && (currentWood >= costWood) && (currentMetal >= costWood) && (currentShiny >= costShiny))
        {
            //call different method if building or upgrading
			if (name == "Button_Upgrade") MVCController.Instance.MVCUpgradeSomething();
			else
				MVCController.Instance.MVCBuildSomething(_type);

			//decrement resources based on cost
			_rm.incrementResource(ResourceManagerScript.ResourceType.Trash, -costTrash);
			_rm.incrementResource(ResourceManagerScript.ResourceType.Wood, -costWood);
			_rm.incrementResource(ResourceManagerScript.ResourceType.Stone, -costMetal);
			_rm.incrementResource(ResourceManagerScript.ResourceType.Shiny, -costShiny);
			// Debug.Log("Cost Approved");
		}
        else
        {
           // Debug.LogError("Cost is not approved");
        }
    }

	//Changes Button for a different structure type and level
	public void ChangeButton(string type, int lvl)
	{
		_type = type;
		_level = lvl;

		//update title of button
		string txt = "";
		int _maxlevel = 10;
		if (textTitle != null)
		{
			if (_type == "house")
			{
				txt = "House (LVL ";
				_maxlevel = bHouse.getMaxLevel();
			}
			else if (_type == "farm")
			{
				txt = "Farm (LVL ";
				_maxlevel = bFarm.getMaxLevel();
			}
			else if (_type == "banner")
			{
				txt = "Banner (LVL ";
				_maxlevel = bBanner.getMaxLevel();
			}
			else if (_type == "outpost")
			{
				txt = "Outpost (LVL ";
				_maxlevel = bOutpost.getMaxLevel();
			}
			else if (_type == "towncenter")
			{
				txt = "Town Center (LVL ";
				_maxlevel = bTownCenter.getMaxLevel();
			}
			else if (_type == "garbagecan")
			{
				txt = "Garbage Can (LVL ";
				_maxlevel = bGarbageCan.getMaxLevel();
			}
			else if (_type == "woodpile")
			{
				txt = "Wood Pile (LVL ";
				_maxlevel = bWoodPile.getMaxLevel();
			}
			else if (_type == "stonepile")
			{
				txt = "Stone Pile (LVL ";
				_maxlevel = bStonePile.getMaxLevel();
			}
			else
			{
				txt = "not specified (LVL ";
				_maxlevel = 0;
			}
			
			//determine if level is greater than max level, then disable button, otherwise enable
			if (lvl > _maxlevel)
			{
				txt += "MAX)";
				GetComponent<Button>().interactable = false;
			}
			else
			{
				txt += _level.ToString() + ")";
				GetComponent<Button>().interactable = true;
			}

			//set title of button
			textTitle.text = txt;
		}

		UpdateButton();
	}
    //used in 
    public void updateState()
    {
        if (_rm.GetResourceCount(ResourceManagerScript.ResourceType.Shiny) >= _royalGuardSlotCost)
        {
            //subtract the cost - arbitrary for now and i guess heres an okay spot
            //so i dont have to duplicate and add more on click events for each button
            _rm.incrementResource(ResourceManagerScript.ResourceType.Shiny, -_royalGuardSlotCost);


            // find the player and tell him to refresh the royal guard:
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                PlayerStats ps = player.GetComponent<PlayerStats>();
                if (ps)
                {
                    ps.unlockWorkerSlot();
                    ps.ShowRoyalGuard(true);
                }
            }
        }
    }

    public void Demolish()
    {
        //Debug.Log("Heard Demolish");
        MVCController.Instance.MVCDemolishSomething();
    }

    /**Called by "Event Trigger Pointer Enter/Exit on Button*/
    public void MouseEnter()
    {
       // Debug.Log("HEARD ENTER");
        //MVCController.Instance.CheckClicks(false);
    }
    public void MouseExit()
    {
       // Debug.Log("HEARD EXIT");
        //MVCController.Instance.CheckClicks(true);
    }
}
