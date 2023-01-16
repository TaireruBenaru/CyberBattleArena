using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BattleManager: MonoBehaviour
{
    public DriveInfo[] allDrives;

    IEnumerator Start()
    {
        yield return StartCoroutine(CustomMenu());
    }

    IEnumerator CustomMenu()
    {
        allDrives = DriveInfo.GetDrives();

        foreach (DriveInfo d in allDrives)
        {
            if(d.IsReady && d.DriveType == DriveType.Removable)
            {
                Debug.Log("Potential Match! Looking for Chip Data...");
                Debug.Log(d.Name);

                if(File.Exists(d.Name + @"code.cdat"))
                {
                    Debug.LogError("Match!");
                }
            }
        }

        yield return null;
    }
}