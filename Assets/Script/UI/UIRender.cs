using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRender : MonoBehaviour
{
    [SerializeField] private GameObject cursorColor;
    private void Start()
    {
    }

    public void SetPositionCusor(Vector3 position) {
        cursorColor.transform.position = position;
    }

}
