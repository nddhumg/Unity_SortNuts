
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlay : MonoBehaviour
{
    [SerializeField] private Button btnReset;
    [SerializeField] private TMP_Text textLevel;
    [SerializeField] private GameObject canvasNext;
    [SerializeField] private Button btnNextLevel;
    [SerializeField] private GameObject panel;
    [SerializeField] private Button btnChangeMode;

    private void Start()
    {
        btnReset.onClick.AddListener(ResetLevel);
        CtrlPlay.instance.OnWin += Win;
        btnNextLevel.onClick.AddListener(OnClickNextLevel);
        btnChangeMode.onClick.AddListener(ChangeMode);
    }

    protected void ResetLevel() {
        CtrlPlay.instance.LevelManager.ReloadMap();
    }

    protected void ChangeMode()
    {
        GameMode.instance.ChangeMode(GameMode.Mode.Render);
    }


    protected void Win() {
        canvasNext.SetActive(true);
    }

    protected void OnClickNextLevel() {
        bool isNextMap = CtrlPlay.instance.LevelManager.NextMap();
        if (!isNextMap)
        {
            CtrlPlay.instance.LevelManager.ReloadMap();
        }
        textLevel.text = "Level " + CtrlPlay.instance.LevelManager.LevelCurrent;
        canvasNext.SetActive(false);
    }

}
