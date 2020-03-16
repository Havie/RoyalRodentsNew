using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public static class sSaveSystem 
{

    public static void SavePlayer(PlayerStats ps, PlayerMovement pm)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerdata.txt";

        FileStream steam = new FileStream(path, FileMode.Create); // appears to overwrite old

        sPlayerData data = new sPlayerData(ps, pm);

        formatter.Serialize(steam, data);
        steam.Close();

    }
    public static sPlayerData LoadPlayerData()
    {

        string path = Application.persistentDataPath + "/playerdata.txt";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            sPlayerData data= formatter.Deserialize(stream) as sPlayerData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Cant find player file in" + path);
            return null;
        }
    }

    public static void SaveBuildings(BuildableObject[] buildings)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/buildings.txt";
        FileStream steam = new FileStream(path, FileMode.Create);


    }







    //JSon
    public static void LoadData()
    {
        var data = PlayerPrefs.GetString("PlayerData");
        JsonUtility.FromJson<sPlayerData>(data);
    }

}


