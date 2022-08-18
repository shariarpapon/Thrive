using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Harvestable", menuName = "Scriptables/Harvestable/New Harvestable")]
public class Harvestable : ScriptableObject
{
    public bool hasRquirement;
    public GameObject particle;
    public GameObject dropContainer;
    public ItemType requiredType;
    public float harvestDelay;

    [Header("SFX")] 
    public bool start_SFX;
    public AudioSet SFX;
    [Space]
    public bool end_SFX;
    public AudioSet finalSFX;
    public Harvest[] harvests;
}

[Serializable]
public struct Harvest
{
    public Item dropItem;
    public Frequency frequency;
}

[Serializable]
public struct Frequency
{
    public int minAmount;
    public int maxAmount;
    [Range(0,1 )]
    public float chance;
}
