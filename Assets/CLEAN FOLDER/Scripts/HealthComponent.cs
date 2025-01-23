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
        animator = GetComponent<Animator>(); // Animator'� al�yoruz
    }

    public void TakeDamage(int damageAmount)
    {
        Health = Mathf.Max(0, Health - damageAmount);
        Debug.Log($"Health -> {Health}");

        // Hasar al�nd���nda animasyonu tetikle
        if (animator != null)
        {
            animator.SetTrigger("TakeDamage"); // "TakeDamage" trigger'�n� tetikle
        }

        if (Health == 0)
        {
            StartCoroutine(Die()); // �l�m an�nda coroutine ba�lat
        }
    }

    private IEnumerator Die()
    {
        if (animator != null)
        {
            DisableMovement();

            animator.SetTrigger("Die"); // "Die" trigger'�n� tetikle

            // �l�m animasyonunun bitmesini bekle
            yield return new WaitForSeconds(3);
        }

        Destroy(gameObject);

        
    }

    private void DisableMovement()
    {
        // Yaln�zca hareket bile�enlerini devre d��� b�rak
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
