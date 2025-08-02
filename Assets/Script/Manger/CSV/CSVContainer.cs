using Cathei.BakingSheet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVContainer : SheetContainerBase
{
    public CSVContainer(Microsoft.Extensions.Logging.ILogger logger) : base(logger) {
        DataSpawnSheet = new();
    }

    public DataSpawnSheet DataSpawnSheet { get; private set; }
}
