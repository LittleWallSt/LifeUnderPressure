using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class DataManager
{
    private static List<DataStruct> Data;

    private static Action OnSaveData;
    private static string FilePath;

    public static void Init()
    {
        FilePath = Application.persistentDataPath + "/testsave00.sav";
        LoadData();
    }

    public static void SaveData()
    {
        Call_OnSaveData();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(FilePath);
        bf.Serialize(file, Data);
        file.Close();
        UnityEngine.Debug.Log("Data saved in binary format at " + FilePath);
    }

    public static IEnumerator LoadData()
    {
        if (File.Exists(FilePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(FilePath, FileMode.Open);
            Data = (List<DataStruct>)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            UnityEngine.Debug.Log("No save data found");
            Data = new List<DataStruct>();
        }
        yield return null;
    }
    private static void Add(string name, int value)
    {
        Data.Add(new DataStruct(name, value));
    }
    private static void Update(string name, int value)
    {
        for (int i = 0; i < Data.Count; i++)
        {
            if (Data[i].name == name)
            {
                DataStruct d = Data[i];
                d.value = value;
                Data[i] = d;
                break;
            }
        }
    }
    public static void Write(string name, int value)
    {
        if (Exist(name))
        {
            Update(name, value);
            return;
        }
        Add(name, value);
    }
    public static int Get(string name, int defaultValue)
    {
        foreach(DataStruct dataStruct in Data)
        {
            if (dataStruct.name == name) return dataStruct.value;
        }
        return defaultValue;
    }
    public static bool Exist(string name)
    {
        foreach(DataStruct dataStruct in Data)
        {
            if(dataStruct.name == name) return true;
        }
        return false;
    }
    public static void Clear()
    {
        if (!File.Exists(FilePath + "/testsave00.sav")) return;
        File.Delete(FilePath + "/testsave00.sav");
    }
    public static void Reset()
    {
        Data = null;
        OnSaveData = null;
    }
    // Action
    public static void Assign_OnSaveData(Action action)
    {
        OnSaveData += action;
    }
    public static void Remove_OnSaveData(Action action)
    {
        OnSaveData -= action;
    }
    private static void Call_OnSaveData()
    {
        if (OnSaveData != null) OnSaveData();
    }
    // Data Struct
    [Serializable]
    public struct DataStruct
    {
        public string name;
        public int value;
        public DataStruct(string name, int value)
        {
            this.name = name;
            this.value = value;
        }
    }
}
