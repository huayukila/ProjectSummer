using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class UIKit : ScriptableObject
{
    private static UIKit m_Instance;

    // ���������X�N���v�g�ۑ��p�[�X
    private static string m_ClassPath = "Assets/Script/UI/Panel";

    private bool isCompile = false;

    public static UIKit Default
    {
        get
        {
            if (m_Instance != null) return m_Instance;

            var filePath = "Assets/Script/Utils/UIKit/Editor/UIKit.asset";
            if (File.Exists(filePath))
            {
                return m_Instance = AssetDatabase.LoadAssetAtPath<UIKit>(filePath);
            }

            m_Instance = CreateInstance<UIKit>();
            AssetDatabase.CreateAsset(m_Instance, filePath);
            AssetDatabase.SaveAssets();
            return m_Instance;
        }
    }

    [MenuItem("UIKit/CreateScript")]
    public static void CreateCode()
    {
        Default.isCompile = true;
        GameObject selectedPrefab = Selection.activeGameObject as GameObject;
        if (selectedPrefab == null ||
            PrefabUtility.GetPrefabAssetType(selectedPrefab) == PrefabAssetType.NotAPrefab)
        {
            Debug.Log("�I�������̂�UIPrefab�ł͂Ȃ��ł��I�I");
            return;
        }

        Debug.Log("UI�R�[�h������" + selectedPrefab.name);

        string scriptName = selectedPrefab.GetComponent<ViewController>().GetType().FullName;
        string filePath = Path.Combine(m_ClassPath, scriptName + ".Designer.cs");

        StringBuilder scriptBuilder = new StringBuilder();

        scriptBuilder.AppendLine("using UnityEngine;");
        scriptBuilder.AppendLine("public partial class " + scriptName);
        scriptBuilder.AppendLine("{");

        AddMemberWithUIObject(selectedPrefab.transform, scriptBuilder);

        scriptBuilder.AppendLine("}");
        File.WriteAllText(filePath, scriptBuilder.ToString());
        Debug.Log("UI�R�[�h��������: " + filePath);
        AssetDatabase.Refresh();
        scriptBuilder.Clear();
    }

    private static void AddMemberWithUIObject(Transform transform, StringBuilder scriptBuilder)
    {
        Bind[] binds = transform.GetComponentsInChildren<Bind>();
        foreach (var bind in binds)
        {
            string componentType = bind.GetComponentName();
            string memberName = bind.gameObject.name;
            scriptBuilder.AppendLine($"\tpublic {componentType} {memberName};");
        }
    }

    private void AddReferencesToPrefab()
    {
        if (!Default.isCompile)
            return;
        GameObject prefab = Selection.activeGameObject;
        var viewController = prefab.GetComponent<ViewController>();
        if (viewController == null)
        {
            Debug.LogWarning("Prefab��viewController���A�^�b�`���ꂢ�Ȃ��I");
            return;
        }

        var serializedObject = new SerializedObject(viewController);
        var binds = prefab.GetComponentsInChildren<Bind>();

        foreach (var bind in binds)
        {
            var memberName = bind.gameObject.name;
            var serializedProperty = serializedObject.FindProperty(memberName);
            if (serializedProperty != null)
            {
                serializedProperty.objectReferenceValue = bind.GetComponent(bind.GetComponentName());
            }
            else
            {
                Debug.LogWarning($"{memberName}��������܂���I");
            }
        }

        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
        EditorUtility.SetDirty(viewController);
        AssetDatabase.SaveAssets();
    }

    [DidReloadScripts]
    static void Compile()
    {
        Default.AddReferencesToPrefab();
        Default.isCompile = false;
    }
}