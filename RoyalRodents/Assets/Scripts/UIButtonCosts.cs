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
