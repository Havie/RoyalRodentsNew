using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    ExitZone _parent;
    private float _teleportX; // gotten from the desitnation obj
    CameraController _cameraController;

    public GameObject _destinationObject;  //used to teleport to position

    public int _goToZone;
    public bool _goToZoneIsRight;

    // Start is called before the first frame update
    void Start()
    {
        _parent = this.transform.parent.GetComponent<ExitZone>();
        _cameraController = Camera.main.GetComponent<CameraController>();

        if (_destinationObject)
            _teleportX = _destinationObject.transform.position.x;

        else
        {
            _teleportX = 0;
            Debug.Log("teleportDestination for teleporter script not assigned properly");
        }
    }

   public void imClicked()
    {
        // TO-DO:
        // check time of day to see if player goes with royal guard or brings army

        //print("Clicked me:" + this.gameObject);

        //get the outposts from parent
        if(_parent)
            _parent.childClicked(this);


    }

    public void startInvasion()
    {
        SpawnVolume spawner=this.GetComponent<SpawnVolume>();
        if(spawner)
        {
            spawner.SpawnSomething();
        }
    }

    public void Teleport(List<GameObject> objects, bool shutDown)
    {
        GameManager.Instance.setCurrentZone(_goToZone, _goToZoneIsRight);
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerStats ps = player.GetComponent<PlayerStats>();
        if (ps)
        {
            //If the player is in his zone when the enemy king dies, do not teleport
            if ((shutDown && !ps.inPlayerZone()) || (!shutDown))
            {

                //Tell the Camera its new bounds 
                if (_cameraController)
                    _cameraController.ChangeZone(_goToZone, _goToZoneIsRight);
                else
                    Debug.LogError("Wth? why is there no camera for " + this.gameObject.name);

                foreach (var g in objects)
                {
                    g.transform.position = new Vector3(_teleportX, g.transform.position.y, 0);

                    //if Player tell him to update his Zone
                    if (g == player)
                    {
                       ps.UpdateInPlayerZone();
                    }
                }
            }
        }
        if(shutDown)
        {
            //turn off way to get back
            _destinationObject.SetActive(false);
        }

    }
    
}
