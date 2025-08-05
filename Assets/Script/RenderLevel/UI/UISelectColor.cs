using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISelectColor : MonoBehaviour
{
    [SerializeField] private UIRender uiManager;
    [SerializeField] private Image img;
    [SerializeField] private ColorEnum color;
    [SerializeField] private Button btnSelect;
    [SerializeField] private Material mat;

    private void Start()
    {
        btnSelect.onClick.AddListener(Select);
    }

    private void Select() {
        CtrlRender.instance.ChangeColor(color);
        uiManager.SetPositionCusor(new Vector3(transform.position.x, transform.position.y + 60, transform.position.z));
    }
}
