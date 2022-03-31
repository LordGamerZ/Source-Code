using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

//Written by Abdul Galeel Ali

public class UIControl : MonoBehaviour
{
    public static UIControl Instance;

    public EventSystem EV;
    public GraphicRaycaster GR;
    public Transform ProfileContainer;

    public GameObject MainScreen;
    public GameObject LoseScreen;
    public GameObject WinScreen;
    public GameObject OptionsMenu;

    public GameObject PrefabProfileIcon;

    public Profile SetPathProfile;
    public Profile RemoveForestProfile;
    public ProfileToolTip ToolTip;

    public TextMeshProUGUI WoodText;
    public TextMeshProUGUI OreText;
    public TextMeshProUGUI GoldText;
    public TextMeshProUGUI TimerDisplay;
    public TextMeshProUGUI TurnDisplay;

    public Sprite SetPathIcon;
    public Sprite RemoveForestIcon;
    public Sprite ArrowSprite;
    public Sprite HourGlassSprite;
    public Image ChangeTurnImage;

    public float Wood;
    public float Stone;
    public float Gold;

    public float TurnTime;
    public float TurnTimer;

    public int NumTurns;

    public bool IsBuilding;
    public bool IsRecruiting;
    public bool OptionsOpen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(gameObject);

        IsBuilding = false;
        IsRecruiting = false;
        OptionsOpen = false;

        OptionsMenu.SetActive(false);

        gameObject.SetActive(false);

        ToolTip.gameObject.SetActive(false); 
    }

    private void Start()
    {
        ClientSend.UpdateResources(0, 0, 0);
    }

    private void Update()
    {
        TurnTimer += Time.deltaTime;
        TimerDisplay.text = "Time left " + (TurnTime - Mathf.RoundToInt(TurnTimer)).ToString();

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptions();
        }

        PointerEventData PointerEventData = new(EV)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> hits = new();
        GR.Raycast(PointerEventData, hits);

        bool isProfile = false;

        for (int i = 0; i < hits.Count; i++)
        {
            ProfileIcon profileIcon = hits[i].gameObject.GetComponent<ProfileIcon>();
            if (profileIcon)
            {
                isProfile = true;
                string additional = "";
                if(profileIcon.P as BuildingProfile)
                {
                    additional = "Built on " + (profileIcon.P as BuildingProfile).BiomeType.ToString();
                }

                ToolTip.SetTexts(profileIcon.P.name, profileIcon.P.Description, profileIcon.P.ProductionTime, profileIcon.P.GoldCost, profileIcon.P.StoneCost, profileIcon.P.WoodCost, additional);
                break;
            }
        }

        if(!isProfile)
        {
            ToolTip.gameObject.SetActive(false);
        }
        else
        {
            ToolTip.gameObject.SetActive(true);
            ToolTip.transform.position = Input.mousePosition;
        }
    }

    public void DisplayLose()
    {
        CloseProfileMenu();
        MainScreen.SetActive(false);

        if (OptionsOpen)
        {
            ToggleOptions();
        }

        LoseScreen.SetActive(true);
    }

    public void DisplayWin()
    {
        CloseProfileMenu();
        MainScreen.SetActive(false);

        if (OptionsOpen)
        {
            ToggleOptions();
        }

        WinScreen.SetActive(true);
    }

    public void Leave()
    {
        Client.Instance.LobbyNum = 0;
        Client.Instance.LoadScene("CreateJoinLobby");
    }

    public void ToggleOptions()
    {
        OptionsOpen = !OptionsOpen;
        OptionsMenu.SetActive(OptionsOpen);
    }

    public void Quit()
    {
        ClientSend.LeaveLobby();
        Client.Instance.LobbyNum = 0;
        Client.Instance.LoadScene("CreateJoinLobby");
    }

    public void UpdateResources(float wood, float stone, float gold)
    {
        WoodText.text = "Wood " + wood.ToString();
        OreText.text = "Stone " + stone.ToString();
        GoldText.text = "Gold " + gold.ToString();

        Wood = wood;
        Stone = stone;
        Gold = gold;
    }

    public void ShowRecruitMenu()
    {
        if (IsRecruiting)
        {
            CloseProfileMenu();
        }

        BasicFactoryCommands factory = PlayerInteraction.Instance.Selected.GetComponent<BasicFactoryCommands>();

        if (factory)
        {
            IsRecruiting = true;

            for (int i = 0; i < factory.Profiles.Count; i++)
            {
                ProfileIcon profileIcon = Instantiate(PrefabProfileIcon, ProfileContainer).GetComponent<ProfileIcon>();

                profileIcon.GetComponent<Image>().sprite = factory.Profiles[i].Icon;
                profileIcon.P = factory.Profiles[i];
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(ProfileContainer.GetComponent<RectTransform>());
        }
    }

    public void ShowBuildMenu()
    {
        if (IsBuilding)
        {
            CloseProfileMenu();
        }

        BuilderCommands builder = PlayerInteraction.Instance.Selected.GetComponent<BuilderCommands>();

        if (builder)
        {
            IsBuilding = true;

            for (int i = 0; i < builder.Profiles.Count; i++)
            {
                ProfileIcon profileIcon = Instantiate(PrefabProfileIcon, ProfileContainer).GetComponent<ProfileIcon>();

                profileIcon.GetComponent<Image>().sprite = builder.Profiles[i].Icon;
                profileIcon.P = builder.Profiles[i];
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(ProfileContainer.GetComponent<RectTransform>());
    }

    public void CloseProfileMenu()
    {
        if (IsRecruiting || IsBuilding)
        {
            IsRecruiting = false;
            IsBuilding = false;

            for (int i = ProfileContainer.childCount - 1; i > -1; i--)
            {
                Destroy(ProfileContainer.GetChild(i).gameObject);
            }
        }
    }

    public void NextTurn()
    {
        if (PlayerInteraction.Instance.IsTurn)
        {
            ClientSend.NextTurn();
        }
    }

    public void ChangeArrowSprite(bool isTurn)
    {
        TurnTimer = 0; 

        if (isTurn)
        {
            ChangeTurnImage.sprite = ArrowSprite;
        }
        else
        {
            ChangeTurnImage.sprite = HourGlassSprite;
        }
    }

    public void UpdateTurns()
    {
        NumTurns += 1;
        TurnDisplay.text = "Turn " + NumTurns.ToString();
        PlayerInteraction.Instance.NextTurn();
    }

    public bool CanAfford(float wood, float stone, float gold)
    {
        if(Wood + wood >= 0)
        {
            if(Stone + stone >= 0)
            {
                if(Gold + gold >= 0)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
