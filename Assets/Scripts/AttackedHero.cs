using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

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

        // 敵フィールドにシールドがあれば攻撃できない
        CardController[] enemyFieldCards = GameManager.instance.GetEnemyFieldCards();
        if (Array.Exists(enemyFieldCards, card => card.model.ability == ABILITY.SHIELD))
        {
            return;
        }
        if (attacker.model.canAttack)
        {
            GameManager.instance.AttackToHero(attacker, true);
            GameManager.instance.CheckHeroHp();
        }
    }
}
