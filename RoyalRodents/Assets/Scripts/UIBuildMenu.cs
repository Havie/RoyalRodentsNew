using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBuildMenu : MonoBehaviour
{


    public Button[] buttons;

    private static bool active;


    // Start is called before the first frame update
    void Start()
    {
        showMenu(false, Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showMenu(bool cond, Vector2 loc)
    {
        active = cond;
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
        }
    }


    public bool isActive()
    {
        return active;
    }
    public static bool isActiveStatic()
    {
        return active;
    }

    //TMP
    public RectTransform getRect()
    {
        return this.transform.GetComponent<RectTransform>();
    }
}
