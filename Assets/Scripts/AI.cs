using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AI : MonoBehaviour
{
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.instance;
    }
    public IEnumerator EnemyTurn()
    {
        Debug.Log("Enemyのターン");
        // フィールドのカードを攻撃可能にする
        CardController[] enemyFieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
        gameManager.SettingCanAttackView(enemyFieldCardList, true);

        yield return new WaitForSeconds(1);

        /* 場にカードをだす */
        // 手札のカードリストを取得
        CardController[] handCardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();

        // コスト以下のカードがあれば、カードをフィールドに出し続ける
        // 条件：モンスター
        while (Array.Exists(handCardList, card =>
            card.model.cost <= gameManager.enemy.manaCost
            && (!card.IsSpell || (card.IsSpell && card.CanUseSpell()))))
        {
            // コスト以下のカードリストを取得
            CardController[] selectableHandCardList = Array.FindAll(handCardList, card =>
                card.model.cost <= gameManager.enemy.manaCost
                && (!card.IsSpell || (card.IsSpell && card.CanUseSpell())));
            // 場に出すカードを選択
            CardController selectedCard = selectableHandCardList[0];
            // スペルカードなら使用する
            if (selectedCard.IsSpell)
            {
                CastSpellOf(selectedCard);
            }
            else
            {
                // カードを移動
                StartCoroutine(selectedCard.movement.MoveToField(gameManager.enemyFieldTransform));
                selectedCard.OnField();
            }
            yield return new WaitForSeconds(1);
            handCardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();
        }



        yield return new WaitForSeconds(1);

        /* 攻撃 */
        // フィールドのカードリストを取得
        CardController[] fieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
        //攻撃可能カードがあれば攻撃を繰り返す
        while (Array.Exists(fieldCardList, card => card.model.canAttack))
        {
            // 攻撃可能カードを取得
            CardController[] enemyCanAttackCardList = Array.FindAll(fieldCardList, card => card.model.canAttack); // 検索：Array.FindAll
            CardController[] playerFieldCardList = gameManager.playerFieldTransform.GetComponentsInChildren<CardController>();

            // attacker カードを選択
            CardController attacker = enemyCanAttackCardList[0];

            if (playerFieldCardList.Length > 0)
            {
                // defender カードを選択
                // シールドカードのみ攻撃対象にする
                if (Array.Exists(playerFieldCardList, card => card.model.ability == ABILITY.SHIELD))
                {
                    playerFieldCardList = Array.FindAll(playerFieldCardList, card => card.model.ability == ABILITY.SHIELD);
                }

                CardController defender = playerFieldCardList[0];
                // attacker と defender を戦わせる
                StartCoroutine(attacker.movement.MoveToTarget(defender.transform));
                yield return new WaitForSeconds(0.51f);
                gameManager.CardsBattle(attacker, defender);

            }
            else
            {
                StartCoroutine(attacker.movement.MoveToTarget(gameManager.playerHero));
                yield return new WaitForSeconds(0.25f);
                gameManager.AttackToHero(attacker);
                yield return new WaitForSeconds(0.25f);
                gameManager.CheckHeroHp();
            }
            fieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        gameManager.ChangeTurn();
    }

    void CastSpellOf(CardController card)
    {
        CardController target = null;
        if (card.model.spell == SPELL.HEAL_FRIEND_CARD)
        {
            CardController[] friendCards = gameManager.GetFieldCards(card.model.isPlayerCard);
            if (friendCards.Length == 0)
            {
                return;
            }
            target = friendCards[0];
        }
        else if (card.model.spell == SPELL.DAMAGE_ENEMY_CARD)
        {
            CardController[] enemyCards = gameManager.GetFieldCards(!card.model.isPlayerCard);
            if (enemyCards.Length == 0)
            {
                return;
            }
            target = enemyCards[0];
        }
        card.UseSpellTo(target);
    }

}