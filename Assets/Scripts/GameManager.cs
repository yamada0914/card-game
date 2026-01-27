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

    public bool isPlayerTurn;

    //デッキの生成
    List<int> playerDeck = new List<int>() { 3, 1, 2, 2, 2};
    List<int> enemyDeck = new List<int>() {3, 2, 1, 2, 1};

    [SerializeField] Text playerHeroHpText;
    [SerializeField] Text enemyHeroHpText;

    const int INITIAL_HERO_HP = 2;
    const int INITIAL_MANA_COST = 10;
    const int TIME_LIMIT = 8;

    int playerHeroHp;
    int enemyHeroHp;

    [SerializeField] Transform playerHero;

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

    public void OnClickTurnEndButton()
    {
        if (isPlayerTurn)
        {
            ChangeTurn();
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

    void SettingCanAttackView(CardController[] fieldCardList, bool canAttack)
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

    IEnumerator EnemyTurn()
    {
        Debug.Log("Enemyのターン");
        // フィールドのカードを攻撃可能にする
        CardController[] enemyFieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(enemyFieldCardList, true);

        yield return new WaitForSeconds(1);

        /* 場にカードをだす */
        // 手札のカードリストを取得
        CardController[] handCardList = enemyHandTransform.GetComponentsInChildren<CardController>();

        // コスト以下のカードがあれば、カードをフィールドに出し続ける
        while (Array.Exists(handCardList, card => card.model.cost <= enemyManaCost))
        {
            // コスト以下のカードリストを取得
            CardController[] selectableHandCardList = Array.FindAll(handCardList, card => card.model.cost <= enemyManaCost);
            // 場に出すカードを選択
            CardController enemyCard = selectableHandCardList[0];
            // カードを移動
            StartCoroutine(enemyCard.movement.MoveToField(enemyFieldTransform));
            ReduceManaCost(enemyCard.model.cost, false);
            enemyCard.model.isFieldCard = true;
            handCardList = enemyHandTransform.GetComponentsInChildren<CardController>();
            yield return new WaitForSeconds(1);
        }



        yield return new WaitForSeconds(1);

        /* 攻撃 */
        // フィールドのカードリストを取得
        CardController[] fieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        //攻撃可能カードがあれば攻撃を繰り返す
        while (Array.Exists(fieldCardList, card => card.model.canAttack))
        {
            // 攻撃可能カードを取得
            CardController[] enemyCanAttackCardList = Array.FindAll(fieldCardList, card => card.model.canAttack); // 検索：Array.FindAll
            CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();

            // attacker カードを選択
            CardController attacker = enemyCanAttackCardList[0];

            if (playerFieldCardList.Length > 0)
            {
                // defender カードを選択
                CardController defender = playerFieldCardList[0];
                // attacker と defender を戦わせる
                StartCoroutine(attacker.movement.MoveToTarget(defender.transform));
                yield return new WaitForSeconds(0.25f);
                CardsBattle(attacker, defender);
            }
            else
            {
                StartCoroutine(attacker.movement.MoveToTarget(playerHero));
                yield return new WaitForSeconds(0.25f);
                AttackToHero(attacker, false);
                yield return new WaitForSeconds(0.25f);
                CheckHeroHp();
            }
            fieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
            yield return new WaitForSeconds(1);
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
    }
    void CheckHeroHp()
    {
        if (playerHeroHp <= 0 || enemyHeroHp <= 0)
        {
            ShowResultPanel(playerHeroHp);
        }
    }
    void ShowResultPanel(int heroHp)
    {
        StopAllCoroutines();
        resultPanel.SetActive(true);
        if (heroHp <= 0)
        {
            resultText.text = "Lose";
        }
        else
        {
            resultText.text = "Win";
        }
    }
}