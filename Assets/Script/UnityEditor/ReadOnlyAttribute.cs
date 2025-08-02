using UnityEditor;
using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        bool wasEnabled = GUI.enabled;
        GUI.enabled = false;

        if (property.propertyType == SerializedPropertyType.Generic)
        {
            // Vẽ Foldout để mở rộng class
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                SerializedProperty childProperty = property.Copy();
                SerializedProperty endProperty = property.GetEndProperty();

                // Duyệt qua từng property con mà không bị lặp lại chính class cha
                if (childProperty.NextVisible(true))
                {
                    do
                    {
                        // Bỏ qua field "Array Size" nếu là danh sách (List<T> hoặc Array)
                        if (childProperty.propertyType == SerializedPropertyType.ArraySize)
                            continue;

                        // Đảm bảo không vẽ chính đối tượng cha
                        if (SerializedProperty.EqualContents(childProperty, endProperty))
                            break;

                        EditorGUILayout.PropertyField(childProperty, true);
                    }
                    while (childProperty.NextVisible(false)); // Duyệt các phần tử ngang hàng
                }

                EditorGUI.indentLevel--;
            }
        }
        else
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        GUI.enabled = wasEnabled;
    }
}
#endif
