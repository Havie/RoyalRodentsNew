using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIButtonCosts : MonoBehaviour
{
	//Button Definition
	public string _type;
	public int _level;

	//Current local dictionary
	public Dictionary<string, int> _cost;

	//Cost of Upgrade by Resource Vars
	private int costGold;

	private int costTrash;
	private int costWood;
	private int costMetal;
	private int costShiny;

	//Current Player Resources Local Vars
	private int currentGold;

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

	private Color bad = Color.red;
	private Color good = Color.black;

    // Start is called before the first frame update
    void Start()
    {
        //text = this.gameObject.transform.GetComponent<TextMeshProUGUI>();
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
		currentGold = GameManager.Instance._gold;

		//update local vars from player resources
		currentTrash = ResourceManagerScript.Instance.Trash;
		currentWood = ResourceManagerScript.Instance.Wood;
		currentMetal = ResourceManagerScript.Instance.Metal;
		currentShiny = ResourceManagerScript.Instance.Shiny;
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
		else if (_type.Equals("wall"))
		{
			_cost = bWall.getCost(_level);
		}
		else if (_type.Equals("tower"))
		{
			_cost = bTower.getCost(_level);
		}
		else if (_type.Equals("towncenter"))
		{
			_cost = bTownCenter.getCost(_level);
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
			foreach (string key in _cost.Keys)
			{
				int tmp;
				_cost.TryGetValue(key, out tmp);

				if (key.Equals("Trash"))
					costTrash = tmp;
				else if (key.Equals("Wood"))
					costWood = tmp;
				else if (key.Equals("Metal"))
					costMetal = tmp;
				else if (key.Equals("Shiny"))
					costShiny = tmp;
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
				textTrashCost.text = currentTrash.ToString() + "/" + costTrash;

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
				textWoodCost.text = currentWood.ToString() + "/" + costWood;

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
				textMetalCost.text = currentMetal.ToString() + "/" + costMetal;

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
				textShinyCost.text = currentShiny.ToString() + "/" + costShiny;

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
        Debug.LogWarning("Heard UI ApproveCosts");
        UpdateButton();

        if ((currentTrash >= costTrash) && (currentWood >= costWood) && (currentMetal >= costWood) && (currentShiny >= costShiny))
        {
            if (name == "Button_Upgrade") MVCController.Instance.MVCUpgradeSomething();
			else
				MVCController.Instance.MVCBuildSomething(_type);

			ResourceManagerScript.Instance.incrementTrash(-costTrash);
			ResourceManagerScript.Instance.incrementWood(-costWood);
			ResourceManagerScript.Instance.incrementMetal(-costMetal);
			ResourceManagerScript.Instance.incrementShiny(-costShiny);
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

		//Debug.Log("ChangeButton set to " + type + ", lvl " + lvl.ToString());

		//update title of button
		if (textTitle != null)
		{
			if (_type == "house")
				textTitle.text = "House (LVL " + _level.ToString() + ")";
			else if (_type == "farm")
				textTitle.text = "Farm (LVL " + _level.ToString() + ")";
			else if (_type == "wall")
				textTitle.text = "Wall (LVL " + _level.ToString() + ")";
			else if (_type == "tower")
				textTitle.text = "Tower (LVL " + _level.ToString() + ")";
			else if (_type == "towncenter")
				textTitle.text = "Town Center (LVL " + _level.ToString() + ")";
			else
				textTitle.text = "not specified (LVL " + _level.ToString() + ")";
		}

		UpdateButton();
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
