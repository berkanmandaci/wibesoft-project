using UnityEngine;
using WibeSoft.Core.Singleton;

namespace WibeSoft.Core.Managers
{
    public class SaveManager : SingletonBehaviour<SaveManager>
    {
        public void SaveGameData()
        {
            Debug.Log("Saving game data...");
        }

        public void LoadGameData()
        {
            Debug.Log("Loading game data...");
        }

        private string GetSavePath()
        {
            return Application.persistentDataPath + "/save.json";
        }
    }
} 