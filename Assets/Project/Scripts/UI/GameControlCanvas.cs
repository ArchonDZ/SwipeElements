using Cysharp.Threading.Tasks;
using Elements.Systems;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Elements.UI
{
    public class GameControlCanvas : MonoBehaviour
    {
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button restartButton;

        [Inject] private LevelSystem levelSystem;

        private void Awake()
        {
            nextLevelButton.onClick.AddListener(LoadNextLevel);
            restartButton.onClick.AddListener(RestartLevel);
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
            restartButton.interactable = false;
        }
    }
}
