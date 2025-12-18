using Cysharp.Threading.Tasks;
using Elements.Systems;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Elements.UI
{
    public class GameControlCanvas : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private string levelTextStringFormat;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button restartButton;

        private LevelSystem levelSystem;

        [Inject]
        public void Initialize(LevelSystem levelSystem)
        {
            this.levelSystem = levelSystem;
            levelSystem.IsLoaded.Subscribe(_ => UpdateText()).AddTo(this);
        }

        private void Awake()
        {
            nextLevelButton.onClick.AddListener(LoadNextLevel);
            restartButton.onClick.AddListener(RestartLevel);
        }

        private void UpdateText()
        {
            levelText.SetText(string.Format(levelTextStringFormat, levelSystem.CurrentLevelIndex + 1));
        }

        private void LoadNextLevel()
        {
            LoadNextLevelAsync().Forget();
        }

        private async UniTaskVoid LoadNextLevelAsync()
        {
            nextLevelButton.interactable = false;
            await levelSystem.LoadNextLevel();
            nextLevelButton.interactable = true;
        }

        private void RestartLevel()
        {
            RestartLevelAsync().Forget();
        }

        private async UniTaskVoid RestartLevelAsync()
        {
            restartButton.interactable = false;
            await levelSystem.RestartLevel();
            restartButton.interactable = true;
        }
    }
}
