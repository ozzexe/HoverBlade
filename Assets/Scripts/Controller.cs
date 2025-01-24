   using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Controller : MonoBehaviour
{
    [SerializeField]
    private InputActionReference movementControl;
    [SerializeField] private float AnimBlendSpeed = 8.9f;
    [SerializeField]
    private InputActionReference jumpControl;
    [SerializeField]
    private InputActionReference runControl; // Run input action
    [SerializeField]
    public float playerSpeed = 4.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float rotationSpeed = 4f;
    [SerializeField]
    public float runSpeed = 9.0f; // Run speed multiplier

    [Header("Hoverboard Settings")]
    public Transform hoverboardSeat;
    public HBcontroller hoverboardController;
    private bool isRiding = false;
    private bool canMountHoverboard = false;

    [Header("Hoverboard Call Settings")]
    public Transform hoverboardCallPosition; // Hoverboard'un geleceði hedef pozisyon
    public KeyCode callHoverboardKey = KeyCode.H; // Hoverboard'u çaðýrmak için kullanýlacak tuþ

    private Animator _animator;
    private bool _hasAnimator;
    private int _xVelHash;
    private int _yVelHash;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraMainTransform;
    [SerializeField] AudioSource attackSFX;

    private bool isRunning = false; // Koþma durumu

    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
        runControl.action.Enable();
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
        runControl.action.Disable();
    }

    private void Start()
    {
        _hasAnimator = TryGetComponent<Animator>(out _animator);
        controller = gameObject.GetComponent<CharacterController>();
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
        // Hoverboard'u çaðýrma
        if (Input.GetKeyDown(callHoverboardKey) && hoverboardController != null)
        {
            hoverboardController.CallToPosition(hoverboardCallPosition.position);
        }

        // Hoverboard'a binme/indiþ iþlemi
        if (canMountHoverboard && Input.GetKeyDown(KeyCode.E))
        {
            if (!isRiding)
                MountHoverboard();
            else
                DismountHoverboard();
        }

        if (isRiding) return; // Hoverboard'dayken hareket kontrolleri devre dýþý

        if (Input.GetKeyDown(KeyCode.F))
        {
            PerformAttack();
        }

        // Koþma kontrolü
        isRunning = runControl.action.ReadValue<float>() > 0.1f; // Left Shift tuþuna basýlý olup olmadýðýný kontrol et

        // Karakter hareketi
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movementInput = movementControl.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);
        move = cameraMainTransform.forward * move.z + cameraMainTransform.right * move.x;
        move.y = 0f;

        // Koþma hýzýný uygula
        float currentSpeed = isRunning ? runSpeed : playerSpeed;
        controller.Move(move * Time.deltaTime * currentSpeed);

        float movementSpeed = move.magnitude;

        if (_hasAnimator)
        {
            _animator.SetFloat(_xVelHash, movementInput.x);
            _animator.SetFloat(_yVelHash, movementInput.y);
            _animator.SetFloat("Speed", movementSpeed, AnimBlendSpeed, Time.deltaTime);
            _animator.SetBool("IsRunning", isRunning); // Koþma animasyonunu tetikle
        }

        if (jumpControl.action.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (movementInput != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movementInput.x, movementInput.y) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void MountHoverboard()
    {
        isRiding = true;
        controller.enabled = false;

        if (hoverboardSeat != null)
        {
            transform.position = hoverboardSeat.position;
            transform.rotation = hoverboardSeat.rotation;
            transform.parent = hoverboardSeat;
            hoverboardController.Mount();
        }
        else
        {
            Debug.LogError("Hoverboard Seat not assigned!");
        }

        hoverboardController.SetControlled(true);

        if (_hasAnimator)
        {
            _animator.SetFloat("Speed", 0);
        }
    }

    private void DismountHoverboard()
    {
        isRiding = false;
        controller.enabled = true;
        transform.parent = null;
        hoverboardController.SetControlled(false);
        hoverboardController.Dismount();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Board"))
        {
            Debug.Log("Hoverboard ile etkileþime geçildi");
            canMountHoverboard = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Board"))
        {
            Debug.Log("Hoverboard'dan uzaklaþýldý");
            canMountHoverboard = false;
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











