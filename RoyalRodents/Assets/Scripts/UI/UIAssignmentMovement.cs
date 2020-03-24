using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAssignmentMovement : MonoBehaviour
{
    public bool _right;
    private CameraController _CC;
    private float _Amount = 0.5f;
    private bool _Active;
    private bool _MobileMode;

    private void Start()
    {
       _CC= Camera.main.GetComponent<CameraController>();
        if (_CC == null)
            Debug.LogError("Cant Find Camera Controller");

        _MobileMode = GameManager.Instance.getMobileMode();
    }
    private void Update()
    {
        if(_Active)
        {
            if (!_MobileMode && Input.GetMouseButtonUp(0))
                _Active = false;
            
            else if(_MobileMode && Input.touchCount == 0)
                _Active = false;


            TellToMove();
        }
    }

    private void TellToMove()
    {
        if (_right)
        {
            _CC.MoveCamera(_Amount);
        }
        else
        {
            _CC.MoveCamera(-_Amount);
        }
    }

    public void imClicked()
    {
        _Active = true;
    }
}
