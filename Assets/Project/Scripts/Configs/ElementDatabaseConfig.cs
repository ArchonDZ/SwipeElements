using Elements.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elements.Configs
{
    [CreateAssetMenu(fileName = nameof(ElementDatabaseConfig), menuName = "Configs/" + nameof(ElementDatabaseConfig))]
    public class ElementDatabaseConfig : ScriptableObject
    {
        [SerializeField] private List<ElementContext> elementContexts;

        public bool TryGetElementByType(ElementType elementType, out ElementContext elementContext)
        {
            int index = elementContexts.FindIndex(x => x.ElementType == elementType);
            if (index != -1)
            {
                elementContext = elementContexts[index];
                return true;
            }

            elementContext = new ElementContext();
            return false;
        }
    }

    [Serializable]
    public struct ElementContext
    {
        public ElementType ElementType;
        public Element Prefab;
    }

    public enum ElementType
    {
        None = 0,
        Fire = 1,
        Water = 2,
    }
}
