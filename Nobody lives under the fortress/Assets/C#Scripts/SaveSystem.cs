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
    private Dictionary<string, bool> availableStipulationPool = new Dictionary<string, bool>();

    public Profile profile;
    private void Start()
    {
        CreateNewProfile();
        SaveProfile();
    }
    public void CreateNewProfile()
    {
        foreach (var st in stipulationPool.pool)
        {
            availableStipulationPool.Add(st, false);
        }
        profile = new Profile(profileName, 0, availableStipulationPool);
    }
    public void SaveProfile()
    {
        string json = JsonUtility.ToJson(profile);
        File.WriteAllText(Path.Combine(Application.dataPath, "Resources/" + profileName + ".json"), json);
    }
    public void LoadProfile()
    {
        {
            string str = File.ReadAllText(Path.Combine(Application.dataPath, "Resources/" + profileName + ".json"));
            profile = JsonUtility.FromJson<Profile>(str);

            // Преобразуем список в словарь
            // profile.availableStipulationPool = profile.availableStipulationPool.ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
[Serializable]
public class Profile
{
    public string profileName;
    public int lastNode;
    public List<KeyValuePair<string, bool>> availableStipulationPool; // Заменяем на List<KeyValuePair<string, bool>>

    public Profile(string ProfileName, int LastNode, Dictionary<string, bool> AvailableStipulationPool)
    {
        profileName = ProfileName;
        lastNode = LastNode;

        availableStipulationPool = new List<KeyValuePair<string, bool>>(AvailableStipulationPool);
    }
}
