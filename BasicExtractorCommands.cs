using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Written by Abdul Galeel Ali

public class BasicExtractorCommands : UnitSelectable
{
    public float WoodIncome;
    public float StoneIncome;
    public float GoldIncome;

    public float Effectiveness;

    public void Awake()
    {
        SelectedSprite.enabled = false;

        Effectiveness = 1;
    }

    public override void Death()
    {
        CurrentPos.Remove(3);
        CurrentPos.Building = null;
        Destroy(gameObject);
    }

    public override void NextTurn()
    {
        if (OwnerID == Client.Instance.MyID)
        {
            ClientSend.UpdateResources(WoodIncome * Effectiveness, StoneIncome * Effectiveness, GoldIncome * Effectiveness);
        }
    }
}