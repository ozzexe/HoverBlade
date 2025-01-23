using UnityEngine;
using BeykozEdu.FSM;

public class Rush : BaseState<EnemyAIData>
{
    private Transform[] waypoints;
    private Vector3 currentWaypoint;
    private int currentWaypointIndex = 0;
    private int rushCount = 0; // Saya�
    private float rushSpeed = 7f; // At�l�m h�z�
    private bool isRushing = false;
    private int damageToPlayer = 15; // Oyuncuya verilecek hasar
    private float rushCooldown = 2f; // 1 saniyelik bekleme s�resi
    private float rushTimer = 0f; // Zamanlay�c�
    private bool canRushAgain = true; // Yeni at�l�m yap�p yapamayaca��n� kontrol etmek i�in
    private bool hasDamagedPlayer = false; // Oyuncuya hasar verilip verilmedi�ini kontrol etmek i�in

    private static readonly int OnRush = Animator.StringToHash("OnRush");
    private static readonly int OnIdle = Animator.StringToHash("OnIdle");

    public override void OnEnter()
    {
        Debug.Log("Rush durumuna giri� yapt�m.");

        // Waypoints'i al
        waypoints = StateData.waypoints;
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("Waypointler atanmam��!");
            return;
        }

        // �lk waypoint'e git
        SelectNextWaypoint();
        isRushing = true;
    }

    public override void OnExit()
    {
        StateData.EnemyAnimator.ResetTrigger(OnRush);
        StateData.EnemyAnimator.ResetTrigger(OnIdle);
        Debug.Log("Rush durumundan ��k�� yapt�m.");
    }

    public override void OnUpdate()
    {
        if (!isRushing) return;

        // At�l�m yapabilmek i�in zamanlay�c�y� kontrol et
        if (canRushAgain)
        {
            // Waypointe bak
            LookAtWaypoint();

            // At�l�m yap
            Vector3 targetPosition = new Vector3(currentWaypoint.x, StateData.Enemy.position.y, currentWaypoint.z);
            StateData.Enemy.position = Vector3.MoveTowards(StateData.Enemy.position, targetPosition, rushSpeed * Time.deltaTime);

            // Hedef waypoint'e ula�t� m� kontrol et
            if (Vector3.Distance(StateData.Enemy.position, targetPosition) < 0.5f)
            {
                Debug.Log($"Waypoint'e ula��ld�! Saya�: {rushCount + 1}");
                rushCount++;

                // Waypoint de�i�tir
                if (rushCount >= 5)
                {
                    StateMachineHandler.AddState(new Jumping(), StateData);

                    return; // At�l�m tamamland���nda dur
                }

                // Beklemeye ge�
                StateData.EnemyAnimator.SetTrigger(OnIdle);
                canRushAgain = false;
                rushTimer = rushCooldown; // Zamanlay�c�y� s�f�rla
                hasDamagedPlayer = false; // Hasar verme durumunu s�f�rla
            }

            // Oyuncu ile temas varsa hasar ver
            if (StateData.Player != null)
            {
                Vector3 playerPosition = new Vector3(StateData.Player.position.x, StateData.Enemy.position.y, StateData.Player.position.z);
                if (Vector3.Distance(StateData.Enemy.position, playerPosition) < 1f && !hasDamagedPlayer)
                {
                    DamagePlayer();
                    hasDamagedPlayer = true; // Hasar verildi�ini i�aretle
                }
            }
        }
        else
        {
            // Bekleme s�resi
            rushTimer -= Time.deltaTime;
            if (rushTimer <= 0)
            {
                // Yeni hedef waypoint'e git
                SelectNextWaypoint();
                StateData.EnemyAnimator.SetTrigger(OnRush);
                canRushAgain = true; // Yeni at�l�m yapmaya izin ver
            }
        }
    }

    private void LookAtWaypoint()
    {
        // Y eksenini yok sayarak y�n�n� waypointe do�ru �evir
        Vector3 direction = (currentWaypoint - StateData.Enemy.position).normalized;
        direction.y = 0; // Y eksenindeki d�n��� yok say

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
            Debug.Log($"Rush s�ras�nda oyuncuya {damageToPlayer} hasar verildi. Kalan can: {playerHealth.Health}");
        }
        else
        {
            Debug.LogWarning("Oyuncuya hasar verilemedi. Sa�l�k bile�eni eksik veya can de�eri 0.");
        }
    }
}
