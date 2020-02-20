using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIButtonCosts : MonoBehaviour
{
    int cost;
	public string _type;
	public int _level;
	public Dictionary<string, int> _cost;

	//Current Player Resources Local Vars
	public int currentGold;

	public int currentTrash;
	public int currentWood;
	public int currentMetal;
	public int currentShiny;

	public TextMeshProUGUI text;

    public Color bad = Color.red;
    public Color good = Color.black;

    // Start is called before the first frame update
    void Start()
    {
        text=  this.gameObject.transform.GetComponent<TextMeshProUGUI>();
        UpdateButton();

	}

     void Update()
    {
        UpdateButton();
    }

	//updates all variables to update the button
	void UpdateButton()
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
		currentTrash = GameManager.Instance._rm.Trash;
		currentWood = GameManager.Instance._rm.Wood;
		currentMetal = GameManager.Instance._rm.Metal;
		currentShiny = GameManager.Instance._rm.Shiny;
	}

    //Needs to get called elsewhere from some other system such as the game manager when we increment a resource, not in Update, will be figured out later.
    public void UpdateCosts()
    {
		if (_type.Equals("house"))
		{
			if (_level.Equals(1))
				_cost = bHouse._costLevel1;
			if (_level.Equals(2))
				_cost = bHouse._costLevel2;
			if (_level.Equals(3))
				_cost = bHouse._costLevel3;
		}

		if (_cost != null)
		{
			foreach (string key in _cost.Keys)
			{
				int tmp;
				_cost.TryGetValue(key, out tmp);
				cost = tmp;
			}
		}
    }

	//update UI button text
	private void UpdateCostsText()
	{
		if (text != null)
		{
			text.text = currentTrash.ToString() + "/" + cost;
			if (currentTrash < cost)
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
       // Debug.Log("request to approve");
        if (type.Equals("house"))
        {

        }

        if (currentTrash >= cost)
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
