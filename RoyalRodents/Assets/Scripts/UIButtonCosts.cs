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

	//Get Build Scripts for Each Struture
	//public bHouse b_house;

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

	public TextMeshProUGUI text;

	private Color bad = Color.red;
	private Color good = Color.black;

    // Start is called before the first frame update
    void Start()
    {
        text = this.gameObject.transform.GetComponent<TextMeshProUGUI>();
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
			if (_level == 1)
				_cost = bHouse._costLevel1;
			if (_level == 2)
				_cost = bHouse._costLevel2;
			if (_level == 3)
				_cost = bHouse._costLevel3;
		}
		else if (_type.Equals("farm"))
		{
			if (_level == 1)
				_cost = bFarm._costLevel1;
			if (_level == 2)
				_cost = bFarm._costLevel2;
			if (_level == 3)
				_cost = bFarm._costLevel3;
		}
		else if (_type.Equals("wall"))
		{
			if (_level == 1)
				_cost = bWall._costLevel1;
			if (_level == 2)
				_cost = bWall._costLevel2;
			if (_level == 3)
				_cost = bWall._costLevel3;
		}
		else if (_type.Equals("tower"))
		{
			if (_level == 1)
				_cost = bTower._costLevel1;
			if (_level == 2)
				_cost = bTower._costLevel2;
			if (_level == 3)
				_cost = bTower._costLevel3;
		}
		else if (_type.Equals("towncenter"))
		{
			if (_level == 1)
				_cost = bTownCenter._costLevel1;
			if (_level == 2)
				_cost = bTownCenter._costLevel2;
			if (_level == 3)
				_cost = bTownCenter._costLevel3;
			if (_level == 4)
				_cost = bTownCenter._costLevel4;
			if (_level == 5)
				_cost = bTownCenter._costLevel5;
		}
		else
			Debug.LogError("Build button not properly defined unknown:::" +_type);

		//set default costs to zero before recalculating
		costTrash = -1;
		costWood = -1;
		costMetal = -1;
		costShiny = -1;

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
		if (text != null)
		{
			//only shows trash cost at the moment *
			text.text = currentTrash.ToString() + "/" + costTrash;
			if (currentTrash < costTrash)
			{
				text.color = bad;
			}
			else
				text.color = good;
		}
        else
            Debug.LogError("UI Costs cant find Text");
	}
    
    //Makes sure if the button is clicked, we can afford the cost, Then we let the MVC controller know were good to go
    public void ApproveCosts(string type)
    {
		UpdateButton();

        if (currentTrash >= costTrash)
        {
            MVCController.Instance.buildSomething(type);
			// Debug.Log("Cost Approved");
        }
        else
        {
           // Debug.LogError("Cost is not approved");
        }
    }


    public void Demolish()
    {
        //Debug.Log("Heard Demolish");
        MVCController.Instance.DemolishSomething();
    }

    /**Called by "Event Trigger Pointer Enter/Exit on Button*/
    public void MouseEnter()
    {
       // Debug.Log("HEARD ENTER");
        MVCController.Instance.CheckClicks(false);
    }
    public void MouseExit()
    {
       // Debug.Log("HEARD EXIT");
        MVCController.Instance.CheckClicks(true);
    }
}
