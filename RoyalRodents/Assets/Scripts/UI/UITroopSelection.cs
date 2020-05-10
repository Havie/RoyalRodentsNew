using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UITroopSelection : MonoBehaviour
{
    private static UITroopSelection _instance;

    [SerializeField] TextMeshProUGUI _amounts;

    private GameObject _confirmButton;
    private GameObject _cancelButton;
    private GameObject _AssignmentModeButton;
    private CameraController _cameraController;

    private int _numTroops = 0;

    private int _numTroopsMax = 11;

    private ExitZone _zone;

    //Singleton
    public static UITroopSelection Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<UITroopSelection>(); ;
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            //if not, set instance to this
            _instance = this;
        }
        //If instance already exists and it's not this:
        else if (_instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
            return;
        }

    }
    //delay to turn everything off once ready
    IEnumerator startDelay()
    {
        while(_confirmButton == null || _cancelButton == null || _AssignmentModeButton == null)
        {
            yield return new WaitForSeconds(0.0001f);
        }
        ShowSelection(false, 0, null);
    }

    void Start()
    {
        GameObject child = transform.GetChild(0).gameObject; //hack
        _amounts= child.GetComponent<TextMeshProUGUI>();
        _cameraController = Camera.main.GetComponent<CameraController>();

        StartCoroutine(startDelay());

    }
    public void setAssignmentModeButton(GameObject go)
    {
        _AssignmentModeButton = go;
    }
    public void gatherButtonChildren(GameObject go, bool isConfirm)
    {
        if (isConfirm)
            _confirmButton = go;
        else
            _cancelButton = go;
    }
    private void setText(string s)
    {
        TextMeshProUGUI text = this.GetComponent<TextMeshProUGUI>();
        if (text)
            text.text = s;
        else
            Debug.LogError("cant find text");
    }
    //used to teleport back from neutral/enemy zone
    public void ShowSelection(bool cond, ExitZone zone)
    {
        if (_confirmButton == null || _cancelButton == null || _AssignmentModeButton == null)
        {
            Debug.LogError("One of The Objects in UI Troop Selection is null");
            return;
        }

        _zone = zone;
        _confirmButton.SetActive(cond);
        _cancelButton.SetActive(cond);
        this.gameObject.SetActive(cond);
        _amounts.text = "";
        setText("Leave Zone?");

    }
    //used to teleport from player zone
    public void ShowSelection(bool cond, int maxTroops, ExitZone zone)
    {
        if(_confirmButton==null || _cancelButton==null || _AssignmentModeButton == null)
        {
            Debug.LogError("One of The Objects in UI Troop Selection is null");
            return;
        }
        _zone = zone;
        _numTroopsMax = maxTroops;
        this.gameObject.SetActive(cond);
        //set the text
        setText("Dispatch Garrison Troops");
        _confirmButton.SetActive(cond);
        _cancelButton.SetActive(cond);
        ShowAssignmentButton(!cond);
        MVCController.Instance.TurnOffBuildMenus();
        UIAssignmentMenu.Instance.ShowOutpostWorkers(cond);
        //order matters here
        if (cond)
        {
            UIAssignmentMenu.Instance.ShowArrowButtons(cond);
            _cameraController.setOverrideMode(cond);
        }
        else
        {
            _cameraController.setOverrideMode(cond);
            UIAssignmentMenu.Instance.ShowArrowButtons(cond);
        }

        updateText();
    }

    public void SelectionMade(bool cond)
    {
        if(cond)
        {
            _zone.confirmed();
        }
        else
        {
            //canceled
        }
        _numTroops= 0;
        // Turn all the outposts back to unselected
        foreach(BuildableObject b in _zone.getOutposts())
        {
            bOutpost outpost = b.gameObject.GetComponent<bOutpost>();
            if(outpost)
            {
                outpost.resetSprite(b.getLevel());
            }
        }

        ShowSelection(false, 0, null);

    }
    public void addTroops(int amnt)
    {
        if (_numTroops + amnt >= 0)
            _numTroops += amnt;
        else
            Debug.LogError("Subtracting more troops than went in??");

        updateText();
    }
    public void updateText()
    {
        _amounts.text = "( " + _numTroops + " / " + _numTroopsMax + " )";
    }
    public void ShowAssignmentButton(bool cond)
    {
        print("Show assignment: " + cond);
        _AssignmentModeButton.SetActive(cond);
    }

}
