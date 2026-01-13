using UnityEngine;
using UnityEngine.UI;

public class CardViews : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text hpText;
    [SerializeField] Text atText;
    [SerializeField] Text costText;
    [SerializeField] Image iconImage;

    public void Show(CardModel cardModel)
    {
        nameText.text = cardModel.cardName;
        hpText.text = cardModel.hp.ToString();
        atText.text = cardModel.at.ToString();
        costText.text = cardModel.cost.ToString();
        iconImage.sprite = cardModel.icon;
    }
}
