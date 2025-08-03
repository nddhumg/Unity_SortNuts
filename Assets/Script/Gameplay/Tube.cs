using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using DG.Tweening;

public class Tube : MonoBehaviour
{
    private Stack<Nut> nuts = new();
    [SerializeField] private SOConfigGame gameConfig;
    [SerializeField] private Transform nutsParent;
    private Vector3 positionPush;
    private Vector3 positionPop;
    private Vector3 positionNutZero;

    public Transform NutsParent => nutsParent;
    public Vector3 PositionPop { get => positionPop; }
    public Vector3 PositionPush { get => positionPush;}

    public int CountNuts => nuts.Count;

    private void Start()
    {
        positionNutZero = transform.position;
        positionNutZero.y = gameConfig.PositionYNutZero;

        positionPop = transform.position;
        positionPop.y = gameConfig.PositionYNutPop;

        positionPush = positionNutZero;
        foreach (Transform nutGO in nutsParent)
        {
            Nut nut = nutGO.GetComponent<Nut>();
            nuts.Push(nut);
            nut.SetTubeCurrent(this);
            nutGO.position = positionPush;
            positionPush.y += gameConfig.NutHeight / 2;
        }
        if (nuts.Count != 0)
            positionPush.y -= gameConfig.NutHeight / 2;
    }

    public Nut PeekNuts()
    {
        if (nuts.Count == 0)
            return null;
        return nuts.Peek();
    }

    public void PopNut()
    {
        nuts.Pop();
        if (nuts.Count != 0)
            positionPush.y -= gameConfig.NutHeight / 2;
        if (nuts.Count == 0)
            return;
        Nut nutCurrent = nuts.Peek();
        if (nutCurrent.IsHidden)
        {
            nutCurrent.Show();
        }
    }

    public bool PushNuts(Nut nut)
    {
        Sequence sequence = DOTween.Sequence();
        if (nuts.Count == gameConfig.MaxNutInTube)
            return false;
        Tube tubeLast = nut.GetTubeCurrent();
        if (IsSameColor(nut))
        {
            if (nuts.Count != 0)
            {
                positionPush.y += gameConfig.NutHeight / 2;
            }
            nuts.Push(nut);
            tubeLast.PopNut();

            sequence.Append((nut.PushAnimation(this)));
            for (int i = nuts.Count - 1; i < gameConfig.MaxNutInTube; i++)
            {
                Nut nutNext = tubeLast.PeekNuts();
                if (nutNext == null)
                    break;
                if (IsSameColor(nutNext))
                {
                    positionPush.y += gameConfig.NutHeight / 2;
                    nuts.Push(nutNext);
                    tubeLast.PopNut();
                    sequence.Append(nutNext.PushAutoAnimation(this));
                }
                else { break; }
            }
            sequence.AppendCallback(() =>
            {
                if (IsWin())
                {
                    FinishTube();
                    GameCtrl.instance.CompletedTube();
                }
            });
            return true;
        }
        return false;
    }

    private void FinishTube()
    {
        foreach (Nut nut in nuts)
        {
            Destroy(nut.gameObject);
        }
        nuts.Clear();
        positionPush = positionNutZero;
    }

    private bool IsWin()
    {
        if (nuts.Count != gameConfig.MaxNutInTube)
            return false;
        ColorEnum colorCheck = nuts.Peek().ColorName;
        foreach (Nut nut in nuts)
        {
            if (colorCheck != nut.ColorName || nut.IsHidden)
                return false;
        }
        return true;
    }


    private bool IsSameColor(Nut nut)
    {
        if (nuts.Count == 0)
            return true;
        return nuts.Peek().ColorName == nut.ColorName;
    }

}
