using System;
using System.Collections;
using UnityEngine;
public class GameCtrl : Singleton<GameCtrl>
{
    [SerializeField] private Nut nutSelect;
    [SerializeField] private Tube tubeLast;
    [SerializeField] private LayerMask layerTube;
    [SerializeField] private float rayDistance = 20;
    private int countTubeComplerable;
    private int countTubeCompleted = 0;
    public Action OnWin;
    [SerializeField] private int levelCurrent;

    private LevelManager levelManager;


    [SerializeField] private string pathPrefabMap;
    public LevelManager LevelManager => levelManager;
    void Update()
    {
        ClickDetector();
    }

    protected override void Awake()
    {
        base.Awake();
        levelManager = new(pathPrefabMap, levelCurrent);
    }

    private void Start()
    {
        levelManager.OnNextMap += OnNextMap;
    }

    public void SetCountTubeFinish(int count)
    {
        countTubeComplerable += count;
    }

    public void CompletedTube()
    {
        countTubeCompleted++;
        if (countTubeCompleted == countTubeComplerable)
        {
            OnWin?.Invoke();
        }
    }

    private void OnNextMap() {
        StartCoroutine(WaitForMapData(levelManager));
        countTubeCompleted = 0;
    }

    IEnumerator WaitForMapData(LevelManager levelManager)
    {
        while (levelManager.MapCurrentData == null)
            yield return null;
        countTubeComplerable = levelManager.MapCurrentData.CountTubeFinish;
    }

    private void ClickDetector()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red, 0.5f);
            if (Physics.Raycast(ray, out hit, 100, layerTube))
            {
                Tube tubeSelect = hit.collider.GetComponent<Tube>();
                SelectTube(tubeSelect);
            }
        }
    }

    private void SelectTube(Tube tubeSelect)
    {
        if (nutSelect == null)
        {
            nutSelect = tubeSelect.PeekNuts();
            tubeLast = tubeSelect;
            nutSelect?.Pop();
            return;
        }
        if (tubeSelect == tubeLast)
        {
            nutSelect.Revert();
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
        tubeLast.PopNut();
        nutSelect.Push(tubeSelect);
        nutSelect = null;
    }
}
