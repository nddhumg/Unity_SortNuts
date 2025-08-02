using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public sealed class ButtonAttribute : Attribute
{
    public readonly string Name;
    public readonly string Row;
    public readonly float Space;
    public readonly bool HasRow;
	public ButtonAttribute(string name = null, string row = null, float space = 0f)
    {
        Row = row;
        HasRow = !string.IsNullOrEmpty(Row);
        Name = name;
        Space = space;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Object), true), CanEditMultipleObjects]
public class ObjectEditor : Editor
{
    private ButtonsDrawer _buttonsDrawer;

    private void OnEnable()
    {
        _buttonsDrawer = new ButtonsDrawer(target);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        _buttonsDrawer.DrawButtons(targets);
        SceneView.RepaintAll();
    }
}

public class ButtonsDrawer
{
    public readonly List<IGrouping<string, ButtonType>> ButtonGroups;

    public ButtonsDrawer(object target)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        var methods = target.GetType().GetMethods(flags);
        var buttons = new List<ButtonType>();
        var rowNumber = 0;

        foreach (MethodInfo method in methods)
        {
			var buttonAttribute = (ButtonAttribute)method.GetCustomAttributes(typeof(ButtonAttribute), true).FirstOrDefault();


            if (buttonAttribute == null)
                continue;

            buttons.Add(new ButtonType(method, buttonAttribute));
        }

		ButtonGroups = buttons.GroupBy(button =>
			{
				var attribute = button.ButtonAttribute;
				var hasRow = attribute.HasRow;
				return hasRow ? attribute.Row : string.Format("__{0}", rowNumber++);
			}).ToList();
	}

    public void DrawButtons(IEnumerable<object> targets)
    {
        foreach (var buttonGroup in ButtonGroups)
        {
            if (buttonGroup.Count() > 0)
            {
                var space = buttonGroup.First().ButtonAttribute.Space;
				if (space != 0) GUILayout.Space(space);

            }
            using (new EditorGUILayout.HorizontalScope())
            {
                foreach (var button in buttonGroup)
                {
                    button.Draw(targets);
                }
            }
        }
    }
}

public class ButtonType
{
	public readonly string DisplayName;
	public readonly MethodInfo Method;
	public readonly ButtonAttribute ButtonAttribute;
	public object[] _parameterValues; // Lưu trữ giá trị tham số

	public ButtonType(MethodInfo method, ButtonAttribute buttonAttribute)
	{
		ButtonAttribute = buttonAttribute;
		DisplayName = string.IsNullOrEmpty(buttonAttribute.Name)
			? ObjectNames.NicifyVariableName(method.Name)
			: buttonAttribute.Name;

		Method = method;

		// Khởi tạo mảng tham số nếu phương thức có tham số
		var parameters = method.GetParameters();
		if (parameters.Length > 0)
		{
			_parameterValues = new object[parameters.Length];
		}
	}

	private bool _foldoutParameters = true; // Biến để kiểm soát trạng thái gập mở

	internal void Draw(IEnumerable<object> targets)
	{
		var parameters = Method.GetParameters();

		if (parameters.Length > 0)
		{
			using (new EditorGUILayout.VerticalScope())
			{
				_foldoutParameters = EditorGUILayout.Foldout(_foldoutParameters, "Parameters " + DisplayName, true);

				if (_foldoutParameters)
				{
					for (int i = 0; i < parameters.Length; i++)
					{
						_parameterValues[i] = DrawParameterField(parameters[i], _parameterValues[i]);
					}
				}
			}
		}
		if (!GUILayout.Button(DisplayName, GUILayout.ExpandHeight(true))) return;

		foreach (object target in targets)
		{
			// Gọi phương thức với các tham số đã nhập
			Method.Invoke(target, _parameterValues);
		}
	}


	// Hàm để vẽ trường nhập liệu cho tham số
	private object DrawParameterField(ParameterInfo parameter, object currentValue)
	{
		string labelWithPlaceholder = parameter.Name + " (" + parameter.ParameterType.Name + ")";


		switch (parameter.ParameterType.Name)
		{
		case "Int32":
			return EditorGUILayout.IntField(labelWithPlaceholder, (int)(currentValue ?? 0));
		case "Single":
			return EditorGUILayout.FloatField(labelWithPlaceholder, (float)(currentValue ?? 0f));
		case "String":
			return EditorGUILayout.TextField(labelWithPlaceholder, (string)(currentValue ?? string.Empty));
		case "Boolean":
			return EditorGUILayout.Toggle(labelWithPlaceholder, (bool)(currentValue ?? false));
		case "Vector3":
			return EditorGUILayout.Vector3Field(labelWithPlaceholder, (Vector3)(currentValue ?? Vector3.zero));

		default:
			Debug.LogWarning("Unsupported parameter type: " + parameter.ParameterType.Name);
			return null;
		}
	}

}

#endif
