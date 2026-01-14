using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform playerHandTransform, playerFieldTransform, enemyHandTransform, enemyFieldTransform;
    [SerializeField] CardController cardPrefab;

    bool isPlayerTurn;

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
        SettingInitHand();
        isPlayerTurn = true;
        TurnCalc();
    }

    void SettingInitHand()
    {
        for (int i = 0; i < 3; i++)
        {
            CreateCard(playerHandTransform);
            CreateCard(enemyHandTransform);
        }
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
            CreateCard(playerHandTransform);
        }
        else
        {
            CreateCard(enemyHandTransform);
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

        // 場にカードを出す
        CardController[] handCardList = enemyHandTransform.GetComponentsInChildren<CardController>();
        CardController enemyCard = handCardList[0];
        enemyCard.movement.SetCardTransform(enemyFieldTransform);

        // 攻撃
        // フィールのカードリストを取得
        CardController[] filedCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        CardController[] enemyCanAttackCardList = Array.FindAll(filedCardList, card => card.model.canAttack);
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();

        if (enemyCanAttackCardList.Length > 0 && playerFieldCardList.Length > 0)
        {
            // attacker 選択
            CardController attacker = enemyCanAttackCardList[0];
            // defender 選択
            CardController defender = playerFieldCardList[0];
            // 戦闘開始
            CardsBattle(attacker, defender);
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

    void CreateCard(Transform hand)
    {
        // カードの生成とデータの受け渡し
        CardController card = Instantiate(cardPrefab, hand, false);
        card.Init(1);
    }

}