using System.Text;
using System;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;

namespace Elements.Systems
{
    public class SaveSystem
    {
        public async UniTask<Data> LoadAsync(string fileName)
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            if (!File.Exists(filePath))
                return null;

            string text = await File.ReadAllTextAsync(filePath);
            byte[] jsonData = Convert.FromBase64String(text);
            return JsonConvert.DeserializeObject<Data>(Encoding.UTF8.GetString(jsonData));
        }

        public async UniTask SaveAsync(string fileName, Data objToJson)
        {
            byte[] jsonData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(objToJson));
            await File.WriteAllTextAsync(Path.Combine(Application.persistentDataPath, fileName), Convert.ToBase64String(jsonData));
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