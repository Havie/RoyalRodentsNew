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


    private bool _active;

    private void Awake()
    {
        SetUpFromChildren();
    }


    void Start()
    {
        MVCController.Instance.SetUpRecruitMenu(this);
        showMenu(false, Vector3.zero, "empty");
    }

 
    // a little unnecessary to be an array but other scripts are arrays
    public void showMenu(bool cond, Vector2 loc, string name)
    {
       // Debug.Log("Show MENU for : " + name);

        _active = cond;

        //Move the location up a bit?
        loc.y = loc.y + 30;

        this.transform.position = loc;

        // Debug.Log("The Menu loc moves to :" + loc);

        foreach (Button b in buttons)
        {
            b.gameObject.SetActive(cond);
        }
        //TMP?
        _CostPop.text = "1";
        _CostFood.text = "1";

        _Name.text = name;
    }


    public bool isActive()
    {
        return _active;
    }

    public void Recruit()
    {
        Debug.Log("Recruit " + _Name.text);
        // Check RM if we have enough Food   // Sound?

        //Check RM if we have enough Pop   // Sound?

        //Handle it in MVC
        MVCController.Instance.Recruit();

    }
    /**Called by "Event Trigger Pointer Enter/Exit on Button*/
    public void MouseEnter()
    {
         //Debug.Log("HEARD ENTER");
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
            if(t)
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
    }
}
