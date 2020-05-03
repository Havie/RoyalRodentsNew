using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInstantConstruct : MonoBehaviour
{


    public void InstantContruction()
    {
        if (ResourceManagerScript.Instance.GetResourceCount(ResourceManagerScript.ResourceType.Shiny) >= 1)
        {
            //Get parented building
            Transform parent = this.transform.parent;
            if (parent)
            {   // get grandparent
                Transform gparent = parent.transform.parent;
                if (gparent)
                {
                    BuildableObject b = gparent.GetComponent<BuildableObject>();
                    if (b)
                    {
                        b.IncrementConstruction(100);
                        ResourceManagerScript.Instance.incrementResource(ResourceManagerScript.ResourceType.Shiny, -1);
                        parent.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

}
