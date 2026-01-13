using UnityEngine;

public class CardController : MonoBehaviour
{
    CardViews views; // 見かけ(view)に関することを操作
    CardModel model; // データ(model)に関することを操作

    public void Awake()
    {
        views = GetComponent<CardViews>();
    }

    public void Init(int cardID)
    {
        model = new CardModel(cardID);
        views.Show(model);
    }
}