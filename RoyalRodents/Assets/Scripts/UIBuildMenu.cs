using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBuildMenu : MonoBehaviour
{
    /** Class Can't be a singleton becuz upgrade Menu uses this class too */

    public Button[] buttons;

    private bool _active;
    private GameObject _current;


    // Start is called before the first frame update
    void Start()
    {
        showMenu(false, Vector3.zero, null, null);
    }



    public void showMenu(bool cond, Vector2 loc, GameObject o, BuildableObject building)
    {
        _active = cond;
        _current = o;

        //Tell MVC which building this is on
        if(cond)
            MVCController.Instance.setLastClicked(o);


        //Move the location up a bit?
        loc.y = loc.y + 30;

        this.transform.position = loc;

        // Debug.Log("The Menu loc moves to :" + loc);

        foreach (Button b in buttons)
        {
            b.gameObject.SetActive(cond);

            //Dont want to do this if were turning them off 
            if (cond && building != null)
            {
                //When Enabling Upgrade Buttons, change the button based on the type and level of last structure clicked
                if (b.name == "Button_Upgrade")
                {
                    UIButtonCosts buttonScript = b.GetComponent<UIButtonCosts>();
                    if (buttonScript != null)
                    {
                        BuildableObject.BuildingType type = building.getType();
                        int level = building.getLevel();

                        switch (type)
                        {
                            case (BuildableObject.BuildingType.House):
                                buttonScript.ChangeButton("house", level + 1);
                                break;
                            case (BuildableObject.BuildingType.Farm):
                                buttonScript.ChangeButton("farm", level + 1);
                                break;
                            case (BuildableObject.BuildingType.Tower):
                                buttonScript.ChangeButton("tower", level + 1);
                                break;
                            case (BuildableObject.BuildingType.Wall):
                                buttonScript.ChangeButton("wall", level + 1);
                                break;
                            case (BuildableObject.BuildingType.TownCenter):
                                buttonScript.ChangeButton("towncenter", level + 1);
                                break;
                        }
                    }
                }
            }
        }
    }


    public bool isActive()
    {
        Debug.Log("BUILD MENU ACTIVE=" + _active);
        return _active;
    }

    public GameObject getLastObj()
    {
        return _current;
    }

}
