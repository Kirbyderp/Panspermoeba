using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpace
{
    int id;
    int[] adjs; //ID's of adjacent spaces
    int[] dirs; //dirs[i] indicates the direction to the space with ID adjs[i] 

    float xPos;
    float yPos;

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

    public int[] getAdjs()
    {
        return adjs;
    }

    public int[] getDirs()
    {
        return dirs;
    }

    public float getXPos()
    {
        return xPos;
    }

    public float getYPos()
    {
        return yPos;
    }
}
