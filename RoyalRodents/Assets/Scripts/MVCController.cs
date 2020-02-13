using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MVCController : MonoBehaviour
{
    private static MVCController _instance;

    public  GameObject lastClicked;

    public UIBuildMenu _BuildMenu;

    public bool checkingClicks;

    public static MVCController Instance
    {
        get
        {
            if (_instance == null)
                _instance = new MVCController();
            return _instance;
        }
    }


     void Start()
    {
        GameObject o = GameObject.FindGameObjectWithTag("BuildMenu");
        _BuildMenu = o.GetComponent<UIBuildMenu>();
        checkingClicks = true;
    }



    public void Update()
    {
        
        if (Input.GetMouseButtonDown(0) && checkingClicks)
        {

            Vector3 MouseRaw = Input.mousePosition;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(MouseRaw);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider != null)
            {
              // Debug.Log("Hit result:" + hit.collider.gameObject);
                if (lastClicked == hit.collider.gameObject)
                    return;
                lastClicked = hit.collider.gameObject;

                if (lastClicked.GetComponent<BuildableObject>() || lastClicked.GetComponent<Button>())
                {
                   // Debug.Log("Last Clicked is a buildingobj:" + lastClicked.name);
                   
                    lastClicked.GetComponent<BuildableObject>().imClicked();
                    _BuildMenu.showMenu(true, MouseRaw);
                }
                else if (UIBuildMenu.isActive2())
                {
                    _BuildMenu.showMenu(false, Vector3.zero);
                }

            }
            else if (UIBuildMenu.isActive2())
            {
                _BuildMenu.showMenu(false, Vector3.zero);
            }
        }
    }

        public void buildSomething(string type)
        {
            if (lastClicked == null)
                return;
            print("lastClicked: " + lastClicked +" in BuildSomething");
            if (lastClicked.GetComponent<BuildableObject>())
            {
                 Debug.Log("Found Buildable Object");
                lastClicked.GetComponent<BuildableObject>().BuildSomething(type);
            }

         }

    public void CheckClicks(bool b)
    {
        checkingClicks = b;
    }

}

