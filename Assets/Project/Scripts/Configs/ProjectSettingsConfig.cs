using UnityEngine;

namespace Elements.Configs
{
    [CreateAssetMenu(fileName = nameof(ProjectSettingsConfig), menuName = "Configs/" + nameof(ProjectSettingsConfig))]
    public class ProjectSettingsConfig : ScriptableObject
    {
        [SerializeField] private string savesFileName = "save.dat";
        [SerializeField] private string levelsFolder = "Levels";
        [SerializeField] private string levelFileNameFormat = "level_{0}.json";

        public string SavesFileName => savesFileName;
        public string LevelsFolder => levelsFolder;
        public string LevelFileNameFormat => levelFileNameFormat;
    }
}