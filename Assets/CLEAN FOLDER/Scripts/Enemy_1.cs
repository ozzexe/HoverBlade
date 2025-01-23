using UnityEngine;

public class Enemy_1 : EnemyBase
{
    protected override void Attack()
    {
        agent.SetDestination(transform.position);

        Vector3 lookDirection = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookDirection);

        if (Vector3.Distance(transform.position, player.position) > enemyDataSO.attackRange)
        {
            currentState = EnemyState.Chase;
            animator.SetFloat("Speed", enemyDataSO.chaseSpeed);
            animator.ResetTrigger("Attack");
        }
    }
}
