using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform playerHandTransform, enemyHandTransform;
    [SerializeField] CardController cardPrefab;

    bool isPlayerTurn;

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
        TurnCalc();
    }

    void PlayerTurn()
    {
        Debug.Log("PlayerTurn");
    }
    void EnemyTurn()
    {
        Debug.Log("EnemyTurn");
        ChangeTurn();
    }

    void CreateCard(Transform hand)
    {
        // カードの生成とデータの受け渡し
        CardController card = Instantiate(cardPrefab, hand, false);
        card.Init(1);
    }

}