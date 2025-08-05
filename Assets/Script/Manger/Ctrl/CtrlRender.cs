using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlRender : Singleton<CtrlRender>
{
    private ColorEnum colorSelect;
    private Ability abilityCurrent;
    private Nut nutSelect;
    private Tube tubeSelect;
    [SerializeField] private Material matNutSelect;
    [SerializeField] private float rayDistance;
    [SerializeField] private LayerMask layerNut;
    [SerializeField] private SOConfigGame gameConfig;
    [SerializeField] private RenderLevel renderLevel;
    private GameObject map;
    public enum Ability
    {
        Nothing,
        ChangeColor,
        DeleteTube,
        DeleteNut,
        Show,
        Hidden,
    }

    public void CreateMap() {
        map = renderLevel.SpawnMode();
    }

    public void DestroyMap() { 
        Destroy(map);
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);
        if (Physics.Raycast(ray, out hit, 100, layerNut))
        {
            nutSelect?.ResetColor();
            nutSelect = hit.collider.GetComponent<Nut>();
            nutSelect.SetColor(matNutSelect);
        }
        else
        {
            nutSelect?.ResetColor();
            nutSelect = null;
        }

        //if()

        if (Input.GetMouseButtonDown(0) && nutSelect != null)
        {
            switch (abilityCurrent)
            {
                case Ability.ChangeColor:
                    nutSelect.SetColor(colorSelect, gameConfig.GetMaterial(colorSelect));
                    break;
                case Ability.DeleteTube:
                    Destroy(nutSelect.GetTubeCurrent().gameObject);
                    nutSelect = null;
                    break;
                case Ability.Show:
                    nutSelect.SetupHidden(false, matNutSelect);
                    break;
                case Ability.Hidden:
                    nutSelect.SetupHidden(true, gameConfig.GetMaterialHidden());
                    break;
                case Ability.DeleteNut:
                    nutSelect.GetTubeCurrent().ListRemoveNut(nutSelect);
                    Destroy(nutSelect.gameObject);
                    nutSelect = null;
                    break;


            }
        }

    }

    public void ChangeColor(ColorEnum colorName)
    {
        abilityCurrent = Ability.ChangeColor;
        colorSelect = colorName;
    }

    public void SetAbility(CtrlRender.Ability ability)
    {
        this.abilityCurrent = ability;
    }
}
