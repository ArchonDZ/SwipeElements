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
            levelSystem.IsLoaded.Subscribe(Loading).AddTo(this);
        }

        private void Awake()
        {
            nextLevelButton.onClick.AddListener(LoadNextLevel);
            restartButton.onClick.AddListener(RestartLevel);
        }

        private void Loading(bool value)
        {
            nextLevelButton.interactable = value;
            restartButton.interactable = value;

            if (value)
                UpdateText();
        }

        private void UpdateText()
        {
            levelText.SetText(string.Format(levelTextStringFormat, levelSystem.CurrentLevelIndex + 1));
        }

        private void LoadNextLevel()
        {
            levelSystem.LoadNextLevel().Forget();
        }

        private void RestartLevel()
        {
            levelSystem.RestartLevel().Forget();
        }
    }
}
