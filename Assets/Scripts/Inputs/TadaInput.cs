using UnityEngine;


public class TadaInput : MonoBehaviour
{
    [TextArea(2, 10)]
    public string notes = "This class have public static input variables and methods that can be easily accessed by any other class.";

    private readonly static bool debug = false;

    public static bool IsMouseActive { get { return _isMouseActive; } private set { _isMouseActive = value; } }
    private static bool _isMouseActive;

    public static Vector3 MouseInput { get { return _MouseInput; } private set { _MouseInput = value; } }
    private static Vector3 _MouseInput;

    public static Vector3 MousePixelPos { get { return _MousePixelPos; } private set { _MousePixelPos = value; } }
    private static Vector3 _MousePixelPos;

    public static Vector3 MouseWorldPos { get { return _MouseWorldPos; } private set { _MouseWorldPos = value; } }
    private static Vector3 _MouseWorldPos;

    public enum ThisKey
    { 
        None, MoveLeft, MoveRight, MoveUp, MoveDown, PrimaryAction, SecondaryAction,
        PreviousWeapon, NextWeapon, PreviousUseRate, NextUseRate, 
        MouseAnyMovement, Dash, Pause, Count
    }
    private static ThisKey[] currentKeys;
    private static ThisKey[] currentKeysDown;
    private static ThisKey[] currentKeysUp;
    private static bool[] currentAxisDown;

    public static Vector2 MoveAxisSmoothInput { get { return _MoveAxisSmoothInput; } private set { _MoveAxisSmoothInput = value; } }
    private static Vector2 _MoveAxisSmoothInput;

    public static Vector2 MoveAxisRawInput { get { return _MoveAxisRawInput; } private set { _MoveAxisRawInput = value; } }
    private static Vector2 _MoveAxisRawInput;

    public static Vector2 AimAxisSmoothInput { get { return _AimAxisSmoothInput; } private set { _AimAxisSmoothInput = value; } }
    private static Vector2 _AimAxisSmoothInput;

    public static Vector2 AimAxisRawInput { get { return _AimAxisRawInput; } private set { _AimAxisRawInput = value; } }
    private static Vector2 _AimAxisRawInput;

