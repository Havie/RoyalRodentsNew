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

        FileStream stream = new FileStream(path, FileMode.Create); // appears to overwrite old

        sPlayerData data = new sPlayerData(ps, pm);

        formatter.Serialize(stream, data);
        stream.Close();

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
        FileStream stream = new FileStream(path, FileMode.Create);

        if (buildings == null)
            Debug.LogWarning("Buildings are Null?");
        sBuildingData data = new sBuildingData(buildings);

        formatter.Serialize(stream, data);
        stream.Close();

    }

    public static sBuildingData LoadBuildingData()
    {
        string path = Application.persistentDataPath + "/buildings.txt";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            sBuildingData data = formatter.Deserialize(stream) as sBuildingData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Cant find building file in" + path);
            return null;
        }
    }

    public static void SaveRodents(List<Rodent> rodents)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/rodents.txt";
        FileStream stream = new FileStream(path, FileMode.Create);

        if (rodents == null)
            Debug.LogWarning("rodents are Null?");
        sRodentData data = new sRodentData(rodents);

        formatter.Serialize(stream, data);
        stream.Close();

    }
    public static sRodentData LoadRodentData()
    {
        string path = Application.persistentDataPath + "/rodents.txt";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            sRodentData data = formatter.Deserialize(stream) as sRodentData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Cant find RodentData file in" + path);
            return null;
        }
    }
    public static void SaveResources(ResourceManagerScript rm)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/resources.txt";
        FileStream stream = new FileStream(path, FileMode.Create);

        if (rm == null)
            Debug.LogWarning("rodents are Null?");
        sResourceData data = new sResourceData(rm);

        formatter.Serialize(stream, data);
        stream.Close();

    }
    public static sResourceData LoadResources()
    {
        string path = Application.persistentDataPath + "/resources.txt";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            sResourceData data = formatter.Deserialize(stream) as sResourceData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Cant find ResourceData file in" + path);
            return null;
        }
    }
    public static void SaveTime( )
    {
        //To-Do:

    }
    public static void LoadTime( )
    {
        //To-Do:

    }




    //JSon
    public static void LoadData()
    {
        var data = PlayerPrefs.GetString("PlayerData");
        JsonUtility.FromJson<sPlayerData>(data);
    }

}


