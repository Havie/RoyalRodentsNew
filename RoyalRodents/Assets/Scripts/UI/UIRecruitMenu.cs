﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRecruitMenu : MonoBehaviour
{
    public GameObject _menuDummy;
    public Button[] buttons;
    private TextMeshProUGUI _CostFood;
    private TextMeshProUGUI _CostPop;
    private TextMeshProUGUI _Name;
    private TextMeshProUGUI _Name2; //dismiss menu
    private Image _weaponClass;
    private Image _weaponClass2; //dismiss menu
    private Sprite _iconMelee;
    private Sprite _iconRanged;
    [SerializeField] private Rodent _Rodent;

    private bool _active;

    private Color bad = Color.red;
    private Color good = Color.black;

    private int _FoodCost;
    private int _PopCost;

    private void Awake()
    {
        SetUpFromChildren();
    }


    void Start()
    {
        //MVCController.Instance.SetUpRecruitMenu(this);
        showMenu(false, Vector3.zero, "empty", 1, 1);

        _iconMelee = Resources.Load<Sprite>("UI/sword_icon");
        _iconRanged = Resources.Load<Sprite>("UI/bow_icon");

        if (_menuDummy)
            this.gameObject.transform.SetParent(_menuDummy.transform);
        else
        {
            _menuDummy = GameObject.FindGameObjectWithTag("RecruitMenu");
            if (_menuDummy)
                this.gameObject.transform.SetParent(_menuDummy.transform);
            else
                Debug.Log("Cant find RecruitMenu");
        }
    }

    private void LateUpdate()
    {
        if(_Rodent)
        {
            this.transform.position = _Rodent.transform.position;
        }
    }

    public void showMenu(bool cond, Rodent r)
    {

        _active = cond;
       //Debug.Log("ShowMENU:" + r.getName());
        if (cond)
        {
            // Tell the MVC were the active menu
            MVCController.Instance.SetRecruitMenu(this);


            _Rodent = r;
            buttons[0].gameObject.SetActive(true);
            buttons[1].gameObject.SetActive(false);
			buttons[2].gameObject.SetActive(false);

			Vector3 loc = r.transform.position;
            loc.y = loc.y + 1;
            this.transform.position = loc;

            //get costs
            _FoodCost = r.getRecruitmentCost();
            _PopCost = r.getPopulationCost();
            //show costs
            _CostPop.text = _PopCost.ToString();
            _CostFood.text = _FoodCost.ToString();

            //show class
            if (r.isRanged())
                _weaponClass.sprite = _iconRanged;
            else
                _weaponClass.sprite = _iconMelee;


            //Check if we have enough food
            if (ResourceManagerScript.Instance.GetResourceCount(ResourceManagerScript.ResourceType.Food) < _FoodCost)
            {
                _CostFood.color = bad;
            }
            else
                _CostFood.color = good;

			//Check if there is enough population capacity
			if (ResourceManagerScript.Instance.getCurrentPopulation() >= ResourceManagerScript.Instance.getPopulationCapacity())
			{
				_CostPop.color = bad;
			}
			else
				_CostPop.color = good;

			_Name.text = r.getName();
        }
        else // will turn off all buttons
        {
            foreach (Button b in buttons)
            {
                b.gameObject.SetActive(false);
            }
        }

    }

    //Used by Canvas Version - basically left over code, all this does currently, is turn off the item
    public void showMenu(bool cond, Vector2 loc, string name, int FoodCost, int PopCost)
    {
        //Debug.Log("Show Canvas MENU for : " + name);

        _active = cond;


        foreach (Button b in buttons)
        {
            b.gameObject.SetActive(cond);
        }
        //Only need to update these things if visible
        if (cond)
        {
            buttons[0].gameObject.SetActive(true);
            buttons[1].gameObject.SetActive(false);
            buttons[2].gameObject.SetActive(false);

            //Move the location up a bit?
            loc.y = loc.y + 30;
            this.transform.position = loc;
            _FoodCost = FoodCost;
            _PopCost = PopCost;

            _CostPop.text = _FoodCost.ToString();
            _CostFood.text = _PopCost.ToString();


            //Check if we have enough food
            if (ResourceManagerScript.Instance.GetResourceCount(ResourceManagerScript.ResourceType.Food) < FoodCost)
            {
                _CostFood.color = bad;
            }
            else
                _CostFood.color = good;

            //Check if there is enough population capacity
            if (ResourceManagerScript.Instance.getCurrentPopulation() >= ResourceManagerScript.Instance.getPopulationCapacity())
			{
				_CostPop.color = bad;
			}
			else
				_CostPop.color = good;

			_Name.text = name;
        }
        else // will turn off all buttons
        {
            foreach (Button b in buttons)
            {
                b.gameObject.SetActive(false);
            }
        }
    }

    public void showKingGuardMenu(bool cond, Rodent r)
    {
        // Tell the MVC were the active menu
        MVCController.Instance.SetRecruitMenu(this);

        _active = cond;
        buttons[1].gameObject.SetActive(cond);
        _Name2.text = r.getName();
    }

    //Old canvas version - unused now
    public void showKingGuardMenu(bool cond, Vector2 loc, string name)
    {
        _active = cond;
        buttons[1].gameObject.SetActive(cond);
        _Name2.text = name;
    }
    
	public void showDismissMenu(bool cond, Rodent r)
	{
        //MVCController.Instance.TurnThingsoff();

		// Tell the MVC were the active menu
		MVCController.Instance.SetRecruitMenu(this);
		_Rodent = r;

		_active = cond;
		buttons[2].gameObject.SetActive(cond);
		_Name2.text = r.getName();

        if (r.isRanged())
            _weaponClass2.sprite = _iconRanged;
        else
            _weaponClass2.sprite = _iconMelee;
    }

    public bool isActive()
    {
        return _active;
    }

    /** Communicates with ResourceManager to check if we can afford recruitment
     * Upon Recruitment tells ResourceManager to update costs
     * Tells MVC its time to recruit */
    public void Recruit()
    {
       // Debug.Log("!!!!Recruit " + _Name.text);

        // Check RM if we have enough Food  
        if (ResourceManagerScript.Instance.GetResourceCount(ResourceManagerScript.ResourceType.Food) < _FoodCost)
        {
            //To-Do: Play Sound
            return;
        }

        //Check RM if we have enough Pop
        if (ResourceManagerScript.Instance.getCurrentPopulation() >= ResourceManagerScript.Instance.getPopulationCapacity())
		{
			//To-Do: Play Sound
			return;
		}

        //Tell MVC to go ahead
        MVCController.Instance.Recruit(_Rodent, this);
        //Update Resource Manager
        ResourceManagerScript.Instance.incrementResource(ResourceManagerScript.ResourceType.Food, -_FoodCost);
		ResourceManagerScript.Instance.UpdateCurrentPopulation();
	}

    public void JoinGuard()
    {
        //Debug.Log("Heard Join Guard");

        //To-Do -everything

        showMenu(false, Vector3.zero, null, 1, 0);
    }

    public void DismissRodent()
    {
		//Debug.Log("Dismiss rodent button pressed");

		//Unassign from its assigned structure
		Employee _Job= _Rodent.GetJob();
        if(_Job)
		    _Job.Dismiss(_Rodent);
		//remove from player rodent list (in gamemanager)
		GameManager.Instance.RemovePlayerRodent(_Rodent);
		//make rodent available again (no hat)
		_Rodent.setTeam(0);
        _Rodent.ShowDismissMenu(false);

        showMenu(false, null);


	}


    /**Called by "Event Trigger Pointer Enter/Exit on Button*/
    public void MouseEnter()
    {
        // Debug.Log("HEARD ENTER");
       // MVCController.Instance.CheckClicks(false);
    }
    public void MouseExit()
    {
       // Debug.Log("HEARD EXIT");
        //MVCController.Instance.CheckClicks(true);
    }
    private void SetUpFromChildren()
    {
        Transform _ButtonChild = this.transform.GetChild(0);
        if (_ButtonChild)
        {
            Transform t = _ButtonChild.transform.Find("Text_Cost_Food");
            if (t)
                _CostFood = t.GetComponent<TextMeshProUGUI>();
            else
                Debug.LogError("Cant Find Text_Cost_Food");

            t = _ButtonChild.transform.Find("Text_Cost_Pop");
            if (t)
                _CostPop = t.GetComponent<TextMeshProUGUI>();
            else
                Debug.LogError("Cant Find Text_Cost_Pop");

            t = _ButtonChild.transform.Find("Text_Name");
            if (t)
                _Name = t.GetComponent<TextMeshProUGUI>();
            else
                Debug.LogError("Cant Find Text_Name (recruit)");
            t = _ButtonChild.transform.Find("Class_icon");
            if (t)
                _weaponClass = t.GetComponent<Image>();
            else
                Debug.LogError("Cant Find Class_icon (recruit)");
        }
        else
            Debug.LogError("Cant Find RecruitMenu Child");
        _ButtonChild = this.transform.GetChild(2);
        if (_ButtonChild)
        {
            Transform t = _ButtonChild.transform.Find("Text_Name3");
            if (t)
                _Name2 = t.GetComponent<TextMeshProUGUI>();
            else
                Debug.LogError("Cant Find Text_Name3 (dismiss)");
            t = _ButtonChild.transform.Find("Class_icon");
            if(t)
            {
                _weaponClass2 = t.GetComponent<Image>();
            }
            else
                Debug.LogError("Cant Find Class_icon (dismiss)");
        }
    }
     

}
