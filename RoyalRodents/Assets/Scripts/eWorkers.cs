using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eWorkers : MonoBehaviour
{
    [SerializeField]
    private Employee[] _Workers;

    // Start is called before the first frame update
    void Awake()
    {
        //count how many Employees are in Children
        _Workers= GetComponentsInChildren<Employee>();
    }

    public Employee[] getWorkers()
    {
        return _Workers;
    }
}
