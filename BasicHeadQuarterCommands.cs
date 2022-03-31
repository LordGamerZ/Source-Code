using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Written by Abdul Galeel Ali

public class BasicHeadQuarterCommands : BasicFactoryCommands
{
    public float WoodIncome;
    public float OreIncome;
    public float GoldIncome;

    public float Effectiveness;

    public void Awake()
    {
        SelectedSprite.enabled = false;

        Effectiveness = 1;
    }

    public void Setup()
    {
        if (Physics.Raycast(new Vector3(transform.position.x, 10, transform.position.z + 5), Vector3.down * 20, out RaycastHit hitOne, 20))
        {
            hitOne.transform.GetComponent<HexPos>().SetPath(true);
        }

        if (Physics.Raycast(new Vector3(transform.position.x - 5, 10, transform.position.z - 2.5f), Vector3.down * 20, out RaycastHit hitTwo, 20))
        {
            hitTwo.transform.GetComponent<HexPos>().SetPath(true);
        }

        if (Physics.Raycast(new Vector3(transform.position.x + 5, 10, transform.position.z - 2.5f), Vector3.down * 20, out RaycastHit hitThree, 20))
        {
            hitThree.transform.GetComponent<HexPos>().SetPath(true);
        }

        if (OwnerID == Client.Instance.MyID)
        {
            int factoryID = GetComponent<ServerObject>().ViewID;

            if (Physics.Raycast(new Vector3(transform.position.x, 10, transform.position.z - 5), Vector3.down * 20, out RaycastHit hitFour, 20))
            {
                ClientSend.CreateUnit(hitFour.transform.GetComponent<ServerObject>().ViewID, factoryID, 0);
            }

            if (Physics.Raycast(new Vector3(transform.position.x - 5, 10, transform.position.z + 2.5f), Vector3.down * 20, out RaycastHit hitFive, 20))
            {
                ClientSend.CreateUnit(hitFive.transform.GetComponent<ServerObject>().ViewID, factoryID, 0);
            }

            if (Physics.Raycast(new Vector3(transform.position.x + 5, 10, transform.position.z + 2.5f), Vector3.down * 20, out RaycastHit hitSix, 20))
            {
                ClientSend.CreateUnit(hitSix.transform.GetComponent<ServerObject>().ViewID, factoryID, 0);
            }
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

        if (OwnerID == Client.Instance.MyID)
        {
            ClientSend.UpdateResources(WoodIncome * Effectiveness, OreIncome * Effectiveness, GoldIncome * Effectiveness);
        }
    }

    public override void Death()
    {
        CurrentPos.Remove(3);
        CurrentPos.Building = null;

        if (OwnerID == Client.Instance.MyID)
        {
            ClientSend.LeaveLobby();
            PlayerInteraction.Instance.Lose();
        }
    }
}