using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public class LevelManager
{
    private int levelCurrent = 1;
    private readonly string pathPrefabMap;
    private AsyncOperationHandle<GameObject> handleCurrent;
    private AsyncOperationHandle<GameObject> handleNext;
    private MapData mapCurrentData;
    private bool isFinalLevel = false;

    public MapData MapCurrentData => mapCurrentData;
    public int LevelCurrent => levelCurrent;
    public bool IsFinishLevel => isFinalLevel;
    public Action OnNextMap;

    public LevelManager(string pathPrefabMap, int level)
    {
        this.pathPrefabMap = pathPrefabMap;
        levelCurrent = level;
        handleCurrent = LoadLevel(levelCurrent);
        handleCurrent.Completed += ActivateMap;

        LoadLevel(levelCurrent + 1);
    }

    public bool NextMap()
    {
        if (isFinalLevel)
            return false;
        levelCurrent++;
        if (handleCurrent.IsValid())
        {
            UnityEngine.Object.Destroy(handleCurrent.Result);
            Addressables.Release(handleCurrent);
        }
        handleCurrent = handleNext;
        if (handleCurrent.IsDone)
        {
            ActivateMap(handleCurrent);
        }
        LoadLevel(levelCurrent + 1);
        return true;
    }

    public void ReloadMap()
    {
        Addressables.ReleaseInstance(handleCurrent);

        handleCurrent = Addressables.InstantiateAsync($"{pathPrefabMap}/Level{levelCurrent}.prefab");
        handleCurrent.Completed += h =>
        {
            ActivateMap(h);
        };
    }

    private AsyncOperationHandle<GameObject> LoadLevel(int level)
    {
        handleNext = Addressables.InstantiateAsync($"{pathPrefabMap}/Level{level}.prefab");
        handleNext.Completed += h =>
        {
            if (h.Status == AsyncOperationStatus.Succeeded)
            {
                h.Result.SetActive(false);
            }
            else
            {
                isFinalLevel = true;
                Debug.LogWarning("Reached the final level!");
            }
        };
        return handleNext;
    }

    private void ActivateMap(AsyncOperationHandle<GameObject> handle)
    {
        handle.Result.SetActive(true);
        mapCurrentData = handle.Result.GetComponent<MapData>();
        OnNextMap?.Invoke();
    }
}

