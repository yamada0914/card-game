using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
// 攻撃される側
public class AttackedHero : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // attacker 選択
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();
        if (attacker == null)
        {
            return;
        }

        if (attacker.model.canAttack)
        {
            GameManager.instance.AttackToHero(attacker, true);
        }
        else
        {
            Debug.Log("攻撃できません");
        }
    }
}
