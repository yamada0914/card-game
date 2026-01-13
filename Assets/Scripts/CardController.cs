using UnityEngine;

public class CardController : MonoBehaviour
{
    CardViews views;        // 見かけ(view)に関することを操作
    CardModel model;        // データ(model)に関することを操作
    public CardMovement movement;  // カードの移動に関することを操作

    public void Awake()
    {
        views = GetComponent<CardViews>();
        movement = GetComponent<CardMovement>();
    }

    public void Init(int cardID)
    {
        model = new CardModel(cardID);
        views.Show(model);
    }
}