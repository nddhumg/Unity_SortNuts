using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SO/Config")]
public class SOConfigGame : ScriptableObject
{
    [SerializeField] private float nutHeight;
    [SerializeField] private int maxNutInTube;
    [SerializeField] private float positionYNutPop;
    [SerializeField] private float positionYNutZero;
    [SerializeField] private List<ColorNut> color;
    [SerializeField] private Material materialColorHidden;
    public float NutHeight => nutHeight;
    public int MaxNutInTube => maxNutInTube;

    public float PositionYNutPop => positionYNutPop;
    public float PositionYNutZero => positionYNutZero;
    public Material GetMaterial(ColorEnum colorEnum)
    {
        return color.Find(c => c.enumColor == colorEnum).material;
    }
    public Material GetMaterialHidden() => materialColorHidden;
}

[Serializable]
public class ColorNut
{
    public ColorEnum enumColor;
    public Material material;
}

