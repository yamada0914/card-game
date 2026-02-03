using UnityEngine;
using System.Collections.Generic;

public class GamePlayerManager : MonoBehaviour
{
    public List<int> deck = new List<int>();

    public int heroHp;
    public int manaCost;
    public int defaultManaCost;

    public void Init(List<int> cardDeck)
    {
        deck = cardDeck;
        heroHp = 10;
        manaCost = 10;
        defaultManaCost = 10;
    }

    public void IncreaseManaCost()
    {
        defaultManaCost++;
        manaCost = defaultManaCost;
    }
}
