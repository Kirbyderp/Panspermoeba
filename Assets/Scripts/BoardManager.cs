using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    //Horizontal/Vertical distance between the centers of two ORTHogonal or DIAGonal spaces
    public static readonly float ORTH_DIST = 1.36f;
    public static readonly float DIAG_DIST = 1.36f / Mathf.Sqrt(2f);
    
    //A 2D array containing representations of all 8 board states
    public static readonly BoardSpace[,] BOARDS =
                   { { new BoardSpace(0, new int[] {1, 2}, new int[] {1, 7}, -2 * DIAG_DIST, 0),
                       new BoardSpace(1, new int[] {0, 3}, new int[] {5, 7}, -DIAG_DIST, DIAG_DIST),
                       new BoardSpace(2, new int[] {0, 3}, new int[] {3, 1}, -DIAG_DIST, -DIAG_DIST),
                       new BoardSpace(3, new int[] {1, 2, 4, 5}, new int[] {3, 5, 1, 7}, 0, 0),
                       new BoardSpace(4, new int[] {3, 6}, new int[] {5, 7}, DIAG_DIST, DIAG_DIST),
                       new BoardSpace(5, new int[] {3, 6}, new int[] {3, 1}, DIAG_DIST, -DIAG_DIST),
                       new BoardSpace(6, new int[] {4, 5}, new int[] {3, 5}, 2 * DIAG_DIST, 0) },
                     { new BoardSpace(0, new int[] {1, 2}, new int[] {2, 0}, -ORTH_DIST, -ORTH_DIST),
                       new BoardSpace(1, new int[] {0, 3}, new int[] {6, 0}, -ORTH_DIST, 0),
                       new BoardSpace(2, new int[] {0, 3}, new int[] {4, 2}, 0, -ORTH_DIST),
                       new BoardSpace(3, new int[] {1, 2, 4, 5}, new int[] {4, 6, 2, 0}, 0, 0),
                       new BoardSpace(4, new int[] {3, 6}, new int[] {6, 0}, 0, ORTH_DIST),
                       new BoardSpace(5, new int[] {3, 6}, new int[] {4, 2}, ORTH_DIST, 0),
                       new BoardSpace(6, new int[] {4, 5}, new int[] {4, 6}, ORTH_DIST, ORTH_DIST) },
                     { new BoardSpace(0, new int[] {1, 2}, new int[] {3, 1}, 0, -2 * DIAG_DIST),
                       new BoardSpace(1, new int[] {0, 3}, new int[] {7, 1}, -DIAG_DIST, -DIAG_DIST),
                       new BoardSpace(2, new int[] {0, 3}, new int[] {5, 3}, DIAG_DIST, -DIAG_DIST),
                       new BoardSpace(3, new int[] {1, 2, 4, 5}, new int[] {5, 7, 3, 1}, 0, 0),
                       new BoardSpace(4, new int[] {3, 6}, new int[] {7, 1}, -DIAG_DIST, DIAG_DIST),
                       new BoardSpace(5, new int[] {3, 6}, new int[] {5, 3}, DIAG_DIST, DIAG_DIST),
                       new BoardSpace(6, new int[] {4, 5}, new int[] {5, 7}, 0, 2 * DIAG_DIST) },
                     { new BoardSpace(0, new int[] {1, 2}, new int[] {4, 2}, ORTH_DIST, -ORTH_DIST),
                       new BoardSpace(1, new int[] {0, 3}, new int[] {0, 2}, 0, -ORTH_DIST),
                       new BoardSpace(2, new int[] {0, 3}, new int[] {6, 4}, ORTH_DIST, 0),
                       new BoardSpace(3, new int[] {1, 2, 4, 5}, new int[] {6, 0, 4, 2}, 0, 0),
                       new BoardSpace(4, new int[] {3, 6}, new int[] {0, 2}, -ORTH_DIST, 0),
                       new BoardSpace(5, new int[] {3, 6}, new int[] {6, 4}, 0, ORTH_DIST),
                       new BoardSpace(6, new int[] {4, 5}, new int[] {6, 0}, -ORTH_DIST, ORTH_DIST) },
                     { new BoardSpace(0, new int[] {1, 2}, new int[] {5, 3}, 2 * DIAG_DIST, 0),
                       new BoardSpace(1, new int[] {0, 3}, new int[] {1, 3}, DIAG_DIST, -DIAG_DIST),
                       new BoardSpace(2, new int[] {0, 3}, new int[] {7, 5}, DIAG_DIST, DIAG_DIST),
                       new BoardSpace(3, new int[] {1, 2, 4, 5}, new int[] {7, 1, 5, 3}, 0, 0),
                       new BoardSpace(4, new int[] {3, 6}, new int[] {1, 3}, -DIAG_DIST, -DIAG_DIST),
                       new BoardSpace(5, new int[] {3, 6}, new int[] {7, 5}, -DIAG_DIST, DIAG_DIST),
                       new BoardSpace(6, new int[] {4, 5}, new int[] {7, 1}, -2 * DIAG_DIST, 0) },
                     { new BoardSpace(0, new int[] {1, 2}, new int[] {6, 4}, ORTH_DIST, ORTH_DIST),
                       new BoardSpace(1, new int[] {0, 3}, new int[] {2, 4}, ORTH_DIST, 0),
                       new BoardSpace(2, new int[] {0, 3}, new int[] {0, 6}, 0, ORTH_DIST),
                       new BoardSpace(3, new int[] {1, 2, 4, 5}, new int[] {0, 2, 6, 4}, 0, 0),
                       new BoardSpace(4, new int[] {3, 6}, new int[] {2, 4}, 0, -ORTH_DIST),
                       new BoardSpace(5, new int[] {3, 6}, new int[] {0, 6}, -ORTH_DIST, 0),
                       new BoardSpace(6, new int[] {4, 5}, new int[] {0, 2}, -ORTH_DIST, -ORTH_DIST) },
                     { new BoardSpace(0, new int[] {1, 2}, new int[] {7, 5}, 0, 2 * DIAG_DIST),
                       new BoardSpace(1, new int[] {0, 3}, new int[] {3, 5}, DIAG_DIST, DIAG_DIST),
                       new BoardSpace(2, new int[] {0, 3}, new int[] {1, 7}, -DIAG_DIST, DIAG_DIST),
                       new BoardSpace(3, new int[] {1, 2, 4, 5}, new int[] {1, 3, 7, 5}, 0, 0),
                       new BoardSpace(4, new int[] {3, 6}, new int[] {3, 5}, DIAG_DIST, -DIAG_DIST),
                       new BoardSpace(5, new int[] {3, 6}, new int[] {1, 7}, -DIAG_DIST, -DIAG_DIST),
                       new BoardSpace(6, new int[] {4, 5}, new int[] {1, 3}, 0, -2 * DIAG_DIST) },
                     { new BoardSpace(0, new int[] {1, 2}, new int[] {0, 6}, -ORTH_DIST, ORTH_DIST),
                       new BoardSpace(1, new int[] {0, 3}, new int[] {4, 6}, 0, ORTH_DIST),
                       new BoardSpace(2, new int[] {0, 3}, new int[] {2, 0}, -ORTH_DIST, 0),
                       new BoardSpace(3, new int[] {1, 2, 4, 5}, new int[] {2, 4, 0, 6}, 0, 0),
                       new BoardSpace(4, new int[] {3, 6}, new int[] {4, 6}, ORTH_DIST, 0),
                       new BoardSpace(5, new int[] {3, 6}, new int[] {2, 0}, 0, -ORTH_DIST),
                       new BoardSpace(6, new int[] {4, 5}, new int[] {2, 4}, ORTH_DIST, -ORTH_DIST) } };

    private int boardState = 0; //Int from 0-7
    private int[] boardAngles = {0, 45, 90, 135, 180, 225, 270, 315};
    private int nextState = 1;
    private float rotationSpeed = 60f;

    private GameObject player;
    private PlayerController playerController;
    private GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player Microbe");
        playerController = player.GetComponent<PlayerController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InvokeRepeating("RotateAsteroid", 0, 1/rotationSpeed);
        }

        if (!playerController.GetIsMoving() && Input.GetKeyDown(KeyCode.H))
        {
            ResourceManager.Scavenge(4);
            gameManager.UpdateHandDisplay();
        }
    }

    public int GetBoardState()
    {
        return boardState;
    }

    void RotateAsteroid()
    {
        transform.Rotate(new Vector3(0, 0, 1));
        player.transform.rotation = Quaternion.Euler(0, 0, 0);
        if (transform.rotation.eulerAngles.z >= 360)
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        if ((int)(transform.rotation.eulerAngles.z + .1f) == boardAngles[nextState])
        {
            boardState = nextState;
            //nextState = Random.Range(0, 8);
            nextState++;
            if (nextState == 8)
            {
                nextState = 0;
            }
            transform.rotation = Quaternion.Euler(0, 0, boardAngles[boardState]);
            playerController.SetCurXPos(BOARDS[boardState, playerController.GetCurSpace()].GetXPos() +
                                        transform.position.x);
            playerController.SetCurYPos(BOARDS[boardState, playerController.GetCurSpace()].GetYPos() +
                                        transform.position.y);
            CancelInvoke();
        }
    }
}
