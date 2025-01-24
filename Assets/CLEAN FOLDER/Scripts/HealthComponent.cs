using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private int health = 100;
    private Animator animator;

    public int Health { get; set; }

    private void Start()
    {
        Health = health;
        animator = GetComponent<Animator>(); 
    }

    public void TakeDamage(int damageAmount)
    {
        Health = Mathf.Max(0, Health - damageAmount);
        Debug.Log($"Health -> {Health}");

        // Hasar alýndýðýnda animasyonu tetikle
        if (animator != null)
        {
            animator.SetTrigger("TakeDamage"); 
        }

        if (Health == 0)
        {
            StartCoroutine(Die()); 
        }
    }

    private IEnumerator Die()
    {
        if (animator != null)
        {
            DisableMovement();

            animator.SetTrigger("Die"); 

            
            yield return new WaitForSeconds(3);
        }

        if (CompareTag("Player")) 
        {
            SceneManager.LoadScene("Arayüz"); 
        }
        else
        {
            Destroy(gameObject); 
        }


    }

    private void DisableMovement()
    {
        
        var navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }

        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = true; 
        }

    }
}
