using Cathei.BakingSheet.Unity;
using Cathei.BakingSheet;
using UnityEngine;
using Microsoft.Extensions.Logging;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = ("SO/Tool/CsvManager"))]
public class CsvCollectionManager : ScriptableObject
{
    private static string path = "Assets/Csv";
    private static CsvSheetConverter converter;
    private static CSVContainer sheetContainer;

    public static CSVContainer SheetContainer
    {
        get
        {
            if (sheetContainer != null)
            {
                LoadSheetContainer();
                return sheetContainer;
            }
            GenericSheetContainer();
            return sheetContainer;
        }
    }

    [Button]
    public static void GenericSheetContainer()
    {
        sheetContainer = new(new UnityLogger());
        converter = new CsvSheetConverter(path);
        LoadSheetContainer();
    }

    public static async void LoadSheetContainer()
    {
        await sheetContainer.Bake(converter);
    }

    [Button]
    public async void CreateSheet()
    {
        GenericSheetContainer();
        await sheetContainer.Store(converter);
    }
}
#endif