    private bool isScrollWheelActive;
   
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        InitializeInputArrays();
    }

    private void Update()
    {
        if (_MouseInput.sqrMagnitude > 0)
            if (!_isMouseActive)
                _isMouseActive = true;

        _MouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (_MouseInput.magnitude > 1)
            _MouseInput *= (100f / _MouseInput.magnitude) / 100f;

        _MousePixelPos = Input.mousePosition;
        _MousePixelPos.z = 20f;
        _MouseWorldPos = cam.ScreenToWorldPoint(_MousePixelPos);
        _MouseWorldPos.z = 0f;

        _MoveAxisSmoothInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // NOTE: I should add this axis clamping to a Utility class.
        // Clamp axis magnitude to have a value that doesn't go higher than 1 if it's a diagonal vector.
        if (_MoveAxisSmoothInput.magnitude > 1)
            _MoveAxisSmoothInput *= (100f / _MoveAxisSmoothInput.magnitude)/100f;

        _MoveAxisRawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Clamp axis magnitude to have a value that doesn't go higher than 1 if it's a diagonal vector.
        if (_MoveAxisRawInput.magnitude > 1)
            _MoveAxisRawInput *= (100f / _MoveAxisRawInput.magnitude) / 100f;

        _AimAxisSmoothInput = new Vector2(Input.GetAxis("HorizontalAim"), Input.GetAxis("VerticalAim"));

        if (_AimAxisRawInput.magnitude > 1)
            _AimAxisSmoothInput *= (100f / _AimAxisSmoothInput.magnitude) / 100f;

        _AimAxisRawInput = new Vector2(Input.GetAxisRaw("HorizontalAim"), Input.GetAxisRaw("VerticalAim"));

        // Clamp axis magnitude to have a value that doesn't go higher than 1 if it's a diagonal vector.
        if (_AimAxisRawInput.magnitude > 1)
            _AimAxisRawInput *= (100f / _AimAxisRawInput.magnitude) / 100f;

        if (Input.GetKey(KeyCode.A))
            StoreCurrentKey(ThisKey.MoveLeft);

        if (Input.GetKey(KeyCode.D))
            StoreCurrentKey(ThisKey.MoveRight);

        if (Input.GetKey(KeyCode.W))
            StoreCurrentKey(ThisKey.MoveUp);

        if (Input.GetKey(KeyCode.S))
            StoreCurrentKey(ThisKey.MoveDown);

        if (Input.GetMouseButton(0))
            StoreCurrentKey(ThisKey.PrimaryAction);

        if (Input.GetMouseButton(1))
            StoreCurrentKey(ThisKey.SecondaryAction);

        if (Input.GetKeyDown(KeyCode.A))
            StoreCurrentKeyDown(ThisKey.MoveLeft);

        if (Input.GetKeyDown(KeyCode.D))
            StoreCurrentKeyDown(ThisKey.MoveRight);

        if (Input.GetKeyDown(KeyCode.W))
            StoreCurrentKeyDown(ThisKey.MoveUp);

        if (Input.GetKeyDown(KeyCode.S))
            StoreCurrentKeyDown(ThisKey.MoveDown);

        if (Input.GetKeyDown(KeyCode.Space))
            StoreCurrentKeyDown(ThisKey.Dash);

        if (Input.GetKeyDown(KeyCode.C))
            StoreCurrentKeyDown(ThisKey.PreviousUseRate);

        if (Input.GetKeyDown(KeyCode.V))
            StoreCurrentKeyDown(ThisKey.NextUseRate);

        if (Input.GetKeyDown(KeyCode.E))
            StoreCurrentKeyDown(ThisKey.NextWeapon);

        if (Input.GetKeyDown(KeyCode.Q))
            StoreCurrentKeyDown(ThisKey.PreviousWeapon);

        if (Input.GetMouseButtonDown(0))
            StoreCurrentKeyDown(ThisKey.PrimaryAction);

        if (Input.GetMouseButtonDown(1))
            StoreCurrentKeyDown(ThisKey.SecondaryAction);

        if (Input.GetKeyDown(KeyCode.Escape))
            StoreCurrentKeyDown(ThisKey.Pause);

        if (Input.mouseScrollDelta.y > 0)
        {
            isScrollWheelActive = true;
            StoreCurrentKeyDown(ThisKey.NextWeapon);
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            isScrollWheelActive = true;
            StoreCurrentKeyDown(ThisKey.PreviousWeapon);
        }

        if (Input.GetKeyUp(KeyCode.A))
            StoreCurrentKeyUp(ThisKey.MoveLeft);

        if (Input.GetKeyUp(KeyCode.D))
            StoreCurrentKeyUp(ThisKey.MoveRight);

        if (Input.GetKeyUp(KeyCode.W))
            StoreCurrentKeyUp(ThisKey.MoveUp);

        if (Input.GetKeyUp(KeyCode.S))
            StoreCurrentKeyUp(ThisKey.MoveDown);

        if (Input.GetKeyUp(KeyCode.Space))
            StoreCurrentKeyUp(ThisKey.Dash);

        if (Input.GetKeyUp(KeyCode.C))
            StoreCurrentKeyUp(ThisKey.PreviousUseRate);

        if (Input.GetKeyUp(KeyCode.V))
            StoreCurrentKeyUp(ThisKey.NextUseRate);

        if (Input.GetKeyUp(KeyCode.V))
            StoreCurrentKeyUp(ThisKey.NextUseRate);

        if (Input.GetKeyUp(KeyCode.E))
            StoreCurrentKeyUp(ThisKey.NextWeapon);

        if (Input.GetKeyUp(KeyCode.Q))
            StoreCurrentKeyUp(ThisKey.PreviousWeapon);

        if (Input.GetMouseButtonUp(0))
            StoreCurrentKeyUp(ThisKey.PrimaryAction);

        if (Input.GetMouseButtonUp(1))
            StoreCurrentKeyUp(ThisKey.SecondaryAction);

        if (Input.GetKeyUp(KeyCode.Escape))
            StoreCurrentKeyUp(ThisKey.Pause);

        // TODO: Improve mouseScrollDelta input conditions.
        if (Input.mouseScrollDelta.y == 0 && isScrollWheelActive)
        {
            StoreCurrentKeyUp(ThisKey.NextWeapon);
            StoreCurrentKeyUp(ThisKey.PreviousWeapon);
            isScrollWheelActive = false;
        }
    }

    public static bool GetKey(ThisKey key)
    {
        int index = (int)key;

        if (currentKeys[index] == key)
        {
            if (debug)
                Debug.Log("Key: " + key.ToString());
            return true;
        }
        return false;
    }

    public static bool GetKeyDown(ThisKey key)
    {
        int index = (int)key;
        if (currentKeysDown[index] == key)
        {
            currentKeysDown[index] = ThisKey.None;
            if (debug)
                Debug.Log("KeyDown: " + key.ToString());
            return true;
        }
        return false;
    }

    public static bool GetKeyUp(ThisKey key)
    {
        int index = (int)key;
        if (currentKeysUp[index] == key)
        {
            currentKeysUp[index] = ThisKey.None;
            if (debug)
                Debug.Log("KeyUp: " + key.ToString());
            return true;
        }
        return false;
    }

    private static void InitializeInputArrays()
    {
        int length = (int)ThisKey.Count + 1;
        currentKeys = new ThisKey[length];
        currentKeysDown = new ThisKey[length];
        currentKeysUp = new ThisKey[length];
        currentAxisDown = new bool[length];

        for (int i = 0; i < length; i++)
        {
            currentKeys[i] = ThisKey.None;
            currentKeysDown[i] = ThisKey.None;
            currentKeysUp[i] = ThisKey.None;
            currentAxisDown[i] = false;
        }
    }

    private static void StoreCurrentKey(ThisKey key)
    {
        currentKeys[(int)key] = key;
    }

    private static void StoreCurrentKeyDown(ThisKey key)
    {
        currentKeysDown[(int)key] = key;
    }

    private static void StoreCurrentKeyUp(ThisKey key)
    {
        currentKeysUp[(int)key] = key;
        currentKeysDown[(int)key] = ThisKey.None;
        currentKeys[(int)key] = ThisKey.None;
    }

    private static void StoreCurrentAxisAsKeyType(ThisKey key, float rawAxisValue)
    {
        int index = (int)key;
        if (rawAxisValue > 0) 
        {
            if (!currentAxisDown[index]) // DOWN
            {
                currentAxisDown[index] = true;
                StoreCurrentKeyDown(key);
            }
            StoreCurrentKey(key); // HOLD
        }
        if (rawAxisValue == 0 && currentAxisDown[index]) // UP
        {
            currentAxisDown[index] = false;
            StoreCurrentKeyUp(key);
        }
    }
}
