using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button btnReset;
    [SerializeField] private TMP_Text textLevel;
    [SerializeField] private GameObject canvasNext;
    [SerializeField] private Button btnNextLevel;

    private void Start()
    {
        btnReset.onClick.AddListener(Reset);
        GameCtrl.instance.OnWin += Win;
        btnNextLevel.onClick.AddListener(OnClickNextLevel);
    }

    protected void Reset() {
        GameCtrl.instance.LevelManager.ReloadMap();
    }

    protected void Win() {
        canvasNext.SetActive(true);
    }

    protected void OnClickNextLevel() {
        bool isNextMap = GameCtrl.instance.LevelManager.NextMap();
        if (!isNextMap) {
            GameCtrl.instance.LevelManager.ReloadMap();
        }
        textLevel.text = "Level " + GameCtrl.instance.LevelManager.LevelCurrent;
        canvasNext.SetActive(false);
    }
}
