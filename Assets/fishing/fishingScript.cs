using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KModkit;
using System.Security.Cryptography;
using System;
using System.Linq;

public class fishingScript : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMBombModule bombModule;
    public KMAudio audio;
    public GameObject FishingRod;
    public GameObject Hook;
    public GameObject Line;
    public GameObject Sprites;
    public TextMesh FishCaught;
    public TextMesh TrashCollected;
    public KMSelectable Reel;
    public Sprite[] Fish;
    public SpriteRenderer Fishies;

    string Indicators;
    int ParallelPort, PS2Port, Vowels, Consonants, Evens, Odds, FishGoal, FishTotal, TrashTotal;

    private bool isActive;
    private bool isSolved;
    private static int moduleIdCounter = 1;
    private int moduleId;

    private bool showFish, showHook, showReel, showRod;

    // Use this for initialization
    void Start () {
        isSolved = false;
        showFish = false;
        showHook = true;
        showReel = true;
        showRod = true;
        FishTotal = 0;
        TrashTotal = 0;
        moduleId = moduleIdCounter++;
        bombModule.OnActivate += Activate;
        Reel.OnInteract += delegate { ReelFish(); return false; };
        GetSolveFish();
    }

    
    // Update is called once per frame
    void Update () {
        FishCaught.text = "Fish: " + FishTotal;
        TrashCollected.text = "Trash: " + TrashTotal;

		if(!showFish)
        {
            Sprites.gameObject.SetActive(false);
        }
        else
        {
            Sprites.gameObject.SetActive(true);
        }
        if (!showHook)
        {
            Hook.gameObject.SetActive(false);
            Line.gameObject.SetActive(true);
        }
        else
        {
            Hook.gameObject.SetActive(true);
            Line.gameObject.SetActive(false);
        }
        if (!showReel)
        {
            Reel.gameObject.SetActive(false);
        }
        else
        {
            Reel.gameObject.SetActive(true);
        }
        if (!showRod)
        {
            FishingRod.gameObject.SetActive(false);
        }
        else
        {
            FishingRod.gameObject.SetActive(true);
        }
    }

    void GetSolveFish()
    {
        ParallelPort = Bomb.GetPortCount(Port.Parallel);
        PS2Port = Bomb.GetPortCount(Port.PS2);
        for (int i = 0; i < 6; i++)
        {
            if ("AEIOU".Contains(Bomb.GetSerialNumber()[i]))
            {
                Vowels++;
            }
            if ("BCDFGHJKLMNPQRSTVWXYZ".Contains(Bomb.GetSerialNumber()[i]))
            {
                Consonants++;
            }
        }
        for (int i = 0; i < 6; i++)
        {
            if ("02468".Contains(Bomb.GetSerialNumber()[i]))
            {
                Evens++;
            }
            if ("13579".Contains(Bomb.GetSerialNumber()[i]))
            {
                Odds++;
            }
        }

        if (ParallelPort > 0)
        {
            FishGoal += 8;
        }
        if (PS2Port > 0)
        {
            FishGoal += 4;
        }
        if (Vowels == 6-Consonants-Evens-Odds)
        {
            FishGoal += 2;
        }
        if (Evens == 6-Odds-Consonants-Vowels)
        {
            FishGoal += 1;
        }
        FishGoal = FishGoal + 46;
        Debug.LogFormat("[Fishing #{0}] Fish Need to Catch: {1}", moduleId, Fish[FishGoal]);
    }
    private void Activate()
    {
        isActive = true;
    }

    void ReelFish()
    {
        showReel = false;
        showHook = false;
        StartCoroutine(Fishing());
    }

    IEnumerator Fishing()
    {
        audio.PlaySoundAtTransform("Fishing_rod_cast", transform);
        yield return new WaitForSecondsRealtime(5.0f);
        int arrayFish = UnityEngine.Random.Range(0, Fish.Length);
        Debug.LogFormat("[Fishing #{0}] Module Count: {1}", moduleId, Bomb.GetSolvableModuleNames().Count());
        if (FishTotal >= Bomb.GetSolvableModuleNames().Count()*2)
        {
            Fishies.sprite = Fish[FishGoal];
            arrayFish = FishGoal;
        }
        else
        {
            Fishies.sprite = Fish[arrayFish];
        }
        showFish = true;
        showRod = false;
        yield return new WaitForSecondsRealtime(2.0f);
        audio.PlaySoundAtTransform("Fishing_rod_reel_in2", transform);
        if((arrayFish >= 18 && arrayFish <=45)||(arrayFish >=92 && arrayFish <= 99))
        {
            TrashTotal++;
        }
        else
        {
            FishTotal++;
        }
        Debug.LogFormat("[Fishing #{0}] Caught: {1}", moduleId, Fish[arrayFish]);
        if (FishGoal == arrayFish)
        {
            bombModule.HandlePass();
            showFish = false;
        }
        else
        {
            showRod = true;
            showReel = true;
            showHook = true;
            showFish = false;
        }
    }
}