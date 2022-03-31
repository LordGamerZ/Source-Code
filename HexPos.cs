using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Written by Abdul Galeel Ali

public enum Terrains
{
    Land,
    Sea,
    Amphibious
}

public enum Biomes
{
    Plains,
    Hills,
    Forest
}

public class HexPos : ServerObject
{
    public MeleeCommands MeleeUnit;
    public RangedCommands RangedUnit;
    public BuilderCommands Builder;
    public UnitSelectable Building;

    public BillBoard Board;

    public int Showing;
    public bool IsPath;

    public Terrains TerrainType;
    public Biomes BiomeType;

    private void Awake()
    {
        Building = null;
        MeleeUnit = null;
        RangedUnit = null;
        Builder = null;
        IsPath = false;
        Showing = -1;
    }

    public void Remove(int whichUnit)
    {
        if (whichUnit == 0)
        {
            Board.MeleeIcon.SetActive(false);
        }
        if (whichUnit == 1)
        {
            Board.RangedIcon.SetActive(false);
        }
        if (whichUnit == 2)
        {
            Board.BuilderIcon.SetActive(false);
        }
        if (whichUnit == 3)
        {
            Board.BuildingIcon.SetActive(false);
        }

        if (MeleeUnit && whichUnit != 0)
        {
            MeleeUnit.Model.SetActive(true);
            Showing = 0;
        }
        else if (RangedUnit && whichUnit != 1)
        {
            RangedUnit.Model.SetActive(true);
            Showing = 1;
        }
        else if (Builder && whichUnit != 2)
        {
            Builder.Model.SetActive(true);
            Showing = 2;
        }
        else if (Building && whichUnit != 3)
        {
            Building.Model.SetActive(true);
            Showing = 3;
        }
        else
        {
            Showing = -1;
            Board.gameObject.SetActive(false);
        }
    }

    public void Select(int whichUnit)
    {
        if (Showing != -1)
        {
            if (Showing == 0)
            {
                MeleeUnit.Model.SetActive(false);
            }
            else if (Showing == 1)
            {
                RangedUnit.Model.SetActive(false);
            }
            else if (Showing == 2)
            {
                Builder.Model.SetActive(false);
            }
            else if (Showing == 3)
            {
                Building.Model.SetActive(false);
            }
        }

        Board.gameObject.SetActive(true);

        if (whichUnit == 0)
        {
            MeleeUnit.Model.SetActive(true);
            Board.MeleeIcon.SetActive(true);
        }
        else if (whichUnit == 1)
        {
            RangedUnit.Model.SetActive(true);
            Board.RangedIcon.SetActive(true);
        }
        else if (whichUnit == 2)
        {
            Builder.Model.SetActive(true);
            Board.BuilderIcon.SetActive(true);
        }
        else if (whichUnit == 3)
        {
            Building.Model.SetActive(true);
            Board.BuildingIcon.SetActive(true);
        }

        Showing = whichUnit;
    }

    public void SetPath(bool isPath)
    {
        IsPath = isPath;

        if (isPath)
        {

        }
        else
        {

        }
    }
}