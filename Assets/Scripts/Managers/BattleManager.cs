using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public partial class BattleManager: MonoBehaviour
{
    public DriveInfo[] allDrives;

    IEnumerator Start()
    {
        InitCMenuSelection();
        yield return StartCoroutine(CustomMenu());
    }
}