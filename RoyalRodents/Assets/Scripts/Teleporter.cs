using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    ExitZone _parent;

    // Start is called before the first frame update
    void Start()
    {
        _parent = this.transform.parent.GetComponent<ExitZone>();
    }

   public void imClicked()
    {
        // TO-DO:
        // check time of day to see if player goes with royal guard or brings army

        print("Clicked me:" + this.gameObject);

        //get the outposts from parent
        if(_parent)
            _parent.childClicked(this);
        // show some menu that lets you select which outpost to pool from
        // outpost should show # of employees in button

       // startInvasion();

    }

    public void startInvasion()
    {
        SpawnVolume spawner=this.GetComponent<SpawnVolume>();
        if(spawner)
        {
            spawner.SpawnSomething();
        }
    }

    public void Teleport(List<GameObject> objects, Vector3 loc)
    {

        foreach (var g in objects)
        {
            g.transform.position = loc;
        }

    }
}
