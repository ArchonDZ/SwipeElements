using Elements.Configs;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomPropertyDrawer(typeof(ElementType))]
public class ElementTypeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.Enum)
        {
            int[] enumValue = System.Enum.GetValues(fieldInfo.FieldType).Cast<int>().ToArray();
            string[] displayNames = property.enumDisplayNames;

            for (int i = 0; i < displayNames.Length; i++)
            {
                displayNames[i] = $"{enumValue[i]} - {displayNames[i]}";
            }

            property.enumValueIndex = EditorGUI.Popup(position, label.text, property.enumValueIndex, displayNames);
        }
    }
}