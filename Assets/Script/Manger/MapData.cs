using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    [SerializeField] protected int countTubeFinish;
    public int CountTubeFinish { get => countTubeFinish; set => countTubeFinish = value; }
}
