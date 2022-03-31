using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Written by Abdul Galeel Ali

public enum SelectableTypes
{
    Melee,
    Ranged,
    Builder,
    Barracks,
    HeadQuarters,
    Extractor
}

public abstract class UnitSelectable : ServerObject
{
    public SelectableTypes SelectableType;
    public Terrains MovementType;

    public GameObject Model;

    public int OwnerID;
    public int TeamNum;

    public SpriteRenderer SelectedSprite;

    public float MaxHealth;
    public float CurrentHealth;

    public HexPos CurrentPos;

    public void ChangeHealth(float Amount)
    {
        if (CurrentHealth + Amount > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        else if (CurrentHealth + Amount < 0)
        {
            Death();
        }
        else
        {
            CurrentHealth += Amount;
        }
    }

    public abstract void Death();

    public abstract void NextTurn();
}