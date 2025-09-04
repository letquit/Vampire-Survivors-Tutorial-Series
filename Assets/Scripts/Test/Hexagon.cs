using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hexagon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [HideInInspector] public Color SpriteNewColor = Color.blue;

    public void ColorSprite(Color color)
    {
        _spriteRenderer.color = color;
    }

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            ColorSprite(SpriteNewColor);
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(Hexagon))]
public class HexagonCustomInspector : Editor
{
    private SerializedProperty spriteNewColor;

    private void OnEnable()
    {
        spriteNewColor = serializedObject.FindProperty("SpriteNewColor");
    }

    public override void OnInspectorGUI()
    {
        // 显示public和serializeField字段
        base.OnInspectorGUI();
        serializedObject.Update();
        
        Hexagon hex = (Hexagon)target;  // 获取目标对象
        EditorGUILayout.Space(15f);
        
        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.LabelField("Color", GUILayout.Width(45f));
        EditorGUILayout.PropertyField(spriteNewColor, GUIContent.none);
        
        Color newColor = spriteNewColor.colorValue;
        if (GUILayout.Button("Apply Color", GUILayout.Width(90f)))
        {
            hex.ColorSprite(newColor);
        }
        
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif