using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataChip: ScriptableObject
{
    public string Name;
    public string Description;

    public ChipType Type;
    public ChipElement Element;
    public int Memory;
}

public enum ChipIndex
{
    BlankChip = 0,
    HeartRecovery = 1,
    Barrier = 2,
    SuperBarrier = 3,
    UltraBarrier = 4,
}

public enum ChipType
{
    None,
    Offense,
    Support,
    Recovery
}

public enum ChipElement
{
    None,
    Water,
    Flame,
    Wind,
    Electric,
    Null,
}