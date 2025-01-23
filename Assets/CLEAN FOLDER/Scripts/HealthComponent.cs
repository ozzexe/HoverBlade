using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private int health = 100;
    private Animator animator;

    public int Health { get; set; }

    private void Start()
    {
        Health = health;
        animator = GetComponent<Animator>(); // Animator'ý alýyoruz
    }

    public void TakeDamage(int damageAmount)
    {
        Health = Mathf.Max(0, Health - damageAmount);
        Debug.Log($"Health -> {Health}");

        // Hasar alýndýðýnda animasyonu tetikle
        if (animator != null)
        {
            animator.SetTrigger("TakeDamage"); // "TakeDamage" trigger'ýný tetikle
        }

        if (Health == 0)
        {
            StartCoroutine(Die()); // Ölüm anýnda coroutine baþlat
        }
    }

    private IEnumerator Die()
    {
        if (animator != null)
        {
            DisableMovement();

            animator.SetTrigger("Die"); // "Die" trigger'ýný tetikle

            // Ölüm animasyonunun bitmesini bekle
            yield return new WaitForSeconds(3);
        }

        Destroy(gameObject);

        
    }

    private void DisableMovement()
    {
        // Yalnýzca hareket bileþenlerini devre dýþý býrak
        var navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }

        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = true; // Rigidbody'yi durdur
        }

    }
}
