using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIAssignmentMenu : MonoBehaviour
{
    public GameObject _buttonTemplate;

    private bool _active;
    private Button[] _buttons=new Button[10];
    private int _index;
    private int _aspectHeight;
    

    // Start is called before the first frame update
    void Start()
    {
        MVCController.Instance.SetUpAssignmentMenu(this);
        if (_aspectHeight == 0)
            _aspectHeight = 30;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void showMenu(bool cond)
    {
        _active = cond;

       for (int i=0; i<_index; ++i)
        {
           _buttons[i].gameObject.SetActive(cond);
        }

        if (!_active)
            _index = 0;
    }

    //Will need to send in portrait later on
    public void CreateButton(Rodent rodent)
    {
        GameObject o = Instantiate(_buttonTemplate);
        o.gameObject.transform.SetParent(this.transform);
        o.transform.localPosition = new Vector3(0, -_aspectHeight + (_index* _aspectHeight), 0);
        Button b= o.GetComponent<Button>();
        if (b)
        {
           UIRodentHolder holder= b.GetComponent<UIRodentHolder>();
            if (holder)
                holder.setRodent(rodent);

            _buttons[_index] = b;
            if(_index<_buttons.Length) // need to find a way to scroll someday
                ++_index;
            Transform t = o.transform.Find("Name");
            TextMeshProUGUI text = t.GetComponent<TextMeshProUGUI>();
            if (text)
                text.text = rodent.getName();

            //To-Do:Assign Image 
        }
        showMenu(true);
    }
}
