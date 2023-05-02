using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private TMPro.TextMeshProUGUI gText, pText, bText;
    private GameObject thermobar, radibar;
    private readonly float[] thermoY = { -3.475f, -3.195f, -2.915f, -2.635f, -2.355f, -2.075f, -1.795f,
                                         -1.515f, -1.235f, -0.955f, -0.675f, -0.395f, -0.115f, 0 }, //.28 diff
                             thermoS = { .225f, .785f, 1.345f, 1.905f, 2.465f, 3.025f, 3.585f,
                                         4.145f, 4.705f, 5.265f, 5.825f, 6.385f, 6.945f, 7.175f }, //.56 diff
                             radiY = { -3.395f, -3.075f, -2.755f, -2.435f, -2.115f, -1.795f,
                                       -1.475f, -1.155f, -0.835f, -0.515f, -0.195f}, //.32 diff
                             radiS = { .38f, 1.02f, 1.66f, 2.3f, 2.94f, 3.58f,
                                       4.22f, 4.86f, 5.5f, 6.14f, 6.78f}; //.64 diff
    private int thermoLevel = 0, radiLevel = 0, endLevel = 0, count = 0;
    private PlayerController playerController;
    private BoardManager boardManager;

    private SpriteRenderer playerMicrobe;
    private GameObject scavAmtIndicatorToggle;
    private GameObject[,] scavAmtIndicators;

    private GameObject[] actionButtonsDeselected = new GameObject[5],
                         actionButtonsSelected = new GameObject[5];
    
    private GameObject infoScreen, infoPage1, infoPage2;
    private GameObject[] infoButtonsDeselected = new GameObject[2],
                         infoButtonsSelected = new GameObject[2];
    private readonly float[] trackerYPos = { 2.955f, 2.245f, 1.525f, .83f, .115f, -.59f, -1.3f, -2.02f };
    private GameObject turnTracker, actionTracker;
    private int activePage = 1;

    bool waitingForEndTurnAnim = false;
    int endTurnToLower = 0, numLowered = 0;

    int turnNumber = 1;

    // Start is called before the first frame update
    void Start()
    {
        thermobar = GameObject.Find("Thermobar");
        radibar = GameObject.Find("Radibar");

        gText = GameObject.Find("Glucose Text").GetComponent<TMPro.TextMeshProUGUI>();
        pText = GameObject.Find("Phosphate Text").GetComponent<TMPro.TextMeshProUGUI>();
        bText = GameObject.Find("Base Pair Text").GetComponent<TMPro.TextMeshProUGUI>();
        ResourceManager.SetUpDeck();

        playerController = GameObject.Find("Player Microbe").GetComponent<PlayerController>();
        boardManager = GameObject.Find("Game Board").GetComponent<BoardManager>();

        playerMicrobe = GameObject.Find("Player Microbe").GetComponent<SpriteRenderer>();
        scavAmtIndicatorToggle = GameObject.Find("ScavAmt Indicators");
        scavAmtIndicators = new GameObject[7, 4];
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                scavAmtIndicators[i, j] = GameObject.Find("Circle Indicator" + i + j);
            }
        }
        scavAmtIndicatorToggle.SetActive(false);

        actionButtonsDeselected[0] = GameObject.Find("Raise Temp Button D");
        actionButtonsDeselected[1] = GameObject.Find("Raise Gene Button D");
        actionButtonsDeselected[2] = GameObject.Find("Scavenge Button D");
        actionButtonsDeselected[3] = GameObject.Find("Info Page Button D");
        actionButtonsDeselected[4] = GameObject.Find("End Turn Button D");

        actionButtonsSelected[0] = GameObject.Find("Raise Temp Button S");
        actionButtonsSelected[1] = GameObject.Find("Raise Gene Button S");
        actionButtonsSelected[2] = GameObject.Find("Scavenge Button S");
        actionButtonsSelected[3] = GameObject.Find("Info Page Button S");
        actionButtonsSelected[4] = GameObject.Find("End Turn Button S");
        foreach (GameObject aButton in actionButtonsSelected)
        {
            aButton.SetActive(false);
        }

        infoScreen = GameObject.Find("Info Screen");
        infoPage1 = GameObject.Find("Info Page 1");
        infoPage2 = GameObject.Find("Info Page 2");
        infoButtonsDeselected[0] = GameObject.Find("Left Button D");
        infoButtonsDeselected[1] = GameObject.Find("Right Button D");
        infoButtonsSelected[0] = GameObject.Find("Left Button S");
        infoButtonsSelected[1] = GameObject.Find("Left Button S");
        
        infoButtonsSelected[0].SetActive(false);
        infoButtonsSelected[1].SetActive(false);
        turnTracker = GameObject.Find("Turn Tracker");
        actionTracker = GameObject.Find("Action Tracker");
        infoPage2.SetActive(false);
        infoScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHandDisplay()
    {
        List<Resource> playerHand = ResourceManager.GetPlayerHand();
        int gCount = 0, pCount = 0, bCount = 0;
        for (int i = 0; i < playerHand.Count; i++)
        {
            Resource card = playerHand[i];
            if (card.ToString().Equals(Resource.TYPES[1]))
            {
                gCount++;
            }
            else if (card.ToString().Equals(Resource.TYPES[2]))
            {
                pCount++;
            }
            else if (card.ToString().Equals(Resource.TYPES[3]))
            {
                bCount++;
            }
        }
        gText.text = "" + gCount;
        pText.text = "" + pCount;
        bText.text = "" + bCount;
    }

    public bool CanRaiseTemp(int numSteps)
    {
        return (thermoLevel + numSteps) > 0 && (thermoLevel + numSteps) < 13;
    }
    
    public void RaiseThermobar(int numSteps)
    {
        endLevel = thermoLevel + numSteps;
        InvokeRepeating("ThermobarAnim", 0, 1/60f);
    }

    private void ThermobarAnim()
    {
        Vector3 yVel = new Vector3(0, (thermoY[endLevel] - thermoY[thermoLevel]) / 30, 0);
        Vector3 sVel = new Vector3(0, (thermoS[endLevel] - thermoS[thermoLevel]) / 30, 0);
        thermobar.transform.localPosition += yVel;
        thermobar.transform.localScale += sVel;
        count++;
        if (count == 30)
        {
            count = 0;
            thermobar.transform.localPosition = new Vector3(thermobar.transform.localPosition.x,
                                                            thermoY[endLevel],
                                                            thermobar.transform.localPosition.z);
            thermobar.transform.localScale = new Vector3(thermobar.transform.localScale.x,
                                                         thermoS[endLevel],
                                                         thermobar.transform.localScale.z);
            thermoLevel = endLevel;
            CancelInvoke();
            if (!waitingForEndTurnAnim)
            {
                playerController.SetWaitingForAnim(false);
            }
            else
            {
                numLowered++;
                if (numLowered < endTurnToLower)
                {
                    RaiseThermobar(-1);
                }
                else
                {
                    EndTurnRadi();
                }
            }
        }
    }

    public bool CanRaiseGene(int numSteps)
    {
        return (radiLevel + numSteps) > 0 && (radiLevel + numSteps) < 11;
    }

    public void RaiseRadibar(int numSteps)
    {
        endLevel = radiLevel + numSteps;
        InvokeRepeating("RadibarAnim", 0, 1/60f);
    }

    private void RadibarAnim()
    {
        Vector3 yVel = new Vector3(0, (radiY[endLevel] - radiY[radiLevel]) / 30, 0);
        Vector3 sVel = new Vector3(0, (radiS[endLevel] - radiS[radiLevel]) / 30, 0);
        radibar.transform.localPosition += yVel;
        radibar.transform.localScale += sVel;
        count++;
        if (count == 30)
        {
            count = 0;
            radibar.transform.localPosition = new Vector3(radibar.transform.localPosition.x,
                                                          radiY[endLevel],
                                                          radibar.transform.localPosition.z);
            radibar.transform.localScale = new Vector3(radibar.transform.localScale.x,
                                                       radiS[endLevel],
                                                       radibar.transform.localScale.z);
            radiLevel = endLevel;
            CancelInvoke();
            if (!waitingForEndTurnAnim)
            {
                playerController.SetWaitingForAnim(false);
            }
            else
            {
                numLowered++;
                if (numLowered < endTurnToLower)
                {
                    RaiseRadibar(-1);
                }
                else
                {
                    EndTurnFinal();
                }
            }
        }
    }

    public void SelectButton(int index)
    {
        actionButtonsSelected[index].SetActive(true);
        if (index == 2)
        {
            playerMicrobe.enabled = false;
            scavAmtIndicatorToggle.SetActive(true);
        }
        actionButtonsDeselected[index].SetActive(false);
    }

    public void DeselectButton(int index)
    {
        actionButtonsDeselected[index].SetActive(true);
        if (index == 2)
        {
            scavAmtIndicatorToggle.SetActive(false);
            playerMicrobe.enabled = true;
        }
        actionButtonsSelected[index].SetActive(false);
    }

    public void SelectInfoButton(int index)
    {
        infoButtonsSelected[index].SetActive(true);
        infoButtonsDeselected[index].SetActive(false);
    }

    public void DeselectInfoButton(int index)
    {
        infoButtonsDeselected[index].SetActive(true);
        infoButtonsSelected[index].SetActive(false);
    }

    public void ShowInfoScreen()
    {
        infoScreen.SetActive(true);
        if (activePage == 2)
        {
            turnTracker.transform.position = new Vector3(turnTracker.transform.position.x,
                                                         trackerYPos[turnNumber - 1],
                                                         turnTracker.transform.position.z);
            int actionNum = playerController.GetActionNum();
            if (actionNum <= 6)
            {
                actionTracker.transform.position = new Vector3(actionTracker.transform.position.x,
                                                               trackerYPos[actionNum - 1],
                                                               actionTracker.transform.position.z);
            }
            else
            {
                actionTracker.transform.position = new Vector3(turnTracker.transform.position.x,
                                                               trackerYPos[7],
                                                               turnTracker.transform.position.z);
            }
        }
    }

    public void HideInfoScreen()
    {
        infoScreen.SetActive(false);
    }

    public void InfoButtonAction()
    {
        if (activePage == 1)
        {
            activePage = 2;
            infoPage1.SetActive(false);
            infoPage2.SetActive(true);
            turnTracker.transform.position = new Vector3(turnTracker.transform.position.x,
                                                         trackerYPos[turnNumber - 1],
                                                         turnTracker.transform.position.z);
            int actionNum = playerController.GetActionNum();
            if (actionNum <= 6)
            {
                actionTracker.transform.position = new Vector3(actionTracker.transform.position.x,
                                                               trackerYPos[actionNum - 1],
                                                               actionTracker.transform.position.z);
            }
            else
            {
                actionTracker.transform.position = new Vector3(turnTracker.transform.position.x,
                                                               trackerYPos[7],
                                                               turnTracker.transform.position.z);
            }
        }
        else
        {
            activePage = 1;
            infoPage2.SetActive(false);
            infoPage1.SetActive(true);
        }
    }

    public void EndTurnBoard()
    {
        waitingForEndTurnAnim = true;
        boardManager.RotateBoard();
    }

    public void EndTurnThermo()
    {
        BoardSpace curSpace = BoardManager.BOARDS[boardManager.GetBoardState(), playerController.GetCurSpace()];
        numLowered = 0;
        if (CanRaiseTemp(curSpace.GetTempReduce()))
        {
            endTurnToLower = -curSpace.GetTempReduce();
        }
        else
        {
            endTurnToLower = thermoLevel;
        }
        if (numLowered < endTurnToLower)
        {
            RaiseThermobar(-1);
        }
        else if (endTurnToLower == 0)
        {
            EndTurnRadi();
        }
    }

    public void EndTurnRadi()
    {
        BoardSpace curSpace = BoardManager.BOARDS[boardManager.GetBoardState(), playerController.GetCurSpace()];
        numLowered = 0;
        if (CanRaiseGene(curSpace.GetGeneReduce()))
        {
            endTurnToLower = -curSpace.GetGeneReduce();
        }
        else
        {
            endTurnToLower = radiLevel;
        }
        if (numLowered < endTurnToLower)
        {
            RaiseRadibar(-1);
        }
        else if (endTurnToLower == 0)
        {
            EndTurnFinal();
        }
    }

    public void EndTurnFinal()
    {
        if (turnNumber < 7)
        {
            turnNumber++;
        }
        waitingForEndTurnAnim = false;
        playerController.ResetActionNum();
        playerController.SetWaitingForAnim(false);
    }
}
