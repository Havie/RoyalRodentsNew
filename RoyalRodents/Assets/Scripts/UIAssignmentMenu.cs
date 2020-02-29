using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIAssignmentMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _buttonTemplate;

    private bool _active;
    private Button[] _buttons=new Button[10];
    private int _index;
    private int _used;
    private int _aspectHeight;
    MVCController controller;
    private List<Rodent> _rList;


    // Start is called before the first frame update
    void Start()
    {
        MVCController.Instance.SetUpAssignmentMenu(this);
        //We will need to actually calculate this somehow at some point
        if (_aspectHeight == 0)
            _aspectHeight = 30;

        //Get our prefab if it isn't manually assigned
        if(!_buttonTemplate)
            _buttonTemplate= Resources.Load<GameObject>("UI/Button_Rodent");


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void showMenu(bool cond)
    {
        //Debug.Log("ShowMenu::"+cond);
        _active = cond;

       for (int i=0; i<_index; ++i)
        {
           _buttons[i].gameObject.SetActive(cond);
        }

        //If we turn off the menu, reset the index and list
        if (!_active)
        {
            _index = 0;
            _rList = null;
        }
    }
    public bool isActive()
    {
        return _active;
    }

    public void CreateButton(List<Rodent> _PlayerRodents)
    {
        if (_PlayerRodents == _rList)
            return;
        _rList = _PlayerRodents;


        foreach (Rodent r in _PlayerRodents)
        {
            if (r.GetRodentStatus() == Rodent.eStatus.Available)
            {
                // Debug.Log(r.getName() + "  is Available");
                CreateButton(r);
            }
        }
    }

    //Will need to send in portrait later on
    public void CreateButton(Rodent rodent)
    {
       // Debug.Log("Make a Button for :" +rodent.getName());

        //Make new Buttons
        if (_index >= _used)
        {
           // Debug.Log("Make a new Button!");
           //Make a new button from prefab
            GameObject o = Instantiate(_buttonTemplate);
            o.gameObject.transform.SetParent(this.transform);
            //offset it to stack upwards
            o.transform.localPosition = new Vector3(0, -_aspectHeight + (_index * _aspectHeight), 0);
            Button b = o.GetComponent<Button>();
            if (b)
            {
                UIRodentHolder holder = b.GetComponent<UIRodentHolder>();
                if (holder)
                    holder.setRodent(rodent);

                _buttons[_index] = b;
                if (_index < _buttons.Length) // need to find a way to scroll someday
                {
                    ++_index;
                    ++_used;
                }
                //Assign Text
                Transform t = b.transform.Find("Name");
                if (t)
                {
                    TextMeshProUGUI text = t.GetComponent<TextMeshProUGUI>();
                    if (text)
                        text.text = rodent.getName();
                }
                //Assign Image 
                t = b.transform.Find("Portrait");
                if(t)
                {
                    Image image = t.GetComponent<Image>();
                    if (image)
                        image.sprite = rodent.GetPortrait();
                }


            }
        }
        //Reuse Old Buttons
        else
        {
          //  Debug.Log("Reuse a Button! for " + rodent.getName()+ " @Index:" +_index +"    used:"+ _used );
            Button b = _buttons[_index];
            if (b)
            {
                UIRodentHolder holder = b.GetComponent<UIRodentHolder>();
                if (holder)
                    holder.setRodent(rodent);

                    ++_index;

                Transform t = b.transform.Find("Name");
 
                TextMeshProUGUI text = t.GetComponent<TextMeshProUGUI>();
                if (t)
                {
                    if (text)
                        text.text = rodent.getName();

                }
                //Assign Image 
                t = b.transform.Find("Portrait");
                if (t)
                {
                    Image image = t.GetComponent<Image>();
                    if (image)
                        image.sprite = rodent.GetPortrait();
                }
            }

        }
        showMenu(true);
    }


}
