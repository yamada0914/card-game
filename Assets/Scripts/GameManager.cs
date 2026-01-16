using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform playerHandTransform, playerFieldTransform, enemyHandTransform, enemyFieldTransform;
    [SerializeField] CardController cardPrefab;

    bool isPlayerTurn;

    //デッキの生成
    List<int> playerDeck = new List<int>() { 3, 1, 1, 2, 2};
    List<int> enemyDeck = new List<int>() {3, 2, 1, 2, 1};

    [SerializeField] Text playerHeroHpText;
    [SerializeField] Text enemyHeroHpText;

    const int INITIAL_HERO_HP = 30;

    int playerHeroHp;
    int enemyHeroHp;

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
        playerHeroHp = INITIAL_HERO_HP;
        enemyHeroHp = INITIAL_HERO_HP;
        ShowHeroHp();
        SettingInitHand();
        isPlayerTurn = true;
        TurnCalc();
    }

    void SettingInitHand()
    {
        for (int i = 0; i < 3; i++)
        {
            GiveCardToHand(playerDeck, playerHandTransform);
            GiveCardToHand(enemyDeck, enemyHandTransform);
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
        card.Init(cardID);
    }

    void TurnCalc()
    {
        if (isPlayerTurn)
        {
            PlayerTurn();
        }
        else
        {
            EnemyTurn();
        }
    }

    public void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        if (isPlayerTurn)
        {
            GiveCardToHand(playerDeck, playerHandTransform);
        }
        else
        {
            GiveCardToHand(enemyDeck, enemyHandTransform);
        }
        TurnCalc();
    }

    void PlayerTurn()
    {
        Debug.Log("PlayerTurn");
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        foreach (CardController card in playerFieldCardList)
        {
            card.SetCanAttack(true);
        }
    }
    void EnemyTurn()
    {
        Debug.Log("EnemyTurn");

        CardController[] enemyFieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        foreach (CardController card in enemyFieldCardList)
        {
            card.SetCanAttack(true);
        }

        // 場にカードを出す
        CardController[] handCardList = enemyHandTransform.GetComponentsInChildren<CardController>();
        CardController enemyCard = handCardList[0];
        enemyCard.movement.SetCardTransform(enemyFieldTransform);

        // 攻撃
        // フィールのカードリストを取得
        CardController[] filedCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        CardController[] enemyCanAttackCardList = Array.FindAll(filedCardList, card => card.model.canAttack);
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();

        if (enemyCanAttackCardList.Length > 0)
        {
            // attacker 選択
            CardController attacker = enemyCanAttackCardList[0];
            if(playerFieldCardList.Length > 0)
            {
                // defender 選択
                CardController defender = playerFieldCardList[0];
                // 戦闘開始
                CardsBattle(attacker, defender);
            }
            else
            {
                AttackToHero(attacker, false);
            }
        }

        ChangeTurn();
    }

    public void CardsBattle(CardController attacker, CardController defender)
    {
        attacker.Attack(defender);
        defender.Attack(attacker);
        attacker.CheckIsAlive();
        defender.CheckIsAlive();
    }

    void ShowHeroHp()
    {
        playerHeroHpText.text = playerHeroHp.ToString();
        enemyHeroHpText.text = enemyHeroHp.ToString();
    }
    public void AttackToHero(CardController attacker, bool isPlayerCard)
    {
        if (isPlayerCard)
        {
            enemyHeroHp -= attacker.model.at;
        }
        else
        {
            playerHeroHp -= attacker.model.at;
        }
        attacker.SetCanAttack(false);
        ShowHeroHp();
    }
}