using Cysharp.Threading.Tasks;
using Elements.Configs;
using Elements.Entities;
using UnityEngine;
using Zenject;

namespace Elements.Systems
{
    public class GridSystem : MonoBehaviour
    {
        [Inject] private DiContainer container;
        [Inject] private ElementDatabaseConfig elementDatabaseConfig;
        [Inject] private CameraSystem cameraSystem;
        [Inject] private BackgroudSystem backgroudSystem;

        private LevelSystem levelSystem;
        private byte[,] elementValues;
        private Element[,] elements;

        [Inject]
        public async UniTaskVoid Initialize(LevelSystem levelSystem)
        {
            this.levelSystem = levelSystem;
            await UniTask.WaitUntil(() => levelSystem.IsLoaded);

            elementValues = levelSystem.ElementsGrig;

            int order = 0;
            int columns = elementValues.GetLength(0);
            int rows = elementValues.GetLength(1);

            elements = new Element[columns, rows];
            cameraSystem.UpdateCameraFocus(new Vector2(columns, rows));
            backgroudSystem.SetupBackground();

            for (int i = 0; i < elementValues.GetLength(0); i++)
            {
                for (int j = 0; j < elementValues.GetLength(1); j++)
                {
                    byte cell = elementValues[i, j];
                    if (cell != 0 && elementDatabaseConfig.TryGetElementByType((ElementType)cell, out ElementContext context))
                    {
                        Element element = container.InstantiatePrefabForComponent<Element>(context.Prefab, new Vector2(i, j), Quaternion.identity, null);
                        element.SetSortingOrder(order);
                        elements[i, j] = element;
                        order++;
                    }
                }
            }
        }
    }
}