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
    private bool isMoving = false;
    
    // Start is called before the first frame update
    void Start()
    {
        board = GameObject.Find("Game Board").GetComponent<BoardManager>();
        curXPos = board.transform.position.x;
        curYPos = board.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (board.getBoardState() % 2 == 1 && !isMoving)
        {
            if (Input.GetKey(KeyCode.D))
            {
                if (canMove(0, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
                {
                    isMoving = true;
                    int spaceID = BoardManager.BOARDS[board.getBoardState(), curSpace].getAdjs()
                                  [indexOfDir(0, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs())];
                    nextSpace = spaceID;
                    nextXPos = BoardManager.BOARDS[board.getBoardState(), spaceID].getXPos() +
                               board.transform.position.x;
                    nextYPos = BoardManager.BOARDS[board.getBoardState(), spaceID].getYPos() +
                               board.transform.position.y;
                    InvokeRepeating("move", 0, 1/60f);
                }
            }
            else if (Input.GetKey(KeyCode.W) && canMove(2, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
            {
                if (canMove(2, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
                {
                    isMoving = true;
                    int spaceID = BoardManager.BOARDS[board.getBoardState(), curSpace].getAdjs()
                                  [indexOfDir(2, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs())];
                    nextSpace = spaceID;
                    nextXPos = BoardManager.BOARDS[board.getBoardState(), spaceID].getXPos() +
                               board.transform.position.x;
                    nextYPos = BoardManager.BOARDS[board.getBoardState(), spaceID].getYPos() +
                               board.transform.position.y;
                    InvokeRepeating("move", 0, 1 / 60f);
                }
            }
            else if (Input.GetKey(KeyCode.A) && canMove(4, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
            {
                if (canMove(4, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
                {
                    isMoving = true;
                    int spaceID = BoardManager.BOARDS[board.getBoardState(), curSpace].getAdjs()
                                  [indexOfDir(4, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs())];
                    nextSpace = spaceID;
                    nextXPos = BoardManager.BOARDS[board.getBoardState(), spaceID].getXPos() +
                               board.transform.position.x;
                    nextYPos = BoardManager.BOARDS[board.getBoardState(), spaceID].getYPos() +
                               board.transform.position.y;
                    InvokeRepeating("move", 0, 1 / 60f);
                }
            }
            else if (Input.GetKey(KeyCode.S) && canMove(6, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
            {
                if (canMove(6, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
                {
                    isMoving = true;
                    int spaceID = BoardManager.BOARDS[board.getBoardState(), curSpace].getAdjs()
                                  [indexOfDir(6, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs())];
                    nextSpace = spaceID;
                    nextXPos = BoardManager.BOARDS[board.getBoardState(), spaceID].getXPos() +
                               board.transform.position.x;
                    nextYPos = BoardManager.BOARDS[board.getBoardState(), spaceID].getYPos() +
                               board.transform.position.y;
                    InvokeRepeating("move", 0, 1 / 60f);
                }
            }
        }
        else if (board.getBoardState() % 2 == 0 && !isMoving)
        {
            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W))
            {
                if (canMove(1, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
                {
                    isMoving = true;
                    int spaceID = BoardManager.BOARDS[board.getBoardState(), curSpace].getAdjs()
                                  [indexOfDir(1, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs())];
                    nextSpace = spaceID;
                    nextXPos = BoardManager.BOARDS[board.getBoardState(), spaceID].getXPos() +
                               board.transform.position.x;
                    nextYPos = BoardManager.BOARDS[board.getBoardState(), spaceID].getYPos() +
                               board.transform.position.y;
                    InvokeRepeating("move", 0, 1 / 60f);
                }
            }
            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
            {
                if (canMove(3, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
                {
                    isMoving = true;
                    int spaceID = BoardManager.BOARDS[board.getBoardState(), curSpace].getAdjs()
                                  [indexOfDir(3, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs())];
                    nextSpace = spaceID;
                    nextXPos = BoardManager.BOARDS[board.getBoardState(), spaceID].getXPos() +
                               board.transform.position.x;
                    nextYPos = BoardManager.BOARDS[board.getBoardState(), spaceID].getYPos() +
                               board.transform.position.y;
                    InvokeRepeating("move", 0, 1 / 60f);
                }
            }
            else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
            {
                if (canMove(5, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
                {
                    isMoving = true;
                    int spaceID = BoardManager.BOARDS[board.getBoardState(), curSpace].getAdjs()
                                  [indexOfDir(5, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs())];
                    nextSpace = spaceID;
                    nextXPos = BoardManager.BOARDS[board.getBoardState(), spaceID].getXPos() +
                               board.transform.position.x;
                    nextYPos = BoardManager.BOARDS[board.getBoardState(), spaceID].getYPos() +
                               board.transform.position.y;
                    InvokeRepeating("move", 0, 1 / 60f);
                }
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            {
                if (canMove(7, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
                {
                    isMoving = true;
                    int spaceID = BoardManager.BOARDS[board.getBoardState(), curSpace].getAdjs()
                                  [indexOfDir(7, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs())];
                    nextSpace = spaceID;
                    nextXPos = BoardManager.BOARDS[board.getBoardState(), spaceID].getXPos() +
                               board.transform.position.x;
                    nextYPos = BoardManager.BOARDS[board.getBoardState(), spaceID].getYPos() +
                               board.transform.position.y;
                    InvokeRepeating("move", 0, 1 / 60f);
                }
            }
        }
    }

    public int getCurSpace()
    {
        return curSpace;
    }

    public void setCurXPos(float xIn)
    {
        curXPos = xIn;
    }

    public void setCurYPos(float yIn)
    {
        curYPos = yIn;
    }

    private bool canMove(int dir, int[] allowedDirs)
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

    private int indexOfDir(int dir, int[] allowedDirs)
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

    private void move()
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
            isMoving = false;
        }
    }
}
