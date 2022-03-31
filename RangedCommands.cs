using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Written by Abdul Galeel Ali

public enum RangedTypes
{
    Archer,
    Catapult
}

public class RangedCommands : UnitSelectable
{
    public float MaxMoves;
    public float CurrentMoves;

    public int MaxAttacks;
    public int NumAttacks;

    public float AttackRange;
    public float Damage;

    public RangedTypes RangedType;

    private void Awake()
    {
        SelectedSprite.enabled = false;
        NumAttacks = 0;
        CurrentMoves = 0;

        if (Damage > 0)
        {
            Damage *= -1;
        }
    }

    public void Attack(HexPos hexPos)
    {
        NumAttacks += 1;

        if (RangedType == RangedTypes.Archer)
        {
            if (hexPos.MeleeUnit.MeleeType == MeleeTypes.Cavalry)
            {
                hexPos.MeleeUnit.ChangeHealth(Damage * 2);
            }
            else
            {
                hexPos.MeleeUnit.ChangeHealth(Damage);
            }

            if (hexPos.RangedUnit)
            {
                hexPos.RangedUnit.ChangeHealth(Damage);
            }

            if (hexPos.Builder)
            {
                hexPos.Builder.ChangeHealth(Damage);
            }

            if (hexPos.Building)
            {
                hexPos.Building.ChangeHealth(Damage * 0.1f);
            }
        }
        else
        {
            if (hexPos.MeleeUnit && RangedType == RangedTypes.Archer)
            {
                hexPos.MeleeUnit.ChangeHealth(Damage * 0.1f);
            }

            if (hexPos.RangedUnit)
            {
                hexPos.RangedUnit.ChangeHealth(Damage * 0.1f);
            }

            if (hexPos.Builder)
            {
                hexPos.Builder.ChangeHealth(Damage * 0.1f);
            }

            if (hexPos.Building)
            {
                hexPos.Building.ChangeHealth(Damage * 0.1f);
            }
        }
    }

    public override void Death()
    {
        CurrentPos.Remove(1);
        CurrentPos.RangedUnit = null;
        Destroy(gameObject);
    }

    public int CanAttack(HexPos hexPos)
    {
        if (Vector3.Distance(hexPos.transform.position, CurrentPos.transform.position) < 8 && Vector3.Distance(hexPos.transform.position, CurrentPos.transform.position) > 1)
        {
            if (hexPos.MeleeUnit)
            {
                if (hexPos.MeleeUnit.TeamNum == TeamNum)
                {
                    return 1;
                }
                else if (NumAttacks + 1 <= MaxAttacks)
                {
                    return 2;
                }
            }

            if (hexPos.RangedUnit)
            {
                if (NumAttacks + 1 <= MaxAttacks && hexPos.RangedUnit.TeamNum != TeamNum)
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
            }

            if (hexPos.Building)
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
        else if(Vector3.Distance(hexPos.transform.position, CurrentPos.transform.position) < AttackRange + 3 && Vector3.Distance(hexPos.transform.position, CurrentPos.transform.position) > 8)
        {
            if(hexPos.MeleeUnit)
            {
                if(hexPos.MeleeUnit.TeamNum != TeamNum)
                {
                    return 2;
                }
            }
            else if (hexPos.RangedUnit)
            {
                if (hexPos.RangedUnit.TeamNum != TeamNum)
                {
                    return 2;
                }
            }
            else if (hexPos.Builder)
            {
                if (hexPos.Builder.TeamNum != TeamNum)
                {
                    return 2;
                }
            }
            else if (hexPos.Building)
            {
                if (hexPos.Building.TeamNum != TeamNum)
                {
                    return 2;
                }
            }
        }

        return 0;
    }

    public void Move(HexPos hexPos)
    {
        if (RangedType == RangedTypes.Archer || (RangedType == RangedTypes.Catapult && hexPos.BiomeType != Biomes.Forest))
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

            CurrentPos.RangedUnit = null;
            CurrentPos.Remove(1);
            CurrentPos = hexPos;
            transform.position = hexPos.transform.position;
            hexPos.RangedUnit = this;

            CurrentPos.Select(1);
        }
    }

    public override void NextTurn()
    {
        CurrentMoves = 0;
        NumAttacks = 0;
    }
}
