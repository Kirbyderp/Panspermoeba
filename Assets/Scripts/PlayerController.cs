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

    // Start is called before the first frame update
    void Start()
    {
        board = GameObject.Find("Game Board").GetComponent<BoardManager>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        curXPos = board.transform.position.x;
        curYPos = board.transform.position.y;

    }

    // Update is called once per frame
    void Update()
    {
        if (controlScheme != 0)
        {
            LookForKeyboardMoveInput();
        }
        
        if (controlScheme != 1)
        {
            HandleButtonMouseSelections();
            if (Input.GetKeyDown(KeyCode.Mouse0) && !waitingForAnim)
            {
                PerformAction(GetButtonMouseSelected());
            }
        }
        

        //DEV TOOLS
        if (!waitingForAnim && !inInfoPage)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                //ResourceManager.Scavenge(BoardManager.BOARDS[board.GetBoardState(), curSpace].GetScavAmt(), curSpace);
                ResourceManager.Scavenge(20, curSpace);
                gameManager.UpdateHandDisplay();
            }
            else if (Input.GetKeyDown(KeyCode.T) && ResourceManager.CanRaiseTemp(0) && gameManager.CanRaiseTemp(1))
            {
                waitingForAnim = true;
                ResourceManager.RaiseTemp();
                gameManager.UpdateHandDisplay();
                gameManager.RaiseThermobar(1);
            }
            else if (Input.GetKeyDown(KeyCode.G) && ResourceManager.CanRaiseGene(0) && gameManager.CanRaiseGene(1))
            {
                waitingForAnim = true;
                ResourceManager.RaiseGene();
                gameManager.UpdateHandDisplay();
                gameManager.RaiseRadibar(1);
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

    private void LookForKeyboardMoveInput()
    {
        if (board.GetBoardState() % 2 == 1 && !waitingForAnim && !inInfoPage &&
            (actionNum < 5 || (actionNum == 5 && ResourceManager.HasSugar(1))
                             || (actionNum == 6 && ResourceManager.HasSugar(2))))
        {
            if (Input.GetKey(KeyCode.D))
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
                        ResourceManager.RemoveSugar();
                        gameManager.UpdateHandDisplay();
                    }
                    else if (actionNum == 6)
                    {
                        ResourceManager.RemoveSugar();
                        ResourceManager.RemoveSugar();
                        gameManager.UpdateHandDisplay();
                    }
                    actionNum++;
                    InvokeRepeating("Move", 0, 1 / 60f);
                }
            }
            else if (Input.GetKey(KeyCode.W) && CanMove(2, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs()))
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
                        ResourceManager.RemoveSugar();
                        gameManager.UpdateHandDisplay();
                    }
                    else if (actionNum == 6)
                    {
                        ResourceManager.RemoveSugar();
                        ResourceManager.RemoveSugar();
                        gameManager.UpdateHandDisplay();
                    }
                    actionNum++;
                    InvokeRepeating("Move", 0, 1 / 60f);
                }
            }
            else if (Input.GetKey(KeyCode.A) && CanMove(4, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs()))
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
                        ResourceManager.RemoveSugar();
                        gameManager.UpdateHandDisplay();
                    }
                    else if (actionNum == 6)
                    {
                        ResourceManager.RemoveSugar();
                        ResourceManager.RemoveSugar();
                        gameManager.UpdateHandDisplay();
                    }
                    actionNum++;
                    InvokeRepeating("Move", 0, 1 / 60f);
                }
            }
            else if (Input.GetKey(KeyCode.S) && CanMove(6, BoardManager.BOARDS[board.GetBoardState(), curSpace].GetDirs()))
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
                        ResourceManager.RemoveSugar();
                        gameManager.UpdateHandDisplay();
                    }
                    else if (actionNum == 6)
                    {
                        ResourceManager.RemoveSugar();
                        ResourceManager.RemoveSugar();
                        gameManager.UpdateHandDisplay();
                    }
                    actionNum++;
                    InvokeRepeating("Move", 0, 1 / 60f);
                }
            }
        }
        else if (board.GetBoardState() % 2 == 0 && !waitingForAnim && !inInfoPage &&
                 (actionNum < 5 || (actionNum == 5 && ResourceManager.HasSugar(1))
                                  || (actionNum == 6 && ResourceManager.HasSugar(2))))
        {
            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W))
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
                        ResourceManager.RemoveSugar();
                        gameManager.UpdateHandDisplay();
                    }
                    else if (actionNum == 6)
                    {
                        ResourceManager.RemoveSugar();
                        ResourceManager.RemoveSugar();
                        gameManager.UpdateHandDisplay();
                    }
                    actionNum++;
                    InvokeRepeating("Move", 0, 1 / 60f);
                }
            }
            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
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
                        ResourceManager.RemoveSugar();
                        gameManager.UpdateHandDisplay();
                    }
                    else if (actionNum == 6)
                    {
                        ResourceManager.RemoveSugar();
                        ResourceManager.RemoveSugar();
                        gameManager.UpdateHandDisplay();
                    }
                    actionNum++;
                    InvokeRepeating("Move", 0, 1 / 60f);
                }
            }
            else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
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
                        ResourceManager.RemoveSugar();
                        gameManager.UpdateHandDisplay();
                    }
                    else if (actionNum == 6)
                    {
                        ResourceManager.RemoveSugar();
                        ResourceManager.RemoveSugar();
                        gameManager.UpdateHandDisplay();
                    }
                    actionNum++;
                    InvokeRepeating("Move", 0, 1 / 60f);
                }
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
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
                        ResourceManager.RemoveSugar();
                        gameManager.UpdateHandDisplay();
                    }
                    else if (actionNum == 6)
                    {
                        ResourceManager.RemoveSugar();
                        ResourceManager.RemoveSugar();
                        gameManager.UpdateHandDisplay();
                    }
                    actionNum++;
                    InvokeRepeating("Move", 0, 1 / 60f);
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
        Debug.Log(mousePos);
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
                    gameManager.RaiseThermobar(1);
                }
                else if (index == 1 && ResourceManager.CanRaiseGene(0) && gameManager.CanRaiseGene(1))
                {
                    waitingForAnim = true;
                    ResourceManager.RaiseGene();
                    gameManager.UpdateHandDisplay();
                    actionNum++;
                    gameManager.RaiseRadibar(1);
                }
                else if (index == 2)
                {
                    ResourceManager.Scavenge(BoardManager.BOARDS[board.GetBoardState(), curSpace].GetScavAmt(), curSpace);
                    gameManager.UpdateHandDisplay();
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
                    ResourceManager.RemoveSugar();
                    gameManager.UpdateHandDisplay();
                    actionNum++;
                    gameManager.RaiseThermobar(1);
                }
                else if (index == 1 && ResourceManager.CanRaiseGene(1) && gameManager.CanRaiseGene(1))
                {
                    waitingForAnim = true;
                    ResourceManager.RaiseGene();
                    ResourceManager.RemoveSugar();
                    gameManager.UpdateHandDisplay();
                    actionNum++;
                    gameManager.RaiseRadibar(1);
                }
                else if (index == 2 && ResourceManager.HasSugar(1))
                {
                    ResourceManager.Scavenge(BoardManager.BOARDS[board.GetBoardState(), curSpace].GetScavAmt(), curSpace);
                    ResourceManager.RemoveSugar();
                    gameManager.UpdateHandDisplay();
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
                        ResourceManager.RemoveSugar();
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
                    ResourceManager.RemoveSugar();
                    ResourceManager.RemoveSugar();
                    gameManager.UpdateHandDisplay();
                    actionNum++;
                    gameManager.RaiseThermobar(1);
                }
                else if (index == 1 && ResourceManager.CanRaiseGene(2) && gameManager.CanRaiseGene(1))
                {
                    waitingForAnim = true;
                    ResourceManager.RaiseGene();
                    ResourceManager.RemoveSugar();
                    ResourceManager.RemoveSugar();
                    gameManager.UpdateHandDisplay();
                    actionNum++;
                    gameManager.RaiseRadibar(1);
                }
                else if (index == 2 && ResourceManager.HasSugar(2))
                {
                    ResourceManager.Scavenge(BoardManager.BOARDS[board.GetBoardState(), curSpace].GetScavAmt(), curSpace);
                    ResourceManager.RemoveSugar();
                    ResourceManager.RemoveSugar();
                    gameManager.UpdateHandDisplay();
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
                        ResourceManager.RemoveSugar();
                        ResourceManager.RemoveSugar();
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
}
