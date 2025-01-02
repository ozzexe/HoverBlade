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
        // Hoverboard'a binme/indi� i�lemi
        if (canMountHoverboard && Input.GetKeyDown(KeyCode.E))
        {
            if (!isRiding)
                MountHoverboard();
            else
                DismountHoverboard();
        }

        if (isRiding) return; // Hoverboard'dayken hareket kontrolleri devre d���

        // Karakter hareketi
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Burada `movement` yerine `movementInput` kullanal�m
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
        controller.enabled = false;  // CharacterController'� devre d��� b�rak

        // Null kontrol�
        if (hoverboardSeat != null)
        {
            transform.position = hoverboardSeat.position;
            transform.rotation = hoverboardSeat.rotation; // Karakterin d�n���n� do�ru �ekilde ayarla
            transform.parent = hoverboardSeat; // Karakter hoverboard'�n alt�na yerle�iyor
            hoverboardController.Mount();
        }
        else
        {
            Debug.LogError("Hoverboard Seat not assigned!");
        }

        hoverboardController.SetControlled(true); // Hoverboard'� kontrol etmeye ba�la

        if (_hasAnimator)
        {
            _animator.SetFloat("Speed", 0);
        }
    }

    private void DismountHoverboard()
    {
        isRiding = false;
        controller.enabled = true;  // CharacterController'� tekrar aktif et
        transform.parent = null;  // Karakter hoverboard'dan ayr�l�r
        hoverboardController.SetControlled(false); // Hoverboard kontrol�n� sonland�r
        hoverboardController.Dismount();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Hoverboard ile etkile�ime girme
        if (other.CompareTag("Board"))
        {
            Debug.Log("Hoverboard ile etkile�ime ge�ildi");
            canMountHoverboard = true;  // Hoverboard ile etkile�ime ge�ilebilir
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Board"))
        {
            Debug.Log("Hoverboard'dan uzakla��ld�");
            canMountHoverboard = false;  // Hoverboard'dan uzakla��ld���nda etkile�im sona erer
        }
    }
}










