using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBuildMenu : MonoBehaviour
{


    public Button[] buttons;

    private bool _active;
    private GameObject _current;


    // Start is called before the first frame update
    void Start()
    {
        showMenu(false, Vector3.zero,null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showMenu(bool cond, Vector2 loc, GameObject o)
    {
        _active = cond;
        _current = o;
        //GameObject o = GameObject.FindGameObjectWithTag("Canvas");
        //Canvas canvas = o.GetComponent<Canvas>();
        //Vector2 difference= canvas.GetComponent<RectTransform>().anchoredPosition- this.transform.GetComponent<RectTransform>().anchoredPosition;
        //this.transform.GetComponent<RectTransform>().anchoredPosition = loc;

        //Move the Loc up a bit?
        loc.y = loc.y + 30;

        this.transform.position = loc;

       // Debug.Log("The Menu loc moves to :" + loc);

       foreach (Button b in buttons)
       {
           b.gameObject.SetActive(cond);
            
           //When Enabling Upgrade Buttons, change the button based on the type and level of last strucuture clicked
           if (b.name == "Button_Upgrade")
           {
               UIButtonCosts buttonScript = b.GetComponent<UIButtonCosts>();
               if (buttonScript != null)
                    buttonScript.ChangeButton("house", 2); //***only updates to house lvl 2 button for now. Should get based on the type and level of last strucuture clicked
            }
       }
    }


    public bool isActive()
    {
        return _active;
    }

    public GameObject getLastObj()
    {
        return _current;
    }
 
}
