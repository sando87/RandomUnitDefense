using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffState
{
    None, Keep, End
}

public abstract class BuffBase
{
    public abstract BuffState UpdateTargetSpec(in SpecProperty targetBasicSpec, out SpecProperty targetbuffedSpec);
}
