using System.Text;
using System;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Elements.Systems
{
    public class SaveSystem
    {
        public async UniTask<Data> LoadAsync(string fileName, CancellationToken cancellationToken)
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            if (!File.Exists(filePath))
                return null;

            Data data = null;
            try
            {
                string text = await File.ReadAllTextAsync(filePath, cancellationToken);
                byte[] jsonData = Convert.FromBase64String(text);
                data = JsonConvert.DeserializeObject<Data>(Encoding.UTF8.GetString(jsonData));
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Операция была отменена");
            }
            catch (Exception ex)
            {
                Debug.LogError("Ошибка загрузки файла: " + ex.Message);
            }

            return data;
        }

        public async UniTask SaveAsync(string fileName, Data objToJson, CancellationToken cancellationToken)
        {
            try
            {
                byte[] jsonData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(objToJson));
                await File.WriteAllTextAsync(Path.Combine(Application.persistentDataPath, fileName), Convert.ToBase64String(jsonData), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Операция была отменена");
            }
            catch (Exception ex)
            {
                Debug.LogError("Ошибка сохранения файла: " + ex.Message);
            }
        }
    }

    [Serializable]
    public class Data
    {
        public ushort LevelID;
        public byte[,] ElementsGrid;

        public Data(ushort levelID, byte[,] elementsGrid)
        {
            LevelID = levelID;
            ElementsGrid = elementsGrid;
        }
    }
}