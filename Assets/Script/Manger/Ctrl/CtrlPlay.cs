using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlPlay : Singleton<CtrlPlay>
{
    private Nut nutSelect;
    private Tube tubeLast;
    private int countTubeComplerable;
    private int countTubeCompleted = 0;
    public Action OnWin;
    [SerializeField] private LayerMask layerTube;
    [SerializeField] private float rayDistance = 20;
    [SerializeField] private int levelCurrent;
    [SerializeField] private string pathPrefabMap;
    private LevelManager levelManager;

    public LevelManager LevelManager => levelManager;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red, 0.5f);
            if (Physics.Raycast(ray, out hit, rayDistance, layerTube))
            {

                Tube tubeSelect = hit.collider.GetComponent<Tube>();
                SelectTube(tubeSelect);
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        levelManager = new(pathPrefabMap, levelCurrent);
    }

    protected void Start()
    {
        levelManager.OnNextMap += OnNextMap;
    }


    public void CompletedTube()
    {
        countTubeCompleted++;
        if (countTubeCompleted == countTubeComplerable)
        {
            OnWin?.Invoke();
        }
    }

    public void HandleClick(Tube tubeSelect)
    {
        SelectTube(tubeSelect);
    }

    private void OnNextMap()
    {
        StartCoroutine(WaitForMapData(levelManager));
        countTubeCompleted = 0;
    }

    IEnumerator WaitForMapData(LevelManager levelManager)
    {
        while (levelManager.MapCurrentData == null)
            yield return null;
        countTubeComplerable = levelManager.MapCurrentData.CountTubeFinish;
    }

    private void SelectTube(Tube tubeSelect)
    {
        if (nutSelect == null)
        {
            nutSelect = tubeSelect.PeekNuts();
            tubeLast = tubeSelect;
            nutSelect?.PopAnimation();
            return;
        }
        if (tubeSelect == tubeLast)
        {
            nutSelect.RevertAnimation();
            nutSelect = null;
            tubeSelect = null;
            return;
        }
        bool isPushed = tubeSelect.PushNuts(nutSelect);
        if (!isPushed)
        {
            Debug.Log("Dont push");
            return;
        }
        nutSelect = null;
    }
}
