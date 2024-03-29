﻿using UnityEngine;

/**
 * Spravanie pre ukonecnie stavu v Animator ovladaci,
 * po skonceni animacie smrti vojaka sa vymaze objekt 
 * vojaka a aktualizuje pocty vojakov v GameManager
 * scripte a skontroluje, ci hrac vyhral alebo prehral.
 */

public class DeathAnimatorBehaviour : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Death Soldier ");

        var soldierObject = animator.transform.parent.gameObject;

        if (soldierObject.tag == "EnemySoldier")
        {
            int currentCount = GameManager.Instance.GetEnemySoldiersCount();
            var weapon = soldierObject.transform.Find("Weapon");
            if (weapon)
            {
                GameManager.Instance.SetEnemySoldiersCount(currentCount - 1);
            }
            Debug.Log("Enemy soldiers count " + GameManager.Instance.GetEnemySoldiersCount());

            GameManager.Instance.CheckWinner();
        }
        else if (soldierObject.tag == "Vojak")
        {
            int currentCount = GameManager.Instance.GetPlayerSoldiersCount();
            var weapon = soldierObject.transform.Find("Weapon");
            if (weapon)
            {
                GameManager.Instance.SetPlayerSoldiersCount(currentCount - 1);
            }
            Debug.Log("Player soldiers count " + GameManager.Instance.GetPlayerSoldiersCount());

            GameManager.Instance.CheckWinner();
        }
        Destroy(soldierObject);

    }
}
