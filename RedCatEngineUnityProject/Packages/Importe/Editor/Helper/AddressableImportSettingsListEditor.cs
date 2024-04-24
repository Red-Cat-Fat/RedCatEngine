/// <summary>
/// AddressableImportSettingsListEditor
/// </summary>

using UnityEditor;

namespace Editor.Helper
{
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
#endif

    [CustomEditor(typeof(AddressableImportSettingsList), true), CanEditMultipleObjects]
    public class AddressableImportSettingsListEditor : ScriptableObjectEditor<AddressableImportSettingsList>
    {

    }
}