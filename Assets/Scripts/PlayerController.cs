using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int curSpace = 3, nextSpace;
    private float curXPos, curYPos;
    private float nextXPos, nextYPos;
    private int count = 0;
    private BoardManager board;
    private bool waitingForAnim = false;
    private bool inInfoPage = false;
    private GameManager gameManager;
    private bool[] selectedButtons = {false, false, false, false, false, false, false};
    private int actionNum = 1;
    private int controlScheme = 2; //0 == mouse only, 1 == keyboard only, 2 == both
    
    private GameObject kButtonSelector;
    private int kSelectedButton = 0;
    private Vector3[] kSelectorPositions = { new Vector3(2.66f, -1.586f, 2),
                                             new Vector3(4.86f, -1.586f, 2),
                                             new Vector3(0.46f, -3.786f, 2),
                                             new Vector3(2.66f, -3.786f, 2),
                                             new Vector3(4.86f, -3.786f, 2),
                                             new Vector3(2.66f, -3.786f, -4.5f),
                                             new Vector3(4.86f, -3.786f, -4.5f),
                                             new Vector3(7.06f, -3.786f, -4.5f) };

    //Controls
    KeyCode back = KeyCode.Escape;

    KeyCode lClick = KeyCode.Mouse0;

    KeyCode moveU = KeyCode.W;
    KeyCode moveL = KeyCode.A;
    KeyCode moveD = KeyCode.S;
    KeyCode moveR = KeyCode.D;

    KeyCode selectL = KeyCode.H;
    KeyCode selectR = KeyCode.J;
    KeyCode useSelect = KeyCode.K;

    // Start is called before the first frame update
    void Start()
    {
        board = GameObject.Find("Game Board").GetComponent<BoardManager>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        curXPos = board.transform.position.x;
        curYPos = board.transform.position.y;
        kButtonSelector = GameObject.Find("Keyboard Action Selection");
        if (controlScheme == 0)
        {
            kButtonSelector.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (controlScheme != 0)
        {
            LookForKeyboardMoveInput();
            LookForKeyboardSelectInput();
        }
        
        if (controlScheme != 1)
        {
            HandleButtonMouseSelections();
            if (Input.GetKeyDown(lClick) && !waitingForAnim)
            {
                PerformAction(GetButtonMouseSelected());
            }
        }
        

        //DEV TOOLS
        if (!waitingForAnim && !inInfoPage)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                waitingForAnim = true;
                int[] indices = ResourceManager.Scavenge(4, curSpace);
                gameManager.UpdateHandDisplay();
                StartCoroutine(gameManager.GainResourceDisplay(indices));
                if (!(gameManager.GetEvent7Mod() && curSpace == 3))
                {
                    StartCoroutine(WaitForGainLossAnim());
                }
            }
            else if (Input.GetKeyDown(KeyCode.T) && ResourceManager.CanRaiseTemp(0) && gameManager.CanRaiseTemp(1))
            {
                waitingForAnim = true;
                ResourceManager.RaiseTemp();
                gameManager.UpdateHandDisplay();
                gameManager.RaiseThermobar(1, new int[] {0});
            }
            else if (Input.GetKeyDown(KeyCode.G) && ResourceManager.CanRaiseGene(0) && gameManager.CanRaiseGene(1))
            {
                waitingForAnim = true;
                ResourceManager.RaiseGene();
                gameManager.UpdateHandDisplay();
                gameManager.RaiseRadibar(1, new int[] {0, 3, 6});
            }
        }
    }

    public int GetCurSpace()
    {
        return curSpace;
    }

    public bool GetWaitingForAnim()
    {
        return waitingForAnim;
    }

    public void SetWaitingForAnim(bool input)
    {
        waitingForAnim = input;
    }

    public bool GetInInfoPage()
    {
        return inInfoPage;
    }

    public void SetInInfoPage(bool input)
    {
        inInfoPage = input;
    }

    public void SetCurXPos(float xIn)
    {
        curXPos = xIn;
    }

    public void SetCurYPos(float yIn)
    {
        curYPos = yIn;
    }

    public int GetActionNum()
    {
        return actionNum;
    }

    public void SetActionNum(int actIn)
    {
        actionNum = actIn;
    }

    public KeyCode GetSelectL()
    {
        return selectL;
    }

    public KeyCode GetSelectR()
    {
        return selectR;
    }

    public KeyCode GetUseSelect()
    {
        return useSelect;
    }

    public KeyCode GetBack()
    {
        return back;
    }

    private bool CanMove(int dir, int[] allowedDirs)
    {
        
        for (int i = 0; i < allowedDirs.Length; i++)
        {
            if (dir == allowedDirs[i])
            {
                return true;
            }
        }
        return false;
    }

    private bool CanMove(int spaceID)
    {

        int[] adjs = BoardManager.BOARDS[board.GetBoardState(), curSpace].GetAdjs();
        for (int i = 0; i < adjs.Length; i++)
        {
            if (adjs[i] == spaceID)
            {
                return true;
            }
        }
        return false;
    }

    private int IndexOfDir(int dir, int[] allowedDirs)
    {

        for (int i = 0; i < allowedDirs.Length; i++)
        {
            if (dir == allowedDirs[i])
            {
                return i;
            }
        }
        return -1;
    }

    IEnumerator WaitForGainLossAnim()
    {
        yield return new WaitForSeconds(.55f);
        waitingForAnim = false;
    }

    private void LookForKeyboardMoveInput()
    {
        if (board.GetBoardState() % 2 == 1 && !waitingForAnim && !inInfoPage &&
            (actionNum < 5 || (actionNum == 5 && ResourceManager.HasGlucose(1))
                             || (actionNum == 6 && ResourceManager.HasGlucose(2))))
        {
            if (Input.GetKey(moveR))
            {
                if (CanMove(0, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs()))
                {
                    waitingForAnim = true;
                    int spaceID = BoardManager.BOARDS[board.GetBoardState(), curSpace].GetAdjs()
                                  [IndexOfDir(0, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs())];
                    nextSpace = spaceID;
                    nextXPos = BoardManager.BOARDS[board.GetBoardState(), spaceID].GetXPos() +
                               board.transform.position.x;
                    nextYPos = BoardManager.BOARDS[board.GetBoardState(), spaceID].GetYPos() +
                               board.transform.position.y;
                    if (actionNum == 5)
                    {
                        ResourceManager.RemoveGlucose();
                        gameManager.UpdateHandDisplay();
                    }
                    else if (actionNum == 6)
                    {
                        ResourceManager.RemoveGlucose();
                        ResourceManager.RemoveGlucose();
                        gameManager.UpdateHandDisplay();
                    }
                    actionNum++;
                    InvokeRepeating("Move", 0, 1 / 60f);
                }
            }
            else if (Input.GetKey(moveU) && CanMove(2, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs()))
            {
                if (CanMove(2, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs()))
                {
                    waitingForAnim = true;
                    int spaceID = BoardManager.BOARDS[board.GetBoardState(), curSpace].GetAdjs()
                                  [IndexOfDir(2, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs())];
                    nextSpace = spaceID;
                    nextXPos = BoardManager.BOARDS[board.GetBoardState(), spaceID].GetXPos() +
                               board.transform.position.x;
                    nextYPos = BoardManager.BOARDS[board.GetBoardState(), spaceID].GetYPos() +
                               board.transform.position.y;
                    if (actionNum == 5)
                    {
                        ResourceManager.RemoveGlucose();
                        gameManager.UpdateHandDisplay();
                    }
                    else if (actionNum == 6)
                    {
                        ResourceManager.RemoveGlucose();
                        ResourceManager.RemoveGlucose();
                        gameManager.UpdateHandDisplay();
                    }
                    actionNum++;
                    InvokeRepeating("Move", 0, 1 / 60f);
                }
            }
            else if (Input.GetKey(moveL) && CanMove(4, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs()))
            {
                if (CanMove(4, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs()))
                {
                    waitingForAnim = true;
                    int spaceID = BoardManager.BOARDS[board.GetBoardState(), curSpace].GetAdjs()
                                  [IndexOfDir(4, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs())];
                    nextSpace = spaceID;
                    nextXPos = BoardManager.BOARDS[board.GetBoardState(), spaceID].GetXPos() +
                               board.transform.position.x;
                    nextYPos = BoardManager.BOARDS[board.GetBoardState(), spaceID].GetYPos() +
                               board.transform.position.y;
                    if (actionNum == 5)
                    {
                        ResourceManager.RemoveGlucose();
                        gameManager.UpdateHandDisplay();
                    }
                    else if (actionNum == 6)
                    {
                        ResourceManager.RemoveGlucose();
                        ResourceManager.RemoveGlucose();
                        gameManager.UpdateHandDisplay();
                    }
                    actionNum++;
                    InvokeRepeating("Move", 0, 1 / 60f);
                }
            }
            else if (Input.GetKey(moveD) && CanMove(6, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs()))
            {
                if (CanMove(6, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs()))
                {
                    waitingForAnim = true;
                    int spaceID = BoardManager.BOARDS[board.GetBoardState(), curSpace].GetAdjs()
                                  [IndexOfDir(6, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs())];
                    nextSpace = spaceID;
                    nextXPos = BoardManager.BOARDS[board.GetBoardState(), spaceID].GetXPos() +
                               board.transform.position.x;
                    nextYPos = BoardManager.BOARDS[board.GetBoardState(), spaceID].GetYPos() +
                               board.transform.position.y;
                    if (actionNum == 5)
                    {
                        ResourceManager.RemoveGlucose();
                        gameManager.UpdateHandDisplay();
                    }
                    else if (actionNum == 6)
                    {
                        ResourceManager.RemoveGlucose();
                        ResourceManager.RemoveGlucose();
                        gameManager.UpdateHandDisplay();
                    }
                    actionNum++;
                    InvokeRepeating("Move", 0, 1 / 60f);
                }
            }
        }
        else if (board.GetBoardState() % 2 == 0 && !waitingForAnim && !inInfoPage &&
                 (actionNum < 5 || (actionNum == 5 && ResourceManager.HasGlucose(1))
                                  || (actionNum == 6 && ResourceManager.HasGlucose(2))))
        {
            if (Input.GetKey(moveR) && Input.GetKey(moveU))
            {
                if (CanMove(1, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs()))
                {
                    waitingForAnim = true;
                    int spaceID = BoardManager.BOARDS[board.GetBoardState(), curSpace].GetAdjs()
                                  [IndexOfDir(1, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs())];
                    nextSpace = spaceID;
                    nextXPos = BoardManager.BOARDS[board.GetBoardState(), spaceID].GetXPos() +
                               board.transform.position.x;
                    nextYPos = BoardManager.BOARDS[board.GetBoardState(), spaceID].GetYPos() +
                               board.transform.position.y;
                    if (actionNum == 5)
                    {
                        ResourceManager.RemoveGlucose();
                        gameManager.UpdateHandDisplay();
                    }
                    else if (actionNum == 6)
                    {
                        ResourceManager.RemoveGlucose();
                        ResourceManager.RemoveGlucose();
                        gameManager.UpdateHandDisplay();
                    }
                    actionNum++;
                    InvokeRepeating("Move", 0, 1 / 60f);
                }
            }
            else if (Input.GetKey(moveU) && Input.GetKey(moveL))
            {
                if (CanMove(3, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs()))
                {
                    waitingForAnim = true;
                    int spaceID = BoardManager.BOARDS[board.GetBoardState(), curSpace].GetAdjs()
                                  [IndexOfDir(3, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs())];
                    nextSpace = spaceID;
                    nextXPos = BoardManager.BOARDS[board.GetBoardState(), spaceID].GetXPos() +
                               board.transform.position.x;
                    nextYPos = BoardManager.BOARDS[board.GetBoardState(), spaceID].GetYPos() +
                               board.transform.position.y;
                    if (actionNum == 5)
                    {
                        ResourceManager.RemoveGlucose();
                        gameManager.UpdateHandDisplay();
                    }
                    else if (actionNum == 6)
                    {
                        ResourceManager.RemoveGlucose();
                        ResourceManager.RemoveGlucose();
                        gameManager.UpdateHandDisplay();
                    }
                    actionNum++;
                    InvokeRepeating("Move", 0, 1 / 60f);
                }
            }
            else if (Input.GetKey(moveL) && Input.GetKey(moveD))
            {
                if (CanMove(5, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs()))
                {
                    waitingForAnim = true;
                    int spaceID = BoardManager.BOARDS[board.GetBoardState(), curSpace].GetAdjs()
                                  [IndexOfDir(5, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs())];
                    nextSpace = spaceID;
                    nextXPos = BoardManager.BOARDS[board.GetBoardState(), spaceID].GetXPos() +
                               board.transform.position.x;
                    nextYPos = BoardManager.BOARDS[board.GetBoardState(), spaceID].GetYPos() +
                               board.transform.position.y;
                    if (actionNum == 5)
                    {
                        ResourceManager.RemoveGlucose();
                        gameManager.UpdateHandDisplay();
                    }
                    else if (actionNum == 6)
                    {
                        ResourceManager.RemoveGlucose();
                        ResourceManager.RemoveGlucose();
                        gameManager.UpdateHandDisplay();
                    }
                    actionNum++;
                    InvokeRepeating("Move", 0, 1 / 60f);
                }
            }
            else if (Input.GetKey(moveD) && Input.GetKey(moveR))
            {
                if (CanMove(7, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs()))
                {
                    waitingForAnim = true;
                    int spaceID = BoardManager.BOARDS[board.GetBoardState(), curSpace].GetAdjs()
                                  [IndexOfDir(7, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs())];
                    nextSpace = spaceID;
                    nextXPos = BoardManager.BOARDS[board.GetBoardState(), spaceID].GetXPos() +
                               board.transform.position.x;
                    nextYPos = BoardManager.BOARDS[board.GetBoardState(), spaceID].GetYPos() +
                               board.transform.position.y;
                    if (actionNum == 5)
                    {
                        ResourceManager.RemoveGlucose();
                        gameManager.UpdateHandDisplay();
                    }
                    else if (actionNum == 6)
                    {
                        ResourceManager.RemoveGlucose();
                        ResourceManager.RemoveGlucose();
                        gameManager.UpdateHandDisplay();
                    }
                    actionNum++;
                    InvokeRepeating("Move", 0, 1 / 60f);
                }
            }
        }
    }

    private void LookForKeyboardSelectInput()
    {
        if (!inInfoPage && !waitingForAnim)
        {
            if (Input.GetKeyDown(selectL))
            {
                gameManager.DeselectButton(kSelectedButton);
                kSelectedButton--;
                if (kSelectedButton < 0)
                {
                    kSelectedButton = 4;
                }
                kButtonSelector.transform.position = kSelectorPositions[kSelectedButton];
                gameManager.SelectButton(kSelectedButton);
            }
            else if (Input.GetKeyDown(selectR))
            {
                gameManager.DeselectButton(kSelectedButton);
                kSelectedButton++;
                if (kSelectedButton > 4)
                {
                    kSelectedButton = 0;
                }
                kButtonSelector.transform.position = kSelectorPositions[kSelectedButton];
                gameManager.SelectButton(kSelectedButton);
            }
            else if (Input.GetKeyDown(useSelect))
            {
                PerformAction(kSelectedButton);
                if (kSelectedButton == 3)
                {
                    kSelectedButton = 5;
                    kButtonSelector.transform.position = kSelectorPositions[kSelectedButton];
                }
            }
        }
        else if (!waitingForAnim)
        {
            if (Input.GetKeyDown(selectL))
            {
                if (kSelectedButton == 5)
                {
                    gameManager.DeselectButton(3);
                }
                else
                {
                    gameManager.DeselectInfoButton(kSelectedButton - 6);
                }
                kSelectedButton--;
                if (kSelectedButton < 5)
                {
                    kSelectedButton = 7;
                }
                kButtonSelector.transform.position = kSelectorPositions[kSelectedButton];
                if (kSelectedButton == 5)
                {
                    gameManager.SelectButton(3);
                }
                else
                {
                    gameManager.SelectInfoButton(kSelectedButton - 6);
                }
            }
            else if (Input.GetKeyDown(selectR))
            {
                if (kSelectedButton == 5)
                {
                    gameManager.DeselectButton(3);
                }
                else
                {
                    gameManager.DeselectInfoButton(kSelectedButton - 6);
                }
                kSelectedButton++;
                if (kSelectedButton > 7)
                {
                    kSelectedButton = 5;
                }
                kButtonSelector.transform.position = kSelectorPositions[kSelectedButton];
                if (kSelectedButton == 5)
                {
                    gameManager.SelectButton(3);
                }
                else
                {
                    gameManager.SelectInfoButton(kSelectedButton - 6);
                }
            }
            else if (Input.GetKeyDown(useSelect))
            {
                if (kSelectedButton == 5)
                {
                    PerformAction(3);
                }
                else
                {
                    PerformAction(kSelectedButton - 6);
                }
                if (kSelectedButton == 5)
                {
                    kSelectedButton = 3;
                    kButtonSelector.transform.position = kSelectorPositions[kSelectedButton];
                }
            }
        }
    }

    private void Move()
    {
        Vector3 vel = (new Vector3(nextXPos, nextYPos, 0) - new Vector3(curXPos, curYPos, 0)) / 60f;
        transform.Translate(vel, Space.World);
        count++;
        if (count == 60)
        {
            transform.position = new Vector3(nextXPos, nextYPos, -1);
            curXPos = nextXPos;
            curYPos = nextYPos;
            count = 0;
            curSpace = nextSpace;
            CancelInvoke();
            waitingForAnim = false;
        }
    }

    private void HandleButtonMouseSelections()
    {
        Vector3 mousePos = Input.mousePosition;
        
        if (kSelectedButton != 0 || controlScheme == 0)
        {
            if (mousePos.x > 1153 && mousePos.x < 1343 && mousePos.y > 275 && mousePos.y < 465)
            {
                gameManager.SelectButton(0);
                selectedButtons[0] = true;
            }
            else if (selectedButtons[0])
            {
                gameManager.DeselectButton(0);
                selectedButtons[0] = false;
            }
        }

        if (kSelectedButton != 1 || controlScheme == 0)
        {
            if (mousePos.x > 1391 && mousePos.x < 1579 && mousePos.y > 275 && mousePos.y < 465)
            {
                gameManager.SelectButton(1);
                selectedButtons[1] = true;
            }
            else if (selectedButtons[1])
            {
                gameManager.DeselectButton(1);
                selectedButtons[1] = false;
            }
        }

        if (kSelectedButton != 2 || controlScheme == 0)
        {
            if (mousePos.x > 915 && mousePos.x < 1105 && mousePos.y > 39 && mousePos.y < 226)
            {
                gameManager.SelectButton(2);
                selectedButtons[2] = true;
            }
            else if (selectedButtons[2])
            {
                gameManager.DeselectButton(2);
                selectedButtons[2] = false;
            }
        }

        if (kSelectedButton != 3 || controlScheme == 0)
        {
            if (mousePos.x > 1153 && mousePos.x < 1343 && mousePos.y > 39 && mousePos.y < 226)
            {
                gameManager.SelectButton(3);
                selectedButtons[3] = true;
            }
            else if (selectedButtons[3])
            {
                gameManager.DeselectButton(3);
                selectedButtons[3] = false;
            }
        }

        if (kSelectedButton != 4 || controlScheme == 0)
        {
            if (mousePos.x > 1391 && mousePos.x < 1579 && mousePos.y > 39 && mousePos.y < 226)
            {
                gameManager.SelectButton(4);
                selectedButtons[4] = true;
            }
            else if (selectedButtons[4])
            {
                gameManager.DeselectButton(4);
                selectedButtons[4] = false;
            }
        }

        if (kSelectedButton != 6 || controlScheme == 0)
        {
            if (mousePos.x > 1391 && mousePos.x < 1579 && mousePos.y > 39 && mousePos.y < 226 && inInfoPage)
            {
                gameManager.SelectInfoButton(0);
                selectedButtons[5] = true;
            }
            else if (selectedButtons[5])
            {
                gameManager.DeselectInfoButton(0);
                selectedButtons[5] = false;
            }
        }

        if (kSelectedButton != 7 || controlScheme == 0)
        {
            if (mousePos.x > 1625 && mousePos.x < 1815 && mousePos.y > 39 && mousePos.y < 226 && inInfoPage)
            {
                gameManager.SelectInfoButton(1);
                selectedButtons[6] = true;
            }
            else if (selectedButtons[6])
            {
                gameManager.DeselectInfoButton(1);
                selectedButtons[6] = false;
            }
        }
    }

    private int GetButtonMouseSelected()
    {
        Vector3 mousePos = Input.mousePosition;
        
        //Action Buttons
        if (mousePos.x > 1153 && mousePos.x < 1343 && mousePos.y > 275 && mousePos.y < 465 && !inInfoPage)
        {
            return 0;
        }

        if (mousePos.x > 1391 && mousePos.x < 1579 && mousePos.y > 275 && mousePos.y < 465 && !inInfoPage)
        {
            return 1;
        }

        if (mousePos.x > 915 && mousePos.x < 1105 && mousePos.y > 39 && mousePos.y < 226 && !inInfoPage)
        {
            return 2;
        }

        if (mousePos.x > 1153 && mousePos.x < 1343 && mousePos.y > 39 && mousePos.y < 226)
        {
            return 3;
        }

        if (mousePos.x > 1391 && mousePos.x < 1579 && mousePos.y > 39 && mousePos.y < 226 && !inInfoPage)
        {
            return 4;
        }

        //Info Screen Page Buttons
        if (mousePos.x > 1391 && mousePos.x < 1579 && mousePos.y > 39 && mousePos.y < 226 && inInfoPage)
        {
            return 0;
        }

        if (mousePos.x > 1625 && mousePos.x < 1815 && mousePos.y > 39 && mousePos.y < 226 && inInfoPage)
        {
            return 1;
        }

        //Board Spaces
        int boardState = board.GetBoardState();
        if (boardState % 4 == 0)
        {
            if ((600 - 399) / 2 - Mathf.Abs(mousePos.x - (600 + 399) / 2) >= Mathf.Abs(mousePos.y - 803 + (600 - 399) / 2)
                && !inInfoPage)
            {
                if (boardState == 0)
                {
                    return 10;
                }
                else
                {
                    return 16;
                }
        }

        if ((706 - 505) / 2 - Mathf.Abs(mousePos.x - (706 + 505) / 2) >= Mathf.Abs(mousePos.y - 909 + (706 - 505) / 2)
                && !inInfoPage)
            {
                if (boardState == 0)
                {
                    return 11;
                }
                else
                {
                    return 15;
                }
            }

            if ((706 - 505) / 2 - Mathf.Abs(mousePos.x - (706 + 505) / 2) >= Mathf.Abs(mousePos.y - 697 + (706 - 505) / 2)
                && !inInfoPage)
            {
                if (boardState == 0)
                {
                    return 12;
                }
                else
                {
                    return 14;
                }
            }

            if ((812 - 610) / 2 - Mathf.Abs(mousePos.x - (812 + 610) / 2) >= Mathf.Abs(mousePos.y - 803 + (812 - 610) / 2)
                && !inInfoPage)
            {
                return 13;
            }

            if ((917 - 716) / 2 - Mathf.Abs(mousePos.x - (917 + 716) / 2) >= Mathf.Abs(mousePos.y - 909 + (917 - 716) / 2)
                && !inInfoPage)
            {
                if (boardState == 0)
                {
                    return 14;
                }
                else
                {
                    return 12;
                }
            }

            if ((917 - 716) / 2 - Mathf.Abs(mousePos.x - (917 + 716) / 2) >= Mathf.Abs(mousePos.y - 697 + (917 - 716) / 2)
                && !inInfoPage)
            {
                if (boardState == 0)
                {
                    return 15;
                }
                else
                {
                    return 11;
                }
            }

            if ((1023 - 822) / 2 - Mathf.Abs(mousePos.x - (1023 + 822) / 2) >= Mathf.Abs(mousePos.y - 803 + (1023 - 822) / 2)
                && !inInfoPage)
            {
                if (boardState == 0)
                {
                    return 16;
                }
                else
                {
                    return 10;
                }
            }
        }
        else if (boardState % 4 == 1)
        {
            if (mousePos.x > 490 && mousePos.x < 632 && mousePos.y > 481 && mousePos.y < 625)
            {
                if (boardState == 1)
                {
                    return 10;
                }
                else
                {
                    return 16;
                }
            }

            if (mousePos.x > 490 && mousePos.x < 632 && mousePos.y > 632 && mousePos.y < 774)
            {
                if (boardState == 1)
                {
                    return 11;
                }
                else
                {
                    return 15;
                }
            }

            if (mousePos.x > 639 && mousePos.x < 782 && mousePos.y > 481 && mousePos.y < 625)
            {
                if (boardState == 1)
                {
                    return 12;
                }
                else
                {
                    return 14;
                }
            }

            if (mousePos.x > 639 && mousePos.x < 782 && mousePos.y > 632 && mousePos.y < 774)
            {
                return 13;
            }

            if (mousePos.x > 639 && mousePos.x < 782 && mousePos.y > 782 && mousePos.y < 925)
            {
                if (boardState == 1)
                {
                    return 14;
                }
                else
                {
                    return 12;
                }
            }

            if (mousePos.x > 788 && mousePos.x < 931 && mousePos.y > 632 && mousePos.y < 774)
            {
                if (boardState == 1)
                {
                    return 15;
                }
                else
                {
                    return 11;
                }
            }

            if (mousePos.x > 788 && mousePos.x < 931 && mousePos.y > 782 && mousePos.y < 925)
            {
                if (boardState == 1)
                {
                    return 16;
                }
                else
                {
                    return 10;
                }
            }
        }
        else if (boardState % 4 == 2)
        {
            if ((812 - 610) / 2 - Mathf.Abs(mousePos.x - (812 + 610) / 2) >= Mathf.Abs(mousePos.y - 591 + (812 - 610) / 2)
                && !inInfoPage)
            {
                if (boardState == 2)
                {
                    return 10;
                }
                else
                {
                    return 16;
                }
            }

            if ((706 - 505) / 2 - Mathf.Abs(mousePos.x - (706 + 505) / 2) >= Mathf.Abs(mousePos.y - 697 + (706 - 505) / 2)
                && !inInfoPage)
            {
                if (boardState == 2)
                {
                    return 11;
                }
                else
                {
                    return 15;
                }
            }

            if ((917 - 716) / 2 - Mathf.Abs(mousePos.x - (917 + 716) / 2) >= Mathf.Abs(mousePos.y - 697 + (917 - 716) / 2)
                && !inInfoPage)
            {
                if (boardState == 2)
                {
                    return 12;
                }
                else
                {
                    return 14;
                }
            }

            if ((812 - 610) / 2 - Mathf.Abs(mousePos.x - (812 + 610) / 2) >= Mathf.Abs(mousePos.y - 803 + (812 - 610) / 2)
                && !inInfoPage)
            {
                return 13;
            }

            if ((706 - 505) / 2 - Mathf.Abs(mousePos.x - (706 + 505) / 2) >= Mathf.Abs(mousePos.y - 909 + (706 - 505) / 2)
                    && !inInfoPage)
            {
                if (boardState == 2)
                {
                    return 14;
                }
                else
                {
                    return 12;
                }
            }

            if ((917 - 716) / 2 - Mathf.Abs(mousePos.x - (917 + 716) / 2) >= Mathf.Abs(mousePos.y - 909 + (917 - 716) / 2)
                && !inInfoPage)
            {
                if (boardState == 2)
                {
                    return 15;
                }
                else
                {
                    return 11;
                }
            }

            if ((812 - 610) / 2 - Mathf.Abs(mousePos.x - (812 + 610) / 2) >= Mathf.Abs(mousePos.y - 1014 + (812 - 610) / 2)
                && !inInfoPage)
            {
                if (boardState == 2)
                {
                    return 16;
                }
                else
                {
                    return 10;
                }
            }
        }
        else
        {
            if (mousePos.x > 788 && mousePos.x < 931 && mousePos.y > 481 && mousePos.y < 625)
            {
                if (boardState == 3)
                {
                    return 10;
                }
                else
                {
                    return 16;
                }
            }

            if (mousePos.x > 639 && mousePos.x < 782 && mousePos.y > 481 && mousePos.y < 625)
            {
                if (boardState == 3)
                {
                    return 11;
                }
                else
                {
                    return 15;
                }
            }

            if (mousePos.x > 788 && mousePos.x < 931 && mousePos.y > 632 && mousePos.y < 774)
            {
                if (boardState == 3)
                {
                    return 12;
                }
                else
                {
                    return 14;
                }
            }

            if (mousePos.x > 639 && mousePos.x < 782 && mousePos.y > 632 && mousePos.y < 774)
            {
                return 13;
            }

            if (mousePos.x > 490 && mousePos.x < 632 && mousePos.y > 632 && mousePos.y < 774)
            {
                if (boardState == 3)
                {
                    return 14;
                }
                else
                {
                    return 12;
                }
            }

            if (mousePos.x > 639 && mousePos.x < 782 && mousePos.y > 782 && mousePos.y < 925)
            {
                if (boardState == 3)
                {
                    return 15;
                }
                else
                {
                    return 11;
                }
            }

            if (mousePos.x > 490 && mousePos.x < 632 && mousePos.y > 782 && mousePos.y < 925)
            {
                if (boardState == 3)
                {
                    return 16;
                }
                else
                {
                    return 10;
                }
            }
        }

        return -1;
    }

    private void PerformAction(int index)
    {
        if (actionNum < 5)
        {
            if (index != -1 && !inInfoPage)
            {
                if (index == 0 && ResourceManager.CanRaiseTemp(0) && gameManager.CanRaiseTemp(1))
                {
                    waitingForAnim = true;
                    ResourceManager.RaiseTemp();
                    gameManager.UpdateHandDisplay();
                    actionNum++;
                    gameManager.RaiseThermobar(1, new int[] {0});
                }
                else if (index == 1 && ResourceManager.CanRaiseGene(0) && gameManager.CanRaiseGene(1))
                {
                    waitingForAnim = true;
                    ResourceManager.RaiseGene();
                    gameManager.UpdateHandDisplay();
                    actionNum++;
                    gameManager.RaiseRadibar(1, new int[] { 0, 3, 6 });
                }
                else if (index == 2)
                {
                    waitingForAnim = true;
                    int[] indices = ResourceManager.Scavenge(BoardManager.BOARDS[board.GetBoardState(),
                                                             curSpace].GetScavAmt(), curSpace);
                    gameManager.UpdateHandDisplay();
                    StartCoroutine(gameManager.GainResourceDisplay(indices));
                    if (!(gameManager.GetEvent7Mod() && curSpace == 3))
                    {
                        StartCoroutine(WaitForGainLossAnim());
                    }
                    actionNum++;
                }
                else if (index == 3)
                {
                    inInfoPage = true;
                    gameManager.ShowInfoScreen();
                }
                else if (index == 4)
                {
                    waitingForAnim = true;
                    gameManager.EndTurnInit();
                }
                else if (index >= 10 && index <= 16)
                { 
                    if (CanMove(index % 10))
                    {
                        waitingForAnim = true;
                        nextSpace = index % 10;
                        nextXPos = BoardManager.BOARDS[board.GetBoardState(), index % 10].GetXPos() +
                                   board.transform.position.x;
                        nextYPos = BoardManager.BOARDS[board.GetBoardState(), index % 10].GetYPos() +
                                   board.transform.position.y;
                        actionNum++;
                        InvokeRepeating("Move", 0, 1 / 60f);
                    }
                }
            }
            else if (index != -1 && inInfoPage)
            {
                if (index == 3)
                {
                    gameManager.HideInfoScreen();
                    inInfoPage = false;
                }
                else if (index == 0)
                {
                    gameManager.InfoButtonAction();
                }
                else if (index == 1)
                {
                    gameManager.InfoButtonAction();
                }
            }
        }
        else if (actionNum == 5)
        {
            if (index != -1 && !inInfoPage)
            {
                if (index == 0 && ResourceManager.CanRaiseTemp(1) && gameManager.CanRaiseTemp(1))
                {
                    waitingForAnim = true;
                    ResourceManager.RaiseTemp();
                    ResourceManager.RemoveGlucose();
                    gameManager.UpdateHandDisplay();
                    actionNum++;
                    gameManager.RaiseThermobar(1, new int[] { 0, 1 });
                }
                else if (index == 1 && ResourceManager.CanRaiseGene(1) && gameManager.CanRaiseGene(1))
                {
                    waitingForAnim = true;
                    ResourceManager.RaiseGene();
                    ResourceManager.RemoveGlucose();
                    gameManager.UpdateHandDisplay();
                    actionNum++;
                    gameManager.RaiseRadibar(1, new int[] { 0, 1, 3, 6 });
                }
                else if (index == 2 && ResourceManager.HasGlucose(1))
                {
                    waitingForAnim = true;
                    int[] indices = ResourceManager.Scavenge(BoardManager.BOARDS[board.GetBoardState(),
                                                             curSpace].GetScavAmt(), curSpace);
                    ResourceManager.RemoveGlucose();
                    gameManager.UpdateHandDisplay();
                    int[][] lgIndices = UpdateIndices(indices, 1);
                    if (lgIndices[0].Length > 0)
                    {
                        StartCoroutine(gameManager.LoseResourceDisplay(lgIndices[0]));
                    }
                    StartCoroutine(gameManager.GainResourceDisplay(lgIndices[1]));
                    if (!(gameManager.GetEvent7Mod() && curSpace == 3))
                    {
                        StartCoroutine(WaitForGainLossAnim());
                    }
                    actionNum++;
                }
                else if (index == 3)
                {
                    inInfoPage = true;
                    gameManager.ShowInfoScreen();
                }
                else if (index == 4)
                {
                    waitingForAnim = true;
                    gameManager.EndTurnInit();
                }
                else if (index >= 10 && index <= 16)
                {
                    if (CanMove(index % 10))
                    {
                        waitingForAnim = true;
                        nextSpace = index % 10;
                        nextXPos = BoardManager.BOARDS[board.GetBoardState(), index % 10].GetXPos() +
                                   board.transform.position.x;
                        nextYPos = BoardManager.BOARDS[board.GetBoardState(), index % 10].GetYPos() +
                                   board.transform.position.y;
                        ResourceManager.RemoveGlucose();
                        gameManager.UpdateHandDisplay();
                        actionNum++;
                        InvokeRepeating("Move", 0, 1 / 60f);
                    }
                }
            }
            else if (index != -1 && inInfoPage)
            {
                if (index == 3)
                {
                    gameManager.HideInfoScreen();
                    inInfoPage = false;
                }
                else if (index == 0)
                {
                    gameManager.InfoButtonAction();
                }
                else if (index == 1)
                {
                    gameManager.InfoButtonAction();
                }
            }
        }
        else if (actionNum == 6)
        {
            if (index != -1 && !inInfoPage)
            {
                if (index == 0 && ResourceManager.CanRaiseTemp(2) && gameManager.CanRaiseTemp(1))
                {
                    waitingForAnim = true;
                    ResourceManager.RaiseTemp();
                    ResourceManager.RemoveGlucose();
                    ResourceManager.RemoveGlucose();
                    gameManager.UpdateHandDisplay();
                    actionNum++;
                    gameManager.RaiseThermobar(1, new int[] { 0, 1, 2 });
                }
                else if (index == 1 && ResourceManager.CanRaiseGene(2) && gameManager.CanRaiseGene(1))
                {
                    waitingForAnim = true;
                    ResourceManager.RaiseGene();
                    ResourceManager.RemoveGlucose();
                    ResourceManager.RemoveGlucose();
                    gameManager.UpdateHandDisplay();
                    actionNum++;
                    gameManager.RaiseRadibar(1, new int[] { 0, 1, 2, 3, 6 });
                }
                else if (index == 2 && ResourceManager.HasGlucose(2))
                {
                    waitingForAnim = true;
                    int[] indices = ResourceManager.Scavenge(BoardManager.BOARDS[board.GetBoardState(),
                                                             curSpace].GetScavAmt(), curSpace);
                    ResourceManager.RemoveGlucose();
                    ResourceManager.RemoveGlucose();
                    gameManager.UpdateHandDisplay();
                    int[][] lgIndices = UpdateIndices(indices, 2);
                    if (lgIndices[0].Length > 0)
                    {
                        StartCoroutine(gameManager.LoseResourceDisplay(lgIndices[0]));
                    }
                    StartCoroutine(gameManager.GainResourceDisplay(lgIndices[1]));
                    if (!(gameManager.GetEvent7Mod() && curSpace == 3))
                    {
                        StartCoroutine(WaitForGainLossAnim());
                    }
                    actionNum++;
                }
                else if (index == 3)
                {
                    inInfoPage = true;
                    gameManager.ShowInfoScreen();
                }
                else if (index == 4)
                {
                    waitingForAnim = true;
                    gameManager.EndTurnInit();
                }
                else if (index >= 10 && index <= 16)
                {
                    if (CanMove(index % 10))
                    {
                        waitingForAnim = true;
                        nextSpace = index % 10;
                        nextXPos = BoardManager.BOARDS[board.GetBoardState(), index % 10].GetXPos() +
                                   board.transform.position.x;
                        nextYPos = BoardManager.BOARDS[board.GetBoardState(), index % 10].GetYPos() +
                                   board.transform.position.y;
                        ResourceManager.RemoveGlucose();
                        ResourceManager.RemoveGlucose();
                        gameManager.UpdateHandDisplay();
                        actionNum++;
                        InvokeRepeating("Move", 0, 1 / 60f);
                    }
                }
            }
            else if (index != -1 && inInfoPage)
            {
                if (index == 3)
                {
                    gameManager.HideInfoScreen();
                    inInfoPage = false;
                }
                else if (index == 0)
                {
                    gameManager.InfoButtonAction();
                }
                else if (index == 1)
                {
                    gameManager.InfoButtonAction();
                }
            }
        }
        else
        {
            if (index != -1 && !inInfoPage)
            {
                if (index == 3)
                {
                    inInfoPage = true;
                    gameManager.ShowInfoScreen();
                }
                else if (index == 4)
                {
                    waitingForAnim = true;
                    gameManager.EndTurnInit();
                }
            }
            else if (index != -1 && inInfoPage)
            {
                if (index == 3)
                {
                    gameManager.HideInfoScreen();
                    inInfoPage = false;
                }
                else if (index == 0)
                {
                    gameManager.InfoButtonAction();
                }
                else if (index == 1)
                {
                    gameManager.InfoButtonAction();
                }
            }
        }
    }

    private int[][] UpdateIndices(int[] indices, int gLost)
    {
        int[] lIndices = new int[] { };
        int[] gIndices = new int[] { };

        if (gLost == 1)
        {
            int gIndex = -1;
            for (int i = 0; i < indices.Length; i++)
            {
                if (indices[i] > gIndex && indices[i] < 4)
                {
                    gIndex = i;
                }
            }
            
            if (gIndex > -1)
            {
                gIndices = new int[indices.Length - 1];
                for (int i = 0; i < indices.Length; i++)
                {
                    if (i < gIndex)
                    {
                        gIndices[i] = indices[i];
                    }
                    else if (i > gIndex)
                    {
                        gIndices[i - 1] = indices[i];
                    }
                }
            }
            else
            {
                lIndices = new int[] { 0 };
                gIndices = indices;
            }
        }
        else if (gLost == 2)
        {
            int gIndex1 = -1, gIndex2 = -1;
            for (int i = 0; i < indices.Length; i++)
            {
                if (indices[i] > gIndex1 && indices[i] < 4)
                {
                    if (gIndex1 > gIndex2)
                    {
                        gIndex2 = gIndex1;
                    }
                    gIndex1 = i;
                }
            }

            if (gIndex1 > -1 && gIndex2 > -1)
            {
                gIndices = new int[indices.Length - 2];
                for (int i = 0; i < indices.Length; i++)
                {
                    if (i < gIndex1 && i < gIndex2)
                    {
                        gIndices[i] = indices[i];
                    }
                    else if (i < gIndex1 && i > gIndex2)
                    {
                        gIndices[i - 1] = indices[i];
                    }
                    else if (i > gIndex1)
                    {
                        gIndices[i - 2] = indices[i];
                    }
                }
            }
            else if (gIndex1 > -1)
            {
                lIndices = new int[] { 0 };
                gIndices = new int[indices.Length - 1];
                for (int i = 0; i < indices.Length; i++)
                {
                    if (i < gIndex1)
                    {
                        gIndices[i] = indices[i];
                    }
                    else if (i > gIndex1)
                    {
                        gIndices[i - 1] = indices[i];
                    }
                }
            }
            else
            {
                lIndices = new int[] { 0, 1 };
                gIndices = indices;
            }
        }

        return new int[][] { lIndices, gIndices };
    }
}
