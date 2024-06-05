/// <summary>
/// LabelAttribute,
/// modified from https://github.com/dbrizov/NaughtyAttributes/blob/master/Assets/NaughtyAttributes/Scripts/Editor/PropertyDrawers/LabelPropertyDrawer.cs
/// </summary>

using System;
using UnityEditor;
using UnityEngine;

namespace Editor.Attributes
{

    [AttributeUsage(AttributeTargets.Field)]
    public class LabelAttribute : PropertyAttribute
    {
        public string Label { get; private set; }

        public LabelAttribute(string label)
        {
            this.Label = label;
        }
    }

#if UNITY_EDITOR && !ODIN_INSPECTOR
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    public class LabelAttributeDrawer : PropertyDrawer
    {
        private LabelAttribute Attribute
        {
            get { return _attribute ?? (_attribute = attribute as LabelAttribute); }
        }

        private LabelAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var guiContent = new GUIContent(Attribute.Label);
            EditorGUI.PropertyField(position, property, guiContent, true);
        }
    }

#endif
}