using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    private static List<Resource> resourceDeck;
    private static List<Resource> playerHand;
    private static List<Resource> discardPile;


    public static List<Resource> GetPlayerHand()
    {
        return playerHand;
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

    public static void Scavenge(int cardNum)
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
}
