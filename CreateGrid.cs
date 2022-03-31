using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Written by Abdul Galeel Ali

public class CreateGrid : MonoBehaviour
{
    public int[] BoardTypes;
    public int BoardLength;

    public int[] IDs;
    public int[] TeamNums;
    public int[] BaseIDs;
    public Color[] Colors;

    public GameObject HeadQuartersPrefab;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetSceneByBuildIndex(level).name == "Game")
        {
            int num = 0;
            int numCastles = 0;
            int[] basePositions = new int[BaseIDs.Length];

            for (int i = 0; i < BoardLength; i++)
            {
                float zPos;

                if (i % 2 == 0)
                {
                    zPos = -0.01f;
                }
                else
                {
                    zPos = 2.59f;
                }

                for (int j = 0; j < BoardLength; j++)
                {
                    GameObject gridPosPrefab = null;

                    if (BoardTypes[num] == 0)
                    {
                        gridPosPrefab = PlayerInteraction.Instance.HexPosPrefab;
                    }
                    else if (BoardTypes[num] == 1)
                    {
                        gridPosPrefab = PlayerInteraction.Instance.HillPosPrefab;
                    }
                    else if (BoardTypes[num] == 2)
                    {
                        gridPosPrefab = PlayerInteraction.Instance.ForestPosPrefab;
                    }

                    if (numCastles < BaseIDs.Length)
                    {
                        if (i == 4 || i == BoardLength - 5)
                        {
                            if (j == 4 || j == BoardLength - 5)
                            {
                                gridPosPrefab = PlayerInteraction.Instance.HexPosPrefab;
                                basePositions[numCastles] = num;
                                numCastles += 1;
                            }
                        }
                    }

                    PlayerInteraction.Instance.ServerObjects[num] = Instantiate(gridPosPrefab, new Vector3(i * 4.49f, 0, zPos + (j * 5)), Quaternion.identity).GetComponent<ServerObject>();
                    PlayerInteraction.Instance.ServerObjects[num].ViewID = num;
                    PlayerInteraction.Instance.ServerObjects[num].ViewType = PacketModule.ViewTypes.gridPos;

                    num += 1;
                }
            }

            for (int i = 0; i < basePositions.Length; i++)
            {
                HexPos baseHexPos = PlayerInteraction.Instance.ServerObjects[basePositions[i]].GetComponent<HexPos>();
                BasicHeadQuarterCommands headQuarters = Instantiate(HeadQuartersPrefab, baseHexPos.transform.position, Quaternion.identity).GetComponent<BasicHeadQuarterCommands>();
                headQuarters.OwnerID = IDs[i];

                foreach (Transform tf in headQuarters.Model.transform.Find("Colours"))
                {
                    tf.GetComponent<Renderer>().material.color = Colors[i];
                }

                if (IDs[i] == Client.Instance.MyID)
                {
                    PlayerInteraction.Instance.TeamNum = TeamNums[i];
                    PlayerInteraction.Instance.MyBase = headQuarters;
                    PlayerInteraction.Instance.transform.position = new Vector3(headQuarters.transform.position.x, 30, headQuarters.transform.position.z);
                }

                headQuarters.TeamNum = TeamNums[i];
                headQuarters.GetComponent<ServerObject>().ViewID = BaseIDs[i];
                headQuarters.GetComponent<ServerObject>().ViewType = PacketModule.ViewTypes.building;
                headQuarters.CurrentPos = baseHexPos;

                baseHexPos.Building = headQuarters;
                baseHexPos.Select(3);
                headQuarters.Setup();

                PlayerInteraction.Instance.ServerObjects[BaseIDs[i]] = headQuarters.GetComponent<ServerObject>();
            }

            Destroy(gameObject);
        }
        else if (SceneManager.GetSceneByBuildIndex(level).name != "Lobby")
        {
            Destroy(gameObject);
        }
    }
}