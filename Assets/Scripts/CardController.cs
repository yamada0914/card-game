using UnityEngine;

public class CardController : MonoBehaviour
{
    CardViews views;        // 見かけ(view)に関することを操作
    public CardModel model;        // データ(model)に関することを操作
    public CardMovement movement;  // カードの移動に関することを操作

    GameManager gameManager;
    public bool IsSpell
    {
        get { return model.spell != SPELL.NONE; }
    }
    public void Awake()
    {
        gameManager = GameManager.instance;
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

    public void Heal(CardController friendCard)
    {
        model.Heal(friendCard);
        friendCard.RefreshView();
    }

    public void RefreshView()
    {
        views.Refresh(model);
    }
    public void SetCanAttack(bool canAttack)
    {
        model.canAttack = canAttack;
        views.SetActivateSelectablePanel(canAttack);
    }

    public void OnField()
    {
        gameManager.ReduceManaCost(model.cost, model.isPlayerCard);
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
            RefreshView();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CanUseSpell()
    {
        switch (model.spell)
        {
            case SPELL.DAMAGE_ENEMY_CARD:
            case SPELL.DAMAGE_ENEMY_CARDS:
                CardController[] enemyCards = gameManager.GetFieldCards(!model.isPlayerCard);
                if (enemyCards.Length == 0)
                {
                    return false;
                }
                return true;
            case SPELL.DAMAGE_ENEMY_HERO:
            case SPELL.HEAL_FRIEND_HERO:
                return true;

            case SPELL.HEAL_FRIEND_CARD:
            case SPELL.HEAL_FRIEND_CARDS:
                CardController[] friendCards = gameManager.GetFieldCards(model.isPlayerCard);
                if (friendCards.Length == 0)
                {
                    return false;
                }
                return true;
            case SPELL.NONE:
                return false;
        }
        return false;
    }

    public void UseSpellTo(CardController target)
    {
        switch (model.spell)
        {
            // 敵カード 1 体に攻撃
            case SPELL.DAMAGE_ENEMY_CARD:
                if (target == null)
                {
                    return;
                }
                if (target.model.isPlayerCard == model.isPlayerCard)
                {
                    return;
                }
                Attack(target);
                target.CheckIsAlive();
                break;
            // 敵カード全員に攻撃
            case SPELL.DAMAGE_ENEMY_CARDS:
                CardController[] enemyCards = gameManager.GetFieldCards(!this.model.isPlayerCard);
                foreach (CardController enemyCard in enemyCards)
                {
                    Attack(enemyCard);
                }
                foreach (CardController enemyCard in enemyCards)
                {
                    enemyCard.CheckIsAlive();
                }
                break;
            case SPELL.DAMAGE_ENEMY_HERO:
                gameManager.AttackToHero(this);
                gameManager.CheckHeroHp();
                break;
            case SPELL.HEAL_FRIEND_CARD:
                if (target == null)
                {
                    return;
                }
                if (target.model.isPlayerCard != model.isPlayerCard)
                {
                    return;
                }
                Heal(target);
                break;
            case SPELL.HEAL_FRIEND_CARDS:
                CardController[] friendCards = gameManager.GetFieldCards(this.model.isPlayerCard);
                foreach (CardController friendCard in friendCards)
                {
                    Heal(friendCard);
                }
                break;
            case SPELL.HEAL_FRIEND_HERO:
                gameManager.HealToHero(this);
                break;
            case SPELL.NONE:
                return;
        }
        gameManager.ReduceManaCost(model.cost, model.isPlayerCard);
        Destroy(this.gameObject);
    }
}