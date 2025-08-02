
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
public class RenderLevel : MonoBehaviour
{
    [Header("Info spawn")]
    [SerializeField, Range(3, 9)] private int countTube = 3;
    [SerializeField] private int countTubeFinish = 1;
    [SerializeField] private int countColorFinish = 1;
    [SerializeField] private int countHiddenNut;
    [SerializeField] private int loopCount;
    [SerializeField] private int countTubeEmpty;
    [SerializeField] private string level; 


    [Header("Info base")]
    [SerializeField] private SOConfigGame configGame;
    [SerializeField] private float positionYParent;
    [SerializeField, Range(1, 3)] private int maxRow = 3;
    [SerializeField] private float verticalDeviation;
    [SerializeField] private Vector2 boundLeftRight;
    [SerializeField] private Vector2 boundTopDown;
    [SerializeField] private Vector2 spacing;
    [SerializeField] private Vector2 size;
    [SerializeField] private GameObject tube;
    [SerializeField] private GameObject nut;
    private Vector3 positionNutCurrent;

    [Button]
    public void Spawn()
    {
        Vector3 posiitionSpawn = new Vector3(boundLeftRight.x + size.x / 2, 0, boundTopDown.y + size.y / 2);
        GameObject parentMap = new GameObject("Level" + level);
        GameObject ctrlMap = new GameObject("Ctrl");
        MapData mapdata = parentMap.AddComponent<MapData>();
        ctrlMap.transform.parent = parentMap.transform;
        parentMap.transform.position = new Vector3(0, positionYParent, 0);
        NormalizeData();
        mapdata.CountTubeFinish = countTubeFinish;
        List<Tube> tubes = GenerateTube(parentMap, posiitionSpawn);
        List<TubeRender> tubeRenders = Enumerable
                                     .Range(0, countTube)
                                     .Select(i => new TubeRender(configGame.MaxNutInTube, tubes[i]))
                                     .ToList();
        GenerateNut(tubeRenders);

    }
    [Button]
    public void SpawnDataCSV()
    {
        foreach (var data in CsvCollectionManager.SheetContainer.DataSpawnSheet)
        {
            level = data.Id;
            countTube = data.CountTube;
            countTubeFinish = data.CountTubeFinish;
            countColorFinish = data.CountColorFinish;
            countHiddenNut = data.CountHiddenNut;
            loopCount = data.LoopCount;
            countTubeEmpty = data.CountTubeEmpty;
            Spawn();
        }
    }

    [Button]
    public void SpawnDataCSV(string id)
    {
        foreach (var data in CsvCollectionManager.SheetContainer.DataSpawnSheet)
        {
            if (id != data.Id)
                continue;
            level = data.Id;
            countTube = data.CountTube;
            countTubeFinish = data.CountTubeFinish;
            countColorFinish = data.CountColorFinish;
            countHiddenNut = data.CountHiddenNut;
            loopCount = data.LoopCount;
            countTubeEmpty = data.CountTubeEmpty;
            Spawn();
        }
    }


    protected List<Tube> GenerateTube(GameObject parentMap, Vector3 posiitionSpawn)
    {
        int index = 0;
        List<Tube> result = new List<Tube>();
        for (int column = 0; column < (countTube + maxRow - 1) / maxRow; column++)
        {
            for (int row = 0; row < maxRow && index != countTube; row++)
            {
                GameObject tubeGO = Instantiate(tube);
                tubeGO.transform.parent = parentMap.transform;
                tubeGO.transform.localPosition = posiitionSpawn;
                posiitionSpawn.x += size.x + spacing.x;
                index++;
                Tube tubeScript = tubeGO.GetComponent<Tube>();
                result.Add(tubeScript);
            }
            posiitionSpawn.z += size.y + spacing.y + verticalDeviation * column;
            posiitionSpawn.x = boundLeftRight.x + size.x / 2;
        }
        return result;
    }

