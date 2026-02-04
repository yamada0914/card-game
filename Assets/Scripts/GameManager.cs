using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GamePlayerManager player;
    public GamePlayerManager enemy;
    [SerializeField] AI enemyAI;
    [SerializeField] UIManager uiManager;
    public Transform playerHandTransform, playerFieldTransform, enemyHandTransform, enemyFieldTransform;
    [SerializeField] CardController cardPrefab;

    public bool isPlayerTurn;

    //デッキの生成

    const int INITIAL_HERO_HP = 10;
    const int INITIAL_MANA_COST = 10;
    const int TIME_LIMIT = 20;


    [SerializeField] public Transform playerHero;


    int timeCount;
    // シングルトン化
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        uiManager.HideResultPanel();
        player.Init(new List<int>() {3, 4, 2, 2, 2});
        enemy.Init(new List<int>() {3, 2, 2, 2, 1});

        uiManager.ShowHeroHp(player.heroHp, enemy.heroHp);
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
        SettingInitHand();
        isPlayerTurn = true;
        TurnCalc();
    }

    public void ReduceManaCost(int cost, bool isPlayerCard)
    {
        if (isPlayerCard)
        {
            player.manaCost -= cost;
        }
        else
        {
            enemy.manaCost -= cost;
        }
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
    }

    public void RestartGame()
    {
        ClearTransformChildren(playerHandTransform, enemyHandTransform, playerFieldTransform, enemyFieldTransform);
        player.deck = new List<int>() { 3, 1, 1, 2, 2};
        enemy.deck = new List<int>() {3, 2, 1, 2, 1};
        StartGame();
    }

    void ClearTransformChildren(params Transform[] transforms)
    {
        foreach (Transform parent in transforms)
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    void SettingInitHand()
    {
        for (int i = 0; i < 3; i++)
        {
            GiveCardToHand(player.deck, playerHandTransform);
            GiveCardToHand(enemy.deck, enemyHandTransform);
        }
    }

    void GiveCardToHand(List<int> deck, Transform hand)
    {
        if (deck.Count == 0)
        {
            return;
        }
        int cardID = deck[0];
        deck.RemoveAt(0);
        CreateCard(cardID, hand);
    }

    void CreateCard(int cardID, Transform hand)
    {
        // カードの生成とデータの受け渡し
        CardController card = Instantiate(cardPrefab, hand, false);
        if (hand.name == "PlayerHand")
        {
            card.Init(cardID, true);
        }
        else
        {
            card.Init(cardID, false);
        }
    }

    void TurnCalc()
    {
        StopAllCoroutines();
        StartCoroutine(CountDown());
        if (isPlayerTurn)
        {
            PlayerTurn();
        }
        else
        {
            StartCoroutine(enemyAI.EnemyTurn());
        }
    }

    IEnumerator CountDown()
    {
        timeCount = TIME_LIMIT;
        uiManager.UpdateTimeCount(timeCount);

        while (timeCount > 0)
        {
            yield return new WaitForSeconds(1); // 1 秒待機s
            timeCount--;
            uiManager.UpdateTimeCount(timeCount);
        }
        ChangeTurn();
    }

    public CardController[] GetPlayerFieldCards(bool isPlayer)
    {
        if (isPlayer)
        {
            return enemyFieldTransform.GetComponentsInChildren<CardController>();
        }
        else
        {
            return playerFieldTransform.GetComponentsInChildren<CardController>();
        }
    }

    public void OnClickTurnEndButton()
    {
        if (isPlayerTurn)
        {
            ChangeTurn();
        }
    }

    public CardController[] GetFieldCards(bool isPlayer)
    {
        if (isPlayer)
        {
            return playerFieldTransform.GetComponentsInChildren<CardController>();
        }
        else
        {
            return enemyFieldTransform.GetComponentsInChildren<CardController>();
        }
    }

    public void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;

        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        CardController[] enemyFieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(playerFieldCardList, false);
        SettingCanAttackView(enemyFieldCardList, false);

        if (isPlayerTurn)
        {
            player.IncreaseManaCost();
            GiveCardToHand(player.deck, playerHandTransform);
        }
        else
        {
            enemy.IncreaseManaCost();
            GiveCardToHand(enemy.deck, enemyHandTransform);
        }
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
        TurnCalc();
    }

    public void SettingCanAttackView(CardController[] fieldCardList, bool canAttack)
    {
        foreach (CardController card in fieldCardList)
        {
            card.SetCanAttack(canAttack);
        }
    }

    void PlayerTurn()
    {
        Debug.Log("PlayerTurn");
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(playerFieldCardList, true);
    }

    public void CardsBattle(CardController attacker, CardController defender)
    {
        attacker.Attack(defender);
        defender.Attack(attacker);
        attacker.CheckIsAlive();
        defender.CheckIsAlive();
    }

    public void AttackToHero(CardController attacker)
    {
        if (attacker.model.isPlayerCard)
        {
            enemy.heroHp -= attacker.model.at;
        }
        else
        {
            player.heroHp -= attacker.model.at;
        }
        uiManager.ShowHeroHp(player.heroHp, enemy.heroHp);
    }
    public void HealToHero(CardController healer)
    {
        if (healer.model.isPlayerCard)
        {
            player.heroHp += healer.model.at;
        }
        else
        {
            enemy.heroHp += healer.model.at;
        }
        healer.SetCanAttack(false);
        uiManager.ShowHeroHp(player.heroHp, enemy.heroHp);
    }
    public void CheckHeroHp()
    {
        if (player.heroHp <= 0 || enemy.heroHp <= 0)
        {
            uiManager.ShowResultPanel(player.heroHp);
        }
    }
    void ShowResultPanel(int heroHp)
    {
        StopAllCoroutines();
        uiManager.ShowResultPanel(heroHp);
    }
}