using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveManager
{
    public static void SaveData(int time)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Game.spaceArena";

        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData playerData = new PlayerData(time);

        formatter.Serialize(stream, playerData);
        stream.Close();
    }

    public static PlayerData LoadData()
    {
        string path = Application.persistentDataPath + "/Game.spaceArena";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;

            stream.Close();

            return data;
        }

        Debug.LogError("Error: Save file not found in " + path);
        return null;
    }
}

[Serializable]
public class PlayerData
{
    public int saveTimePlayed;

    public PlayerData(int time)
    {
        saveTimePlayed = time;
    }
}