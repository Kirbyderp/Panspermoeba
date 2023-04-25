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
    private GameManager gameManager;
    private bool[] selectedButtons = {false, false, false, false, false};

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
        LookForMoveInput();
        HandleButtonSelections();
        if (Input.GetKeyDown(KeyCode.Mouse0) && !waitingForAnim)
        {
            int index = GetButtonSelected();
            if (index != -1)
            {
                if (index == 0 && ResourceManager.CanRaiseTemp() && gameManager.CanRaiseTemp(1))
                {
                    waitingForAnim = true;
                    ResourceManager.RaiseTemp();
                    gameManager.UpdateHandDisplay();
                    gameManager.RaiseThermobar(1);
                }
                else if (index == 1 && ResourceManager.CanRaiseGene() && gameManager.CanRaiseGene(1))
                {
                    waitingForAnim = true;
                    ResourceManager.RaiseGene();
                    gameManager.UpdateHandDisplay();
                    gameManager.RaiseRadibar(1);
                }
                else if (index == 2)
                {
                    ResourceManager.Scavenge(BoardManager.BOARDS[board.GetBoardState(), curSpace].GetScavAmt(), curSpace);
                    gameManager.UpdateHandDisplay();
                }
                else if (index == 3)
                {
                    //bring up info page
                }
                else if (index == 4)
                {
                    waitingForAnim = true;
                    gameManager.EndTurn();
                    waitingForAnim = false;
                }
            }
        }

        if (!waitingForAnim)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                ResourceManager.Scavenge(BoardManager.BOARDS[board.GetBoardState(), curSpace].GetScavAmt(), curSpace);
                gameManager.UpdateHandDisplay();
            }
            else if (Input.GetKeyDown(KeyCode.T) && ResourceManager.CanRaiseTemp() && gameManager.CanRaiseTemp(1))
            {
                waitingForAnim = true;
                ResourceManager.RaiseTemp();
                gameManager.UpdateHandDisplay();
                gameManager.RaiseThermobar(1);
            }
            else if (Input.GetKeyDown(KeyCode.G) && ResourceManager.CanRaiseGene() && gameManager.CanRaiseGene(1))
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

    public void SetCurXPos(float xIn)
    {
        curXPos = xIn;
    }

    public void SetCurYPos(float yIn)
    {
        curYPos = yIn;
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

    private void LookForMoveInput()
    {
        if (board.GetBoardState() % 2 == 1 && !waitingForAnim)
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
                    InvokeRepeating("Move", 0, 1 / 60f);
                }
            }
        }
        else if (board.GetBoardState() % 2 == 0 && !waitingForAnim)
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

    private void HandleButtonSelections()
    {
        Vector3 mousePos = Input.mousePosition;

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
    }

    private int GetButtonSelected()
    {
        Vector3 mousePos = Input.mousePosition;

        if (mousePos.x > 1153 && mousePos.x < 1343 && mousePos.y > 275 && mousePos.y < 465)
        {
            return 0;
        }

        if (mousePos.x > 1391 && mousePos.x < 1579 && mousePos.y > 275 && mousePos.y < 465)
        {
            return 1;
        }

        if (mousePos.x > 915 && mousePos.x < 1105 && mousePos.y > 39 && mousePos.y < 226)
        {
            return 2;
        }

        if (mousePos.x > 1153 && mousePos.x < 1343 && mousePos.y > 39 && mousePos.y < 226)
        {
            return 3;
        }

        if (mousePos.x > 1391 && mousePos.x < 1579 && mousePos.y > 39 && mousePos.y < 226)
        {
            return 4;
        }

        return -1;
    }
}
