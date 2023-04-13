using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    int boardState = 0; //Int from 0-7
    int[] boardStates = {0, 45, 90, 135, 180, 225, 270, 315};
    int nextState = 7;
    float rotationSpeed = 60f;
    
    GameObject player;
    int curPlayerSpace = 3;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player Microbe");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InvokeRepeating("RotateAsteroid", 0, 1/rotationSpeed);
        }
    }

    void RotateAsteroid()
    {
        transform.Rotate(new Vector3(0, 0, 1));
        player.transform.rotation = Quaternion.Euler(0, 0, 0);
        if (transform.rotation.eulerAngles.z >= 360)
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        if ((int)(transform.rotation.eulerAngles.z + .1f) == boardStates[nextState])
        {
            boardState = nextState;
            nextState = Random.Range(0, 8);
            transform.rotation = Quaternion.Euler(0, 0, boardStates[boardState]);
            CancelInvoke();
        }
    }
}
