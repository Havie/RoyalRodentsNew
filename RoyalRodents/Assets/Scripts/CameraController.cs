using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform _target;
    private PlayerMovement _playerMovement;
    private Vector3 _offset;

    private float _NegYCap = -194;
    private float _NegXCap = -54;
    private float _PosXCap = 15;

    private bool _CharacterMode=true;
    private bool _OverrideMode = false; //Really should have been on GameManger


    // Start is called before the first frame update
    void Start()
    {
        if (_target == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            if (go)
            {
                _target = go.transform;
                _playerMovement = go.GetComponent<PlayerMovement>();
            }
            else
                Debug.LogError("Camera has no target will crash");
        }

        if(_target)
            setCharacterMode(true);

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(_CharacterMode)
            this.transform.position = new Vector3(_target.position.x, _target.transform.position.y, transform.position.z);

       checkInBounds();

    }
    //AssignmentMode
    public void MoveCamera(float amount)
    {
        if(!_CharacterMode)
         this.transform.position += new Vector3(amount, 0, 0);
    }
    //Used to Jump Camera for Notification Buttons
    public void SetCameraX(float posX)
    {
        if (!_CharacterMode)
            this.transform.position = new Vector3(posX, transform.position.y, transform.position.z);
    }
    //used by outpost to lock us into assignment mode without allowing UiAssignment Interaction
    public void setOverrideMode(bool cond)
    {
        _OverrideMode = cond;

        _CharacterMode = !cond;
        if (_playerMovement)
        {
            if (_CharacterMode)
                _playerMovement.setControlled(true);
            else
                _playerMovement.setControlled(false);
        }
    }
    public bool getOverrideMode()
    {
        return _OverrideMode;
    }

    public void setCharacterMode(bool cond)
    {
        if (!_OverrideMode)
        {
            _CharacterMode = cond;
            if (_playerMovement)
            {
                if (_CharacterMode)
                    _playerMovement.setControlled(true);
                else
                    _playerMovement.setControlled(false);
            }
        }
    }
    private void checkInBounds()
    {
        if(this.transform.position.x > _PosXCap)
            this.transform.position = new Vector3(_PosXCap, this.transform.position.y, this.transform.position.z);
        else if (this.transform.position.x < _NegXCap)
            this.transform.position = new Vector3(_NegXCap, this.transform.position.y, this.transform.position.z);

        if(this.transform.position.y < _NegYCap)
            this.transform.position = new Vector3(this.transform.position.x, _NegYCap, this.transform.position.z);
    }

    //Going to have to get these numbers to the subject script to prevent wandering off?
    // or just make it so the rodents dont get close to the edges
    public void ChangeZone(int zone, bool isRight)
    {
        //Stop Player from wandering off screen
        if(_playerMovement)
            _playerMovement.StopMoving();

        if (zone == 1) // player
        {
            _NegXCap = -54.7f;
            _PosXCap = 15;
        }
        else if (zone == 0) // neutral
        {
            if (isRight)
            {
                _NegXCap = 58;
                _PosXCap = 128;
            }
            else
            {
                _NegXCap = -166;
                _PosXCap = -96;
            }
        }
        else if (zone == 2) // enemy
        {
            if (isRight)
            {
                _NegXCap = 172;
                _PosXCap = 242;
                //Spawn an Enemy King
                EventSystem.Instance.SpawnKing("right");
            }
            else
            {
                _NegXCap = -275;
                _PosXCap = -205;
                EventSystem.Instance.SpawnKing("left");
            }
        }

    }
}

