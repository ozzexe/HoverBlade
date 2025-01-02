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
    public Transform hoverboardSeat;
    public HBcontroller hoverboardController;
    private bool isRiding = false;
    private bool canMountHoverboard = false;

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
        // Hoverboard'a binme/indiþ iþlemi
        if (canMountHoverboard && Input.GetKeyDown(KeyCode.E))
        {
            if (!isRiding)
                MountHoverboard();
            else
                DismountHoverboard();
        }

        if (isRiding) return; // Hoverboard'dayken hareket kontrolleri devre dýþý

        // Karakter hareketi
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Burada `movement` yerine `movementInput` kullanalým
        Vector2 movementInput = movementControl.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);
        move = cameraMainTransform.forward * move.z + cameraMainTransform.right * move.x;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        float movementSpeed = move.magnitude;

        if (_hasAnimator)
        {
            _animator.SetFloat(_xVelHash, movementInput.x);
            _animator.SetFloat(_yVelHash, movementInput.y);
            _animator.SetFloat("Speed", movementSpeed, AnimBlendSpeed, Time.deltaTime);
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
        controller.enabled = false;  // CharacterController'ý devre dýþý býrak

        // Null kontrolü
        if (hoverboardSeat != null)
        {
            transform.position = hoverboardSeat.position;
            transform.rotation = hoverboardSeat.rotation; // Karakterin dönüþünü doðru þekilde ayarla
            transform.parent = hoverboardSeat; // Karakter hoverboard'ýn altýna yerleþiyor
            hoverboardController.Mount();
        }
        else
        {
            Debug.LogError("Hoverboard Seat not assigned!");
        }

        hoverboardController.SetControlled(true); // Hoverboard'ý kontrol etmeye baþla

        if (_hasAnimator)
        {
            _animator.SetFloat("Speed", 0);
        }
    }

    private void DismountHoverboard()
    {
        isRiding = false;
        controller.enabled = true;  // CharacterController'ý tekrar aktif et
        transform.parent = null;  // Karakter hoverboard'dan ayrýlýr
        hoverboardController.SetControlled(false); // Hoverboard kontrolünü sonlandýr
        hoverboardController.Dismount();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Hoverboard ile etkileþime girme
        if (other.CompareTag("Board"))
        {
            Debug.Log("Hoverboard ile etkileþime geçildi");
            canMountHoverboard = true;  // Hoverboard ile etkileþime geçilebilir
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Board"))
        {
            Debug.Log("Hoverboard'dan uzaklaþýldý");
            canMountHoverboard = false;  // Hoverboard'dan uzaklaþýldýðýnda etkileþim sona erer
        }
    }
}










