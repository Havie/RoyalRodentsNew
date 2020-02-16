using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIButtonCosts : MonoBehaviour
{
    public int cost;
    public int currentGold;
    public TextMeshProUGUI text;
    MVCController controller;

    public Color bad = Color.red;
    public Color good = Color.black;

    // Start is called before the first frame update
    void Start()
    {
        GameObject o = GameObject.FindGameObjectWithTag("MVC");
        if(o)
        {
            if (o.GetComponent<MVCController>())
                controller = o.GetComponent<MVCController>();
            else
                Debug.LogError("UI Costs cant find MVC Controller");
        }
       
        text=  this.gameObject.transform.GetComponent<TextMeshProUGUI>();
        UpdateCosts();
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
        Debug.Log("request to approve");

        if (currentGold >= cost)
        {
            controller.buildSomething(type);
           Debug.Log("Cost Approved");
        }
        else
        {
            Debug.LogError("Cost is not approved");
        }
    }

    public void Demolish()
    {
        Debug.Log("Heard Demolish");
        controller.DemolishSomething();
    }

    /**Called by "Event Trigger Pointer Enter/Exit on Button*/
    public void MouseEnter()
    {
        Debug.Log("HEARD ENTER");
        controller.CheckClicks(false);
    }
    public void MouseExit()
    {
        Debug.Log("HEARD EXIT");
        controller.CheckClicks(true);
    }
}
