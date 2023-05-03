using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    private static bool[] eventsOccurred = {false, false, false, false, false, false, false, false};

    public static bool HasEventOccurred(int index)
    {
        return eventsOccurred[index];
    }

    public static int PickEvent()
    {
        int initIndex = Random.Range(0, 7);
        int index = initIndex;
        bool pickedEvent = false, hitZero = false;
        while (!pickedEvent)
        {
            if (eventsOccurred[index] && !hitZero)
            {
                index--;
            }
            else if (eventsOccurred[index] && hitZero)
            {
                index++;
            }
            else
            {
                pickedEvent = true;
            }
            
            if (index < 0)
            {
                index = initIndex + 1;
                hitZero = true;
            }
        }
        return index;
    }
    
    public static void TriggerEvent(int index)
    {
        if (index == 0)
        {

        }
        else if (index == 1)
        {

        }
        else if (index == 2)
        {

        }
        else if (index == 3)
        {

        }
        else if (index == 4)
        {

        }
        else if (index == 5)
        {

        }
        else if (index == 6)
        {

        }
        else if (index == 7)
        {

        }
    }
}
