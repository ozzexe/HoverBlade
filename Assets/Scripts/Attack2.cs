using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(AudioSource))]
public class Attack2 : MonoBehaviour
{
    [SerializeField] private float AnimBlendSpeed = 8.9f;
    [SerializeField] public float playerSpeed = 4.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float rotationSpeed = 4f;

    private Animator _animator;
    private bool _hasAnimator;
    private int _xVelHash;
    private int _yVelHash;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraMainTransform;

    private AudioSource attackSFX; 

    private void Start()
    {
        _hasAnimator = TryGetComponent<Animator>(out _animator);
        controller = GetComponent<CharacterController>();
        cameraMainTransform = Camera.main.transform;

        _xVelHash = Animator.StringToHash("X_Velocity");
        _yVelHash = Animator.StringToHash("Y_Velocity");

        attackSFX = GetComponent<AudioSource>();
        if (attackSFX == null)
        {
            Debug.LogWarning("AudioSource bulunamadý! Lütfen karaktere bir AudioSource ekleyin.");
        }
    }

    void Update()
    {
        // Karakter hareketi
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = cameraMainTransform.forward * move.z + cameraMainTransform.right * move.x;
        move.y = 0f;

        controller.Move(move * Time.deltaTime * playerSpeed);

        float movementSpeed = move.magnitude;

        if (_hasAnimator)
        {
            _animator.SetFloat(_xVelHash, Input.GetAxis("Horizontal"));
            _animator.SetFloat(_yVelHash, Input.GetAxis("Vertical"));
            _animator.SetFloat("Speed", movementSpeed, AnimBlendSpeed, Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space) && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (move != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        if (_hasAnimator)
        {
            _animator.SetTrigger("Attack");
        }

        if (attackSFX != null)
        {
            attackSFX.Play();
        }

        Debug.Log("Attack triggered!");
    }
}
