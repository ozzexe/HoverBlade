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
    public float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float rotationSpeed = 4f;

    [Header("Hoverboard Settings")]
    public Transform hoverboardSeat; // Hoverboard �zerindeki pozisyon
    public HBcontroller hoverboardController; // Hoverboard'un kontrol scripti
    private bool isRiding = false; // Karakter hoverboard �zerinde mi?
    private bool canMountHoverboard = false; // Hoverboard'a binebilir mi?

    private Animator _animator;
    private bool _hasAnimator;
    private int _xVelHash;
    private int _yVelHash;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraMainTransform;

    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
    }

    private void Start()
    {
        _hasAnimator = TryGetComponent<Animator>(out _animator);
        controller = gameObject.GetComponent<CharacterController>();
        cameraMainTransform = Camera.main.transform;

        _xVelHash = Animator.StringToHash("X_Velocity");
        _yVelHash = Animator.StringToHash("Y_Velocity");
    }

    void Update()
    {
        // Hoverboard'a binme/inme i�lemi
        if (canMountHoverboard && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleHoverboard();
        }

        // E�er hoverboard �zerindeyse, karakterin kontrol� devre d��� b�rak�l�r
        if (isRiding) return;

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        Vector2 movement = movementControl.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        move = cameraMainTransform.forward * move.z + cameraMainTransform.right * move.x;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        float movementSpeed = move.magnitude;

        if (_hasAnimator)
        {
            // X and Y velocity parameters for animator
            _animator.SetFloat(_xVelHash, movement.x);
            _animator.SetFloat(_yVelHash, movement.y);
            _animator.SetFloat("Speed", movementSpeed, AnimBlendSpeed, Time.deltaTime);
        }

        if (jumpControl.action.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (movement != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void ToggleHoverboard()
    {
        isRiding = !isRiding;

        if (isRiding)
        {
            // Karakter hoverboard'a biniyor
            controller.enabled = false;
            transform.position = hoverboardSeat.position; // Karakter hoverboard pozisyonuna ge�iyor
            transform.parent = hoverboardSeat; // Hoverboard'a ba�lan�yor
            hoverboardController.enabled = true; // Hoverboard kontrol� aktif
        }
        else
        {
            // Karakter hoverboard'dan iniyor
            controller.enabled = true;
            transform.parent = null; // Hoverboard'dan ayr�l�yor
            hoverboardController.enabled = false; // Hoverboard kontrol� pasif
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hoverboard"))
        {
            canMountHoverboard = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hoverboard"))
        {
            canMountHoverboard = false;
        }
    }
}

