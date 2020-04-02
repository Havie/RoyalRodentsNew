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

    private CameraController _cameraController;


    // Start is called before the first frame update
    void Start()
    {
        showMenu(false, Vector3.zero, null, null);
        _cameraController = Camera.main.GetComponent<CameraController>();


    }

    public void showMenu(bool cond, Vector2 loc, GameObject o, BuildableObject building)
    {
        // dont want to enable buttons if we are in override mode
        if (cond && _cameraController.getOverrideMode())
            return;

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
							case (BuildableObject.BuildingType.Outpost):
								buttonScript.ChangeButton("outpost", level + 1);
								break;
							case (BuildableObject.BuildingType.Banner):
								buttonScript.ChangeButton("banner", level + 1);
								break;
							case (BuildableObject.BuildingType.TownCenter):
								buttonScript.ChangeButton("towncenter", level + 1);
								break;
							case (BuildableObject.BuildingType.GarbageCan):
								buttonScript.ChangeButton("garbagecan", level + 1);
								break;
							case (BuildableObject.BuildingType.WoodPile):
								buttonScript.ChangeButton("woodpile", level + 1);
								break;
							case (BuildableObject.BuildingType.StonePile):
								buttonScript.ChangeButton("stonepile", level + 1);
								break;
						}
					}
				}
				//If building clicked is town hall, disable demolish button, else enable it
				if (b.name == "Button_Demolish")
				{
					BuildableObject.BuildingType type = building.getType();
					if (type == BuildableObject.BuildingType.TownCenter || type == BuildableObject.BuildingType.GarbageCan || type == BuildableObject.BuildingType.WoodPile || type == BuildableObject.BuildingType.StonePile)
					{
						b.GetComponent<Button>().interactable = false;
					}
					else
						b.GetComponent<Button>().interactable = true;
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
