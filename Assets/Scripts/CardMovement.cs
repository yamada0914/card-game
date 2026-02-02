using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Transform defaultParent;

    public bool isDraggable;

    public void OnBeginDrag(PointerEventData eventData)
    {
        CardController card = GetComponent<CardController>();
        if (card.model.isPlayerCard && GameManager.instance.isPlayerTurn && !card.model.isFieldCard && card.model.cost <= GameManager.instance.playerManaCost)
        {
            isDraggable = true;
        }

        else if (card.model.isPlayerCard && GameManager.instance.isPlayerTurn && card.model.isFieldCard && card.model.canAttack)
        {
            isDraggable = true;
        }
        else
        {
            isDraggable = false;
        }

        if (!isDraggable)
        {
            return;
        }
        defaultParent = transform.parent;
        transform.SetParent(defaultParent.parent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable)
        {
            return;
        }
        transform.position = eventData.position;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable)
        {
            return;
        }
        transform.SetParent(defaultParent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public IEnumerator MoveToField(Transform field)
    {
        // 一度親を Canvas に変更する
        transform.SetParent(defaultParent.parent);
        // Dotween でカードをフィールドに移動
        transform.DOMove(field.position, 0.25f);
        yield return new WaitForSeconds(0.25f);

        defaultParent = field;
        transform.SetParent(defaultParent);
    }
    public IEnumerator MoveToTarget(Transform target)
    {
        // 現在の位置と並びを取得
        Vector3 currentPosition = transform.position;
        int siblingIndex = transform.GetSiblingIndex();

        // 一度親を Canvas に変更する
        transform.SetParent(defaultParent.parent);
        // Dotween でカードを対象に移動
        transform.DOMove(target.position, 0.25f);
        yield return new WaitForSeconds(0.25f);
        // 元の位置に戻す
        transform.position = currentPosition;
        transform.SetParent(defaultParent);
        transform.SetSiblingIndex(siblingIndex);
    }

    void Start()
    {
        defaultParent = transform.parent;
    }
}
