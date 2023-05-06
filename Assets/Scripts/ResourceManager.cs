using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    private static List<Resource> resourceDeck;
    private static List<Resource> playerHand;
    private static List<Resource> discardPile;
    private static GameManager gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    private static PlayerController playerController = GameObject.Find("Player Microbe")
                                                                 .GetComponent<PlayerController>();


    public static List<Resource> GetPlayerHand()
    {
        return playerHand;
    }

    public static int[] CountRInHand()
    {
        int[] rInHand = { 0, 0, 0, 0 };
        foreach (Resource r in playerHand)
        {
            if (r.ToString().Equals(Resource.TYPES[0]))
            {
                rInHand[0]++;
            }
            else if (r.ToString().Equals(Resource.TYPES[1]))
            {
                rInHand[1]++;
            }
            else if (r.ToString().Equals(Resource.TYPES[2]))
            {
                rInHand[2]++;
            }
            else if (r.ToString().Equals(Resource.TYPES[3]))
            {
                rInHand[3]++;
            }
        }
        return rInHand;
    }

    public static void SetUpDeck()
    {
        resourceDeck = new List<Resource>();
        playerHand = new List<Resource>();
        discardPile = new List<Resource>();
        for (int i = 0; i < 12; i++)
        {
            resourceDeck.Add(new Resource(0));
        }
        for (int i = 0; i < 32; i++)
        {
            resourceDeck.Add(new Resource(1));
        }
        for (int i = 0; i < 4; i++)
        {
            resourceDeck.Add(new Resource(2));
        }
        for (int i = 0; i < 4; i++)
        {
            resourceDeck.Add(new Resource(3));
        }
    }

    public static void ShuffleDeck()
    {
        while (discardPile.Count != 0)
        {
            resourceDeck.Add(discardPile[0]);
            discardPile.RemoveAt(0);
        }
    }

    public static void Draw(int cardNum)
    {
        for (int i = 0; i < cardNum; i++)
        {
            if (resourceDeck.Count == 0 && discardPile.Count != 0)
            {
                ShuffleDeck();
            }

            if (resourceDeck.Count != 0)
            {
                int cardIndex = Random.Range(0, resourceDeck.Count);
                if (resourceDeck[cardIndex].ToString().Equals(Resource.TYPES[0]))
                {
                    discardPile.Add(resourceDeck[cardIndex]);
                }
                else
                {
                    playerHand.Add(resourceDeck[cardIndex]);
                }
                resourceDeck.RemoveAt(cardIndex);
            }
        }
    }

    public static int[] Scavenge(int cardNum, int curSpace)
    {
        int gCount = 0, pCount = 0, bCount = 0;
        for (int i = 0; i < cardNum; i++)
        {
            if (resourceDeck.Count == 0 && discardPile.Count != 0)
            {
                ShuffleDeck();
            }

            if (resourceDeck.Count != 0)
            {
                int cardIndex = Random.Range(0, resourceDeck.Count);
                if (resourceDeck[cardIndex].ToString().Equals(Resource.TYPES[0]))
                {
                    if (gameManager.GetEvent7Mod() && playerController.GetCurSpace() == 3)
                    {
                        playerHand.Add(resourceDeck[cardIndex]);
                    }
                    else
                    {
                        discardPile.Add(resourceDeck[cardIndex]);
                    }
                }
                else
                {
                    playerHand.Add(resourceDeck[cardIndex]);
                    if (resourceDeck[cardIndex].ToString().Equals(Resource.TYPES[1]))
                    {
                        gCount++;
                    }
                    else if (resourceDeck[cardIndex].ToString().Equals(Resource.TYPES[2]))
                    {
                        pCount++;
                    }
                    else
                    {
                        bCount++;
                    }
                }
                resourceDeck.RemoveAt(cardIndex);
            }
        }

        if (cardNum > 1)
        {
            gameManager.DecrementScavIndicatorAmt(curSpace);
            for (int i = 0; i < BoardManager.BOARDS.GetLength(0); i++)
            {
                BoardManager.BOARDS[i, curSpace].DecrementScavAmt();
            }
        }

        int[] indices = new int[gCount + pCount + bCount];
        if (gCount == 1)
        {
            indices[0] = 0;
        }
        else if (gCount == 2)
        {
            indices[0] = 0;
            indices[1] = 1;
        }
        else if (gCount == 3)
        {
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
        }
        else if (gCount == 4)
        {
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 3;
        }

        if (pCount == 1)
        {
            indices[gCount] = 4;
        }
        else if (pCount == 2)
        {
            indices[gCount] = 4;
            indices[gCount + 1] = 5;
        }
        else if (pCount == 3)
        {
            indices[gCount] = 4;
            indices[gCount + 1] = 5;
            indices[gCount + 2] = 6;
        }
        else if (pCount == 4)
        {
            indices[0] = 4;
            indices[1] = 5;
            indices[2] = 6;
            indices[3] = 7;
        }

        if (bCount == 1)
        {
            indices[gCount + pCount] = 8;
        }
        else if (bCount == 2)
        {
            indices[gCount + pCount] = 8;
            indices[gCount + pCount + 1] = 9;
        }
        else if (bCount == 3)
        {
            indices[gCount + pCount] = 8;
            indices[gCount + pCount + 1] = 9;
            indices[gCount + pCount + 2] = 10;
        }
        else if (bCount == 4)
        {
            indices[0] = 8;
            indices[1] = 9;
            indices[2] = 10;
            indices[3] = 11;
        }

        return indices;
    }

    public static bool CanRaiseTemp(int extraS)
    {
        int count = 0;
        foreach(Resource r in playerHand)
        {
            if (r.ToString().Equals(Resource.TYPES[1]))
            {
                count++;
                if (count == extraS + 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static void RaiseTemp()
    {
        foreach (Resource r in playerHand)
        {
            if (r.ToString().Equals(Resource.TYPES[1]))
            {
                discardPile.Add(r);
                playerHand.Remove(r);
                break;
            }
        }
    }

    public static bool CanRaiseGene(int extraS)
    {
        bool hasS = false, hasP = false, hasB = false;
        int count = 0;
        foreach (Resource r in playerHand)
        {
            if (r.ToString().Equals(Resource.TYPES[1]))
            {
                count++;
                if (count == 1 + extraS)
                {
                    hasS = true;
                }
            }
            if (r.ToString().Equals(Resource.TYPES[2]))
            {
                hasP = true;
            }
            if (r.ToString().Equals(Resource.TYPES[3]))
            {
                hasB = true;
            }
            if (hasS && hasP && hasB)
            {
                return true;
            }
        }
        return false;
    }

    public static void RaiseGene()
    {
        foreach (Resource r in playerHand)
        {
            if (r.ToString().Equals(Resource.TYPES[1]))
            {
                discardPile.Add(r);
                playerHand.Remove(r);
                break;
            }
        }

        foreach (Resource r in playerHand)
        {
            if (r.ToString().Equals(Resource.TYPES[2]))
            {
                discardPile.Add(r);
                playerHand.Remove(r);
                break;
            }
        }

        foreach (Resource r in playerHand)
        {
            if (r.ToString().Equals(Resource.TYPES[3]))
            {
                discardPile.Add(r);
                playerHand.Remove(r);
                break;
            }
        }
    }

    public static bool HasGlucose(int num)
    {
        int count = 0;
        foreach (Resource r in playerHand)
        {
            if (r.ToString().Equals(Resource.TYPES[1]))
            {
                count++;
                if (count == num)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static void RemoveGlucose()
    {
        foreach (Resource r in playerHand)
        {
            if (r.ToString().Equals(Resource.TYPES[1]))
            {
                discardPile.Add(r);
                playerHand.Remove(r);
                break;
            }
        }
    }

    public static void RemoveResource(int rType)
    {
        foreach (Resource r in playerHand)
        {
            if (r.ToString().Equals(Resource.TYPES[rType]))
            {
                discardPile.Add(r);
                playerHand.Remove(r);
                break;
            }
        }
    }

    public static void Event0()
    {
        gameManager.SetWaitingForEndTurnAnim1(true);
        int gCount = 0, pCount = 0, bCount = 0;
        if (playerHand.Count > 2)
        {
            for (int cardsRemoved = 0; cardsRemoved < 3; cardsRemoved++)
            {
                int index = Random.Range(0, playerHand.Count);
                if (playerHand[index].ToString().Equals(Resource.TYPES[1]))
                {
                    gCount++;
                }
                else if (playerHand[index].ToString().Equals(Resource.TYPES[2]))
                {
                    pCount++;
                }
                else if (playerHand[index].ToString().Equals(Resource.TYPES[3]))
                {
                    bCount++;
                }
                discardPile.Add(playerHand[index]);
                playerHand.RemoveAt(index);
            }
        }
        else
        {
            while (playerHand.Count != 0)
            {
                if (playerHand[0].ToString().Equals(Resource.TYPES[1]))
                {
                    gCount++;
                }
                else if (playerHand[0].ToString().Equals(Resource.TYPES[2]))
                {
                    pCount++;
                }
                else if (playerHand[0].ToString().Equals(Resource.TYPES[3]))
                {
                    bCount++;
                }
                discardPile.Add(playerHand[0]);
                playerHand.RemoveAt(0);
            }
        }

        int[] indices = new int[gCount + pCount + bCount];
        if (indices.Length > 0)
        {
            if (gCount == 1)
            {
                indices[0] = 0;
            }
            else if (gCount == 2)
            {
                indices[0] = 0;
                indices[1] = 1;
            }
            else if (gCount == 3)
            {
                indices[0] = 0;
                indices[1] = 1;
                indices[2] = 2;
            }

            if (pCount == 1)
            {
                indices[gCount] = 3;
            }
            else if (pCount == 2)
            {
                indices[gCount] = 3;
                indices[gCount + 1] = 4;
            }
            else if (pCount == 3)
            {
                indices[0] = 3;
                indices[1] = 4;
                indices[2] = 5;
            }

            if (bCount == 1)
            {
                indices[gCount + pCount] = 6;
            }
            else if (bCount == 2)
            {
                indices[gCount + pCount] = 6;
                indices[gCount + pCount + 1] = 7;
            }
            else if (bCount == 3)
            {
                indices[0] = 6;
                indices[1] = 7;
                indices[2] = 8;
            }

            gameManager.UpdateHandDisplay();
            gameManager.StartCoroutine(gameManager.LoseResourceDisplay(indices));
        }
        else
        {
            gameManager.SetWaitingForEndTurnAnim1(false);
        }
    }

    public static void Event2()
    {
        gameManager.SetWaitingForEndTurnAnim1(true);
        int gCount = 0, pCount = 0, bCount = 0;
        for (int i = 0; i < 2; i++)
        {
            if (resourceDeck.Count == 0 && discardPile.Count != 0)
            {
                ShuffleDeck();
            }

            if (resourceDeck.Count != 0)
            {
                int cardIndex = Random.Range(0, resourceDeck.Count);
                if (resourceDeck[cardIndex].ToString().Equals(Resource.TYPES[0]))
                {
                    discardPile.Add(resourceDeck[cardIndex]);
                }
                else
                {
                    playerHand.Add(resourceDeck[cardIndex]);
                    if (resourceDeck[cardIndex].ToString().Equals(Resource.TYPES[1]))
                    {
                        gCount++;
                    }
                    else if (resourceDeck[cardIndex].ToString().Equals(Resource.TYPES[2]))
                    {
                        pCount++;
                    }
                    else
                    {
                        bCount++;
                    }
                }
                resourceDeck.RemoveAt(cardIndex);
            }
        }

        int[] indices = new int[gCount + pCount + bCount];
        if (indices.Length > 0)
        {
            if (gCount == 1)
            {
                indices[0] = 0;
            }
            else if (gCount == 2)
            {
                indices[0] = 0;
                indices[1] = 1;
            }

            if (pCount == 1)
            {
                indices[gCount] = 4;
            }
            else if (pCount == 2)
            {
                indices[0] = 4;
                indices[1] = 5;
            }

            if (bCount == 1)
            {
                indices[gCount + pCount] = 8;
            }
            else if (bCount == 2)
            {
                indices[0] = 8;
                indices[1] = 9;
            }
            gameManager.UpdateHandDisplay();
            gameManager.StartCoroutine(gameManager.GainResourceDisplay(indices));
        }
        else
        {
            gameManager.SetWaitingForEndTurnAnim1(false);
        }
    }
}
