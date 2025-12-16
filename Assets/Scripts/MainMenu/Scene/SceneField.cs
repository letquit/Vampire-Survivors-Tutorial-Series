using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 场景字段类，用于在Unity编辑器中引用场景资产
/// </summary>
[Serializable]
public class SceneField
{
    [SerializeField] private Object sceneAsset;
    [SerializeField] private string sceneName = "";
    
    /// <summary>
    /// 获取场景名称
    /// </summary>
    public string SceneName { get => sceneName;}

    /// <summary>
    /// 隐式转换操作符，将SceneField对象转换为字符串
    /// </summary>
    /// <param name="sceneField">要转换的SceneField对象</param>
    /// <returns>场景名称字符串</returns>
    public static implicit operator string(SceneField sceneField)
    {
        return sceneField.sceneName;
    }
}

#if UNITY_EDITOR
/// <summary>
/// SceneField类的自定义属性绘制器，在Unity编辑器中提供场景资产的可视化界面
/// </summary>
[CustomPropertyDrawer(typeof(SceneField))]
public class SceneFieldPropertyDrawer : PropertyDrawer
{
    /// <summary>
    /// 绘制属性的GUI界面
    /// </summary>
    /// <param name="position">绘制区域的位置和大小</param>
    /// <param name="property">要绘制的序列化属性</param>
    /// <param name="label">属性的标签</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, GUIContent.none, property);
        
        // 查找相对属性
        SerializedProperty sceneAsset = property.FindPropertyRelative("sceneAsset");
        SerializedProperty sceneName = property.FindPropertyRelative("sceneName");
        
        // 绘制标签并调整位置
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        
        if (sceneAsset != null)
        {
            // 绘制对象字段，允许用户选择场景资产
            sceneAsset.objectReferenceValue =
                EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);

            // 如果选择了场景资产，则更新场景名称
            if (sceneAsset.objectReferenceValue != null)
            {
                sceneName.stringValue = (sceneAsset.objectReferenceValue as SceneAsset)?.name;
            }
        }
        EditorGUI.EndProperty();
    }
}
#endif