using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Written by Abdul Galeel Ali

public class ProfileToolTip : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Time;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI Cost;

    public void SetTexts(string name, string description, float time, float gold, float ore, float wood, string additional)
    {
        Name.text = name;
        Time.text = "Production Time " + time;
        Description.text = description + "\n\n" + additional;
        Cost.text = "Gold " + gold + "\nStone " + ore + "\nWood " + wood;        
    }
}