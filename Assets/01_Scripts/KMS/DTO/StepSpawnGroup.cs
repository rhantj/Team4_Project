using System.Collections.Generic;
using System;

[Serializable]
public class StepSpawnGroup
{
    public int StepIndexToTrigger;
    public List<SpawnRequest> Requests;
}