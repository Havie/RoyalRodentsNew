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
    
    public void ApproveCosts(string type)
    {
        Debug.Log("request to approve");

        if (currentGold >= cost)
        {
            controller.buildSomething(type);
            Debug.Log("Cost Approved");
        }
    }

}
