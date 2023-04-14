using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpace
{
    private int id;
    private int[] adjs; //ID's of adjacent spaces
    private int[] dirs; //dirs[i] indicates the direction to the space with ID adjs[i] 

    private float xPos;
    private float yPos;

    public BoardSpace()
    {
        id = 0;
        adjs = new int[0];
        dirs = new int[0];
        xPos = 0;
        yPos = 0;
    }

    public BoardSpace(int idIn, int[] adjsIn, int[] dirsIn, float xPosIn, float yPosIn)
    {
        id = idIn;
        adjs = adjsIn;
        dirs = dirsIn;
        xPos = xPosIn;
        yPos = yPosIn;
    }

    public int[] GetAdjs()
    {
        return adjs;
    }

    public int[] GetDirs()
    {
        return dirs;
    }

    public float GetXPos()
    {
        return xPos;
    }

    public float GetYPos()
    {
        return yPos;
    }
}
