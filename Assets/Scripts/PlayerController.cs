using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int curSpace = 3;
    private BoardManager board;
    private bool isMoving = false;
    
    // Start is called before the first frame update
    void Start()
    {
        board = GameObject.Find("Game Board").GetComponent<BoardManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (board.getBoardState() % 1 == 0 && !isMoving)
        {
            if (Input.GetKey(KeyCode.D))
            {
                Debug.Log("call");
                if (canMove(0, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
                {
                    isMoving = true;
                }
            }
            else if (Input.GetKey(KeyCode.W) && canMove(2, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
            {
                Debug.Log("call");
                if (canMove(2, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
                {
                    isMoving = true;
                }
            }
            else if (Input.GetKey(KeyCode.A) && canMove(4, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
            {
                Debug.Log("call");
                if (canMove(4, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
                {
                    isMoving = true;
                }
            }
            else if (Input.GetKey(KeyCode.S) && canMove(6, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
            {
                Debug.Log("call");
                if (canMove(6, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
                {
                    isMoving = true;
                }
            }
        }
        else if (board.getBoardState() % 2 == 0 && !isMoving)
        {
            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W))
            {
                Debug.Log("call");
                if (canMove(1, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
                {
                    Debug.Log("test");
                    isMoving = true;
                }
            }
            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
            {
                Debug.Log("call");
                if (canMove(3, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
                {
                    isMoving = true;
                }
            }
            else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
            {
                Debug.Log("call");
                if (canMove(5, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
                {
                    isMoving = true;
                }
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            {
                Debug.Log("call");
                if (canMove(7, BoardManager.BOARDS[board.getBoardState(), curSpace].getDirs()))
                {
                    isMoving = true;
                }
            }
        }
        Debug.Log(Time.deltaTime);
    }

    public int getCurSpace()
    {
        return curSpace;
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
}
