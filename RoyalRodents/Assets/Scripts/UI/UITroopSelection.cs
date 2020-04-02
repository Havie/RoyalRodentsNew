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
            yield return new WaitForSeconds(0.05f);
        }
        ShowSelection(false);
    }

    void Start()
    {
        _amounts= GetComponentInChildren<TextMeshProUGUI>();
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
    public void ShowSelection(bool cond)
    {
        if(_confirmButton==null || _cancelButton==null || _AssignmentModeButton == null)
        {
            Debug.LogError("One of The Objects in UI Troop Selection is null");
            return;
        }

        this.gameObject.SetActive(cond);
        _confirmButton.SetActive(cond);
        _cancelButton.SetActive(cond);
        _AssignmentModeButton.SetActive(!cond);
        MVCController.Instance.TurnThingsoff();
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
    }

    public void SelectionMade(bool cond)
    {
        if(cond)
        {
            // confirmed
        }
        else
        {
            //canceled
        }

        ShowSelection(false);
    }

    
}
