using Cysharp.Threading.Tasks;
using DG.Tweening;
using Elements.Configs;
using Elements.Entities;
using Elements.MemoryPools;
using R3;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Elements.Systems
{
    public class GridSystem : MonoBehaviour
    {
        [Inject] private ElementDatabaseConfig elementDatabaseConfig;
        [Inject] private ElementPool elementPool;
        [Inject] private CameraSystem cameraSystem;
        [Inject] private BackgroudSystem backgroudSystem;

        private InputSystem inputSystem;
        private LevelSystem levelSystem;

        private byte[,] elementValues;
        private Element[,] elements;
        int columns;
        int rows;

        private Vector2Int element1Coord;
        private Vector2Int element2Coord;

        private List<Tween> tweens = new();
        private Sequence normalizeSequence;

        [Inject]
        public void Initialize(InputSystem inputSystem, GridSettingConfig gridSettingConfig, LevelSystem levelSystem)
        {
            this.inputSystem = inputSystem;
            inputSystem.OnClickDownEvent += OnClickDown;
            inputSystem.OnClickUpEvent += OnClickUp;

            elementPool.Initialize(gridSettingConfig.StartElementPoolCount, gridSettingConfig.MaxElementPoolCount);

            this.levelSystem = levelSystem;
            levelSystem.IsLoaded.Subscribe(UpdateGrid).AddTo(this);
        }

        private void OnDestroy()
        {
            if (inputSystem != null)
            {
                inputSystem.OnClickDownEvent -= OnClickDown;
                inputSystem.OnClickUpEvent -= OnClickUp;
            }
            ClearSequences();
        }

        private void OnClickDown(Vector2 vector2)
        {
            Vector2 screenPoint = cameraSystem.MainCamera.ScreenToWorldPoint(vector2);
            element1Coord = new(Mathf.FloorToInt(screenPoint.x), Mathf.FloorToInt(screenPoint.y));
        }

        private void OnClickUp(Vector2 vector2)
        {
            Vector2 screenPoint = cameraSystem.MainCamera.ScreenToWorldPoint(vector2);
            element2Coord = new(Mathf.FloorToInt(screenPoint.x), Mathf.FloorToInt(screenPoint.y));

            TrySwapElements();
        }

        private void UpdateGrid(bool isLoaded)
        {
            if (!isLoaded)
                ClearGrid();
            else
                ResetGrid();
        }

        private void ClearGrid()
        {
            ClearSequences();
            elementPool.DespawnAll();
        }

        private void ResetGrid()
        {
            elementValues = levelSystem.ElementsGrig;

            int order = 0;
            columns = elementValues.GetLength(0);
            rows = elementValues.GetLength(1);

            elements = new Element[columns, rows];
            cameraSystem.UpdateCameraFocus(new Vector2(columns, rows));
            backgroudSystem.SetupBackground();

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    byte cell = elementValues[i, j];
                    if (cell != 0 && elementDatabaseConfig.TryGetElementByType((ElementType)cell, out ElementContext context))
                    {
                        elements[i, j] = elementPool.Spawn(context.Prefab, new Vector2(i, j), i * rows + j);
                        order++;
                    }
                }
            }
        }

        private bool TrySwapElements()
        {
            if (!AreExists(element1Coord.x, element1Coord.y, element2Coord.x, element2Coord.y)) return false;
            if (!CanSwap(element1Coord.x, element1Coord.y, element2Coord.x, element2Coord.y)) return false;

            normalizeSequence = DOTween.Sequence();
            tweens.Add(normalizeSequence);

            Swap(element1Coord.x, element1Coord.y, element2Coord.x, element2Coord.y);
            NormalizeGrid();
            CheckCompleteGrid();
            return true;
        }

        private bool AreExists(int col1, int row1, int col2, int row2)
        {
            if (row1 < 0 || col1 < 0 || row2 < 0 || col2 < 0) return false;
            if (rows <= row1 || columns <= col1 || rows <= row2 || columns <= col2) return false;
            return true;
        }

        private bool CanSwap(int col1, int row1, int col2, int row2)
        {
            if (elementValues[col1, row1] == 0 && elementValues[col2, row2] == 0) return false;
            if (elementValues[col1, row1] != 0 && elementValues[col2, row2] != 0)
            {
                return AreNeighbors(col1, row1, col2, row2) && elements[col1, row1].IsAvailable && elements[col2, row2].IsAvailable;
            }

            return AreHorizontalNeighbors(col1, row1, col2, row2) &&
                ((elements[col1, row1] != null && elements[col1, row1].IsAvailable) ||
                (elements[col2, row2] != null && elements[col2, row2].IsAvailable));
        }

        private bool AreNeighbors(int col1, int row1, int col2, int row2)
        {
            if (AreHorizontalNeighbors(col1, row1, col2, row2)) return true;
            if (AreVerticalNeighbors(col1, row1, col2, row2)) return true;
            return false;
        }

        private bool AreHorizontalNeighbors(int col1, int row1, int col2, int row2) =>
            row1 == row2 && col1 != col2 && Mathf.Abs(col1 - col2) == 1;

        private bool AreVerticalNeighbors(int col1, int row1, int col2, int row2) =>
            col1 == col2 && row1 != row2 && Mathf.Abs(row1 - row2) == 1;

        private void Swap(int col1, int row1, int col2, int row2)
        {
            Sequence swapSequence = DOTween.Sequence();

            byte tempValue = elementValues[col1, row1];
            elementValues[col1, row1] = elementValues[col2, row2];
            elementValues[col2, row2] = tempValue;

            Element tempElement = elements[col1, row1];
            elements[col1, row1] = elements[col2, row2];
            elements[col2, row2] = tempElement;

            if (elements[col1, row1] != null)
            {
                swapSequence.Join(elements[col1, row1].MoveTo(new Vector2(col1, row1), GetOrder(col1, row1)));
            }

            if (elements[col2, row2] != null)
            {
                swapSequence.Join(elements[col2, row2].MoveTo(new Vector2(col2, row2), GetOrder(col2, row2)));
            }

            normalizeSequence.Append(swapSequence);
        }

        private void NormalizeGrid()
        {
            bool checkInProcess = true;
            while (checkInProcess)
            {
                DropElements();
                checkInProcess = TryMatchElements();
            }
        }

        private void DropElements()
        {
            Sequence dropSequence = DOTween.Sequence();

            for (int i = 0; i < columns; i++)
            {
                int emptySpot = -1;
                for (int j = 0; j < rows; j++)
                {
                    if (emptySpot == -1)
                    {
                        if (elementValues[i, j] == 0)
                        {
                            emptySpot = j;
                        }
                    }
                    else
                    {
                        if (elementValues[i, j] != 0)
                        {
                            elementValues[i, emptySpot] = elementValues[i, j];
                            elementValues[i, j] = 0;

                            elements[i, emptySpot] = elements[i, j];
                            elements[i, j] = null;

                            dropSequence.Join(elements[i, emptySpot].MoveTo(new Vector2(i, emptySpot), GetOrder(i, emptySpot)));

                            emptySpot++;
                        }
                    }
                }
            }

            normalizeSequence.Append(dropSequence);
        }

        private bool TryMatchElements()
        {
            Sequence matchSequence = DOTween.Sequence();

            bool foundMatch = false;
            bool[,] toRemove = new bool[columns, rows];

            for (int j = 0; j < rows; j++)
            {
                for (int i = 0; i < columns - 2; i++)
                {
                    byte value = elementValues[i, j];
                    if (value != 0 && value == elementValues[i + 1, j] && value == elementValues[i + 2, j])
                    {
                        toRemove[i, j] = toRemove[i + 1, j] = toRemove[i + 2, j] = true;
                        foundMatch = true;
                    }
                }
            }

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows - 2; j++)
                {
                    byte value = elementValues[i, j];
                    if (value != 0 && value == elementValues[i, j + 1] && value == elementValues[i, j + 2])
                    {
                        toRemove[i, j] = toRemove[i, j + 1] = toRemove[i, j + 2] = true;
                        foundMatch = true;
                    }
                }
            }

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (toRemove[i, j])
                    {
                        elementValues[i, j] = 0;
                        matchSequence.JoinCallback(elements[i, j].Destroy);
                        elements[i, j] = null;
                    }
                }
            }

            matchSequence.AppendInterval(1f);
            normalizeSequence.Append(matchSequence);
            return foundMatch;
        }

        private int GetOrder(int colIndex, int rowIndex) => colIndex * rows + rowIndex;

        private void CheckCompleteGrid()
        {
            bool elementsExists = false;
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (elementValues[i, j] != 0)
                    {
                        elementsExists = true;
                        break;
                    }
                }

                if (elementsExists) break;
            }

            if (elementsExists)
            {
                levelSystem.UpdateLevel(elementValues).Forget();
            }
            else
            {
                normalizeSequence.OnComplete(() =>
                {
                    levelSystem.LoadNextLevel().Forget();
                    tweens.Remove(normalizeSequence);
                });
            }
        }

        private void ClearSequences()
        {
            tweens.ForEach(x => x?.Kill());
            tweens.Clear();
        }
    }
}