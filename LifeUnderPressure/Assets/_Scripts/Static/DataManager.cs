using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

public static class DataManager
{
    private static List<DataStruct> Data;
    private static List<DataStruct> SettingsData;

    private static Action OnSaveData;

    private static string Profile = "main";
    private static string SettingsProfile = "settings";

    private static string FilePath;
    private static string SettingsPath = Application.persistentDataPath + "/" + SettingsProfile + ".sav";

    public static void Init()
    {
        UpdateFilePath();
        LoadAllData();
    }
    public static void SetProfile(string profile)
    {
        if (profile != Profile) Reset();
        Profile = profile;
        UpdateFilePath();
    }
    private static void UpdateFilePath()
    {
        FilePath = Application.persistentDataPath + "/" + Profile + ".sav";
    }
    public static void SaveMainData()
    {
        Call_OnSaveData();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(FilePath);
        bf.Serialize(file, Data);
        file.Close();
        UnityEngine.Debug.Log("Main Data saved in binary format at " + FilePath);
    }
    public static void SaveSettingsData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(SettingsPath);
        bf.Serialize(file, SettingsData);
        file.Close();
        UnityEngine.Debug.Log("Settings Data saved in binary format at " + SettingsPath);
    }

    public static IEnumerator LoadAllData()
    {
        LoadSaveFile(ref Data, FilePath);
        LoadSaveFile(ref SettingsData, SettingsPath);
        yield return null;
    }
    private static void LoadSaveFile(ref List<DataStruct> Dataset, string Path)
    {
        Dataset = new List<DataStruct>();
        if (!File.Exists(Path)) return;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Path, FileMode.Open);
        Dataset = (List<DataStruct>)bf.Deserialize(file);
        file.Close();
    }
    private static void Add(string name, int value, ref List<DataStruct> dataset)
    {
        dataset.Add(new DataStruct(name, value));
    }
    private static void Update(string name, int value, ref List<DataStruct> dataset)
    {
        for (int i = 0; i < dataset.Count; i++)
        {
            if (dataset[i].name == name)
            {
                DataStruct d = dataset[i];
                d.value = value;
                dataset[i] = d;
                break;
            }
        }
    }
    public static void Write(string name, int value)
    {
        if (Exist(name, Data))
        {
            Update(name, value, ref Data);
            return;
        }
        Add(name, value, ref Data);
    }
    public static void WriteSettings(string name, int value)
    {
        if (Exist(name, SettingsData))
        {
            Update(name, value, ref SettingsData);
            return;
        }
        Add(name, value, ref SettingsData);
    }
    public static int Get(string name, int defaultValue)
    {
        foreach(DataStruct dataStruct in Data)
        {
            if (dataStruct.name == name) return dataStruct.value;
        }
        return defaultValue;
    }
    public static int GetSettings(string name, int defaultValue)
    {
        foreach(DataStruct dataStruct in SettingsData)
        {
            if (dataStruct.name == name) return dataStruct.value;
        }
        return defaultValue;
    }
    public static bool Exist(string name, List<DataStruct> dataset)
    {
        foreach(DataStruct dataStruct in dataset)
        {
            if(dataStruct.name == name) return true;
        }
        return false;
    }
    public static bool Remove(string name)
    {
        if (!Exist(name, Data)) return false;

        DataStruct dataToRemove = new DataStruct() { name = "NULL" };
        foreach (DataStruct dataStruct in Data)
        {
            if (dataStruct.name == name)
            {
                dataToRemove = dataStruct;
                break;
            }
        }
        if(dataToRemove.name != "NULL")
        {
            Data.Remove(dataToRemove);
            return true;
        }
        return false;
    }
    public static void Clear()
    {
        if (!File.Exists(FilePath)) return;

        File.Delete(FilePath);
        Reset();
    }
#if UNITY_EDITOR
    [MenuItem("LUP/Clear Main Save")]
    public static void ClearMainSave()
    {
        string filePath = Application.persistentDataPath + "/" + Profile + ".sav";
        if (!File.Exists(filePath)) return;

        File.Delete(filePath);
        Reset();
    }
#endif
    public static void Reset()
    {
        Data = new List<DataStruct>();
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
