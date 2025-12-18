using Cysharp.Threading.Tasks;
using Elements.Configs;
using Newtonsoft.Json;
using R3;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Elements.Systems
{
    public class LevelSystem : MonoBehaviour
    {
        [Inject] private ProjectSettingsConfig projectSettingsConfig;
        [Inject] private SaveSystem saveSystem;

        private int levelCount;
        private bool isLoading;
        private Data savedData;
        private LevelData levelData;
        private readonly ReactiveProperty<bool> isLoaded = new ReactiveProperty<bool>(false);

        public ushort CurrentLevelIndex => savedData?.LevelID ?? 0;
        public ReadOnlyReactiveProperty<bool> IsLoaded => isLoaded;
        public byte[,] ElementsGrig
        {
            get
            {
                byte[,] newArray = new byte[savedData.ElementsGrid.GetLength(0), savedData.ElementsGrid.GetLength(1)];
                Array.Copy(savedData.ElementsGrid, newArray, savedData.ElementsGrid.Length);
                return newArray;
            }
        }

        [Inject]
        public void Initialize()
        {
            LoadDataAsync().Forget();
            levelCount = GetLevelCount();
        }

        public async UniTaskVoid LoadNextLevel()
        {
            IncrementLevelIndex();
            await LoadLevelNotifyAsync(true, true);
        }

        public async UniTaskVoid QuietLoadNextLevel()
        {
            IncrementLevelIndex();
            await LoadLevelNotifyAsync(false, true);
        }

        public async UniTaskVoid RestartLevel()
        {
            await LoadLevelNotifyAsync(true, false);
        }

        public async UniTaskVoid UpdateLevel(byte[,] values)
        {
            savedData.ElementsGrid = values;
            await SaveDataAsync(destroyCancellationToken);
        }

        private void IncrementLevelIndex()
        {
            savedData.LevelID++;
            if (savedData.LevelID >= levelCount)
                savedData.LevelID = 0;
        }

        private async UniTask LoadDataAsync()
        {
            savedData = await saveSystem.LoadAsync(projectSettingsConfig.SavesFileName, destroyCancellationToken);
            if (savedData == null)
            {
                savedData = new Data(0, null);
                await LoadLevelDataAsync(true, destroyCancellationToken);
            }

            isLoaded.Value = true;
        }

        private async UniTask LoadLevelNotifyAsync(bool withNotify, bool newLevel)
        {
            if (withNotify) isLoaded.Value = false;
            await LoadLevelDataAsync(newLevel, destroyCancellationToken);
            await SaveDataAsync(destroyCancellationToken);
            if (withNotify) isLoaded.Value = true;
        }

        private async UniTask LoadLevelDataAsync(bool newLevel, CancellationToken cancellationToken)
        {
            if (isLoading) return;
            isLoading = true;

            bool needLoad = newLevel || levelData == null;
            levelData = needLoad ? await GetLevelDataAsync(cancellationToken) : levelData;
            if (levelData != null)
            {
                savedData.ElementsGrid = levelData.ElementsGrid;
            }
            isLoading = false;
        }

        private async UniTask<LevelData> GetLevelDataAsync(CancellationToken cancellationToken)
        {
            string fileName = string.Format(projectSettingsConfig.LevelFileNameFormat, savedData.LevelID);
            string fullPath = Path.Combine(Application.streamingAssetsPath, projectSettingsConfig.LevelsFolder, fileName);

            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Файл уровня не найден: {fullPath}");
                return null;
            }

            LevelData loadedLevelData = null;
            try
            {
                string jsonText = await File.ReadAllTextAsync(fullPath, cancellationToken);
                loadedLevelData = JsonConvert.DeserializeObject<LevelData>(jsonText);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Операция была отменена");
            }
            catch (Exception ex)
            {
                Debug.LogError("Ошибка загрузки файла: " + ex.Message);
            }

            return loadedLevelData;
        }

        private async UniTask SaveDataAsync(CancellationToken cancellationToken)
        {
            await saveSystem.SaveAsync(projectSettingsConfig.SavesFileName, savedData, cancellationToken);
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