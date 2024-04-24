/// <summary>
/// AddressableImportSettingsEditor
/// </summary>

using UnityEditor;

namespace Editor.Helper
{
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
#endif

    [CustomEditor(typeof(AddressableImportSettings), true), CanEditMultipleObjects]
    public class AddressableImportSettingsEditor : ScriptableObjectEditor<AddressableImportSettings>
    {

    }
}