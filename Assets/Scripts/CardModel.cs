using UnityEngine;

// カードデータそのものとその処理
public class CardModel
{
    public string cardName;
    public int hp;
    public int at;
    public int cost;
    public Sprite icon;
    public ABILITY ability;

    public bool isAlive;
    public bool canAttack;
    public bool isFieldCard;
    public bool isPlayerCard;

    public CardModel(int cardID, bool isPlayer)
    {
        CardEntity cardEntity = Resources.Load<CardEntity>("CardListEntity/Card"+cardID);
        cardName = cardEntity.name;
        hp = cardEntity.hp;
        at = cardEntity.at;
        cost = cardEntity.cost;
        icon = cardEntity.icon;
        ability = cardEntity.ability;
        isAlive = true;
        isPlayerCard = isPlayer;
    }
    void Damage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            hp = 0;
            isAlive = false;
        }
    }

    public void Attack(CardController card)
    {
        card.model.Damage(at);
    }
}