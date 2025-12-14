using Cysharp.Threading.Tasks;
using Elements.Configs;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Elements.Systems
{
    public class LevelSystem : MonoBehaviour, IInitializable
    {
        [Inject] private ProjectSettingsConfig projectSettingsConfig;
        [Inject] private SaveSystem saveSystem;

        private bool isLoaded;
        private int levelCount;
        private Data savedData;
        private LevelData levelData;

        public bool IsLoaded => isLoaded;
        public byte[,] ElementsGrig
        {
            get
            {
                byte[,] newArray = new byte[savedData.ElementsGrid.GetLength(0), savedData.ElementsGrid.GetLength(1)];
                Array.Copy(savedData.ElementsGrid, newArray, savedData.ElementsGrid.Length);
                return newArray;
            }
        }

        public void Initialize()
        {
            LoadDataAsync().Forget();
            levelCount = GetLevelCount();
        }

        public async UniTask LoadNextLevel()
        {
            isLoaded = false;
            savedData.LevelID++;
            if (savedData.LevelID >= levelCount)
                savedData.LevelID = 0;

            await LoadLevelAsync();
            await SaveDataAsync();
        }

        public async UniTask RestartLevel()
        {
            ResetDate();
            await SaveDataAsync();
        }

        public async UniTask UpdateLevel(byte[,] values)
        {
            savedData.ElementsGrid = values;
            await SaveDataAsync();
        }

        private async UniTask LoadDataAsync()
        {
            savedData = await saveSystem.LoadAsync(projectSettingsConfig.SavesFileName);
            if (savedData == null)
            {
                savedData = new Data(0, null);
                await LoadLevelAsync();
            }
            else
            {
                isLoaded = true;
            }
        }

        private async UniTask LoadLevelAsync()
        {
            levelData = await GetLevelDataAsync();
            if (levelData != null)
            {
                ResetDate();
                isLoaded = true;
            }
        }

        private void ResetDate()
        {
            savedData.ElementsGrid = levelData.ElementsGrid;
        }

        private async UniTask<LevelData> GetLevelDataAsync()
        {
            LevelData loadedLevelData;
            string fileName = string.Format(projectSettingsConfig.LevelFileNameFormat, savedData.LevelID);
            string fullPath = Path.Combine(Application.streamingAssetsPath, projectSettingsConfig.LevelsFolder, fileName);

            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Файл уровня не найден: {fullPath}");
                return null;
            }

            try
            {
                string jsonText = await File.ReadAllTextAsync(fullPath);
                loadedLevelData = JsonConvert.DeserializeObject<LevelData>(jsonText);
            }
            catch (IOException ex)
            {
                Debug.LogError($"Ошибка чтения файла: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Debug.LogError($"Ошибка парсинга JSON: {ex.Message}");
                return null;
            }

            return loadedLevelData;
        }

        private async UniTask SaveDataAsync()
        {
            await saveSystem.SaveAsync(projectSettingsConfig.SavesFileName, savedData);
        }

        private int GetLevelCount()
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, projectSettingsConfig.LevelsFolder);
            return Directory.EnumerateFiles(fullPath, "*.json").Count();
        }
    }

    [Serializable]
    public class LevelData
    {
        public byte[,] ElementsGrid;
    }
}