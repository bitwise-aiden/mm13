using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

class PlayerSaveDataController : MonoBehaviour
{
    [Serializable]
    public struct SceneData
    {
        public SceneName name;
    }

    [Serializable]
    public class PlayerData
    {
        public List<SceneData> visitedScenes;
        public SceneName currentScene;

        public PlayerData()
        {
            this.visitedScenes = new List<SceneData>();
        }
    }

    public delegate void OnLoad(PlayerData data);
    public OnLoad onLoad;

    public SceneName defaultScene;
    public string saveName = "save_file";

    private PlayerData data;


    // Lifecycle methods
    void Awake()
    {
        this.Load();
    }

    // Public methods

    public void VisitScene(SceneName scene)
    {
        if (!this.HasVisitedScene(scene))
        {
            this.data.visitedScenes.Add(new SceneData { name = scene });
        }

        this.data.currentScene = scene;
    }

    public bool HasVisitedScene(SceneName scene)
    {
        foreach (var visitedScene in this.data.visitedScenes)
        {
            if (scene == visitedScene.name) return true;
        }

        return false;
    }

    public void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + this.saveName;

        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, this.data);

        File.WriteAllText(path + ".json", JsonUtility.ToJson(this.data));

        Debug.Log("Saving");

        stream.Close();
    }

    public void Load()
    {
        string path = Application.persistentDataPath + "/" + this.saveName;

        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            this.data = formatter.Deserialize(stream) as PlayerData;

            stream.Close();
        }
        else
        {
            this.data = new PlayerData();
            this.data.currentScene = this.defaultScene;
        }

        if (this.onLoad != null)
        {
            this.onLoad(this.data);
        }
    }

}
