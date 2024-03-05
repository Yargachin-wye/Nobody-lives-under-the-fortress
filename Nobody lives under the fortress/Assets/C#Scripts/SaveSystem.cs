using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.TextCore.Text;

public class SaveSystem : MonoBehaviour
{
    string profileName = "Profile";
    [SerializeField] private StipulationPool stipulationPool;
    private Dictionary<string, int> availableStipulationPool = new Dictionary<string, int>();

    [SerializeField] public Profile profile;
    string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, profileName + ".json");
    }

    void OnApplicationPause(bool pauseStatus)
    {
        SaveProfile();
    }
    void OnApplicationQuit()
    {
        SaveProfile();
    }
    public void CreateNewProfile()
    {
        Debug.Log(stipulationPool.pool[0]);
        availableStipulationPool = new Dictionary<string, int>();
        foreach (var st in stipulationPool.pool)
        {
            Debug.Log(st);
            availableStipulationPool.Add(st, 0);
        }
        profile = new Profile(profileName, 0, availableStipulationPool, new List<int>());
        SaveProfile();
    }

    public void SaveProfile()
    {
        string json = JsonUtility.ToJson(profile);
        File.WriteAllText(filePath, json);
    }
    public void LoadProfile()
    {
        CreateNewProfile();
        /*
        if (File.Exists(filePath))
        {
            CreateNewProfile();
        }
        else
        {
            string str = File.ReadAllText(filePath);
            profile = JsonUtility.FromJson<Profile>(str);
        }*/
    }
    public void AddGift(string str)
    {
        foreach (var stipulation in profile.stipulations)
        {
            if (stipulation.key == str)
            {
                stipulation.value++;
            }
        }
        SaveProfile();
    }
    public int GetGift(string str)
    {
        foreach (var stipulation in profile.stipulations)
        {
            if (stipulation.key == str)
            {
                return stipulation.value;
            }
        }
        return 0;
    }
}

[Serializable]
public class Profile
{
    public string profileName;
    public int lastNode;
    public List<Stipulation> stipulations;
    public List<int> unrepeatable;
    public Profile(string ProfileName, int LastNode, Dictionary<string, int> AvailableStipulationPool, List<int> UnRepeatable)
    {
        profileName = ProfileName;
        lastNode = LastNode;
        unrepeatable = UnRepeatable;

        stipulations = new List<Stipulation>();
        foreach (var kvp in AvailableStipulationPool)
        {

            stipulations.Add(new Stipulation(kvp.Key, kvp.Value));
        }
    }
}
[Serializable]
public class Stipulation
{
    public string key;
    public int value;

    public Stipulation(string key, int value)
    {
        this.key = key;
        this.value = value;
    }
}