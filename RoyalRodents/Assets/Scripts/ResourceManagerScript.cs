using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManagerScript : MonoBehaviour
{
    //create resource variables
    private int food, trash, wood, metal, shiny;

    //create rodent recruited array list
    public GameObject prefab;
    List<GameObject> goList;

    //setters and getters for resource variable properties
    public int Food
    {
        get
        {
            return food;
        }
        set
        {
            food = value;
        }
    }
    public int Trash
    {
        get
        {
            return trash;
        }
        set
        {
            trash = value;
        }
    }

    public int Wood
    {
        get
        {
            return wood;
        }
        set
        {
            wood = value;
        }
    }

    public int Metal
    {
        get
        {
            return metal;
        }
        set
        {
            metal = value;
        }
    }

    public int Shiny
    {
        get
        {
            return shiny;
        }
        set
        {
            shiny = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        food = 0;
        trash = 0;
        wood = 0;
        metal = 0;
        shiny = 0;

        goList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        //add object to rodent recruit array list
        if (Input.GetKeyDown(KeyCode.I) {
            // This random position is for fun :D
            Vector3 rndPos = new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), Random.Range(-20, 20));

            // Create a new GameObject from prefab and move to random position
            goList.Add((GameObject)Instanciate(prefab, rndPos, Quaternion.Identity);
        }

        //Hold L to show recruit array list
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(goList.Count);
            foreach (GameObject go in goList)
            {
                Debug.Log(go.name);
            }
        }
    }

    /*
    private class Resource
    {
        public enum ResourceType
        {
            Food,
            Trash,
            Wood,
            Metal,
            Shiny
        }
    }
    */
}
