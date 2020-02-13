using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*Model-View-Controller
*Responsible for communication between game_world(Model) and UI(View)
* Keeps track of last clicked object.
* Is a Singleton/Instance 
*/
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
        //responsible for finding out what was left clicked
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

                    //There is a disconnect here, for clarity sake this script should be the only one responisible for telling the UI what to do,
                    // However we will have to check the state of the building clicked from this script then instead
                    _BuildMenu.showMenu(true, MouseRaw);
                }
                // If Menu is active, and we click another object, we want to close the menu
                else if (UIBuildMenu.isActive2())
                {
                    _BuildMenu.showMenu(false, Vector3.zero);
                }

            }
            // If Menu is active, and we click off of the object, we want to close the menu
            else if (UIBuildMenu.isActive2())
            {
                _BuildMenu.showMenu(false, Vector3.zero);
            }
        }
    }

    // Called from "Approve Costs" in UIButtonCosts Script
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

