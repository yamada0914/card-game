using UnityEngine;

public class CardController : MonoBehaviour
{
    CardViews views;        // 見かけ(view)に関することを操作
    public CardModel model;        // データ(model)に関することを操作
    public CardMovement movement;  // カードの移動に関することを操作

    public void Awake()
    {
        views = GetComponent<CardViews>();
        movement = GetComponent<CardMovement>();
    }

    public void Init(int cardID, bool isPlayerCard)
    {
        model = new CardModel(cardID, isPlayerCard);
        views.Show(model);
    }

    public void Attack(CardController enemyCard)
    {
        model.Attack(enemyCard);
        SetCanAttack(false);
    }

    public void SetCanAttack(bool canAttack)
    {
        model.canAttack = canAttack;
        views.SetActivateSelectablePanel(canAttack);
    }

    public void OnField(bool isPlayer)
    {
        GameManager.instance.ReduceManaCost(model.cost, isPlayer);
        model.isFieldCard = true;
        if (model.ability == ABILITY.INIT_ATTACKABLE)
        {
            SetCanAttack(true);
        }
    }
    public void CheckIsAlive()
    {
        if (model.isAlive)
        {
            views.Refresh(model);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}