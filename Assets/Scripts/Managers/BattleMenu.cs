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

    public string codeletters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ!_&#*+%?£@§$";
    double scrambleProgress;
    double scrambleIndex;
    double numberCooldown;
    double maxNumberCooldown = 2.5;

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
            
        }
    }

        IEnumerator CustomMenu()
        {
            bool inMenu = true;

            for (int i = 0; i < customMenu.Count; i++)
            {
                customMenuSelection[i].transform.DOMoveX(-6.75f, 0.2f);

                string text = String.Format(cMenuString[(int)customMenu[i]], attackTypes[0]);

                StartCoroutine(CustMenuScramble(text, i));
            }

            customMenuSelection[cMenuSelection].transform.DOMoveX(-5.6875f, 0.2f);
            cMenuSelRenderer[cMenuSelection].sprite = customMenuSprite[1];
            yield return new WaitForSeconds(0.2f);

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

    IEnumerator CustMenuScramble(string input, int number)
    {
        string message = "";

        numberCooldown = maxNumberCooldown;
        bool scramble = true;
        
        while(scramble)
        {
            message = ScrambleText(input);

            if(message == String.Empty)
            {
                scramble = false;
                customMenuSelection[number].GetComponentInChildren<TextMeshPro>().text = input; 
            }
            else
            {
                customMenuSelection[number].GetComponentInChildren<TextMeshPro>().text = message; 
            }
        
            yield return null;
        }

        yield return null;
    }

    string ScrambleText(string input) 
    { 
        string newstr = ""; 

        if (numberCooldown > 0) 
        {
            numberCooldown -= Time.deltaTime;
            

            for (int i = 0; i < input.Length; i++) 
            {
                double progress = (maxNumberCooldown - numberCooldown) / maxNumberCooldown;
                double index = progress * input.Length;

                if (i < (int)index) 
                {
                    newstr += input[i];
                }
                else 
                {
                    if (input[i] != ' ') 
                    {
                        newstr += (char)codeletters[Rand.Next() % codeletters.Length];
                    }
                    else {
                        newstr += ' ';
                    }
                }
            }
        }

        return newstr;
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