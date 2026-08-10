using System.IO;
using UnityEngine;

public static class IllnessSaveSystem
{
    public static void Save(IllnessDatabase database)
    {
        string json = JsonUtility.ToJson(database, true);

        string path =
            Application.persistentDataPath +
            "/GeneratedIllnesses.json";

        File.WriteAllText(path, json);
    }
}