using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
// 攻撃される側
public class AttackedCard : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();

        // defender 選択
        CardController defender = GetComponent<CardController>();

        if (attacker == null || defender == null)
        {
            return;
        }

        if (attacker.model.canAttack)
        {
            GameManager.instance.CardsBattle(attacker, defender);
        }
        else
        {
            Debug.Log("攻撃できません");
        }
    }
}
