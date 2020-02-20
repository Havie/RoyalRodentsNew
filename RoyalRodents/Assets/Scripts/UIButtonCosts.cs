using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIButtonCosts : MonoBehaviour
{
    int cost;
	public string _type;
	public Dictionary<string, int> _cost;
	public int currentGold;
    public TextMeshProUGUI text;

    public Color bad = Color.red;
    public Color good = Color.black;

    // Start is called before the first frame update
    void Start()
    {
        text=  this.gameObject.transform.GetComponent<TextMeshProUGUI>();
        UpdateCosts();


		if(_type.Equals("house"))
			_cost = bHouse._costLevel1;
	}

     void Update()
    {
        UpdateCosts();
    }

    //Needs to get called elsewhere from some other system such as the game manager when we increment a resource, not in Update, will be figured out later.
    public void UpdateCosts()
    {
        currentGold = GameManager.Instance._gold;
        if (text!=null)
        {
            if (_cost!=null)
            {
                foreach (string key in _cost.Keys)
                {
                    int tmp;
                    _cost.TryGetValue(key, out tmp);
                    cost = tmp;
                }
            }

            text.text = currentGold.ToString() + "/" + cost;
            if (currentGold < cost)
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

        if (currentGold >= cost)
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
        Debug.Log("HEARD ENTER");
        MVCController.Instance.CheckClicks(false);
    }
    public void MouseExit()
    {
        Debug.Log("HEARD EXIT");
        MVCController.Instance.CheckClicks(true);
    }
}
