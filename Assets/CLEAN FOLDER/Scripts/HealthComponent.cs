using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private int health = 100;

    public int Health { get; set; }

    private void Start()
    {
        Health = health;
    }


    public void TakeDamage(int damageAmount)
    {
        Health = Mathf.Max(0, Health - damageAmount);
        Debug.Log($"Health -> {Health}");
        if (Health == 0)
        {
            Destroy(gameObject);
        }
    }
}
