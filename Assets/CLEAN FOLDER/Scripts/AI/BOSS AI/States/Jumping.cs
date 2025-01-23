using BeykozEdu.FSM;
using UnityEngine;

public class Jumping : BaseState<EnemyAIData>
{
    private Rigidbody rb;
    private bool isJumping = false;
    private float jumpForce = 5f; // Zıplama kuvveti
    private LayerMask groundLayer; // Zemin tag'ini kontrol etmek için
    private int damageToPlayer = 20; // Oyuncuya verilecek hasar miktarı

    private static readonly int OnJumping = Animator.StringToHash("OnJumping");

    public override void OnEnter()
    {
        StateData.EnemyAnimator.SetTrigger(OnJumping);
        
        // StateData ve bileşenlerin kontrolü
        if (StateData == null)
        {
            Debug.LogError("StateData atanmamış!");
            return;
        }

        if (StateData.Enemy == null)
        {
            Debug.LogError("Enemy atanmamış!");
            return;
        }

        rb = StateData.Enemy.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Enemy'de Rigidbody bileşeni yok!");
            return;
        }
        
        Debug.Log("Jumping durumuna giriş yaptım.");

        // Ground layer kontrolü
        groundLayer = LayerMask.GetMask("Ground");

        // Zıplama başlat
        if (!isJumping)
        {
            isJumping = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public override void OnExit()
    {
        Debug.Log("Jumping durumundan çıkış yaptım.");
    }

    public override void OnUpdate()
    {
        

        // Zıplama hareketi başladıktan sonra yere indiğini kontrol et
        if (isJumping && IsGrounded())
        {
            isJumping = false; // Zıplama bitti
            Debug.Log("Yere indik!");

            // Oyuncuya hasar ver
            DealDamageToPlayer();

            // Rush durumuna geç
            StateMachineHandler.AddState(new Rush(), StateData);
        }
    }

    // Yere indiğini kontrol eden metot
    private bool IsGrounded()
    {
        // Enemy'nin altına bir raycast gönderiyoruz
        RaycastHit hit;
        if (Physics.Raycast(StateData.Enemy.position, Vector3.down, out hit, 2.1f, groundLayer))
        {
            return true;
        }
        return false;
    }

    // Oyuncuya hasar verme işlemi
    private void DealDamageToPlayer()
    {
        // Boss ile Player arasındaki mesafeyi kontrol et
        if (StateData.Player != null)
        {
            float distanceToPlayer = Vector3.Distance(StateData.Enemy.position, StateData.Player.position);

            if (distanceToPlayer <= 5f)
            {
                if (StateData.playerHealthComp != null && StateData.playerHealthComp.Health > 0)
                {
                    StateData.playerHealthComp.TakeDamage(damageToPlayer);
                    Debug.Log($"Oyuncuya {damageToPlayer} hasar verildi. Kalan can: {StateData.playerHealthComp.Health}");
                }
                else
                {
                    Debug.LogWarning("Player HealthComponent atanmamış veya can değeri 0.");
                }
            }
            else
            {
                Debug.Log($"Oyuncu çok uzakta! Mesafe: {distanceToPlayer}. Hasar verilmedi.");
            }
        }
        else
        {
            Debug.LogWarning("Player bileşeni null.");
        }
    }
}
