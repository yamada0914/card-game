using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

// 攻撃される側
public class SpellDropManager : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        CardController spellCard = eventData.pointerDrag.GetComponent<CardController>();
        CardController target = GetComponent<CardController>();

        if (spellCard == null)
        {
            return;
        }
        spellCard.UseSpellTo(target);
    }
}
