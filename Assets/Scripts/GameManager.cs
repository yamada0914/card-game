using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject resultPanel;
    [SerializeField] Text resultText;
    [SerializeField] Transform playerHandTransform, playerFieldTransform, enemyHandTransform, enemyFieldTransform;
    [SerializeField] CardController cardPrefab;

    bool isPlayerTurn;

    //デッキの生成
    List<int> playerDeck = new List<int>() { 3, 1, 2, 2, 2};
    List<int> enemyDeck = new List<int>() {3, 2, 1, 2, 1};

    [SerializeField] Text playerHeroHpText;
    [SerializeField] Text enemyHeroHpText;

    const int INITIAL_HERO_HP = 2;
    const int INITIAL_MANA_COST = 1;
    const int TIME_LIMIT = 8;

    int playerHeroHp;
    int enemyHeroHp;

    [SerializeField] Text playerManaCostText;
    [SerializeField] Text enemyManaCostText;
    public int playerManaCost;
    int enemyManaCost;

    int playerDefaultManaCost;
    int enemyDefaultManaCost;

    [SerializeField] Text timeCountText;
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
        resultPanel.SetActive(false);
        playerHeroHp = INITIAL_HERO_HP;
        enemyHeroHp = INITIAL_HERO_HP;
        playerManaCost = INITIAL_MANA_COST;
        enemyManaCost = INITIAL_MANA_COST;
        playerDefaultManaCost = INITIAL_MANA_COST;
        enemyDefaultManaCost = INITIAL_MANA_COST;
        ShowHeroHp();
        ShowManaCost();
        SettingInitHand();
        isPlayerTurn = true;
        TurnCalc();
    }
    void ShowManaCost()
    {
        playerManaCostText.text = playerManaCost.ToString();
        enemyManaCostText.text = enemyManaCost.ToString();
    }
    public void ReduceManaCost(int cost, bool isPlayerCard)
    {
        if (isPlayerCard)
        {
            playerManaCost -= cost;
        }
        else
        {
            enemyManaCost -= cost;
        }
        ShowManaCost();
    }

    public void RestartGame()
    {
        ClearTransformChildren(playerHandTransform, enemyHandTransform, playerFieldTransform, enemyFieldTransform);
        playerDeck = new List<int>() { 3, 1, 1, 2, 2};
        enemyDeck = new List<int>() {3, 2, 1, 2, 1};
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
        StopAllCoroutines();
        StartCoroutine(CountDown());
        if (isPlayerTurn)
        {
            PlayerTurn();
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator CountDown()
    {
        timeCount = TIME_LIMIT;
        timeCountText.text = timeCount.ToString();
        while (timeCount > 0)
        {
            yield return new WaitForSeconds(1); // 1 秒待機s
            timeCount--;
            timeCountText.text = timeCount.ToString();
        }
        ChangeTurn();
    }

    public void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        if (isPlayerTurn)
        {
            playerDefaultManaCost++;
            playerManaCost = playerDefaultManaCost;
            GiveCardToHand(playerDeck, playerHandTransform);
        }
        else
        {
            enemyDefaultManaCost++;
            enemyManaCost = enemyDefaultManaCost;
            GiveCardToHand(enemyDeck, enemyHandTransform);
        }
        ShowManaCost();
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
    IEnumerator EnemyTurn()
    {
        Debug.Log("EnemyTurn");

        CardController[] enemyFieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        foreach (CardController card in enemyFieldCardList)
        {
            card.SetCanAttack(true);
        }

        yield return new WaitForSeconds(1);

        // 場にカードを出す
        CardController[] handCardList = enemyHandTransform.GetComponentsInChildren<CardController>();
        CardController[] selectableHandCardList = Array.FindAll(handCardList, card => card.model.cost <= enemyManaCost);
        if (selectableHandCardList.Length > 0)
        {
            CardController enemyCard = selectableHandCardList[0];
            enemyCard.movement.SetCardTransform(enemyFieldTransform);
            ReduceManaCost(enemyCard.model.cost, false);
            enemyCard.model.isFeildCard = true;
        }

        yield return new WaitForSeconds(1);

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

        yield return new WaitForSeconds(1);
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
        CheckHeroHp();
    }
    void CheckHeroHp()
    {
        if (playerHeroHp <= 0 || enemyHeroHp <= 0)
        {
            resultPanel.SetActive(true);
            if (playerHeroHp <= 0)
            {
                resultText.text = "Lose";
            }
            else
            {
                resultText.text = "Win";
            }
        }
    }
}