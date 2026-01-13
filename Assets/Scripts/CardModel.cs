using UnityEngine;

// カードデータそのものとその処理
public class CardModel
{
    public string cardName;
    public int hp;
    public int at;
    public int cost;
    public Sprite icon;

    public CardModel(int cardID)
    {
        CardEntity cardEntity = Resources.Load<CardEntity>("CardListEntity/Card"+cardID);
        cardName = cardEntity.name;
        hp = cardEntity.hp;
        at = cardEntity.at;
        cost = cardEntity.cost;
        icon = cardEntity.icon;
    }
}