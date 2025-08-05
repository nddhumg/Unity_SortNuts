using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRenderSelect : MonoBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private CtrlRender.Ability ability;
    [SerializeField] private UIRender uiRender;
    void Start()
    {
        btn.onClick.AddListener(Select);
    }

    void Select() {
        CtrlRender.instance.SetAbility(ability);
        uiRender.SetPositionCusor(new Vector3(transform.position.x, transform.position.y + 60, transform.position.z));
    }

}
