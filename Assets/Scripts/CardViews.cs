using UnityEngine;
using UnityEngine.UI;

public class CardViews : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text hpText;
    [SerializeField] Text atText;
    [SerializeField] Text costText;
    [SerializeField] Image iconImage;
    [SerializeField] GameObject selectableObject;
    [SerializeField] GameObject shieldPanel;

    public void Show(CardModel cardModel)
    {
        nameText.text = cardModel.cardName;
        hpText.text = cardModel.hp.ToString();
        atText.text = cardModel.at.ToString();
        costText.text = cardModel.cost.ToString();
        iconImage.sprite = cardModel.icon;
        if (cardModel.ability == ABILITY.SHIELD)
        {
            shieldPanel.SetActive(true);
        }
        else
        {
            shieldPanel.SetActive(false);
        }
    }
    public void Refresh(CardModel cardModel)
    {
        hpText.text = cardModel.hp.ToString();
        atText.text = cardModel.at.ToString();
    }
    public void SetActivateSelectablePanel(bool flag)
    {
        selectableObject.SetActive(flag);
    }
}
