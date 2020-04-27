using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    ExitZone _parent;
    private float _teleportX; // must be set in scene
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

    public void Teleport(List<GameObject> objects)
    {
        GameManager.Instance.setCurrentZone(_goToZone, _goToZoneIsRight);
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        //Tell the Camera its new bounds 
        _cameraController.ChangeZone(_goToZone, _goToZoneIsRight);

        foreach (var g in objects)
        {
            g.transform.position = new Vector3(_teleportX, g.transform.position.y, 0);
            
            //if Player tell him to update his Zone
            if(g==player)
            {
                PlayerStats ps = g.GetComponent<PlayerStats>();
                if (ps)
                    ps.UpdateInPlayerZone();
            }
        }

    }
}
