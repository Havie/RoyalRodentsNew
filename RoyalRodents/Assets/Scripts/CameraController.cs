using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform _target;
    private PlayerMovement _playerMovement;
    private Vector3 _offset;
    private float _moveSpeed = 0.5f;
    private float horizontalMove;

    private bool _CharacterMode=true;


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
            this.transform.position = new Vector3(_target.position.x, transform.position.y, transform.position.z);
        else // move freely
        {
            //To-Do: Need some kind of bounds check for Fog of War 


            horizontalMove = Input.GetAxisRaw("Horizontal") * _moveSpeed;
            this.transform.position += new Vector3(horizontalMove, 0, 0);
        }

    }




    public void setCharacterMode(bool cond)
    {
        _CharacterMode = cond;
        if (_playerMovement)
        {
            if (_CharacterMode)
                _playerMovement.setControlled(true);
            else
                _playerMovement.setControlled(false);
        }
        else
            Debug.LogError("No PLayerMovement for Camera Controller");
    }
}
