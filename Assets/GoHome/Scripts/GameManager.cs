using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Xml.Serialization;
using System.IO;

namespace GoHome
{

    // TASK: Save this data using JSON / XML
    [System.Serializable] // Means it can be converted to another format
    public class GameData
    {
        public int score;
        public int level;
    }
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        public static GameManager Instance;
        void Awake()
        {
            Instance = this;
        }
        void OnDestroy()
        {
            Save();
            Instance = null;
        }
        #endregion
        public GameData data = new GameData();
        public string fileName = "GameSave";

        public int currentScore = 0;
        public int currentLevel = 0;
        public bool isGameRunning = true;
        public Transform levelContainer;


        private Level[] levels;

        private void Start()
        {
            Load();
            // Get all levels within level container
            levels = levelContainer.GetComponentsInChildren<Level>();
            // Set level to current
            SetLevel(currentLevel);

        }

        public void GameOver()
        {

        }

        public void EndGame()
        {

        }

        public void Save()
        {
            data.score = currentScore;
            data.level = currentLevel;
            string fullPath = Application.dataPath + "/GoHome/Data/" + fileName + ".json";
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(fullPath, json);

            print("Saved to path" + fullPath);
        }

        void Load()
        {
            string fullPath = Application.dataPath + "/GoHome/Data/" + fileName + ".json";
            string json = File.ReadAllText(fullPath);
            data = JsonUtility.FromJson<GameData>(json);

            print("Loaded from path: " + fullPath);

            currentScore = data.score;
            currentLevel = data.level;
        }

        // Disables all levels except the levelIndex
        public void SetLevel(int levelIndex)
        {
            // Loop through all levels
            for (int i = 0; i < levels.Length; i++)
            {
                GameObject level = levels[i].gameObject;
                level.SetActive(false); // Disable level
                // Is current index (i) the same as levelIndex?
                if (i == levelIndex)
                {
                    //Enable that level instead
                    level.SetActive(true);
                }
            }
        }

        // Switches to the next level when called
        public void NextLevel()
        {
            // Increase currentlevel
            currentLevel++;
            // If currentLevel exceeds level length
            if (currentLevel >= levels.Length)
            {
                // GameOver

            }
            else
            {
                // Set current level
                SetLevel(currentLevel);
            }
        }
        public void NewGame()
        {
            // Get currentScen
            Scene currentScene = SceneManager.GetActiveScene();
            // Reload it
            SceneManager.LoadScene(currentScene.name);
        }
    }
}
