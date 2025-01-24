using UnityEngine;
public class SwordDamage : MonoBehaviour
{
    public int damageAmount = 50; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // 
        {
            HealthComponent enemyHealth = other.GetComponent<HealthComponent>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
            }
        }
    }
}
