using Cathei.BakingSheet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSpawnSheet : Sheet<DataSpawnSheet.Row>
{
    public class Row : SheetRow { 
    
        public int CountTube { get; private set; }
        public int CountTubeFinish { get; private set; }
        public int CountColorFinish { get; private set; }
        public int CountHiddenNut { get; private set; }
        public int LoopCount { get; private set; }
        public int CountTubeEmpty { get; private set; }
    }
}
