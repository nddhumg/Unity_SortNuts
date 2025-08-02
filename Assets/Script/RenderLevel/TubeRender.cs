using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeRender
{
    int countMaxNuts;
    public Stack<NutRender> nuts;
    public Tube tube;
    public bool IsFull => nuts.Count == countMaxNuts;
    public bool IsEmpty => nuts.Count == 0;
    public TubeRender(int countMaxNuts, Tube tube)
    {
        this.countMaxNuts = countMaxNuts;
        nuts = new Stack<NutRender>();
        this.tube = tube;
    }

    public void PushNut(NutRender nutRender) {
        nutRender.indexInTube = nuts.Count;
        nuts.Push(nutRender);
    }

}
