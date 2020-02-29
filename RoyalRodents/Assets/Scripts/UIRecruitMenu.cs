using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRecruitMenu : MonoBehaviour
{
    public Button[] buttons;
    private TextMeshProUGUI _CostFood;
    private TextMeshProUGUI _CostPop;
    private TextMeshProUGUI _Name;
    private TextMeshProUGUI _Name2;

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
        MVCController.Instance.SetUpRecruitMenu(this);
        showMenu(false, Vector3.zero, "empty", 1, 1);
    }


    // a little unnecessary to be an array but other scripts are arrays
    public void showMenu(bool cond, Vector2 loc, string name, int FoodCost, int PopCost)
    {
        // Debug.Log("Show MENU for : " + name);

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

            //Move the location up a bit?
            loc.y = loc.y + 30;
            this.transform.position = loc;
            _FoodCost = FoodCost;
            _PopCost = PopCost;

            _CostPop.text = _FoodCost.ToString();
            _CostFood.text = _PopCost.ToString();


            //Check if we have enough food
            if (ResourceManagerScript.Instance.Food < FoodCost)
            {
                _CostFood.color = bad;
            }
            else
                _CostFood.color = good;

            //To-Do : Add Population check


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

    public void showKingGuardMenu(bool cond, Vector2 loc, string name)
    {
        _active = cond;
        buttons[1].gameObject.SetActive(cond);
        _Name2.text = name;


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
        //Debug.Log("Recruit " + _Name.text);

        // Check RM if we have enough Food  
        if (ResourceManagerScript.Instance.Food < _FoodCost)
        {
            //To-Do: Play Sound
            return;
        }

        //Check RM if we have enough Pop   // Sound
        //To-Do

        //Tell MVC to go ahead
        MVCController.Instance.Recruit();
        //Update Resource Manager
        ResourceManagerScript.Instance.incrementFood(0 - _FoodCost);

    }
    /**Called by "Event Trigger Pointer Enter/Exit on Button*/
    public void MouseEnter()
    {
        // Debug.Log("HEARD ENTER");
        MVCController.Instance.CheckClicks(false);
    }
    public void MouseExit()
    {
        //Debug.Log("HEARD EXIT");
        MVCController.Instance.CheckClicks(true);
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
                Debug.LogError("Cant Find Text_Name");
        }
        else
            Debug.LogError("Cant Find RecruitMenu Child");
        _ButtonChild = this.transform.GetChild(1);
        if (_ButtonChild)
        {
            Transform t = _ButtonChild.transform.Find("Text_Name2");
            if (t)
                _Name2 = t.GetComponent<TextMeshProUGUI>();
            else
                Debug.LogError("Cant Find Text_Name2");
        }
    }
     

}
