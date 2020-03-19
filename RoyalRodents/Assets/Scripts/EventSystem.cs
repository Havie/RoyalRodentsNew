using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventSystem : MonoBehaviour
{
   private static EventSystem _instance;


    //publisher
    public delegate void OnMessageRecieved();
    public event OnMessageRecieved onComplete; //thing that tells the publisher to fire

    public delegate void SpawnEnemies();
    public event SpawnEnemies spawnTrigger;

    public delegate void rodentDied(Rodent r);
    public event rodentDied rodentDead; // this event fires the Delegate
                                        // in a backwards way its like the delegate has a return type of r 
                                        // because it can only call methods subscribed to it that take in a rodent

    public static EventSystem Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<EventSystem>();
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

    // Start is called before the first frame update
    void Start()
    {
        onComplete += WriteMessage;

        OnMessageRecieved msg =Test;

        msg();

        this.transform.parent = GameManager.Instance.transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (spawnTrigger != null)
            {
                spawnTrigger();
            }
        }
    }

    public void IDied(Rodent r)
    {
        if (rodentDead != null)
            rodentDead(r);
    }
    public void SpawnRodents()
    {
        if (spawnTrigger != null)
        {
            spawnTrigger();
            print("Spawned");
        }
    }

    void Test()
    {
        print("Did Test");
    }

    void WriteMessage()
    {
        print("WriteMessage");
    }
}
