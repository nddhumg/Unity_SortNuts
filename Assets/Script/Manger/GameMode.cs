using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : Singleton<GameMode>
{
    [SerializeField] private Material hiddenMaterial;
    [SerializeField] private GameObject uiPlay;
    [SerializeField] private GameObject uiRender;
    [SerializeField] private CtrlPlay ctrlPlay;
    [SerializeField] private CtrlRender ctrlRender;
    private Mode modeCurrent;
    public enum Mode
    {
        Play,
        Test,
        Render,
    }
    public Material HiddenMaterial => hiddenMaterial;

    private void Start()
    {
        modeCurrent = Mode.Play;
    }

    public void ChangeMode(Mode mode)
    {
        EndMode(modeCurrent);
        modeCurrent = mode;
        StartMode(mode); 
    }

    private void StartMode(Mode mode) {
        switch (mode)
        {
            case Mode.Play:
                uiPlay.SetActive(true);
                ctrlPlay.gameObject.SetActive(true);
                ctrlPlay.LevelManager.MapCurrent.SetActive(true);
                break;
            case Mode.Test:
                break;
            case Mode.Render:
                uiRender.SetActive(true);
                ctrlRender.gameObject.SetActive(true);
                ctrlRender.CreateMap();
                break;

        }
    }

    private void EndMode(Mode mode) {
        switch (mode) {
            case Mode.Play:
                uiPlay.SetActive(false);
                ctrlPlay.LevelManager.MapCurrent.SetActive(false);
                ctrlPlay.gameObject.SetActive(false);
                break;
            case Mode.Test:
                break;
            case Mode.Render:
                uiRender.SetActive(false);
                ctrlRender.gameObject.SetActive(false);
                break;

        }
    }
}
