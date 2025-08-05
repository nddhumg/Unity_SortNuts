using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Nut : MonoBehaviour
{
    [SerializeField] private Tube tube;
    [SerializeField, FormerlySerializedAs("color")] private ColorEnum colorName;
    [SerializeField, FormerlySerializedAs("render")] private Renderer render;
    [SerializeField, FormerlySerializedAs("textHidden")] private GameObject textHidden;
    [SerializeField, ReadOnly, FormerlySerializedAs("isHidden")] private bool isHidden;
    [SerializeField, FormerlySerializedAs("materialColor")] private Material colorMaterial;
    public ColorEnum ColorName => colorName;
    public bool IsHidden => isHidden;

    public Tube GetTubeCurrent()
    {
        return tube;
    }

    public void Init(Tube tube) { 
        this.tube = tube;

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    public void SetColor(ColorEnum colorName, Material colorMaterial, bool isChange = true)
    {
        if (isChange)
        {
            this.colorName = colorName;
            this.colorMaterial = colorMaterial;
        }
        render.material = colorMaterial;
    }

    public void SetColor(Material colorMaterial)
    {
        render.material = colorMaterial;
    }


    public void ResetColor()
    {
        if(!isHidden)
            render.material = colorMaterial;
        else
            render.material = GameMode.instance.HiddenMaterial; 
    }

    public void SetupHidden(bool isHidden, Material colorHidden)
    {
        this.isHidden = isHidden;
        textHidden.SetActive(isHidden);
        render.material = isHidden ? colorHidden : colorMaterial;
    }

    public void Show()
    {
        this.isHidden = false;
        textHidden.SetActive(false);
        render.material = colorMaterial;
    }

    public void SetTubeCurrent(Tube tube)
    {
        this.tube = tube;
    }

    public Sequence PopAnimation()
    {
        float timePop = CalculateMoveTime();
        return AnimateNutMove(tube.PositionPop, -CalculateScrewRotation(), CalculateMoveTime());
    }

    public Sequence RevertAnimation()
    {
        float timeRevert = CalculateMoveTime();
        return AnimateNutMove(tube.PositionPush, CalculateScrewRotation(), CalculateMoveTime());
    }

    public Sequence PushAnimation(Tube tubeNew)
    {
        tube = tubeNew;
        transform.parent = tube.NutsParent;
        return AnimateNutMove(tube.PositionPush, CalculateScrewRotation(), CalculateMoveTime())
            .Prepend(transform.DOMove(tube.PositionPop, 0.2f));
    }

    public Sequence PushAutoAnimation(Tube tubeNew)
    {
        Vector3 positionPop = tube.PositionPop;
        Sequence sequence = AnimateNutMove(positionPop, -CalculateScrewRotation(), CalculateMoveTime());
        tube = tubeNew;
        transform.parent = tube.NutsParent;
        sequence.Append(transform.DOMove(tube.PositionPop, 0.2f));
        sequence.Append(AnimateNutMove(tube.PositionPush, CalculateScrewRotation(), CalculateMoveTime()));

        return sequence;
    }

    private Sequence AnimateNutMove(Vector3 targetPos, float rotation, float duration)
    {
        Sequence sequence = DOTween.Sequence()
            .Append(transform.DOMove(targetPos, duration))
            .Join(transform.DOLocalRotate(new Vector3(0, rotation, 0), duration, RotateMode.FastBeyond360))
            .OnComplete(() => transform.localRotation = Quaternion.Euler(0, 0, 0));
        return sequence;
    }


    private float CalculateMoveTime()
    {
        return (5 - tube.CountNuts) * 0.1f;
    }

    private float CalculateScrewRotation()
    {
        return (5 - tube.CountNuts) * 90;
    }
}
