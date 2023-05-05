using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private TMPro.TextMeshProUGUI gText, pText, bText;
    private GameObject[] rLossIndicators = new GameObject[5];
    private GameObject[,] rGainIndicators = new GameObject[2,12];
    private float[] gGainLossXPositions = { -4.6f, -4.4f};

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
    private GameObject[] eventXs = new GameObject[8];
    private int activePage = 1;

    private bool waitingForEndTurnAnim1 = false, waitingForEndTurnAnim2 = false;
    private GameObject eventFade, eventFrame, eventWindow, continueSelect;
    private TMPro.TextMeshProUGUI eventHeader, eventText, eventContinue;
    private int eventID = -1;
    private bool waitingForHavingRead = false;
    private int endTurnToLower = 0, numLowered = 0;

    private int turnNumber = 1;
    private int controlScheme = 2; //0 == mouse only, 1 == keyboard only, 2 == both

    // Start is called before the first frame update
    void Start()
    {
        thermobar = GameObject.Find("Thermobar");
        radibar = GameObject.Find("Radibar");

        gText = GameObject.Find("Glucose Text").GetComponent<TMPro.TextMeshProUGUI>();
        pText = GameObject.Find("Phosphate Text").GetComponent<TMPro.TextMeshProUGUI>();
        bText = GameObject.Find("Base Pair Text").GetComponent<TMPro.TextMeshProUGUI>();
        ResourceManager.SetUpDeck();

        for (int i = 0; i < rLossIndicators.Length; i++)
        {
            if (i < 3)
            {
                rLossIndicators[i] = GameObject.Find("Lose Glucose " + i % 3);
            }
            else if (i < 6)
            {
                rLossIndicators[i] = GameObject.Find("Lose Phosphate " + i % 3);
            }
            else if (i < 9)
            {
                rLossIndicators[i] = GameObject.Find("Lose Base Pair " + i % 3);
            }
            rLossIndicators[i].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        }
        for (int i = 0; i < 12; i++)
        {
            if (i < 4)
            {
                rGainIndicators[0, i] = GameObject.Find("Gain Glucose " + i % 4);
                rGainIndicators[1, i] = GameObject.Find("Gain Glucose " + i % 4 + "V");
            }
            else if (i < 8)
            {
                rGainIndicators[0, i] = GameObject.Find("Gain Phosphate " + i % 4);
                rGainIndicators[1, i] = GameObject.Find("Gain Phosphate " + i % 4 + "V");
            }
            else if (i < 12)
            {
                rGainIndicators[0, i] = GameObject.Find("Gain Base Pair " + i % 4);
                rGainIndicators[1, i] = GameObject.Find("Gain Base Pair " + i % 4 + "V");
            }
            rGainIndicators[0,i].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            rGainIndicators[1,i].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        }

        playerController = GameObject.Find("Player Microbe").GetComponent<PlayerController>();
        boardManager = GameObject.Find("Game Board").GetComponent<BoardManager>();

        playerMicrobe = GameObject.Find("Player Microbe").GetComponent<SpriteRenderer>();
        scavAmtIndicatorToggle = GameObject.Find("ScavAmt Indicators");
        scavAmtIndicators = new GameObject[7, 4];
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                scavAmtIndicators[i, j] = GameObject.Find("Circle Indicator " + i + j);
            }
        }

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

        infoScreen = GameObject.Find("Info Screen");
        infoPage1 = GameObject.Find("Info Page 1");
        infoPage2 = GameObject.Find("Info Page 2");
        infoButtonsDeselected[0] = GameObject.Find("Left Button D");
        infoButtonsDeselected[1] = GameObject.Find("Right Button D");
        infoButtonsSelected[0] = GameObject.Find("Left Button S");
        infoButtonsSelected[1] = GameObject.Find("Left Button S");
        turnTracker = GameObject.Find("Turn Tracker");
        actionTracker = GameObject.Find("Action Tracker");
        for (int i = 0; i < eventXs.Length; i++)
        {
            eventXs[i] = GameObject.Find("Event " + (i + 1) + " X");
        }

        eventFade = GameObject.Find("Event Fade");
        eventFrame = GameObject.Find("Event Frame");
        eventWindow = GameObject.Find("Event Window");
        continueSelect = GameObject.Find("Continue Select");
        eventFade.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        eventFrame.GetComponent<SpriteRenderer>().color = new Color(238f / 255, 238f / 255, 238f / 255, 0);
        eventWindow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        if (controlScheme == 0)
        {
            continueSelect.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }

        eventHeader = GameObject.Find("Event Header").GetComponent<TMPro.TextMeshProUGUI>();
        eventText = GameObject.Find("Event Text").GetComponent<TMPro.TextMeshProUGUI>();
        eventContinue = GameObject.Find("Event Continue").GetComponent<TMPro.TextMeshProUGUI>();
        eventHeader.color = new Color(1, 1, 1, 0);
        eventText.color = new Color(1, 1, 1, 0);
        eventContinue.color = new Color(1, 1, 1, 0);

        StartCoroutine(HideStuff());
    }

    IEnumerator HideStuff()
    {
        yield return new WaitForSeconds(.02f);
        scavAmtIndicatorToggle.SetActive(false);

        foreach (GameObject aButton in actionButtonsSelected)
        {
            aButton.SetActive(false);
        }
        if (controlScheme != 0)
        {
            actionButtonsSelected[0].SetActive(true);
            actionButtonsDeselected[0].SetActive(false);
        }

        foreach (GameObject eventX in eventXs)
        {
            eventX.SetActive(false);
        }

        infoButtonsSelected[0].SetActive(false);
        infoButtonsSelected[1].SetActive(false);
        infoPage2.SetActive(false);
        infoScreen.SetActive(false);

        continueSelect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingForHavingRead)
        {
            bool hasRead = false;
            if (controlScheme != 0 && Input.GetKey(playerController.GetUseSelect()))
            {
                continueSelect.SetActive(false);
                eventContinue.color = new Color(1, 1, 1, 0);
                InvokeRepeating("FadeOutEvent", 0, 1 / 60f);
            }
            else if (controlScheme != 1 && !hasRead)
            {
                Vector3 mousePos = Input.mousePosition;
                if (Input.GetKeyDown(KeyCode.Mouse0) && mousePos.x > 756
                    && mousePos.x < 1161 && mousePos.y > 150 && mousePos.y < 284)
                {
                    continueSelect.SetActive(false);
                    eventContinue.color = new Color(1, 1, 1, 0);
                    InvokeRepeating("FadeOutEvent", 0, 1 / 60f);
                }
            }
            if (hasRead)
            {
                waitingForHavingRead = false;
                hasRead = false;
            }
        }
    }

    public void SetWaitingForEndTurnAnim1(bool waitIn)
    {
        waitingForEndTurnAnim1 = waitIn;
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
    
    public void RaiseThermobar(int numSteps, int[] indices)
    {
        endLevel = thermoLevel + numSteps;
        if (indices.Length != 0)
        {
            StartCoroutine(LoseResourceDisplay(indices));
        }
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
            if (!waitingForEndTurnAnim2)
            {
                playerController.SetWaitingForAnim(false);
            }
            else
            {
                numLowered++;
                if (numLowered < endTurnToLower)
                {
                    RaiseThermobar(-1, new int[] { });
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

    public void RaiseRadibar(int numSteps, int[] indices)
    {
        endLevel = radiLevel + numSteps;
        if (indices.Length != 0)
        {
            StartCoroutine(LoseResourceDisplay(indices));
        }
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
            if (!waitingForEndTurnAnim2)
            {
                playerController.SetWaitingForAnim(false);
            }
            else
            {
                numLowered++;
                if (numLowered < endTurnToLower)
                {
                    RaiseRadibar(-1, new int[] { });
                }
                else
                {
                    EndTurnFinal();
                }
            }
        }
    }

    public void DecrementScavIndicatorAmt(int spaceID)
    {
        int scavAmt = BoardManager.BOARDS[0, spaceID].GetScavAmt();
        if (scavAmt > 1)
        {
            scavAmtIndicators[spaceID, scavAmt - 1].SetActive(false);
        }
    }

    public void SelectButton(int index)
    {
        actionButtonsSelected[index].SetActive(true);
        if (index == 2 && !playerController.GetWaitingForAnim())
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
            for (int i = 0; i < eventXs.Length; i++)
            {
                if (EventManager.HasEventOccurred(i))
                {
                    eventXs[i].SetActive(true);
                }
            }
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
                actionTracker.transform.position = new Vector3(actionTracker.transform.position.x,
                                                               trackerYPos[7],
                                                               actionTracker.transform.position.z);
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
                actionTracker.transform.position = new Vector3(actionTracker.transform.position.x,
                                                               trackerYPos[7],
                                                               actionTracker.transform.position.z);
            }
        }
        else
        {
            activePage = 1;
            infoPage2.SetActive(false);
            infoPage1.SetActive(true);
        }
    }

    public IEnumerator GainResourceDisplay(int[] indices)
    {
        for (int i = 0; i < indices.Length; i++)
        {
            if (indices[i] < 4 && gText.text.Length > 1)
            {
                rGainIndicators[0, indices[i]].transform.position = new Vector3(gGainLossXPositions[1],
                                                                      rGainIndicators[0,i].transform.position.y,
                                                                      rGainIndicators[0,i].transform.position.z);
            }
            else if (indices[i] < 4)
            {
                rGainIndicators[0, indices[i]].transform.position = new Vector3(gGainLossXPositions[0],
                                                                      rGainIndicators[0,i].transform.position.y,
                                                                      rGainIndicators[0,i].transform.position.z);
            }
            rGainIndicators[0, indices[i]].GetComponent<SpriteRenderer>().color = new Color(57f / 255,
                                                                                            209f / 255,
                                                                                            1f / 255);
            rGainIndicators[1, indices[i]].GetComponent<SpriteRenderer>().color = new Color(57f / 255,
                                                                                            209f / 255,
                                                                                            1f / 255);
        }
        yield return new WaitForSeconds(.125f);
        for (int i = 0; i < 26; i++)
        {
            for (int j = 0; j < indices.Length; j++)
            {
                rGainIndicators[0, indices[j]].GetComponent<SpriteRenderer>().color = new Color(57f / 255,
                                                                                                209f / 255,
                                                                                                1f / 255,
                                                                                                1 - i / 26f);
                rGainIndicators[1, indices[j]].GetComponent<SpriteRenderer>().color = new Color(57f / 255,
                                                                                                209f / 255,
                                                                                                1f / 255,
                                                                                                1 - i / 26f);
            }
            yield return new WaitForSeconds(1 / 60f);
        }
        for (int j = 0; j < indices.Length; j++)
        {
            rGainIndicators[0, indices[j]].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            rGainIndicators[1, indices[j]].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        }
        waitingForEndTurnAnim1 = false;
    }

    public IEnumerator LoseResourceDisplay(int[] indices)
    {
        for (int i = 0; i < indices.Length; i++)
        {
            if (indices[i] < 3 && gText.text.Length > 1)
            {
                rLossIndicators[indices[i]].transform.position = new Vector3(gGainLossXPositions[1],
                                                                        rLossIndicators[i].transform.position.y,
                                                                        rLossIndicators[i].transform.position.z);
            }
            else if (indices[i] < 3)
            {
                rLossIndicators[indices[i]].transform.position = new Vector3(gGainLossXPositions[0],
                                                                        rLossIndicators[i].transform.position.y,
                                                                        rLossIndicators[i].transform.position.z);
            }
            rLossIndicators[indices[i]].GetComponent<SpriteRenderer>().color = new Color(233f / 255, 0, 0);
        }
        yield return new WaitForSeconds(.125f);
        for(int i = 0; i < 26; i++)
        {
            for (int j = 0; j < indices.Length; j++)
            {
                rLossIndicators[indices[j]].GetComponent<SpriteRenderer>().color = new Color(233f / 255, 0,
                                                                                             0, 1 - i / 26f);
            }
            yield return new WaitForSeconds(1/60f);
        }
        for (int j = 0; j < indices.Length; j++)
        {
            rLossIndicators[indices[j]].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        }
        waitingForEndTurnAnim1 = false;
    }

    public void EndTurnInit()
    {
        waitingForEndTurnAnim1 = true;
        if (thermoLevel <= 3)
        {
            waitingForEndTurnAnim1 = false;
        }
        else if (thermoLevel > 6 && ResourceManager.HasGlucose(2))
        {
            ResourceManager.RemoveGlucose();
            ResourceManager.RemoveGlucose();
            UpdateHandDisplay();
            StartCoroutine(LoseResourceDisplay(new int[] { 0, 1 }));
        }
        else if (thermoLevel > 3 && ResourceManager.HasGlucose(1))
        {
            ResourceManager.RemoveGlucose();
            UpdateHandDisplay();
            StartCoroutine(LoseResourceDisplay(new int[] { 0 }));
        }
        else if ((thermoLevel > 3 && !ResourceManager.HasGlucose(1)) ||
                 (thermoLevel > 6 && !ResourceManager.HasGlucose(2)))
        {
            //Game Over
        }

        InvokeRepeating("WaitForEndTurnInit", 0, 1 / 60f);
    }

    private void WaitForEndTurnInit()
    {
        if (!waitingForEndTurnAnim1)
        {
            CancelInvoke();
            InvokeRepeating("FadeInEvent", 0, 1 / 60f);
        }
    }

    public void FadeInEvent()
    {
        count++;
        eventFade.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, count / 100f);
        eventFrame.GetComponent<SpriteRenderer>().color = new Color(238f / 255, 238f / 255,
                                                                    238f / 255, count / 60f);
        eventWindow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, count / 60f);
        eventHeader.color = new Color(1, 1, 1, count / 60f);
        if (count == 60)
        {
            count = 0;
            eventFade.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, .6f);
            eventFrame.GetComponent<SpriteRenderer>().color = new Color(238f / 255, 238f / 255,
                                                                        238f / 255, 1);
            eventWindow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
            eventHeader.color = new Color(1, 1, 1, 1);
            CancelInvoke();
            StartCoroutine(LoadEventText());
        }
    }

    IEnumerator LoadEventText()
    {
        eventID = EventManager.PickEvent();

        yield return new WaitForSeconds(.5f);

        eventText.color = new Color(1, 1, 1, 1);
        eventText.text = EventManager.EVENT_TEXT[eventID];

        yield return new WaitForSeconds(1);

        continueSelect.SetActive(true);
        eventContinue.color = new Color(1, 1, 1, 1);
        waitingForHavingRead = true;
    }

    public void FadeOutEvent()
    {
        count++;
        eventFade.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, .6f - count / 100f);
        eventFrame.GetComponent<SpriteRenderer>().color = new Color(238f / 255, 238f / 255,
                                                                    238f / 255, 1 - count / 60f);
        eventWindow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1 - count / 60f);
        eventHeader.color = new Color(1, 1, 1, 1 - count / 60f);
        eventText.color = new Color(1, 1, 1, 1 - count / 60f);
        if (count == 60)
        {
            count = 0;
            eventFade.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            eventFrame.GetComponent<SpriteRenderer>().color = new Color(238f / 255, 238f / 255,
                                                                        238f / 255, 0);
            eventWindow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            eventHeader.color = new Color(1, 1, 1, 0);
            eventText.color = new Color(1, 1, 1, 0);
            CancelInvoke();
            EndTurnEvent();
        }
    }

    public void EndTurnEvent()
    {
        EventManager.TriggerEvent(eventID);
        EndTurnBoard();
    }

    public void EndTurnBoard()
    {
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
            RaiseThermobar(-1, new int[] { });
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
            RaiseRadibar(-1, new int[] { });
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
        waitingForEndTurnAnim2 = false;
        
        if (thermoLevel < 4)
        {
            playerController.SetActionNum(4);
        }
        else if (thermoLevel < 7)
        {
            playerController.SetActionNum(3);
        }
        else if (thermoLevel < 7)
        {
            playerController.SetActionNum(2);
        }
        else
        {
            playerController.SetActionNum(1);
        }
        playerController.SetWaitingForAnim(false);
    }
}
