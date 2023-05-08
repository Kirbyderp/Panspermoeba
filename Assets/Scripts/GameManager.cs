using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool onArcade = true;
    
    private TMPro.TextMeshProUGUI gText, pText, bText;
    private GameObject[] rLossIndicators = new GameObject[9];
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
    private int thermoLevel = 10, radiLevel = 10, endLevel = 0, count = 0;

    private PlayerController playerController;
    private BoardManager boardManager;
    private ScoreManager scoreManager;
    private ResourceManager resourceManager;
    private EventManager eventManager;

    private SpriteRenderer playerMicrobe;
    private GameObject scavAmtIndicatorToggle;
    private GameObject[,] scavAmtIndicators;

    private GameObject[] actionButtonsDeselected = new GameObject[5],
                         actionButtonsSelected = new GameObject[5];
    private GameObject toTitleD, toTitleS;
    
    private GameObject infoScreen, infoPage1, infoPage2;
    private GameObject[] infoButtonsDeselected = new GameObject[2],
                         infoButtonsSelected = new GameObject[2];
    private readonly float[] trackerYPos = { 2.955f, 2.245f, 1.525f, .83f, .115f, -.59f, -1.3f, -2.02f };
    private GameObject turnTracker, actionTracker;
    private GameObject[] eventXs = new GameObject[8];
    private int activePage = 1;

    private bool waitingForEndTurnAnim1 = false, waitingForEndTurnAnim2 = false;
    private int cantSpendToLower = 0;
    private GameObject eventFade, eventFrame, eventWindow, continueSelect;
    private TMPro.TextMeshProUGUI eventHeader, eventText, eventContinue, event7Header, useless7Text;
    private int eventID = -1;
    private bool waitingForHavingRead = false, waitingForEvent7Input = false;
    private bool event3Mod = false, event4Mod = false, event5Mod = false, 
                 event6Mod = false, event7Mod = false, event8Mod = false;
    private GameObject hEventIndOff, rEventIndOff, wEventIndOff, hEventIndOn, rEventIndOn, wEventIndOn;
    private GameObject event7Select, event7UI;
    private float[] event7SelectYPos = { 2f, .7f, -.6f };
    private TMPro.TextMeshProUGUI[] event7EditTexts = new TMPro.TextMeshProUGUI[3];
    private int[] rInHand = new int[4], rToDiscard = new int[4];
    private int event7Selected = 0;
    private int endTurnToLower = 0, numLowered = 0;

    private SpriteRenderer heatVignette, coldVignette, radVignette, standardVignette, gameOverBackground;
    private int deathType = -1; //0 == standard, 1 == cold, 2 == heat, 3 == rad, 4 == victory
    private string[] deathText = {"You ran out of energy to keep yourself alive.",
                                  "You froze to death.",
                                  "Your proteins denatured from overheating.",
                                  "Your DNA has been irreparably damaged.",
                                  "Congratulations, you've survived your journey!"};
    private TMPro.TextMeshProUGUI gameOver, deathReason, scoreText, playAgain,
                                  titleScreen, newHighScore, enterInitials, highScoresHeader;
    private GameObject playAgainSelect, titleScreenSelect;
    private SpriteRenderer gameOverFrame, gameOverWindow;
    private TMPro.TextMeshProUGUI[] initialsText = new TMPro.TextMeshProUGUI[3],
                                    highScores = new TMPro.TextMeshProUGUI[10];
    private char[] initials = { 'A', 'A', 'A' };
    private SpriteRenderer[] initialFrames = new SpriteRenderer[4],
                             initialWindows = new SpriteRenderer[4],
                             initialTris = new SpriteRenderer[7];
    private bool waitingForInitialsInput = false, isNewHighScore = false;
    int selectedInitial = 0, scoreIndex = -1;
    private bool waitingForGameEndInput = false, selectedPlayAgain = true;

    private GameObject stuffHider;

    private int turnNumber = 1;
    private int controlScheme = 2; //0 == mouse only, 1 == keyboard only, 2 == both

    // Start is called before the first frame update
    void Start()
    {
        controlScheme = PlayerPrefs.GetInt("Control Scheme");

        thermobar = GameObject.Find("Thermobar");
        radibar = GameObject.Find("Radibar");

        eventManager = GameObject.Find("Event Manager").GetComponent<EventManager>();
        resourceManager = GameObject.Find("Resource Manager").GetComponent<ResourceManager>();
        gText = GameObject.Find("Glucose Text").GetComponent<TMPro.TextMeshProUGUI>();
        pText = GameObject.Find("Phosphate Text").GetComponent<TMPro.TextMeshProUGUI>();
        bText = GameObject.Find("Base Pair Text").GetComponent<TMPro.TextMeshProUGUI>();
        resourceManager.SetUpDeck();

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
        scoreManager = GameObject.Find("Score Manager").GetComponent<ScoreManager>();

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
        toTitleD = GameObject.Find("Back Button D");

        actionButtonsSelected[0] = GameObject.Find("Raise Temp Button S");
        actionButtonsSelected[1] = GameObject.Find("Raise Gene Button S");
        actionButtonsSelected[2] = GameObject.Find("Scavenge Button S");
        actionButtonsSelected[3] = GameObject.Find("Info Page Button S");
        actionButtonsSelected[4] = GameObject.Find("End Turn Button S");
        toTitleS = GameObject.Find("Back Button S");

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

        hEventIndOff = GameObject.Find("Heat Up Event Indicator Off");
        rEventIndOff = GameObject.Find("Rad Event Indicator Off");
        wEventIndOff = GameObject.Find("Water Event Indicator Off");
        hEventIndOn = GameObject.Find("Heat Up Event Indicator On");
        rEventIndOn = GameObject.Find("Rad Event Indicator On");
        wEventIndOn = GameObject.Find("Water Event Indicator On");

        event7Select = GameObject.Find("Event 7 Select");
        event7UI = GameObject.Find("Event 7 UI");
        event7Header = GameObject.Find("Event 7 Header").GetComponent<TMPro.TextMeshProUGUI>();
        event7EditTexts[0] = GameObject.Find("Glucose 7 Text").GetComponent<TMPro.TextMeshProUGUI>();
        event7EditTexts[1] = GameObject.Find("Phosphate 7 Text").GetComponent<TMPro.TextMeshProUGUI>();
        event7EditTexts[2] = GameObject.Find("Base Pair 7 Text").GetComponent<TMPro.TextMeshProUGUI>();
        useless7Text = GameObject.Find("Useless 7 Text").GetComponent<TMPro.TextMeshProUGUI>();
        event7Header.color = new Color(1, 1, 1, 0);
        event7EditTexts[0].color = new Color(1, 1, 1, 0);
        event7EditTexts[1].color = new Color(1, 1, 1, 0);
        event7EditTexts[2].color = new Color(1, 1, 1, 0);
        useless7Text.color = new Color(1, 1, 1, 0);

        coldVignette = GameObject.Find("Cold Death Vignette").GetComponent<SpriteRenderer>();
        coldVignette.color = new Color(1, 1, 1, 0);
        heatVignette = GameObject.Find("Heat Death Vignette").GetComponent<SpriteRenderer>();
        heatVignette.color = new Color(1, 1, 1, 0);
        radVignette = GameObject.Find("Rad Death Vignette").GetComponent<SpriteRenderer>();
        radVignette.color = new Color(1, 1, 1, 0);
        standardVignette = GameObject.Find("Standard Death Vignette").GetComponent<SpriteRenderer>();
        standardVignette.color = new Color(1, 1, 1, 0);

        gameOverBackground = GameObject.Find("Game Over Background").GetComponent<SpriteRenderer>();
        gameOverBackground.color = new Color(0, 0, 0, 0);
        playAgainSelect = GameObject.Find("Play Again Select");
        titleScreenSelect = GameObject.Find("Title Screen Select");

        gameOver = GameObject.Find("Game Over").GetComponent<TMPro.TextMeshProUGUI>();
        gameOver.color = new Color(1, 1, 1, 0);
        deathReason = GameObject.Find("Death Reason").GetComponent<TMPro.TextMeshProUGUI>();
        deathReason.color = new Color(1, 1, 1, 0);
        scoreText = GameObject.Find("Score Text").GetComponent<TMPro.TextMeshProUGUI>();
        scoreText.color = new Color(1, 1, 1, 0);
        playAgain = GameObject.Find("Play Again").GetComponent<TMPro.TextMeshProUGUI>();
        playAgain.color = new Color(1, 1, 1, 0);
        titleScreen = GameObject.Find("Title Screen").GetComponent<TMPro.TextMeshProUGUI>();
        titleScreen.color = new Color(1, 1, 1, 0);
        newHighScore = GameObject.Find("New High Score").GetComponent<TMPro.TextMeshProUGUI>();
        newHighScore.color = new Color(1, 1, 1, 0);
        enterInitials = GameObject.Find("Enter Initials").GetComponent<TMPro.TextMeshProUGUI>();
        enterInitials.color = new Color(1, 1, 1, 0);
        for (int i = 0; i < initialsText.Length; i++)
        {
            initialsText[i] = GameObject.Find("Initial " + i).GetComponent<TMPro.TextMeshProUGUI>();
            initialsText[i].color = new Color(1, 1, 1, 0);
        }
        for (int i = 0; i < initialFrames.Length; i++)
        {
            initialFrames[i] = GameObject.Find("Initial " + i + " Frame").GetComponent<SpriteRenderer>();
            initialFrames[i].color = new Color(1, 1, 1, 0);
        }
        for (int i = 0; i < initialWindows.Length; i++)
        {
            initialWindows[i] = GameObject.Find("Initial " + i + " Window").GetComponent<SpriteRenderer>();
            initialWindows[i].color = new Color(0, 0, 0, 0);
        }
        for (int i = 0; i < initialTris.Length; i++)
        {
            initialTris[i] = GameObject.Find("Initial Tri " + i).GetComponent<SpriteRenderer>();
            initialTris[i].color = new Color(1, 1, 1, 0);
        }
        highScoresHeader = GameObject.Find("High Scores Header").GetComponent<TMPro.TextMeshProUGUI>();
        highScoresHeader.color = new Color(1, 1, 1, 0);
        for (int i = 0; i < highScores.Length; i++)
        {
            highScores[i] = GameObject.Find("High Score " + i).GetComponent<TMPro.TextMeshProUGUI>();
            highScores[i].color = new Color(1, 1, 1, 0);
        }

        gameOverFrame = GameObject.Find("Game Over Frame").GetComponent<SpriteRenderer>();
        gameOverFrame.color = new Color(1, 1, 1, 0);
        gameOverWindow = GameObject.Find("Game Over Window").GetComponent<SpriteRenderer>();
        gameOverWindow.color = new Color(0, 0, 0, 0);

        stuffHider = GameObject.Find("Stuff Hider");

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
        toTitleS.SetActive(false);

        foreach (GameObject eventX in eventXs)
        {
            eventX.SetActive(false);
        }

        infoButtonsSelected[0].SetActive(false);
        infoButtonsSelected[1].SetActive(false);
        infoPage2.SetActive(false);
        infoScreen.SetActive(false);

        continueSelect.SetActive(false);

        hEventIndOn.SetActive(false);
        rEventIndOn.SetActive(false);
        wEventIndOn.SetActive(false);

        event7Select.SetActive(false);
        event7UI.SetActive(false);

        playAgainSelect.SetActive(false);
        titleScreenSelect.SetActive(false);

        stuffHider.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (onArcade && Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (waitingForHavingRead)
        {
            bool hasRead = false;
            if (controlScheme != 0 && Input.GetKeyDown(playerController.GetUseSelect()))
            {
                hasRead = true;
                continueSelect.SetActive(false);
                eventContinue.color = new Color(1, 1, 1, 0);
                InvokeRepeating("FadeOutEvent", 0, 1 / 60f);
            }
            
            if (controlScheme != 1 && !hasRead)
            {
                Vector3 mousePos = Input.mousePosition;
                if (Input.GetKeyDown(KeyCode.Mouse0) && mousePos.x > 756
                    && mousePos.x < 1161 && mousePos.y > 150 && mousePos.y < 284)
                {
                    hasRead = true;
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

        if (waitingForEvent7Input)
        {
            bool hasRead = false;
            if (controlScheme != 0)
            {
                if (Input.GetKeyDown(playerController.GetSelectL()))
                {
                    if (event7Selected < 3)
                    {
                        if (rToDiscard[event7Selected + 1] > 0)
                        {
                            rToDiscard[event7Selected + 1]--;
                            event7EditTexts[event7Selected].text = "" + rToDiscard[event7Selected + 1];
                        }
                    }
                }
                else if (Input.GetKeyDown(playerController.GetSelectR()))
                {
                    if (event7Selected < 3)
                    {
                        if (rToDiscard[event7Selected + 1] < rInHand[event7Selected + 1])
                        {
                            rToDiscard[event7Selected + 1]++;
                            event7EditTexts[event7Selected].text = "" + rToDiscard[event7Selected + 1];
                        }
                    }
                }
                else if (Input.GetKeyDown(playerController.GetUseSelect()))
                {
                    if (event7Selected < 3)
                    {
                        event7Selected++;
                        if (event7Selected < 3)
                        {
                            event7Select.transform.position = new Vector3(event7Select.transform.position.x,
                                                                          event7SelectYPos[event7Selected],
                                                                          event7Select.transform.position.z);
                        }
                        else
                        {
                            event7Select.SetActive(false);
                            continueSelect.GetComponent<SpriteRenderer>().color = new Color(57f/255, 209f/255,
                                                                                            1f/255, 1);
                        }
                    }
                    else
                    {
                        hasRead = true;
                    }
                }
                else if (Input.GetKeyDown(playerController.GetBack()))
                {
                    if (event7Selected == 3)
                    {
                        event7Selected--;
                        event7Select.SetActive(true);
                        event7Select.transform.position = new Vector3(event7Select.transform.position.x,
                                                                          event7SelectYPos[event7Selected],
                                                                          event7Select.transform.position.z);
                        continueSelect.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                    }
                    else if (event7Selected > 0)
                    {
                        event7Selected--;
                        event7Select.transform.position = new Vector3(event7Select.transform.position.x,
                                                                          event7SelectYPos[event7Selected],
                                                                          event7Select.transform.position.z);
                    }
                }
            }
            
            if (controlScheme != 1 && Input.GetKeyDown(KeyCode.Mouse0) && !hasRead)
            {
                Vector3 mousePos = Input.mousePosition;

                if (mousePos.x > 894 && mousePos.x < 969 && mousePos.y > 721 && mousePos.y < 794)
                {
                    if (rToDiscard[1] > 0)
                    {
                        rToDiscard[1]--;
                        event7EditTexts[0].text = "" + rToDiscard[1];
                    }
                }

                if (mousePos.x > 1112 && mousePos.x < 1185 && mousePos.y > 721 && mousePos.y < 794 )
                {
                    if (rToDiscard[1] < rInHand[1])
                    {
                        rToDiscard[1]++;
                        event7EditTexts[0].text = "" + rToDiscard[1];
                    }
                }

                if (mousePos.x > 894 && mousePos.x < 969 && mousePos.y > 580 && mousePos.y < 655)
                {
                    if (rToDiscard[2] > 0)
                    {
                        rToDiscard[2]--;
                        event7EditTexts[1].text = "" + rToDiscard[2];
                    }
                }

                if (mousePos.x > 1112 && mousePos.x < 1185 && mousePos.y > 580 && mousePos.y < 655)
                {
                    if (rToDiscard[2] < rInHand[2])
                    {
                        rToDiscard[2]++;
                        event7EditTexts[1].text = "" + rToDiscard[2];
                    }
                }

                if (mousePos.x > 894 && mousePos.x < 969 && mousePos.y > 440 && mousePos.y < 512)
                {
                    if (rToDiscard[3] > 0)
                    {
                        rToDiscard[3]--;
                        event7EditTexts[2].text = "" + rToDiscard[3];
                    }
                }

                if (mousePos.x > 1112 && mousePos.x < 1185 && mousePos.y > 440 && mousePos.y < 512)
                {
                    if (rToDiscard[3] < rInHand[3])
                    {
                        rToDiscard[3]++;
                        event7EditTexts[2].text = "" + rToDiscard[3];
                    }
                }

                if (mousePos.x > 756 && mousePos.x < 1161 && mousePos.y > 150 && mousePos.y < 284)
                {
                    hasRead = true;
                }
            }

            if (hasRead)
            {
                waitingForEvent7Input = false;

                int count = 0;
                for (int i = 0; i < rToDiscard.Length; i++)
                {
                    for (int j = 0; j < rToDiscard[i]; j++)
                    {
                        resourceManager.RemoveResource(i);
                        count++;
                    }
                }

                resourceManager.Draw(count);

                if (controlScheme != 0)
                {
                    continueSelect.GetComponent<SpriteRenderer>().color = new Color(57f / 255, 209f / 255,
                                                                                            1f / 255, 1);
                }
                continueSelect.SetActive(false);
                eventContinue.color = new Color(1, 1, 1, 0);
                event7EditTexts[0].color = new Color(1, 1, 1, 0);
                event7EditTexts[1].color = new Color(1, 1, 1, 0);
                event7EditTexts[2].color = new Color(1, 1, 1, 0);
                useless7Text.color = new Color(1, 1, 1, 0);
                event7UI.SetActive(false);
                event7Select.SetActive(false);

                InvokeRepeating("FadeOutEvent7", 0, 1 / 60f);

                hasRead = false;
            }
        }

        if (waitingForInitialsInput)
        {
            bool hasRead = false;
            if (controlScheme != 0)
            {
                if (Input.GetKeyDown(playerController.GetSelectL()))
                {
                    if (selectedInitial < 3)
                    {
                        initials[selectedInitial]--;
                        if (initials[selectedInitial] < 65)
                        {
                            initials[selectedInitial] = 'Z';
                        }
                        initialsText[selectedInitial].text = initials[selectedInitial] + "";
                    }
                }

                if (Input.GetKeyDown(playerController.GetSelectR()))
                {
                    if (selectedInitial < 3)
                    {
                        initials[selectedInitial]++;
                        if (initials[selectedInitial] > 90)
                        {
                            initials[selectedInitial] = 'A';
                        }
                        initialsText[selectedInitial].text = initials[selectedInitial] + "";
                    }
                }

                if (Input.GetKeyDown(playerController.GetUseSelect()))
                {
                    if (selectedInitial < 3)
                    {
                        initialFrames[selectedInitial].color = new Color(1, 1, 1, 1);
                        selectedInitial++;
                        initialFrames[selectedInitial].color = new Color(57f / 255, 209f / 255, 1f / 255, 1);
                    }
                    else if (selectedInitial == 3)
                    {
                        hasRead = true;
                    }
                }

                if (Input.GetKeyDown(playerController.GetBack()))
                {
                    if (selectedInitial > 0)
                    {
                        initialFrames[selectedInitial].color = new Color(1, 1, 1, 1);
                        selectedInitial--;
                        initialFrames[selectedInitial].color = new Color(57f / 255, 209f / 255, 1f / 255, 1);
                    }
                }
            }

            if (controlScheme != 1 && Input.GetKeyDown(KeyCode.Mouse0) && !hasRead)
            {
                Vector3 mousePos = Input.mousePosition;

                if (mousePos.x > 1123 && mousePos.x < 1231 && mousePos.y > 518 && mousePos.y < 582)
                {
                    initials[0]--;
                    if (initials[0] < 65)
                    {
                        initials[0] = 'Z';
                    }
                    initialsText[0].text = initials[0] + "";
                }

                if (mousePos.x > 1123 && mousePos.x < 1231 && mousePos.y > 281 && mousePos.y < 344)
                {
                    initials[0]++;
                    if (initials[0] > 90)
                    {
                        initials[0] = 'A';
                    }
                    initialsText[0].text = initials[0] + "";
                }

                if (mousePos.x > 1285 && mousePos.x < 1392 && mousePos.y > 518 && mousePos.y < 582)
                {
                    initials[1]--;
                    if (initials[1] < 65)
                    {
                        initials[1] = 'Z';
                    }
                    initialsText[1].text = initials[1] + "";
                }

                if (mousePos.x > 1285 && mousePos.x < 1392 && mousePos.y > 281 && mousePos.y < 344)
                {
                    initials[1]++;
                    if (initials[1] > 90)
                    {
                        initials[1] = 'A';
                    }
                    initialsText[1].text = initials[1] + "";
                }

                if (mousePos.x > 1446 && mousePos.x < 1554 && mousePos.y > 518 && mousePos.y < 582)
                {
                    initials[2]--;
                    if (initials[2] < 65)
                    {
                        initials[2] = 'Z';
                    }
                    initialsText[2].text = initials[2] + "";
                }

                if (mousePos.x > 1446 && mousePos.x < 1554 && mousePos.y > 281 && mousePos.y < 344)
                {
                    initials[2]++;
                    if (initials[2] > 90)
                    {
                        initials[2] = 'A';
                    }
                    initialsText[2].text = initials[2] + "";
                }

                if (mousePos.x > 1610 && mousePos.x < 1713 && mousePos.y > 379 && mousePos.y < 484)
                {
                    hasRead = true;
                }
            }

            if (hasRead)
            {
                hasRead = false;
                waitingForInitialsInput = false;
            }
        }

        if (waitingForGameEndInput)
        {
            bool hasRead = true;
            if (controlScheme != 0)
            {
                if (Input.GetKeyDown(playerController.GetSelectL()))
                {
                    if (!selectedPlayAgain)
                    {
                        selectedPlayAgain = true;
                        playAgainSelect.GetComponent<SpriteRenderer>().color = new Color(57f / 255,
                                                                                         209f / 255,
                                                                                         1f / 255);
                        titleScreenSelect.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                    }
                }

                if (Input.GetKeyDown(playerController.GetSelectR()))
                {
                    if (selectedPlayAgain)
                    {
                        selectedPlayAgain = false;
                        titleScreenSelect.GetComponent<SpriteRenderer>().color = new Color(57f / 255,
                                                                                           209f / 255,
                                                                                           1f / 255);
                        playAgainSelect.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                    }
                }

                if (Input.GetKeyDown(playerController.GetUseSelect()))
                {
                    if (selectedPlayAgain)
                    {
                        PlayAgain();
                    }
                    else
                    {
                        ToTitleScreen();
                    }
                    hasRead = true;
                }

                if (Input.GetKeyDown(playerController.GetBack()))
                {
                    ToTitleScreen();
                    hasRead = true;
                }
            }

            if (controlScheme != 1 && Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector3 mousePos = Input.mousePosition;

                if (mousePos.x > 216 && mousePos.x < 625 && mousePos.y > 266 && mousePos.y < 401)
                {
                    PlayAgain();
                    hasRead = true;
                }

                if (mousePos.x > 216 && mousePos.x < 625 && mousePos.y > 80 && mousePos.y < 215)
                {
                    ToTitleScreen();
                    hasRead = true;
                }
            }

            if (hasRead)
            {
                hasRead = false;
                waitingForInitialsInput = false;
            }
        }
    }

    public void SetWaitingForEndTurnAnim1(bool waitIn)
    {
        waitingForEndTurnAnim1 = waitIn;
    }

    public bool GetEvent7Mod()
    {
        return event7Mod;
    }

    public void Event3()
    {
        event3Mod = true;
        waitingForEndTurnAnim1 = false;
    }

    public void Event4()
    {
        event4Mod = true;
        waitingForEndTurnAnim1 = false;
    }

    public void Event5()
    {
        event5Mod = true;
    }

    public void Event6()
    {
        event6Mod = true;
        waitingForEndTurnAnim1 = false;
    }

    public void Event7()
    {
        event7Mod = true;
        waitingForEndTurnAnim1 = false;
    }

    public void UpdateHandDisplay()
    {
        List<Resource> playerHand = resourceManager.GetPlayerHand();
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
    
    public void RaiseThermobar(int numSteps, int[] indices, bool fromPlayerController)
    {
        endLevel = thermoLevel + numSteps;
        if (fromPlayerController && (endLevel == 4 || endLevel == 7 || endLevel == 10))
        {
            playerController.NextActionIsFree();
        }
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
        if (endLevel == 0)
        {
            coldVignette.color = new Color(1, 1, 1, count / 60f);
        }
        else if (endLevel == 12)
        {
            heatVignette.color = new Color(1, 1, 1, count / 60f);
        }
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
            if (endLevel == 0)
            {
                coldVignette.color = new Color(1, 1, 1, .5f);
                deathType = 1;
                waitingForEndTurnAnim1 = true;
                waitingForEndTurnAnim2 = true;
                playerController.SetWaitingForAnim(true);
                InvokeRepeating("FinishDeathAnim", 0, 1 / 60f);
            }
            else if (endLevel == 12)
            {
                heatVignette.color = new Color(1, 1, 1, .5f);
                deathType = 2;
                waitingForEndTurnAnim1 = true;
                waitingForEndTurnAnim2 = true;
                playerController.SetWaitingForAnim(true);
                InvokeRepeating("FinishDeathAnim", 0, 1 / 60f);
            }
            else if (cantSpendToLower > 0)
            {
                cantSpendToLower--;
                if (cantSpendToLower == 0)
                {
                    CancelInvoke();
                    InvokeRepeating("FadeInEvent", 0, 1 / 60f);
                }
                else
                {
                    RaiseThermobar(-1, new int[] { }, false);
                }
            }
            else if (!waitingForEndTurnAnim2)
            {
                playerController.SetWaitingForAnim(false);
            }
            else
            {
                numLowered++;
                if (event8Mod && numLowered < endTurnToLower)
                {
                    RaiseThermobar(1, new int[] { }, false);
                }
                else if (numLowered < endTurnToLower)
                {
                    RaiseThermobar(-1, new int[] { }, false);
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
        return (radiLevel + numSteps) >= 0 && (radiLevel + numSteps) < 11;
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
        if (endLevel == 0)
        {
            radVignette.color = new Color(1, 1, 1, count / 60f);
        }
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
            if (radiLevel == 0)
            {
                radVignette.color = new Color(1, 1, 1, .5f);
                deathType = 3;
                waitingForEndTurnAnim1 = true;
                waitingForEndTurnAnim2 = true;
                playerController.SetWaitingForAnim(true);
                InvokeRepeating("FinishDeathAnim", 0, 1 / 60f);
            }
            else if (!waitingForEndTurnAnim1 && !waitingForEndTurnAnim2)
            {
                playerController.SetWaitingForAnim(false);
            }
            else if (event5Mod)
            {
                event5Mod = false;
                RaiseRadibar(-1, new int[] { });
            }
            else if (waitingForEndTurnAnim1)
            {
                waitingForEndTurnAnim1 = false;
                EndTurnBoard();
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

    public void SelectToTitle()
    {
        toTitleS.SetActive(true);
        toTitleD.SetActive(false);
    }

    public void DeselectToTitle()
    {
        toTitleD.SetActive(true);
        toTitleS.SetActive(false);
    }

    public void ShowInfoScreen()
    {
        infoScreen.SetActive(true);
        if (activePage == 2)
        {
            for (int i = 0; i < eventXs.Length; i++)
            {
                if (eventManager.HasEventOccurred(i))
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
            for (int i = 0; i < eventXs.Length; i++)
            {
                if (eventManager.HasEventOccurred(i))
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
        if (event7Mod && playerController.GetCurSpace() == 3 && !waitingForEndTurnAnim1)
        {
            InvokeRepeating("FadeInEvent7", 0, 1 / 60f);
        }
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

    public void FadeInEvent7()
    {
        count++;
        eventFade.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, count / 100f);
        eventFrame.GetComponent<SpriteRenderer>().color = new Color(238f / 255, 238f / 255,
                                                                    238f / 255, count / 60f);
        eventWindow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, count / 60f);
        event7Header.color = new Color(1, 1, 1, count / 60f);
        if (count == 60)
        {
            count = 0;
            eventFade.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, .6f);
            eventFrame.GetComponent<SpriteRenderer>().color = new Color(238f / 255, 238f / 255,
                                                                        238f / 255, 1);
            eventWindow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
            event7Header.color = new Color(1, 1, 1, 1);
            CancelInvoke();
            StartCoroutine(LoadEvent7UI());
        }
    }

    IEnumerator LoadEvent7UI()
    {
        yield return new WaitForSeconds(.5f);

        continueSelect.SetActive(true);
        continueSelect.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        eventContinue.color = new Color(1, 1, 1, 1);
        rInHand = resourceManager.CountRInHand();
        rToDiscard = new int[] { rInHand[0], 0, 0, 0 };
        event7EditTexts[0].text = "0";
        event7EditTexts[0].color = new Color(1, 1, 1, 1);
        event7EditTexts[1].text = "0";
        event7EditTexts[1].color = new Color(1, 1, 1, 1);
        event7EditTexts[2].text = "0";
        event7EditTexts[2].color = new Color(1, 1, 1, 1);
        useless7Text.text = rInHand[0] + " Useless Minerals";
        useless7Text.color = new Color(1, 1, 1, 1);
        event7UI.SetActive(true);
        if (controlScheme != 0)
        {
            event7Selected = 0;
            event7Select.SetActive(true);
            event7Select.transform.position = new Vector3(event7Select.transform.position.x,
                                                          event7SelectYPos[0],
                                                          event7Select.transform.position.z);
        }
        waitingForEvent7Input = true;
    }

    public void FadeOutEvent7()
    {
        count++;
        eventFade.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, .6f - count / 100f);
        eventFrame.GetComponent<SpriteRenderer>().color = new Color(238f / 255, 238f / 255,
                                                                    238f / 255, 1 - count / 60f);
        eventWindow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1 - count / 60f);
        event7Header.color = new Color(1, 1, 1, 1 - count / 60f);
        if (count == 60)
        {
            count = 0;
            eventFade.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            eventFrame.GetComponent<SpriteRenderer>().color = new Color(238f / 255, 238f / 255,
                                                                        238f / 255, 0);
            eventWindow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            event7Header.color = new Color(1, 1, 1, 0);
            CancelInvoke();
            UpdateHandDisplay();
            playerController.SetWaitingForAnim(false);
        }
    }

    public void EndTurnInit()
    {
        waitingForEndTurnAnim1 = true;
        if (thermoLevel <= 3)
        {
            waitingForEndTurnAnim1 = false;
        }
        else if (thermoLevel > 6 && resourceManager.HasGlucose(2))
        {
            resourceManager.RemoveGlucose();
            resourceManager.RemoveGlucose();
            UpdateHandDisplay();
            StartCoroutine(LoseResourceDisplay(new int[] { 0, 1 }));
        }
        else if (thermoLevel > 6 && resourceManager.HasGlucose(1))
        {
            resourceManager.RemoveGlucose();
            UpdateHandDisplay();
            StartCoroutine(LoseResourceDisplay(new int[] { 0 }));
            cantSpendToLower = 2;
        }
        else if (thermoLevel > 6)
        {
            cantSpendToLower = 4;
            waitingForEndTurnAnim1 = false;
        }
        else if (thermoLevel > 3 && resourceManager.HasGlucose(1))
        {
            resourceManager.RemoveGlucose();
            UpdateHandDisplay();
            StartCoroutine(LoseResourceDisplay(new int[] { 0 }));
        }
        else if (thermoLevel > 3)
        {
            cantSpendToLower = 2;
            waitingForEndTurnAnim1 = false;
        }

        InvokeRepeating("WaitForEndTurnInit", 0, 1 / 60f);
    }

    private void WaitForEndTurnInit()
    {
        if (!waitingForEndTurnAnim1 && cantSpendToLower == 0)
        {
            CancelInvoke();
            InvokeRepeating("FadeInEvent", 0, 1 / 60f);
        }
        else if (!waitingForEndTurnAnim1)
        {
            CancelInvoke();
            RaiseThermobar(-1, new int[] { }, false);
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
        if (turnNumber != 7)
        {
            eventID = eventManager.PickEvent();
        }
        else
        {
            eventID = 8;
        }
        if (eventID == 3)
        {
            hEventIndOff.SetActive(false);
            hEventIndOn.SetActive(true);
        }
        else if (eventID == 4)
        {
            rEventIndOff.SetActive(false);
            rEventIndOn.SetActive(true);
        }
        else if (eventID == 7)
        {
            wEventIndOff.SetActive(false);
            wEventIndOn.SetActive(true);
        }

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
            eventText.color = new Color(0, 0, 0, 0);
            CancelInvoke();
            EndTurnEvent();
        }
    }

    public void EndTurnEvent()
    {
        waitingForEndTurnAnim1 = true;
        if (eventID != 8)
        {
            eventManager.TriggerEvent(eventID);
        }
        else
        {
            event8Mod = true;
            waitingForEndTurnAnim1 = false;
        }
        InvokeRepeating("WaitForEndTurnEvent", 0, 1 / 60f);
    }

    private void WaitForEndTurnEvent()
    {
        if (!waitingForEndTurnAnim1)
        {
            CancelInvoke();
            EndTurnBoard();
        }
    }

    public void EndTurnBoard()
    {
        boardManager.RotateBoard();
    }

    public void EndTurnThermo()
    {
        BoardSpace curSpace = BoardManager.BOARDS[boardManager.GetBoardState(), playerController.GetCurSpace()];
        numLowered = 0;
        if (CanRaiseTemp(curSpace.GetTempReduce() + (event3Mod ? 1 : 0)
                                                  - (event6Mod ? 1 : 0) 
                                                  + (event8Mod ? 7 : 0)))
        {
            endTurnToLower = -(curSpace.GetTempReduce() + (event3Mod ? 1 : 0)
                                                        - (event6Mod ? 1 : 0)
                                                        + (event8Mod ? 7 : 0));
        }
        else if (event8Mod)
        {
            endTurnToLower = thermoLevel - 12;
        }
        else
        {
            endTurnToLower = thermoLevel;
        }
        
        if (endTurnToLower < 0)
        {
            waitingForEndTurnAnim2 = true;
            endTurnToLower *= -1;
            RaiseThermobar(1, new int[] { }, false);
        }
        else if (numLowered < endTurnToLower)
        {
            waitingForEndTurnAnim2 = true;
            RaiseThermobar(-1, new int[] { }, false);
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
        if (event6Mod)
        {
            if (curSpace.GetID() == 3)
            {
                if (CanRaiseGene(event4Mod ? -2 : 0))
                {
                    endTurnToLower = (curSpace.GetID() == 3 && event4Mod) ? 2 : 0;
                }
                else if (CanRaiseGene(-1))
                {
                    endTurnToLower = 1;
                    //Game Over
                }
                else
                {
                    //Game Over
                }
            }
            else
            {
                endTurnToLower = 0;
            }
        }
        else
        {
            if (CanRaiseGene(curSpace.GetGeneReduce() - ((curSpace.GetID() == 3 && event4Mod) ? 2 : 0)))
            {
                endTurnToLower = -(curSpace.GetGeneReduce() - ((curSpace.GetID() == 3 && event4Mod) ? 2 : 0));
            }
            else
            {
                endTurnToLower = radiLevel;
            } 
        }
        
        if (numLowered < endTurnToLower)
        {
            waitingForEndTurnAnim2 = true;
            RaiseRadibar(-1, new int[] { });
        }
        else if (endTurnToLower == 0)
        {
            EndTurnFinal();
            //Game Over
        }
    }

    public void EndTurnFinal()
    {
        turnNumber++;
        if (turnNumber == 8)
        {
            deathType = 4;
            InvokeRepeating("FadeInGameOver", .3f, 1 / 60f);
            return;
        }
        
        if (event6Mod)
        {
            event6Mod = false;
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
        else if (thermoLevel < 10)
        {
            playerController.SetActionNum(2);
        }
        else
        {
            playerController.SetActionNum(1);
        }
        playerController.SetWaitingForAnim(false);
    }

    private void FinishDeathAnim()
    {
        switch (deathType)
        {
            case 0:
                standardVignette.color = new Color(1, 1, 1, .5f + count / 60f);
                break;
            case 1:
                coldVignette.color = new Color(1, 1, 1, .5f + count / 60f);
                break;
            case 2:
                heatVignette.color = new Color(1, 1, 1, .5f + count / 60f);
                break;
            case 3:
                radVignette.color = new Color(1, 1, 1, .5f + count / 60f);
                break;
        }
        count++;
        if (count == 30)
        {
            count = 0;
            switch (deathType)
            {
                case 0:
                    standardVignette.color = new Color(1, 1, 1, 1);
                    break;
                case 1:
                    coldVignette.color = new Color(1, 1, 1, 1);
                    break;
                case 2:
                    heatVignette.color = new Color(1, 1, 1, 1);
                    break;
                case 3:
                    radVignette.color = new Color(1, 1, 1, 1);
                    break;
            }
            CancelInvoke();
            InvokeRepeating("FadeInGameOver", .3f, 1 / 60f);
        }
    }

    private void FadeInGameOver()
    {
        gameOverBackground.color = new Color(0, 0, 0, count / 60f);
        count++;
        if (count == 60)
        {
            count = 0;
            gameOverBackground.color = new Color(0, 0, 0, 1);
            CancelInvoke();
            StartCoroutine(LoadGameOverText());
        }
    }

    IEnumerator LoadGameOverText()
    {
        if (deathType == 4)
        {
            gameOver.text = "You win!";
        }
        gameOver.color = new Color(1, 1, 1, 1);

        yield return new WaitForSeconds(1);

        deathReason.text = deathText[deathType];
        deathReason.color = new Color(1, 1, 1, 1);

        yield return new WaitForSeconds(1);

        scoreManager.GameEndScore(deathType == 4, thermoLevel, radiLevel,
                                  resourceManager.GetPlayerHand().Count, turnNumber - 1);
        scoreText.text = "Score: " + scoreManager.GetScore();
        scoreText.color = new Color(1, 1, 1, 1);

        yield return new WaitForSeconds(1);

        for (int i = 0; i < 10; i++)
        {
            if (scoreManager.GetScore() > int.Parse(PlayerPrefs.GetString("High Score " + i).Substring(6)))
            {
                scoreIndex = i;
                break;
            }
        }

        if (scoreIndex != -1)
        {
            isNewHighScore = true;
            gameOverFrame.color = new Color(1, 1, 1, 1);
            gameOverWindow.color = new Color(0, 0, 0, 1);
            newHighScore.color = new Color(1, 1, 1, 1);
            enterInitials.color = new Color(1, 1, 1, 1);
            for (int i = 0; i < 3; i++)
            {
                initialsText[i].color = new Color(1, 1, 1, 1);
                initialWindows[i].color = new Color(0, 0, 0, 1);
                initialFrames[i].color = new Color(1, 1, 1, 1);
                initialTris[2 * i].color = new Color(1, 1, 1, 1);
                initialTris[2 * i + 1].color = new Color(1, 1, 1, 1);
            }
            if (controlScheme != 0)
            {
                initialFrames[0].color = new Color(57f / 255, 209f / 255, 1f / 255, 1);
            }
            initialWindows[3].color = new Color(0, 0, 0, 1);
            initialFrames[3].color = new Color(1, 1, 1, 1);
            initialTris[6].color = new Color(1, 1, 1, 1);
            waitingForInitialsInput = true;
        }

        InvokeRepeating("WaitForInitialsInput", 0, 1/60f);
    }

    private void WaitForInitialsInput()
    {
        if (!waitingForInitialsInput)
        {
            CancelInvoke();
            if (isNewHighScore)
            {
                gameOverFrame.color = new Color(1, 1, 1, 0);
                gameOverWindow.color = new Color(0, 0, 0, 0);
                newHighScore.color = new Color(1, 1, 1, 0);
                enterInitials.color = new Color(1, 1, 1, 0);
                for (int i = 0; i < 3; i++)
                {
                    initialsText[i].color = new Color(1, 1, 1, 0);
                    initialWindows[i].color = new Color(0, 0, 0, 0);
                    initialFrames[i].color = new Color(1, 1, 1, 0);
                    initialTris[2 * i].color = new Color(1, 1, 1, 0);
                    initialTris[2 * i + 1].color = new Color(1, 1, 1, 0);
                }
                initialWindows[3].color = new Color(0, 0, 0, 0);
                initialFrames[3].color = new Color(1, 1, 1, 0);
                initialTris[6].color = new Color(1, 1, 1, 0);

                string tempScore1 = "", tempScore2;
                for (int i = 0; i < initials.Length; i++)
                {
                    tempScore1 += initials[i] + "";
                }
                tempScore1 += " - " + scoreManager.GetScore();

                for (int i = 0; i < 10; i++)
                {
                    if (i >= scoreIndex)
                    {
                        tempScore2 = PlayerPrefs.GetString("High Score " + i);
                        PlayerPrefs.SetString("High Score " + i, tempScore1);
                        PlayerPrefs.Save();
                        tempScore1 = tempScore2;
                    }
                }

                StartCoroutine(NewHighScoreAnim());
            }
            else
            {
                FinishLoadingGameOver();
            }
        }
    }

    IEnumerator NewHighScoreAnim()
    {
        highScoresHeader.color = new Color(1, 1, 1, 1);
        for (int i = 0; i < highScores.Length; i++)
        {
            yield return new WaitForSeconds(.5f);
            if (!PlayerPrefs.GetString("High Score " + i).Equals("--- - 0"))
            {
                highScores[i].text = PlayerPrefs.GetString("High Score " + i);
            }
            else
            {
                highScores[i].text = "";
            }
            
            if (i == scoreIndex)
            {
                InvokeRepeating("PlayerScoreAnim", 0, .5f);
            }
            else
            {
                highScores[i].color = new Color(1, 1, 1, 1);
            }
        }
    }

    private void PlayerScoreAnim()
    {
        if (count % 2 == 0)
        {
            highScores[scoreIndex].color = new Color(57f / 255, 209f / 255, 1f / 255, 1);
        }
        else
        {
            highScores[scoreIndex].color = new Color(1, 1, 1, 1);
        }
        count++;
        if (count == 10)
        {
            CancelInvoke();
            playAgainSelect.SetActive(true);
            if (controlScheme == 0)
            {
                playAgainSelect.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
            titleScreenSelect.SetActive(true);
            playAgain.color = new Color(1, 1, 1, 1);
            titleScreen.color = new Color(1, 1, 1, 1);

            waitingForGameEndInput = true;
        }
    }

    private void  FinishLoadingGameOver()
    {
        highScoresHeader.color = new Color(1, 1, 1, 1);
        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.GetString("High Score " + i).Equals("--- - 0"))
            {
                highScores[i].text = "";
            }
            else
            {
                highScores[i].text = PlayerPrefs.GetString("High Score " + i);
                highScores[i].color = new Color(1, 1, 1, 1);
            }
        }

        playAgainSelect.SetActive(true);
        if (controlScheme == 0)
        {
            playAgainSelect.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
        titleScreenSelect.SetActive(true);
        playAgain.color = new Color(1, 1, 1, 1);
        titleScreen.color = new Color(1, 1, 1, 1);

        waitingForGameEndInput = true;
    }

    public void ToTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen", LoadSceneMode.Single);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("InGame", LoadSceneMode.Single);
    }
}
