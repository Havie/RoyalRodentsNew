using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RodentNames : MonoBehaviour
{
    private static string[] _Names = new string[33];
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
        _Names[10] = "Randall";
        _Names[11] = "Gerald";
        _Names[12] = "Peanut";
        _Names[13] = "Roy";
        _Names[14] = "Albert";
        _Names[15] = "Bernard";
        _Names[16] = "Edgar";
        _Names[17] = "Gus";
        _Names[18] = "Herbert";
        _Names[19] = "Howard";
        _Names[20] = "Leonard";
        _Names[21] = "Murray";
        _Names[22] = "Stanley";
        _Names[23] = "Walter";
        _Names[24] = "Rusty";
        _Names[25] = "Cheddar";
        _Names[26] = "Whiskers";
        _Names[27] = "Scrat";
        _Names[28] = "Furball";
        _Names[29] = "Almond";
        _Names[30] = "Potato";
        _Names[31] = "Jiff";
        _Names[32] = "Chad";
        _Initialized = true;
    }
}
