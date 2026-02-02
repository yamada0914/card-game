using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

// 攻撃される側
public class AttackedCard : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // attacker 選択
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();

        // defender 選択
        CardController defender = GetComponent<CardController>();

        if (attacker == null || defender == null)
        {
            return;
        }

        if (attacker.model.isPlayerCard == defender.model.isPlayerCard)
        {
            return;
        }

        // 敵フィールドにシールドがあればシールドキャラしか攻撃できない
        CardController[] enemyFieldCards = GameManager.instance.GetEnemyFieldCards();
        if (Array.Exists(enemyFieldCards, card => card.model.ability == ABILITY.SHIELD) && defender.model.ability != ABILITY.SHIELD)
        {
            return;
        }

        if (attacker.model.canAttack)
        {
            GameManager.instance.CardsBattle(attacker, defender);
        }
    }
}
