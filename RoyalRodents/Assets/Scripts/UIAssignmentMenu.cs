using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIAssignmentMenu : MonoBehaviour
{
    private static UIAssignmentMenu _instance;



    public GameObject _ButtonLeft;
    public GameObject _ButtonRight;

    [SerializeField]
    private GameObject _buttonTemplate;
    [SerializeField]
    private List<Rodent> _rList;

    private Quaternion _defaultRotation;

    private bool _active;
    private Button[] _buttons = new Button[10];
    private int _index;
    private int _used;
    private int _aspectHeight;


    private CameraController _cameraController;
    private UIAssignmentVFX _vfx;
    private GameObject _owner;

    public static UIAssignmentMenu Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<UIAssignmentMenu>(); ;
            return _instance;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        MVCController.Instance.SetUpAssignmentMenu(this); // pointless now because were a singleton
        //We will need to actually calculate this somehow at some point
        if (_aspectHeight == 0)
            _aspectHeight = 15;

        //Get our prefab if it isn't manually assigned
        if (!_buttonTemplate)
            _buttonTemplate = Resources.Load<GameObject>("UI/Button_Rodent");

        if (_buttonTemplate)
            _defaultRotation = _buttonTemplate.transform.rotation;

        _cameraController = Camera.main.GetComponent<CameraController>();
        showMenu(false);

    }

    void LateUpdate()
    {
        if (_active)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //setActive(false);
                showMenu(false);
            }
        }
    }
    //new Menu that will tell us which object showed menu
    public void showMenu(bool cond, GameObject ObjectThatCalled)
    {
        showMenu(cond);
        //I am not even sure we need to keep track of this as buttons can be dragged into any receiver..
        _owner = ObjectThatCalled;
    }
    //used internally 
    private void showMenu(bool cond)
    {
       // Debug.Log("ShowMenu::"+cond + "  and index is:" +_index );
        setActive(cond);

        for (int i = 0; i < _index; ++i)
        {
            _buttons[i].gameObject.SetActive(cond);
            _buttons[i].transform.rotation = _defaultRotation;
        }
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p)
        {
            PlayerStats ps = p.GetComponent<PlayerStats>();
            if (ps)
                ps.ShowRoyalGuard(cond);


            PlayerMovement pm = p.GetComponent<PlayerMovement>();
            if (pm)
                pm.StopMoving();


        }
        ShowArrowButtons(cond);


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

    // publicly turned on/off via showMenu
    private void setActive(bool cond)
    {
        _active = cond;
        if (_active)
        {
            //Change Camera Control
            if (_cameraController)
                _cameraController.setCharacterMode(false);
        }
        else
        {
            // Change Camera Control back
            if (_cameraController)
                Camera.main.GetComponent<CameraController>().setCharacterMode(true);
        }

        ToggleVFX();
    }

    public void CreateButtons(List<Rodent> _PlayerRodents)
    {
        if (_PlayerRodents == _rList)
            return;
        _rList = _PlayerRodents;


        FindAvailable();
    }
    private void FindAvailable()
    {

        if (_rList == null)
            return;

       // Debug.Log("FindAvail");
        foreach (Rodent r in _rList)
        {
            if (r.GetRodentStatus() == Rodent.eStatus.Available)
            {
                //Debug.Log(r.getName() + "  is Available");
                CreateButton(r);
            }
        }
    }

    //Will need to send in portrait later on?
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
                if (t)
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
    public void ResetButtons()
    {

        for (int i = 0; i < _index; ++i)
        {
            _buttons[i].gameObject.SetActive(false);
        }

        _index = 0;
        FindAvailable();
    }
    public void ShowArrowButtons(bool cond)
    {
        //Debug.Log("Show Arrow COND???=" + cond);
        _ButtonLeft.gameObject.SetActive(cond);
        _ButtonRight.gameObject.SetActive(cond);
    }

    /** used by UI button */
    public void ToggleMenu()
    {
        //MVCController.Instance.CheckClicks(false);
      //  Debug.Log("ToggleMenu");
        showMenu(!_active);
        ToggleVFX();

        if (_active)
            CreateButtons(GameManager.Instance.getPlayerRodents());

        MVCController.Instance.showRedX(false);
    }
    private void ToggleVFX()
    {
        if (_vfx)
        {
            if (_active)
                _vfx.PlayGlowAnim(true);
            else
                _vfx.PlayGlowAnim(false);
        }
    }

    public void setVFX(UIAssignmentVFX vfx)
    {
        _vfx = vfx;
    }

    /************************************************************************/
    /* Potential issues:
     *  Can Click and recruit random rodents while in Assignment Mode,
     *  do we want this? - i vote no
     *  Can't Click buildings and build them from assignment menu
     *  do we want this?  -i vote no
     */
    /************************************************************************/
}
