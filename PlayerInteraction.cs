using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

//Written by Abdul Galeel Ali

public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction Instance;

    public static int MaxViewIDs;

    public int TeamNum;

    public Camera MainCamera;
    public BasicHeadQuarterCommands MyBase;
    public ServerObject[] ServerObjects;

    public bool IsTurn;

    public UnitSelectable Selected;

    public GameObject HexPosPrefab;
    public GameObject HillPosPrefab;
    public GameObject ForestPosPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }

        DontDestroyOnLoad(gameObject);

        ServerObjects = new ServerObject[MaxViewIDs];

        Instance = this;
        Selected = null;
        IsTurn = false;

        gameObject.SetActive(false);
    }

    private void Update()
    {
        HexPos hexPos = ReturnHexPos();

        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData PED = new PointerEventData(UIControl.Instance.EV)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> Results = new List<RaycastResult>();
            UIControl.Instance.GR.Raycast(PED, Results);

            if (UIControl.Instance.IsBuilding)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Selected.GetComponent<BuilderCommands>().RemoveForest();
                }
            }

            if (Results.Count > 0)
            {
                if (UIControl.Instance.IsBuilding || UIControl.Instance.IsRecruiting)
                {
                    foreach (RaycastResult RR in Results)
                    {
                        if (RR.gameObject.GetComponent<ProfileIcon>())
                        {
                            if (UIControl.Instance.IsBuilding)
                            {
                                Selected.GetComponent<BuilderCommands>().AttemptBuild(RR.gameObject.transform.GetSiblingIndex());
                            }
                            else if (UIControl.Instance.IsRecruiting)
                            {
                                Selected.GetComponent<BasicFactoryCommands>().AddToQueue(RR.gameObject.transform.GetSiblingIndex());
                            }

                            break;
                        }
                    }
                }
            }

            if (hexPos && Results.Count == 0)
            {
                int whichUnit = -1;

                if (Selected)
                {
                    if (hexPos == Selected.CurrentPos)
                    {
                        if (Selected.SelectableType == SelectableTypes.Melee)
                        {
                            if (hexPos.RangedUnit)
                            {
                                whichUnit = 1;
                            }
                            else if (hexPos.Builder)
                            {
                                whichUnit = 2;
                            }
                            else if (hexPos.Building)
                            {
                                whichUnit = 3;
                            }
                        }
                        else if (Selected.SelectableType == SelectableTypes.Ranged)
                        {
                            if (hexPos.Builder)
                            {
                                whichUnit = 2;
                            }
                            else if (hexPos.Building)
                            {
                                whichUnit = 3;
                            }
                        }
                        else if (Selected.SelectableType == SelectableTypes.Builder)
                        {
                            if (hexPos.Building)
                            {
                                whichUnit = 3;
                            }
                        }
                    }
                }

                if(whichUnit == -1)
                {
                    if (hexPos.MeleeUnit)
                    {
                        whichUnit = 0;
                    }
                    else if (hexPos.RangedUnit)
                    {
                        whichUnit = 1;
                    }
                    else if (hexPos.Builder)
                    {
                        whichUnit = 2;
                    }
                    else if (hexPos.Building)
                    {
                        whichUnit = 3;
                    }
                }

                if (whichUnit == 0)
                {
                    if (hexPos.MeleeUnit.TeamNum == TeamNum)
                    {
                        SelectUnit(hexPos.MeleeUnit);
                    }
                    else
                    {
                        DeselectUnit();
                    }
                }
                else if (whichUnit == 1)
                {
                    if (hexPos.RangedUnit.TeamNum == TeamNum)
                    {
                        SelectUnit(hexPos.RangedUnit);
                    }
                    else
                    {
                        DeselectUnit();
                    }
                }
                else if (whichUnit == 2)
                {
                    if (hexPos.Builder.TeamNum == TeamNum)
                    {
                        SelectUnit(hexPos.Builder);
                    }
                    else
                    {
                        DeselectUnit();
                    }
                }
                else if (whichUnit == 3)
                {
                    if (hexPos.Building.TeamNum == TeamNum)
                    {
                        SelectUnit(hexPos.Building);
                    }
                    else
                    {
                        DeselectUnit();
                    }
                }
                else
                {
                    DeselectUnit();
                }
            }
            else if (!hexPos && Results.Count == 0)
            {
                DeselectUnit();
            }
        }
        else if (Input.GetMouseButtonDown(1) && IsTurn && Selected)
        {
            if (hexPos)
            {
                if (Selected.SelectableType == SelectableTypes.Melee)
                {
                    int canAttack = Selected.GetComponent<MeleeCommands>().CanAttack(hexPos);

                    if (canAttack == 2)
                    {
                        ClientSend.UnitAttack(Selected.ViewID, hexPos.ViewID);
                    }
                    else if (canAttack == 1)
                    {
                        ClientSend.MoveUnit(Selected.ViewID, hexPos.ViewID);
                    }
                }
                else if (Selected.SelectableType == SelectableTypes.Ranged)
                {
                    int canAttack = Selected.GetComponent<RangedCommands>().CanAttack(hexPos);

                    if (canAttack == 2)
                    {
                        ClientSend.UnitAttack(Selected.ViewID, hexPos.ViewID);
                    }
                    else if (canAttack == 1)
                    {
                        ClientSend.MoveUnit(Selected.ViewID, hexPos.ViewID);
                    }
                }
                else if (Selected.SelectableType == SelectableTypes.Builder)
                {
                    if (Selected.GetComponent<BuilderCommands>().CanMove(hexPos))
                    {
                        ClientSend.MoveUnit(Selected.ViewID, hexPos.ViewID);
                    }
                }
            }
        }
    }

    public void Lose()
    {
        UIControl.Instance.DisplayLose();
        Destroy(Instance);
    }

    public void Win()
    {
        UIControl.Instance.DisplayWin();
        Destroy(Instance);
    }

    public void RemoveForest(int hexViewID)
    {
        HexPos hexPos = ServerObjects[hexViewID].GetComponent<HexPos>();

        HexPos newHexPos = Instantiate(HexPosPrefab, hexPos.transform.position, Quaternion.identity).GetComponent<HexPos>();
        newHexPos.ViewID = hexPos.ViewID;
        ServerObjects[hexPos.ViewID] = newHexPos;

        if(hexPos.Building)
        {
            newHexPos.Building = hexPos.Building;
            hexPos.Building.CurrentPos = newHexPos;
        }

        if(hexPos.MeleeUnit)
        {
            newHexPos.MeleeUnit = hexPos.MeleeUnit;
            hexPos.MeleeUnit.CurrentPos = newHexPos;
        }

        if (hexPos.RangedUnit)
        {
            newHexPos.RangedUnit = hexPos.RangedUnit;
            hexPos.RangedUnit.CurrentPos = newHexPos;
        }

        if (hexPos.Builder)
        {
            newHexPos.Builder = hexPos.Builder;
            hexPos.Builder.CurrentPos = newHexPos;
        }

        Destroy(hexPos.gameObject);
    }

    public void RemovePlayer(int clientID)
    {
        for (int i = 0; i < ServerObjects.Length; i++)
        {
            if (ServerObjects[i])
            {
                UnitSelectable unitSelectable = ServerObjects[i].GetComponent<UnitSelectable>();
                if (unitSelectable)
                {
                    if (unitSelectable.OwnerID == clientID)
                    {
                        if (unitSelectable.SelectableType == SelectableTypes.Builder)
                        {
                            unitSelectable.CurrentPos.Builder = null;
                        }
                        else if (unitSelectable.SelectableType == SelectableTypes.Melee)
                        {
                            unitSelectable.CurrentPos.MeleeUnit = null;
                        }
                        else if (unitSelectable.SelectableType == SelectableTypes.Ranged)
                        {
                            unitSelectable.CurrentPos.RangedUnit = null;
                        }
                        else
                        {
                            unitSelectable.CurrentPos.Building = null;
                        }

                        Destroy(ServerObjects[i].GetComponent<UnitSelectable>().gameObject);
                    }
                }
            }
        }
    }

    public void NextTurn()
    {
        for (int i = 0; i < ServerObjects.Length; i++)
        {
            if (ServerObjects[i])
            {
                if (!ServerObjects[i].GetComponent<HexPos>() && !ServerObjects[i].GetComponent<Building>())
                {
                    ServerObjects[i].GetComponent<UnitSelectable>().NextTurn();
                }
            }
        }
    }

    public HexPos ReturnHexPos()
    {
        HexPos hexPos = null;

        if(Physics.Raycast(MainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            hexPos = hit.transform.GetComponent<HexPos>();
        }

        return hexPos;
    }

    public void UnitAttack(int unitViewID, int hexPosID)
    {
        if(ServerObjects[unitViewID].GetComponent<MeleeCommands>())
        {
            ServerObjects[unitViewID].GetComponent<MeleeCommands>().Attack(ServerObjects[hexPosID].GetComponent<HexPos>());
        }
        else if (ServerObjects[unitViewID].GetComponent<RangedCommands>())
        {
            ServerObjects[unitViewID].GetComponent<RangedCommands>().Attack(ServerObjects[hexPosID].GetComponent<HexPos>());
        }
    }

    public void MoveUnit(int unitViewID, int hexPosID)
    {
        if (ServerObjects[unitViewID].GetComponent<UnitSelectable>().SelectableType == SelectableTypes.Builder)
        {
            ServerObjects[unitViewID].GetComponent<BuilderCommands>().Move(ServerObjects[hexPosID].GetComponent<HexPos>());
        }
        else if (ServerObjects[unitViewID].GetComponent<MeleeCommands>())
        {
            ServerObjects[unitViewID].GetComponent<MeleeCommands>().Move(ServerObjects[hexPosID].GetComponent<HexPos>());
        }
        else if (ServerObjects[unitViewID].GetComponent<RangedCommands>())
        {
            ServerObjects[unitViewID].GetComponent<RangedCommands>().Move(ServerObjects[hexPosID].GetComponent<HexPos>());
        }
    }

    public void SelectUnit(UnitSelectable unit)
    {
        DeselectUnit();

        if(unit.GetComponent<MeleeCommands>())
        {
            unit.CurrentPos.Select(0);
        }
        else if (unit.GetComponent<RangedCommands>())
        {
            unit.CurrentPos.Select(1);
        }
        else if (unit.GetComponent<BuilderCommands>())
        {
            unit.CurrentPos.Select(2);
        }
        else
        {
            unit.CurrentPos.Select(3);
        }

        unit.SelectedSprite.enabled = true;
        Selected = unit;

        if (unit.SelectableType == SelectableTypes.Builder)
        {
            UIControl.Instance.ShowBuildMenu();
        }
        else if (unit.SelectableType == SelectableTypes.HeadQuarters || unit.SelectableType == SelectableTypes.Barracks)
        {
            UIControl.Instance.ShowRecruitMenu();
        }
    }

    public void DeselectUnit()
    {
        if (Selected)
        {
            Selected.SelectedSprite.enabled = false;
            UIControl.Instance.CloseProfileMenu();
            Selected = null;
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if(SceneManager.GetSceneByBuildIndex(level).name != "Game")
        {
            Destroy(UIControl.Instance.gameObject);
            Destroy(gameObject);
        }
        else
        {
            UIControl.Instance.gameObject.SetActive(true);
        }
    }
}