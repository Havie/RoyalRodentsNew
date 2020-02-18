using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RodentNames : MonoBehaviour
{
    private static string[] _Names = new string[10];
    private static bool _Initialized;


    void Awake()
    {
        Initalize();
    }


    public static string getRandomName()
    {
        if (!_Initialized)
            Initalize();
        int number = Random.Range(0, _Names.Length - 1);
        return _Names[number];

    }

    private static void Initalize()
    {
        _Names[0] = "Fred";
        _Names[1] = "Willace";
        _Names[2] = "Wallace";
        _Names[3] = "Theodore";
        _Names[4] = "Jeff";
        _Names[5] = "Sam";
        _Names[6] = "Thomas";
        _Names[7] = "Brufred";
        _Names[8] = "Jimmy";
        _Names[9] = "Timmy";
        _Initialized = true;
    }
}
