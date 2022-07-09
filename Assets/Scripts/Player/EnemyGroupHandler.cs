using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupHandler : MonoBehaviour
{
    public EnemyBase[] enemies;
    public EnemyInfo[] allEnemies;
    private List<int> enemyIndexes;

    public int aliveEnemyCount;

    public EnemyBase attackingEnemy;
    private Coroutine AI_Loop_Coroutine;
    // Start is called before the first frame update
    void Start()
    {
        allEnemies = new EnemyInfo[enemies.Length];

        for (int i = 0; i < allEnemies.Length; i++)
        {
            allEnemies[i].enemyScript = enemies[i];
            allEnemies[i].enemyAvailable = true;
        }

    }

    public void StartAI()
    {
        AI_Loop_Coroutine = StartCoroutine(AI_Loop(null));
    }


    IEnumerator AI_Loop(EnemyBase enemy)
    {
        if (AliveEnemyCount() == 0)
        {
            StopCoroutine(AI_Loop(null));
            yield break;
        }

        yield return new WaitForSeconds(Random.Range(.5f, 1.5f));

        attackingEnemy = RandomEnemyExcludingOne(enemy);

        if (attackingEnemy == null)
            attackingEnemy = RandomEnemy();

        if (attackingEnemy == null)
            yield break;

        yield return new WaitUntil(() => attackingEnemy.IsRetreating == false);
        yield return new WaitUntil(() => attackingEnemy.IsLockedTarget == false);
        yield return new WaitUntil(() => attackingEnemy.IsStunned == false );

        if (attackingEnemy == null)
            attackingEnemy = RandomEnemy();

        attackingEnemy.SetAttack();

        yield return new WaitUntil(() => attackingEnemy.IsPreparingAttack == false);

        attackingEnemy.SetRetreat();

        yield return new WaitForSeconds(Random.Range(0, .5f));
        if(attackingEnemy!=null)
        {
            attackingEnemy.IsRetreating = false;
            attackingEnemy.IsLockedTarget = false;
            attackingEnemy.IsWaiting = true;
        }
        
        if (AliveEnemyCount() > 0)
            AI_Loop_Coroutine = StartCoroutine(AI_Loop(attackingEnemy));
    }
    public EnemyBase RandomEnemy()
    {
        enemyIndexes = new List<int>();

        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyAvailable)
                enemyIndexes.Add(i);
        }

        if (enemyIndexes.Count == 0)
            return null;

        EnemyBase randomEnemy;
        int randomIndex = Random.Range(0, enemyIndexes.Count);
        randomEnemy = allEnemies[enemyIndexes[randomIndex]].enemyScript;
        randomEnemy.IsRetreating = false;
        randomEnemy.IsLockedTarget = false;
        randomEnemy.IsWaiting = true;
        return randomEnemy;
    }
    public EnemyBase RandomEnemyExcludingOne(EnemyBase exclude)
    {
        enemyIndexes = new List<int>();

        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyAvailable && allEnemies[i].enemyScript != exclude)
                enemyIndexes.Add(i);
        }

        if (enemyIndexes.Count == 0)
            return null;

        EnemyBase randomEnemy;
        int randomIndex = Random.Range(0, enemyIndexes.Count);
        randomEnemy = allEnemies[enemyIndexes[randomIndex]].enemyScript;
        randomEnemy.IsRetreating = false;
        randomEnemy.IsLockedTarget = false;
        randomEnemy.IsWaiting = true;
        return randomEnemy;
    }

    public int AvailableEnemyCount()
    {
        int count = 0;
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyAvailable)
                count++;
        }
        return count;
    }

    public bool AnEnemyIsPreparingAttack()
    {
        foreach (EnemyInfo enemyStruct in allEnemies)
        {
            if (enemyStruct.enemyScript.IsPreparingAttack)
            {
                return true;
            }
        }
        return false;
    }


    public int AliveEnemyCount()
    {
        int count = 0;
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyScript.isActiveAndEnabled)
                count++;
        }
        aliveEnemyCount = count;
        return count;
    }

    public void SetEnemyAvailiability(EnemyBase enemy, bool state)
    {
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyScript == enemy)
                allEnemies[i].enemyAvailable = state;
        }

        if (FindObjectOfType<EnemyDetector>().CurrentTarget() == enemy)
        {
            FindObjectOfType<EnemyDetector>().SetCurrentTarget(null);
        
        }

        if(attackingEnemy == enemy)
        {
            attackingEnemy = null;
            StopCoroutine(AI_Loop(null));
            if (AliveEnemyCount() > 0)
                AI_Loop_Coroutine = StartCoroutine(AI_Loop(attackingEnemy));
        }
    }
}

[System.Serializable]
public struct EnemyInfo
{
    public EnemyBase enemyScript;
    public bool enemyAvailable;
}