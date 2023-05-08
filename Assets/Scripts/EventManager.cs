using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private bool[] eventsOccurred = {false, false, false, false, false, false, false, false};
    private GameManager gameManager;
    private ResourceManager resourceManager;
    private PlayerController playerController;
    public static readonly string[] EVENT_TEXT = { "You are attacked by other microorganisms on " +
                                                     "the asteroid! Lose 3 of your resources or " +
                                                     "all of them if you have less than 3.",
                                                   "You find another microorganism and exchange " +
                                                     "DNA with them. Raise your genetic " +
                                                     "stability by 10%.",
                                                   "You find a dead bacteria and are able to " +
                                                     "harvest 2 resources from it.",
                                                   "The asteroid is traveling closer to the sun. " +
                                                     "From this point on, during the upkeep " +
                                                     "phase, treat all temperature changes as if " +
                                                     "they are +1T compared to their original " +
                                                     "values.",
                                                   "Radioactive material inside the asteroid has " +
                                                     "caused radiation to build up. The center " +
                                                     "tile is now irradiated. During the upkeep " +
                                                     "phase, treat the center tile as if it has " +
                                                     "+2R compared to its original values.",
                                                   "Fringe radiation from a solar flare has hit " +
                                                     "the asteroid! Decrease your genetic " +
                                                     "stability by 2 steps (20%).",
                                                   "Another asteroid has passed in front of your " +
                                                     "asteroid! Ignore all radiation during this " +
                                                     "upkeep phase (except from event 5 if " +
                                                     "applicable), but treat all temperature " +
                                                     "changes as if they are -1T compared to " +
                                                     "their original values for this turn.",
                                                   "A water pocket has been found in the center " +
                                                     "of the asteroid. Once per action, whenever " +
                                                     "you scavenge for resources in the center " +
                                                     "tile, you can discard any number of" +
                                                     "your stored resources to get that many back.",
                                                   "Your asteroid is entering the destination " +
                                                     "planet! During the upkeep phase, treat all " +
                                                     "temperature changes as if they are +7T " +
                                                     "compared to their original values. Don’t " +
                                                     "burn up with the asteroid!"};

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        resourceManager = GameObject.Find("Resource Manager").GetComponent<ResourceManager>();
        playerController = GameObject.Find("Player Microbe").GetComponent<PlayerController>();
    }

    public bool HasEventOccurred(int index)
    {
        return eventsOccurred[index];
    }

    public int PickEvent()
    {
        int initIndex = Random.Range(0, 8);
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
                eventsOccurred[index] = true;
            }
            
            if (index < 0)
            {
                index = initIndex + 1;
                hitZero = true;
            }
        }
        return index;
    }
    
    public void TriggerEvent(int index)
    {
        if (index == 0)
        {
            resourceManager.Event0();
        }
        else if (index == 1)
        {
            gameManager.SetWaitingForEndTurnAnim1(true);
            if (gameManager.CanRaiseGene(1))
            {
                gameManager.RaiseRadibar(1, new int[] { });
            }
            else
            {
                gameManager.SetWaitingForEndTurnAnim1(false);
            }
        }
        else if (index == 2)
        {
            resourceManager.Event2();
        }
        else if (index == 3)
        {
            gameManager.Event3();
        }
        else if (index == 4)
        {
            gameManager.Event4();
        }
        else if (index == 5)
        {
            gameManager.SetWaitingForEndTurnAnim1(true);
            if (gameManager.CanRaiseGene(-2))
            {
                gameManager.Event5();
                gameManager.RaiseRadibar(-1, new int[] { });
            }
            else if (gameManager.CanRaiseGene(-1))
            {
                gameManager.RaiseRadibar(-1, new int[] { });
            }
            else
            {
                gameManager.SetWaitingForEndTurnAnim1(false);
            }
        }
        else if (index == 6)
        {
            gameManager.Event6();
        }
        else if (index == 7)
        {
            gameManager.Event7();
        }
    }
}
