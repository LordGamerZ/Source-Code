using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Written by Abdul Galeel Ali

public class BasicFactoryCommands : UnitSelectable
{
    public List<Profile> Profiles;
    public List<int> BuildQueue; 

    public float Timer;

    public void Awake()
    {
        SelectedSprite.enabled = false;
        BuildQueue = new List<int>();

        Timer = 0;
    }

    public void CreateUnit(HexPos hexPos, int viewID, int profileID, Color color)
    {
        UnitSelectable unitSelectable = Instantiate(Profiles[profileID].Prefab, hexPos.transform.position, Quaternion.identity).GetComponent<UnitSelectable>();

        PlayerInteraction.Instance.ServerObjects[viewID] = unitSelectable;

        unitSelectable.OwnerID = OwnerID;
        unitSelectable.TeamNum = TeamNum;

        unitSelectable.CurrentPos = hexPos;
        unitSelectable.ViewID = viewID;

        if (unitSelectable.SelectableType == SelectableTypes.Melee)
        {
            hexPos.MeleeUnit = unitSelectable as MeleeCommands;
            hexPos.Select(0);
            unitSelectable.Model.GetComponent<Renderer>().material.color = color;
        }
        else if (unitSelectable.SelectableType == SelectableTypes.Ranged)
        {
            hexPos.RangedUnit = unitSelectable as RangedCommands;
            hexPos.Select(1);
            unitSelectable.Model.GetComponent<Renderer>().material.color = color;
        }
        if (unitSelectable.SelectableType == SelectableTypes.Builder)
        {
            hexPos.Builder = unitSelectable as BuilderCommands;
            hexPos.Select(2);
        }
    }

    public void AddToQueue(int unitNum)
    {
        if (UIControl.Instance.CanAfford(-Profiles[unitNum].WoodCost, -Profiles[unitNum].StoneCost, -Profiles[unitNum].GoldCost)) 
        {
            BuildQueue.Add(unitNum);
            ClientSend.UpdateResources(-Profiles[unitNum].WoodCost, -Profiles[unitNum].StoneCost, -Profiles[unitNum].GoldCost); 
        }
    }

    public override void NextTurn()
    {
        if (BuildQueue.Count > 0)
        {
            Timer += 1;

            if (Timer >= Profiles[BuildQueue[0]].ProductionTime)
            {
                ClientSend.CreateUnit(CurrentPos.ViewID, ViewID, BuildQueue[0]);
                Timer = 0;
                BuildQueue.RemoveAt(0);
            }
        }
    }

    public override void Death()
    {
        CurrentPos.Remove(3);
        CurrentPos.Building = null;
        Destroy(gameObject);
    }
}