    protected void GenerateNut(List<TubeRender> tubeRenders)
    {
        List<ColorEnum> colors = GenerateColor();
        List<TubeRender> tubeRenders1 = new List<TubeRender>(tubeRenders);
        for (int i = 0; i < countTubeFinish; i++)
        {
            int indexRandom = UnityEngine.Random.Range(0, tubeRenders1.Count);
            TubeRender tubeRenderRandom = tubeRenders1[indexRandom];
            int indexColor = i < countColorFinish ? i : Random.Range(0, colors.Count);
            for (int countNut = 0; countNut < configGame.MaxNutInTube; countNut++)
            {
                NutRender nutRender = new NutRender(colors[indexColor]);
                tubeRenderRandom.PushNut(nutRender);
            }
            tubeRenders1.Remove(tubeRenderRandom);
        }

        Shuffle(tubeRenders);
        MakeTubesEmpty(tubeRenders);
        MakeHiddenNut(tubeRenders);
        for (int indexTubeRender = 0; indexTubeRender < tubeRenders.Count; indexTubeRender++)
        {
            positionNutCurrent = tube.transform.position;
            positionNutCurrent.y = configGame.PositionYNutZero;
            List<NutRender> nutRendersList = tubeRenders[indexTubeRender].nuts.ToList();
            nutRendersList.Reverse();
            foreach (NutRender nutRender in nutRendersList)
            {
                GameObject nutGO = Instantiate(nut, tubeRenders[indexTubeRender].tube.NutsParent);
                nutGO.transform.localPosition = positionNutCurrent;
                Nut nutScript = nutGO.GetComponent<Nut>();
                nutScript.SetColor(nutRender.color, configGame.GetMaterial(nutRender.color));
                nutScript.SetupHidden(nutRender.isHidden, configGame.GetMaterialHidden());
                positionNutCurrent.y += configGame.NutHeight / 2;
            }
        }
    }

    protected void NormalizeData()
    {
        countTube = Mathf.Clamp(countTube, 3, 9);
        countTubeFinish = Mathf.Clamp(countTubeFinish, 1, countTube-1);
        ColorEnum[] allColors = (ColorEnum[])Enum.GetValues(typeof(ColorEnum));
        countColorFinish = Mathf.Clamp(countColorFinish, 1, allColors.Length);
        countHiddenNut = Mathf.Clamp(countHiddenNut, 0, 3);
        countTubeEmpty = Mathf.Clamp(countTubeEmpty, 0, countTube - countTubeFinish);
        loopCount = Mathf.Max(5, loopCount);
    }
    protected void MakeHiddenNut(List<TubeRender> tubeRenders)
    {
        if (countHiddenNut == 0)
            return;
        foreach (TubeRender tube in tubeRenders)
        {
            if (tube.IsEmpty || tube.nuts.Count == 1)
                continue;
            List<NutRender> nutsList = tube.nuts.ToList();
            nutsList.Reverse();
            int hiddenCount = Mathf.Min(countHiddenNut, nutsList.Count - 1);
            foreach (var item in nutsList.GetRange(0, hiddenCount))
            {
                item.isHidden = true;
            }

        }
    }

    protected void MakeTubesEmpty(List<TubeRender> tubeRenders)
    {
        if (countTubeEmpty == 0)
        {
            return;
        }
        if (countTubeEmpty > countTube - countTubeFinish)
        {
            Debug.LogWarning("Too many empty tubes requested");
            countTubeEmpty = 1;
        }
        List<TubeRender> tubeRendersTemp = new List<TubeRender>(tubeRenders);
        List<TubeRender> tubeEmpty = tubeRendersTemp.OrderBy(_ => UnityEngine.Random.value)
                                                .Take(countTubeEmpty)
                                                .ToList();
        tubeRendersTemp.RemoveAll(t => tubeEmpty.Contains(t));

        foreach (TubeRender tube in tubeEmpty)
        {
            while (!tube.IsEmpty)
            {
                TubeRender target = tubeRendersTemp.Find(t => !t.IsFull);
                if (target == null)
                {
                    Debug.LogError("data");
                    return;
                }
                NutRender nut = tube.nuts.Pop();
                target.PushNut(nut);
            }
        }

    }

    protected void Shuffle(List<TubeRender> tubeRenders)
    {
        for (int step = 0; step < loopCount; step++)
        {
            int fromIndex = Random.Range(0, tubeRenders.Count);
            int toIndex = Random.Range(0, tubeRenders.Count);
            TubeRender fromTube = tubeRenders[fromIndex];
            TubeRender toTube = tubeRenders[toIndex];
            if (fromTube.IsEmpty || fromIndex == toIndex || toTube.IsFull)
                continue;
            toTube.PushNut(fromTube.nuts.Pop());
        }
    }


    protected List<ColorEnum> GenerateColor()
    {
        ColorEnum[] allColors = (ColorEnum[])Enum.GetValues(typeof(ColorEnum));
        List<ColorEnum> colors = new List<ColorEnum>();
        if (countColorFinish < allColors.Length)
        {
            return allColors
                .OrderBy(_ => UnityEngine.Random.value)
                .Take(countColorFinish)
                .ToList();
        }
        colors.AddRange(allColors);

        while (colors.Count < countTubeFinish)
        {
            ColorEnum randomColor = allColors[UnityEngine.Random.Range(0, allColors.Length)];
            colors.Add(randomColor);
        }
        return colors;
    }
}
#endif