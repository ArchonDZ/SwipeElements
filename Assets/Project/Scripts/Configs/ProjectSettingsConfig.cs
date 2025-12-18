using System.IO;
using System.Linq;
using UnityEngine;

namespace Elements.Configs
{
    [CreateAssetMenu(fileName = nameof(ProjectSettingsConfig), menuName = "Configs/" + nameof(ProjectSettingsConfig))]
    public class ProjectSettingsConfig : ScriptableObject
    {
        [SerializeField] private string savesFileName = "save.dat";
        [SerializeField] private string levelsFolder = "Levels";
        [SerializeField] private string levelFileNameFormat = "level_{0}.json";
        [SerializeField] private int levelCount = 0;

        public string SavesFileName => savesFileName;
        public string LevelsFolder => levelsFolder;
        public string LevelFileNameFormat => levelFileNameFormat;
        public int LevelCount => levelCount;

#if UNITY_EDITOR
        [ContextMenu("Update Level Count")]
        public void UpdateIndex()
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, levelsFolder);
            if (!Directory.Exists(fullPath))
            {
                Debug.LogError("Директория не существует!");
                return;
            }

            levelCount = Directory.EnumerateFiles(fullPath, "*.json").Count();
        }
#endif
    }
}