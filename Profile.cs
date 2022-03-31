using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Written by Abdul Galeel Ali

[CreateAssetMenu(fileName = "Profile", menuName = "New Profile")]
public class Profile : ScriptableObject
{
    public string Name;
    public string Description;

    public float StoneCost;
    public float WoodCost;
    public float GoldCost;

    public int ProductionTime;

    public Sprite Icon;
    public GameObject Prefab;
}
