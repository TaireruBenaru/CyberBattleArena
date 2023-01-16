using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using DG.Tweening;
using TMPro;

public partial class BattleManager
{
    public GameObject battleSelectionPrefab;
    public Sprite[] customMenuSprite;

    public int cMenuSelection = 0;

    public List<GameObject> customMenuSelection;
    public List<SpriteRenderer> cMenuSelRenderer;
    List<CMenuOptions> customMenu = new List<CMenuOptions>();
    float[] cMenuSelectionPos = new float[] { 4.375f, 3.125f, 1.875f, 0.625f, -0.625f, -1.875f };
    string[] cMenuString = new string[] { "{0} Attack", "Data Chip", "Tension", "Tactics", "Guard", "Escape" };
    string[] attackTypes = new string[] { "Buster", "Breaker", "Magic"};

    public string codeletters = "&#*+%?£@§$";
    string message;
    public int current_length = 0;
    public bool fadeBuffer = false;

    System.Random Rand = new System.Random();




    void InitCMenuSelection()
    {
        customMenu = new List<CMenuOptions>();

        customMenu.Add(CMenuOptions.ATTACK);
        customMenu.Add(CMenuOptions.DATACHIP);
        customMenu.Add(CMenuOptions.TENSION);
        customMenu.Add(CMenuOptions.TACTICS);
        customMenu.Add(CMenuOptions.GUARD);
        customMenu.Add(CMenuOptions.ESCAPE);
        //customMenu.Add(CMenuOptions.ENDTURN);

        for (int i = 0; i < customMenu.Count; i++)
        {
            customMenuSelection.Add(Instantiate(battleSelectionPrefab, new Vector3(-10f, cMenuSelectionPos[i]), Quaternion.identity));
            cMenuSelRenderer.Add(customMenuSelection[i].GetComponent<SpriteRenderer>());
            Messenger(i);
        }
    }

        IEnumerator CustomMenu()
        {
            bool inMenu = true;

            for (int i = 0; i < customMenu.Count; i++)
            {
                customMenuSelection[i].transform.DOMoveX(-6.75f, 0.2f);
            }
            yield return new WaitForSeconds(0.2f);

            customMenuSelection[cMenuSelection].transform.DOMoveX(-6.75f, 0.2f);
            cMenuSelRenderer[cMenuSelection].sprite = customMenuSprite[0];

            while (inMenu)
            {
                if(InputManager.Instance.upButtonGet)
                {
                    yield return StartCoroutine(DecrementCMenuSelection());
                }

                if(InputManager.Instance.downButtonGet)
                {
                    yield return StartCoroutine(IncrementCMenuSelection());
                }
                

                yield return null;
            }



            yield return null;
        }

        IEnumerator IncrementCMenuSelection()
        {
            customMenuSelection[cMenuSelection].transform.DOMoveX(-6.75f, 0.2f);
            cMenuSelRenderer[cMenuSelection].sprite = customMenuSprite[0];
            
            cMenuSelection++;

            if(cMenuSelection == customMenu.Count)
            {
                cMenuSelection = 0;
            }

            customMenuSelection[cMenuSelection].transform.DOMoveX(-5.6875f, 0.2f);
            cMenuSelRenderer[cMenuSelection].sprite = customMenuSprite[1];

            yield return new WaitForSeconds(0.15f);
        }

    IEnumerator DecrementCMenuSelection()
    {
        customMenuSelection[cMenuSelection].transform.DOMoveX(-6.75f, 0.2f);
        cMenuSelRenderer[cMenuSelection].sprite = customMenuSprite[0];
        
        cMenuSelection--;

        if(cMenuSelection == -1)
        {
            cMenuSelection = customMenu.Count-1;
        }

        customMenuSelection[cMenuSelection].transform.DOMoveX(-5.6875f, 0.2f);
        cMenuSelRenderer[cMenuSelection].sprite = customMenuSprite[1];

        yield return new WaitForSeconds(0.15f);
    }

    void Messenger(int number)
    {
        message = String.Format(cMenuString[(int)customMenu[number]], attackTypes[0]);
        AnimateIn(number, 100);
    }


    public string GenerateRandomString(int length) 
    { 
        string random_text = "";
        while (random_text.Length < length)
        {
            random_text += codeletters[(int)(System.Math.Floor((double)(Rand() * codeletters.Length)))];
        }
        return random_text;
    }

    void AnimateIn(int number, int time) 
    { 
        if (current_length < message.Length) 
        {
            current_length += 2;
            if (current_length > message.Length)  
            {
                current_length = message.Length;
            }
            customMenuSelection[number].GetComponentInChildren<TextMeshPro>().text = GenerateRandomString(current_length);
            time = time + 20;
            AnimateIn(time);
        }
        else
        {
            AnimateFadeBuffer(20);
        }
    }


    void AnimateFadeBuffer(int time) 
    { 
        if (fadeBuffer == false)
        {
            fadeBuffer = new string[message.Length];
            for (int i = 0; i < message.Length; i++)
            {
                fadeBuffer[i] = (System.Math.Floor(System.Math.Random() * 12)) + 1 + " " + message[i];
            }
        }
        string message = "";
        bool do_cycles = false;
        for (int i = 0; i < fadeBuffer.Length; i++)
        {
            string[] fader = fadeBuffer[i].Split(" ");
            if (Convert.ToInt32(fader[0]) > 0) 
            {
                do_cycles = true;
                int curr = Convert.ToInt32(fader[0]);
                curr--;
                message += codeletters[(int)(System.Math.Floor((double)(Rand() * codeletters.Length)))];
            }
            else
            {
                message += fadeBuffer[i][1];
            }
        }
        Console.WriteLine(message);
        if (do_cycles == true)
        {
            time = time + 50;
            AnimateFadeBuffer(time);
        }
        else
        {
            //END
        }
    }

        IEnumerator DataChipMenu()
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

enum CMenuOptions
{
    ATTACK,
    DATACHIP,
    TENSION,
    TACTICS,
    GUARD,
    ESCAPE,
    ENDTURN
}