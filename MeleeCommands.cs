using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Written by Abdul Galeel Ali

public enum MeleeTypes
{
    Infantry,
    Cavalry
}


public class MeleeCommands : UnitSelectable
{
    public float MaxMoves;
    public float CurrentMoves;

    public int MaxAttacks;
    public int NumAttacks;

    public float Damage;

    public MeleeTypes MeleeType;

    public void Awake()
    {
        SelectedSprite.enabled = false;
        CurrentMoves = 0;
        NumAttacks = 0;

        if(Damage > 0)
        {
            Damage *= -1;
        }
    }

    public override void Death()
    {
        CurrentPos.Remove(0);
        CurrentPos.MeleeUnit = null;
        Destroy(gameObject);
    }

    public void Attack(HexPos hexPos)
    {
        NumAttacks += 1;

        if (hexPos.MeleeUnit)
        {
            if (MeleeType == MeleeTypes.Cavalry && hexPos.MeleeUnit.MeleeType == MeleeTypes.Infantry)
            {
                hexPos.MeleeUnit.ChangeHealth(Damage * 2);
                ChangeHealth(hexPos.MeleeUnit.Damage);
            } 
            else if (MeleeType == MeleeTypes.Infantry && hexPos.MeleeUnit.MeleeType == MeleeTypes.Cavalry)
            {
                hexPos.MeleeUnit.ChangeHealth(Damage);
                ChangeHealth(hexPos.MeleeUnit.Damage * 2);
            }
        }
        else if (hexPos.RangedUnit)
        {
            if (hexPos.RangedUnit.RangedType == RangedTypes.Archer)
            {
                hexPos.RangedUnit.ChangeHealth(Damage * 2);
            }
            else
            {
                hexPos.RangedUnit.ChangeHealth(Damage);
            }
        }
        else if (hexPos.Builder)
        {
            hexPos.Builder.ChangeHealth(Damage);
        }
        else if (hexPos.Building)
        {
            hexPos.Building.ChangeHealth(Damage * 0.1f);
        }

        if (CanAttack(hexPos) == 1)
        {
            Move(hexPos);
        }
    }

    public int CanAttack(HexPos hexPos)
    {
        if (Vector3.Distance(hexPos.transform.position, CurrentPos.transform.position) < 8 && Vector3.Distance(hexPos.transform.position, CurrentPos.transform.position) > 1)
        {
            if (CurrentPos.IsPath)
            {
                if (CurrentMoves + 0.5 > MaxMoves)
                {
                    return 0;
                }
            }
            else
            {
                if (CurrentMoves + 1 > MaxMoves)
                {
                    return 0;
                }
            }

            if(hexPos.MeleeUnit)
            {
                if (NumAttacks + 1 <= MaxAttacks && hexPos.MeleeUnit.TeamNum != TeamNum)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }

            if (hexPos.RangedUnit)
            {
                if (hexPos.RangedUnit.TeamNum == TeamNum)
                {
                    return 0;
                }
                else if (NumAttacks + 1 <= MaxAttacks)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }

            if (hexPos.Builder)
            {
                if (hexPos.Builder.TeamNum == TeamNum)
                {
                    return 1;
                }
                else if (NumAttacks + 1 <= MaxAttacks)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }

            if(hexPos.Building)
            {
                if (hexPos.Building.TeamNum == TeamNum)
                {
                    return 1;
                }
                else if (NumAttacks + 1 <= MaxAttacks)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }

            return 1;
        }

        return 0;
    }

    public void Move(HexPos hexPos)
    {
        transform.rotation = Quaternion.Euler(0, Quaternion.LookRotation(hexPos.transform.position - transform.position).eulerAngles.y, 0); 

        if (CurrentPos.IsPath)
        {
            CurrentMoves += 0.5f;
        }
        else
        {
            CurrentMoves += 1;
        }

        CurrentPos.MeleeUnit = null;
        CurrentPos.Remove(0);
        CurrentPos = hexPos;
        transform.position = hexPos.transform.position;
        hexPos.MeleeUnit = this;

        CurrentPos.Select(0);
    }

    public override void NextTurn()
    {
        CurrentMoves = 0;
        NumAttacks = 0;
    }
}