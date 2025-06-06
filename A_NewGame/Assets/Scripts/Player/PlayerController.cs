using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// A class which allows the player to move / look around
// the gameobject must have a rigidbody, a camera as a child and a transform component
// the floor object must have the tag "Ground"
public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    private Camera _camera;

    // for handling player movement
    [Header("Movement Settings")]
    [Tooltip("Multiplier applied to the force when the player moves.(8f)")]
    [SerializeField] private float _speed = 8f;

    [Tooltip("The maximum velocity a player can move at.(5f)")]
    [SerializeField] private float _maxSpeed = 5f;

    [Tooltip("Multiplier applied to the force when the player sprints.(10f)")]
    [SerializeField] private float _sprintSpeed = 10f;

    [Tooltip("The maximum velocity a player can move when sprinting.(7f)")]
    [SerializeField] private float _maxSprintSpeed = 7f;

    [Tooltip("Multiplier applied to the force when the player is crouched.(5f)")]
    [SerializeField] private float _crouchSpeed = 5f;

    [Tooltip("The maximum velocity a player can move when sprinting.(3f)")]
    [SerializeField] private float _maxCrouchSpeed = 3f;

    [Tooltip("The time it takes for the player to enter crouched position.(0.2f)")]
    [SerializeField] private float _timeToCrouch = 0.2f;

    [Tooltip("Multiplier applied to the Impulse when the player jumps.(4f)")]
    [SerializeField] private float _jumpForce = 4f;

    [Tooltip("Multiplier applied to the Impulse when the player is in the air but hasn't jumped.(-9.98f)")]
    [SerializeField] private float _gravityForce = -9.98f;

    [Tooltip("Multiplier applied to the Force when the player is in contact with a Climbing object.(8f)")]
    [SerializeField] private float _climbSpeed = 8f;

    [Tooltip("The maximum speed a player can move at while climbing a Climbing object.(4f)")]
    [SerializeField] private float _maxClimbSpeed = 4f;

    [Tooltip("The outer collider on the player which has no friction, for walls.")]
    [SerializeField] private Collider _outerCollider;

    [Tooltip("The trigger above the player which triggers events when player enters a cell, for CellStuff.")]
    [SerializeField] private Collider _playerTrigger;

    private bool _isGrounded = true;
    private bool _isClimbing = false;
    private Vector3 _playerInput = Vector3.zero;

    // for handling player rotation
    [Header("Rotation Settings")]
    [Tooltip("How far the player can look up or down in degrees.(60f)")]
    [SerializeField] private float _lookLimit = 60f;

    [Tooltip("How much the camera reacts to changes in mouse position.(2f)")]
    [SerializeField] private float _lookSpeed = 2f;

    [Tooltip("Audio Source used for world ambient audio")]
    [SerializeField] private AudioSource _playerAudioSource;

    float rotationX = 0f;   // rotation on x axis = vertical tilt
    float rotationY = 0f;   // rotation on y axis = horizontal rotation

    private MovementStates _movementStates;

    private Vector3 _playerSpawnPoint;

    [Header("Check if Grounded")]
    [Tooltip("Position of player center, used for determining if they are grounded with RayCast.")]
    [SerializeField] private Transform _playerCenter;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private LayerMask _interactableMask;

    // holds the info to freeze x and z rotation
    private RigidbodyConstraints _XZFreeze;


    // contains a list of all keys & keycodes considered as input
    public static readonly Dictionary<string, KeyCode> _inputKeys = new Dictionary<string, KeyCode>
    {
        {"Forward", KeyCode.W },                  // FORWARDS
        {"Left", KeyCode.A },
        {"Back", KeyCode.S },
        {"Right", KeyCode.D },
        {"Crouch", KeyCode.C },                  // CROUCH
        {"Jump", KeyCode.Space },         // JUMP
        {"Sprint", KeyCode.LeftShift },   // SPRINT
        {"Interact", KeyCode.E }          // INTERACT 
    };

    private void Awake()
    {
        if (!TryGetComponent<Rigidbody>(out _rb))
            Debug.LogWarning("No Rigidbody Component!");

        _camera = GetComponentInChildren<Camera>();
        if (_camera == null)
            Debug.LogWarning("No Camera Component!");
        if (Camera.main != null && Camera.main.isActiveAndEnabled)
        {
            Camera.main.gameObject.SetActive(false);
            Debug.Log("Disabled Main Camera");
        }
        _camera.enabled = true;

        if (!TryGetComponent<Transform>(out Transform _transform))
            Debug.LogWarning("No Transform Component!");
        _XZFreeze = _rb.constraints;
        _playerSpawnPoint = _rb.transform.position;
    }
    private void Start()
    {
        DeActivateCurosr();
        StartCoroutine(HandleContinuousInput());
    }

    void Update()
    {
        //PlayerZooped();
        if (CheckForInput())
        {
            CheckMovementState();
            MoveCameraHeight();
            _playerInput = GetInput();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }
        if (Input.GetKeyDown(KeyCode.Z))
            ChangePlayerSens(false);
        if (Input.GetKeyDown(KeyCode.X))
            ChangePlayerSens(true);
    }

    private void ChangePlayerSens(bool more)
    {
        if (more)
            _lookSpeed++;
        else    
            _lookSpeed--;
    }

    private void PlayerZooped()
    {
        if (transform.position.x > 100 || transform.position.x < -100)
        {
            Debug.Log("Player Input: " + _playerInput + ", isGrounded: " + _isGrounded);
            Debug.Log("Current x Veloctiy: " + _rb.linearVelocity.x + ", y velocity: " + _rb.linearVelocity.y + ", z: " + _rb.linearVelocity.z);
            Debug.Log("Velocity Magnitude: " + _rb.linearVelocity.magnitude);
            Debug.LogError("I stop Now X");
        }
        if (transform.position.z > 100 || transform.position.z < -100)
        {
            Debug.Log("Player Input: " + _playerInput + ", isGrounded: " + _isGrounded);
            Debug.Log("Current x Veloctiy: " + _rb.linearVelocity.x + ", y velocity: " + _rb.linearVelocity.y + ", z: " + _rb.linearVelocity.z);
            Debug.Log("Velocity Magnitude: " + _rb.linearVelocity.magnitude);
            Debug.LogError("I stop Now Z");
        }
    }
    private void FixedUpdate()
    {
        HandleNoInput();

        //HandleInput();
        //if (_isGrounded && PlayerNotGrounded())

        ApplyGravity();

        HandleMouseInput();
    }

    // handles the players rotation by finding the mouse's positoon
    private void HandleMouseInput()
    {
        if (Input.GetAxis("Mouse Y") == 0 && Input.GetAxis("Mouse X") == 0)
        {
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
            return;
        }
        else
            _rb.constraints = _XZFreeze;

        if (Input.GetAxis("Mouse Y") != 0)
        {
            HandleMouseVerticleInput();
        }
        if (Input.GetAxis("Mouse X") != 0)
        {
            HandleMouseHorizontalInput();
        }
    }

    // rotate the camera vertically
    private void HandleMouseVerticleInput()
    {
        rotationX += -Input.GetAxis("Mouse Y") * _lookSpeed*0.75f;
        rotationX = Mathf.Clamp(rotationX, -_lookLimit, _lookLimit);
        _camera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    // rotate the camera horizontally
    private void HandleMouseHorizontalInput()
    {
        rotationY += Input.GetAxis("Mouse X") * _lookSpeed;
        transform.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    // locks cursor in center of screen and hides it
    private void DeActivateCurosr()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // perpetually runs when input is recorded, calls another function when there is non which waits for new input
    private IEnumerator HandleContinuousInput()
    {
        Vector3 _currentInput = _playerInput;

        //Debug.Log("Curr Input: " + _currentInput.x + ", " + _currentInput.z);

        while (_playerInput != Vector3.zero && _currentInput == _playerInput)
        {
            //Debug.Log("Player Input: " + _playerInput.x + ", " + _playerInput.z);
            HandleInput();
            yield return new WaitForFixedUpdate();
        }
        if (_currentInput != _playerInput)
        {
            _playerInput = GetInput();
            StartCoroutine(HandleContinuousInput());
        }
        else 
            StartCoroutine(WaitForInput());
        //yield break;
    }

    // perpetually runs until input is recorded, then calls a coroutine to handle said input
    private IEnumerator WaitForInput()
    {
        yield return null;

        while (_playerInput == Vector3.zero)
        {
            yield return new WaitForFixedUpdate();
        }
        StartCoroutine(HandleContinuousInput());
        //yield break;
    }

    // applies forces to the players rigidbody as a result of the input
    private void HandleInput()
    {
        Vector3 CurrentPlayerInput = _playerInput;

        MovementStates currentMovementState = _movementStates;
        if (CurrentPlayerInput == Vector3.zero || _isClimbing)
            return;

        if (CurrentPlayerInput.y > 0)
            Jump();

        //ResetYVaue(ref _playerInput);

        //Debug.Log("MovementSTate: " + _movementStates + ", velocity: " + _rb.linearVelocity);

        switch (currentMovementState)
        {
            case MovementStates.None:
                break;
            case MovementStates.Crouch:
                MovePlayer(_crouchSpeed, _maxCrouchSpeed, CurrentPlayerInput);
                break;
            case MovementStates.Walk:
                MovePlayer(_speed, _maxSpeed, CurrentPlayerInput);
                break;
            case MovementStates.Sprint:
                MovePlayer(_sprintSpeed, _maxSprintSpeed, CurrentPlayerInput);
                break;
        }

    }

    // handels adding force to player rigidbody (or removing force if switching movement states)
    private void MovePlayer(float speed, float maxSpeed, Vector3 currentPlayerInput)
    {
        //Vector3 currentPlayerInput = playerInputPassed;

        if (currentPlayerInput != _playerInput)
            return;

        Vector3 relativeVelocity = Quaternion.Inverse(transform.rotation) * _rb.linearVelocity;

        if (_rb.linearVelocity.magnitude < maxSpeed)
        {
            if (_rb.linearVelocity.magnitude < (maxSpeed / 2))
                _rb.AddRelativeForce(new Vector3(currentPlayerInput.x, 0f, currentPlayerInput.z) * (maxSpeed/2), ForceMode.Impulse);
            else if (_isGrounded && (relativeVelocity.x > (maxSpeed * currentPlayerInput.x) || relativeVelocity.x < (maxSpeed * currentPlayerInput.x)
                || relativeVelocity.z > (maxSpeed * currentPlayerInput.z) || relativeVelocity.z < (maxSpeed * currentPlayerInput.z)))
            {
                _rb.AddRelativeForce((new Vector3((maxSpeed * currentPlayerInput.x - relativeVelocity.x), 0f, (maxSpeed * currentPlayerInput.z - relativeVelocity.z))), ForceMode.Impulse);
                //_rb.AddRelativeForce((new Vector3(0f, 0f, )), ForceMode.Impulse);
            }
            else
                _rb.AddRelativeForce(new Vector3 (currentPlayerInput.x, 0f, currentPlayerInput.z) * speed, ForceMode.Force);
        }
        else
        {
            // to fast forwards
            //Debug.Log("Relative Velocity: " + relativeVelocity.x + ", " + relativeVelocity.z);
            //Debug.Log("Player Desired Speed: " + maxSpeed * currentPlayerInput.x + ", " + maxSpeed * currentPlayerInput.z);
            //Debug.Log("Correction being made: " + (maxSpeed * currentPlayerInput.x - relativeVelocity.x) + ", " + (maxSpeed * currentPlayerInput.z - relativeVelocity.z));
            
            if (relativeVelocity.x > (maxSpeed * currentPlayerInput.x) || relativeVelocity.x < (maxSpeed * currentPlayerInput.x))
                _rb.AddRelativeForce((new Vector3((maxSpeed * currentPlayerInput.x - relativeVelocity.x), 0f, 0f)), ForceMode.Impulse);
            // too fast back
            //else if (relativeVelocity.x < maxSpeed * _playerInput.x)
                //_rb.AddRelativeForce(new Vector3((maxSpeed * _playerInput.x - relativeVelocity.x), 0f, 0), ForceMode.Impulse);
            // too fast to right
            if (relativeVelocity.z  > (maxSpeed * currentPlayerInput.z) || relativeVelocity.z < (maxSpeed * currentPlayerInput.z))
                _rb.AddRelativeForce((new Vector3(0f, 0f, (maxSpeed * currentPlayerInput.z - relativeVelocity.z))), ForceMode.Impulse);
            // too fast to left
            //else if (relativeVelocity.z < maxSpeed * _playerInput.z)
               //_rb.AddRelativeForce(new Vector3(0f, 0f, (maxSpeed * _playerInput.z - relativeVelocity.z)), ForceMode.Impulse);
            
        }
    }

    private void FindGroundAngle()
    {
        //float angle = 0f;
        //Physics.Raycast(_playerCenter.position, Vector3.down, _playerCenter.localPosition.y + 0.1f, _groundMask);
        RaycastHit frontHit = new RaycastHit();
        frontHit.distance = 0f;
        Physics.Raycast(_playerCenter.position, _playerCenter.transform.forward, out frontHit, 2f);
        RaycastHit backHit = new RaycastHit();
        backHit.distance = 0f;
        Physics.Raycast(_playerCenter.position, -_playerCenter.transform.forward, out backHit, 2f);
        float forwardAngle = 0f;

        //if (Physics.Raycast(_playerCenter.position,Vector3.forward,  out frontHit, 2f) || Physics.Raycast(_playerCenter.position, Vector3.back, out backHit, 2f))
        if (frontHit.distance != 0f || backHit.distance != 0f)
        {
            //frontHit.GetValueOrDefault 
            Debug.Log("Front: " + frontHit.distance + ", Back: " + backHit.distance);
            float adjacent = (frontHit.distance >= backHit.distance) ? (frontHit.distance * frontHit.distance) : (backHit.distance * backHit.distance);
            float opposite = _playerCenter.localPosition.y * _playerCenter.localPosition.y;
            forwardAngle = Mathf.Atan2(opposite, adjacent);
            forwardAngle = forwardAngle / 360;
        }
        Debug.Log("ForwardAngle: " + forwardAngle);
        _playerInput = new Vector3(_playerInput.x + (_playerInput.x * forwardAngle / 2), _playerInput.y, _playerInput.z);
        _playerInput = _playerInput.normalized;
        //Ray frontRay = new Ray(_playerCenter.position, Vector3.forward);
        //Ray rightRay = new Ray(_playerCenter.position, Vector3.right);
        //Ray leftRay = new Ray(_playerCenter.position, Vector3.left);
        //Ray backRay = new Ray(_playerCenter.position, Vector3.back);

        //if ()

        //return forwardAngle;
    }

    // checks if there is no horizontal / vertical input and resets the velocity if true
    private void HandleNoInput()
    {
        // must use relaive velocity > rb.linearvelocity because linear is not relative
        Vector3 relativeVelocity = Quaternion.Inverse(transform.rotation) * _rb.linearVelocity;

        //if (_playerInput.x == 0 && (relativeVelocity.x > 0.5 || relativeVelocity.x < -0.5))
        if (_playerInput.x == 0 && (relativeVelocity.x != 0))
            _rb.AddRelativeForce(new Vector3(-(relativeVelocity.x), 0f, 0f), ForceMode.Impulse);
            //_rb.AddRelativeForce(new Vector3(-(relativeVelocity.x / 2), 0f, 0f), ForceMode.Impulse);

        //if (_playerInput.z == 0 && (relativeVelocity.z > 0.5 || relativeVelocity.z < -0.5))
        if (_playerInput.z == 0 && (relativeVelocity.z != 0))
            _rb.AddRelativeForce(new Vector3(0f, 0f, -(relativeVelocity.z)), ForceMode.Impulse);
            //_rb.AddRelativeForce(new Vector3(0f, 0f, -(relativeVelocity.z / 2)), ForceMode.Impulse);
    }

    // changes the y value to zero
    private void ResetYVaue(ref Vector3 vector3)
    {
        vector3 = new Vector3(vector3.x, 0, vector3.z);
    }

    // applies an upwards impulse to the players rigidbody
    private void Jump()
    {
        // can't jump if crouched
        if (PlayerIsGrounded() && _movementStates != MovementStates.Crouch)
        {
            _isGrounded = false;
            _rb.AddForce(new Vector3(0, 1, 0) * _jumpForce, ForceMode.Impulse);
        }
    }

    private IEnumerator Climb()
    {
        yield return null;
        //_rb.constraints = RigidbodyConstraints.None;
        //_rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | _XZFreeze;
        Vector3 climbDirection;
        _rb.useGravity = false;
        float timeBeforeUnclimb = 0;
        bool backOnGround = false;

        //Debug.Log("Start Climbing");
        Vector3 relativeVelocity;// = Quaternion.Inverse(transform.rotation) * _rb.linearVelocity;
        while (_isClimbing)
        {
            climbDirection = GetInput();
            relativeVelocity = Quaternion.Inverse(transform.rotation) * _rb.linearVelocity;

            if (climbDirection.y > 0 || backOnGround)
            {
                _isClimbing = false;               
                break;
            }
            //if (climbDirection.z != 0f)
            //{
            //    //_rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | _XZFreeze;
            //}
            if (climbDirection.z > 0 && relativeVelocity.y < _maxClimbSpeed)
            {
                //Debug.Log("Climbing UP");

                _rb.AddRelativeForce(new Vector3(0f, 1f, 0f) * _climbSpeed, ForceMode.Force);
            }
            if (climbDirection.z < 0 && relativeVelocity.y > -_maxClimbSpeed)
            {
                //Debug.Log("Climbing DOWN");
                _rb.AddRelativeForce(new Vector3(0f, -1f, 0f) * _climbSpeed, ForceMode.Force);
            }
            if ((relativeVelocity.y >= 0.1f || relativeVelocity.y <= -0.1f) && climbDirection.z == 0)
            {
                //Debug.Log("Not Moving");
                _rb.linearVelocity = Vector3.zero;
                //_rb.AddRelativeForce(new Vector3(0f, -(relativeVelocity.y)/2, 0f), ForceMode.Impulse);
                //_rb.constraints = RigidbodyConstraints.FreezePosition;// | _XZFreeze;
                
            }
            //yield return new WaitForSeconds(0.2f);
            yield return null;

            if (timeBeforeUnclimb >= 0.5f)
                backOnGround = !PlayerNotGrounded();
            else
                timeBeforeUnclimb += Time.deltaTime;
        }

        _rb.useGravity = true;
        yield return null;

        //if (!backOnGround)
        

        //Debug.Log("Stop Climbing");

        //_rb.constraints = _XZFreeze;
    }

    private void ApplyGravity()
    {
        if (PlayerNotGrounded() && _isGrounded && !_isClimbing)
        {
            _rb.AddForce(new Vector3(0, 1, 0) * _gravityForce, ForceMode.Impulse);
        }
    }

    // Returns true if there are new keys being pressed, or released.
    private bool CheckForInput()
    {
        foreach (var value in _inputKeys.Values)
        {
            if (Input.GetKeyDown(value) || Input.GetKeyUp(value))
            {
                return true;
            }
        }
        return false;
    }

    // check if player is crouching, sprinting, walking or static (stored in _movementState)
    private void CheckMovementState()
    {
        if (Input.GetKey(_inputKeys["Crouch"]))
        {
            _movementStates = MovementStates.Crouch;
            return;
        }
        if (Input.GetKey(_inputKeys["Sprint"]))
        {
            _movementStates = MovementStates.Sprint;
            return;
        }
        if (GetInput() != Vector3.zero)
        {
            _movementStates = MovementStates.Walk;
            return;
        }
        _movementStates = MovementStates.None;
    }

    // moves camera height to 0.75 if is crouching (else sets it at 1f)
    private void MoveCameraHeight()
    {
        // can't crouch while mid air ---
        if (_movementStates == MovementStates.Crouch)// && PlayerIsGrounded())
        {
            StartCoroutine(CrouchCameraLERP(_camera.transform.localPosition, new Vector3(0f, 1.5f, 0f)));
            //_camera.transform.localPosition = new Vector3(0f, 1.5f, 0f);
        }
        else
        {
            //StopCoroutine("CrouchCameraLERP");
            //StartCoroutine(CrouchCameraLERP(_camera.transform.localPosition, new Vector3(0f, 2f, 0f)));
            _camera.transform.localPosition = new Vector3(0f, 2f, 0f);
        }
    }

    private IEnumerator CrouchCameraLERP(Vector3 startPos, Vector3 endPos)
    {
        float time = 0f;
        float t = 0f;

        while (t < 1 && _movementStates == MovementStates.Crouch)
        {
            yield return null;
            time += Time.deltaTime;
            t = time / _timeToCrouch;
            _camera.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
        }

        if (_movementStates != MovementStates.Crouch)
            _camera.transform.localPosition = new Vector3(0f, 2f, 0f);

        yield return null;
    }

    // Returns a normalized vector 3 for the players input (x,y,z)
    private Vector3 GetInput()
    {
        Vector3 playerInput = Vector3.zero;

        if (Input.GetKey(_inputKeys["Forward"]))
            playerInput += new Vector3(0, 0, 1f);

        if (Input.GetKey(_inputKeys["Back"]))
            playerInput -= new Vector3(0, 0, 1f);

        if (Input.GetKey(_inputKeys["Right"]))
            playerInput += new Vector3(1f, 0, 0);

        if (Input.GetKey(_inputKeys["Left"]))
            playerInput -= new Vector3(1f, 0, 0);

        if (Input.GetKey(_inputKeys["Jump"]))
            playerInput += new Vector3(0, 1f, 0);

        if (Input.GetKeyDown(_inputKeys["Interact"]) && InteractablePresent())
            Interact();

        return playerInput.normalized;
        //return playerInput;
    }

    private void Interact()
    {
        //Debug.Log("Interacting");

        RaycastHit hit;
        Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, 7f, _interactableMask, QueryTriggerInteraction.Ignore);

        if (hit.collider == null)
        {
            Debug.LogError("Interactable is null");
            return;
        }

        if (hit.collider.tag == "Door")
        {
            //Debug.Log("Interacting with Door");
            CellDoor door = hit.collider.GetComponentInParent<CellDoor>();
            if (door == null)
            {
                Debug.LogError("Door Interactable parent is null");
                return;
            }

            door.InteractWithDoor(transform.position);
        }
        else
            Debug.Log("Interaction failed on: " + hit.collider.name +", tag: " + hit.collider.tag);
    }

    public AudioSource GetPlayerAudioSource()
    {
        if (_playerAudioSource == null)
        {
            Debug.LogError("No Player Audio Source!!");
            return null;
        }
        return _playerAudioSource;
    }

    public Camera GetPlayerCam()
    {
        return _camera;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null || collision.gameObject == null)
        {
            Debug.LogWarning("(ENTER) Collision gameobject is null!");
            return;
        }

        //if (collision.transform.parent == null)
        //{
        //    Debug.LogWarning("(ENTER) Collision's parent is null!");
        //    return;
        //}

        if (collision.gameObject.tag == "Ground")
            _isGrounded = true;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || other.gameObject == null)
        {
            Debug.LogWarning("(ENTER) Trigger gameobject is null!");
            return;
        }

        if (other.gameObject.tag == "Climb")
        {
            if (_outerCollider !=  null)
                _outerCollider.enabled = false;
            if (_playerTrigger != null) 
                _playerTrigger.enabled = false;
            _isClimbing = true;
            _isGrounded = true;
            //Debug.Log("StartClimbing collision");
            StartCoroutine(Climb());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null || other.gameObject == null)
        {
            Debug.LogWarning("(EXIT) Trigger gameobject is null!");
            return;
        }

        if (other.gameObject.tag == "Climb")
        {
            //Debug.Log("StopClimbing collision");
            if (_isClimbing)
            {
                _rb.AddForce(new Vector3(0, 1, 0) * _jumpForce, ForceMode.Impulse);
                _rb.AddRelativeForce(new Vector3(_playerInput.x, 0f, _playerInput.z) * (_speed/2), ForceMode.Impulse);
                //_isClimbing = false;
                //_rb.AddRelativeForce(new Vector3(0f, 2f, 0f), ForceMode.Impulse);
                //_rb.AddRelativeForce(new Vector3(0f, 0f, 2f), ForceMode.Impulse);
            }

            if (_outerCollider != null)
                _outerCollider.enabled = true;

            if (_playerTrigger != null)
                _playerTrigger.enabled = true;
            //_isGrounded = true;
            //_rb.useGravity = true;
            StartCoroutine(WaitTheDisableClimbing());
        }
    }

    private IEnumerator WaitTheDisableClimbing()
    {
        yield return new WaitForSeconds(0.1f);
        _isClimbing = false;
    }

    // check if player is currently on a solid surface (ray tracing)
    private bool PlayerIsGrounded()
    {
        return (_isGrounded && Physics.Raycast(_playerCenter.position, Vector3.down, _playerCenter.localPosition.y + 0.5f, _groundMask));
    }

    private bool PlayerNotGrounded()
    {
        return !Physics.Raycast(_playerCenter.position, Vector3.down, _playerCenter.localPosition.y + 0.7f, _groundMask);
    }

    private bool InteractablePresent()
    {
        return Physics.Raycast(_camera.transform.position, _camera.transform.forward, 7f, _interactableMask);
    }

    public Transform GetPlayerCenterTransform()
    {
        return _playerCenter;
    }

    private void Respawn()
    {
        transform.position = _playerSpawnPoint;
    }
}

public enum MovementStates
{
    None,
    Crouch,
    Walk,
    Sprint,
    Climb
};
