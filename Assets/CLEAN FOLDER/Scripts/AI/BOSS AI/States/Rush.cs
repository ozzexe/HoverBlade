using UnityEngine;
using BeykozEdu.FSM;

public class Rush : BaseState<EnemyAIData>
{
    private Transform[] waypoints;
    private Vector3 currentWaypoint;
    private int currentWaypointIndex = 0;
    private int rushCount = 0; // Sayaç
    private float rushSpeed = 7f; // Atýlým hýzý
    private bool isRushing = false;
    private int damageToPlayer = 15; // Oyuncuya verilecek hasar
    private float rushCooldown = 2f; // 1 saniyelik bekleme süresi
    private float rushTimer = 0f; // Zamanlayýcý
    private bool canRushAgain = true; // Yeni atýlým yapýp yapamayacaðýný kontrol etmek için
    private bool hasDamagedPlayer = false; // Oyuncuya hasar verilip verilmediðini kontrol etmek için

    private static readonly int OnRush = Animator.StringToHash("OnRush");
    private static readonly int OnIdle = Animator.StringToHash("OnIdle");

    public override void OnEnter()
    {
        Debug.Log("Rush durumuna giriþ yaptým.");

        // Waypoints'i al
        waypoints = StateData.waypoints;
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("Waypointler atanmamýþ!");
            return;
        }

        // Ýlk waypoint'e git
        SelectNextWaypoint();
        isRushing = true;
    }

    public override void OnExit()
    {
        StateData.EnemyAnimator.ResetTrigger(OnRush);
        StateData.EnemyAnimator.ResetTrigger(OnIdle);
        Debug.Log("Rush durumundan çýkýþ yaptým.");
    }

    public override void OnUpdate()
    {
        if (!isRushing) return;

        // Atýlým yapabilmek için zamanlayýcýyý kontrol et
        if (canRushAgain)
        {
            // Waypointe bak
            LookAtWaypoint();

            // Atýlým yap
            Vector3 targetPosition = new Vector3(currentWaypoint.x, StateData.Enemy.position.y, currentWaypoint.z);
            StateData.Enemy.position = Vector3.MoveTowards(StateData.Enemy.position, targetPosition, rushSpeed * Time.deltaTime);

            // Hedef waypoint'e ulaþtý mý kontrol et
            if (Vector3.Distance(StateData.Enemy.position, targetPosition) < 0.5f)
            {
                Debug.Log($"Waypoint'e ulaþýldý! Sayaç: {rushCount + 1}");
                rushCount++;

                // Waypoint deðiþtir
                if (rushCount >= 5)
                {
                    StateMachineHandler.AddState(new Jumping(), StateData);

                    return; // Atýlým tamamlandýðýnda dur
                }

                // Beklemeye geç
                StateData.EnemyAnimator.SetTrigger(OnIdle);
                canRushAgain = false;
                rushTimer = rushCooldown; // Zamanlayýcýyý sýfýrla
                hasDamagedPlayer = false; // Hasar verme durumunu sýfýrla
            }

            // Oyuncu ile temas varsa hasar ver
            if (StateData.Player != null)
            {
                Vector3 playerPosition = new Vector3(StateData.Player.position.x, StateData.Enemy.position.y, StateData.Player.position.z);
                if (Vector3.Distance(StateData.Enemy.position, playerPosition) < 1f && !hasDamagedPlayer)
                {
                    DamagePlayer();
                    hasDamagedPlayer = true; // Hasar verildiðini iþaretle
                }
            }
        }
        else
        {
            // Bekleme süresi
            rushTimer -= Time.deltaTime;
            if (rushTimer <= 0)
            {
                // Yeni hedef waypoint'e git
                SelectNextWaypoint();
                StateData.EnemyAnimator.SetTrigger(OnRush);
                canRushAgain = true; // Yeni atýlým yapmaya izin ver
            }
        }
    }

    private void LookAtWaypoint()
    {
        // Y eksenini yok sayarak yönünü waypointe doðru çevir
        Vector3 direction = (currentWaypoint - StateData.Enemy.position).normalized;
        direction.y = 0; // Y eksenindeki dönüþü yok say

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            StateData.Enemy.rotation = Quaternion.Slerp(StateData.Enemy.rotation, lookRotation, Time.deltaTime * rushSpeed);
        }
    }

    private void SelectNextWaypoint()
    {
        // Bir sonraki waypoint'e git
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        currentWaypoint = waypoints[currentWaypointIndex].position;

        // Y eksenini yok sayarak waypoint konumunu belirle
        currentWaypoint.y = StateData.Enemy.position.y; // Mevcut Y eksenini koru
    }

    private void DamagePlayer()
    {
        // Oyuncuya hasar ver
        HealthComponent playerHealth = StateData.playerHealthComp;
        if (playerHealth != null && playerHealth.Health > 0)
        {
            playerHealth.TakeDamage(damageToPlayer);
            Debug.Log($"Rush sýrasýnda oyuncuya {damageToPlayer} hasar verildi. Kalan can: {playerHealth.Health}");
        }
        else
        {
            Debug.LogWarning("Oyuncuya hasar verilemedi. Saðlýk bileþeni eksik veya can deðeri 0.");
        }
    }
}
