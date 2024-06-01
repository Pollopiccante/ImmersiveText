using UnityEngine;

public class Player : MonoBehaviour
{
    // editor input
    public float speed = 10f;
    public float cameraTurningSpeed = 15f;
    public float gravity = 30f;
    
    // parameters
    private float _xRotationLimitLower = -90f;
    private float _xRotationLimitUpper = 90f;
    
    // internal game objects
    private CharacterController _characterController;
    private Camera _camera;
    private OptionsController _optionsController;
    
    // internal variables
    private float _xRotation;
    private float _yRotation;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        _characterController = GetComponent<CharacterController>();
        _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        _optionsController = GetComponentInChildren<OptionsController>();
        _optionsController.Exit();
    }
    
    void Update()
    {
        if (Input.GetKeyDown("escape"))
            _optionsController.OpenToggle();
        
        Vector3 forward = transform.forward * Input.GetAxis("Vertical");
        Vector3 right = transform.right * Input.GetAxis("Horizontal");
        Vector3 move = (forward + right) / 2;

        if (_optionsController._flyActive)
        {
            if (Input.GetKey(KeyCode.Space))
                move += Vector3.up;
            if (Input.GetKey(KeyCode.LeftShift))
                move += Vector3.down;
        }
        else
        {
            move += Vector3.down * gravity * Time.deltaTime; // Apply a constant gravity every frame
        }

        _xRotation += cameraTurningSpeed * Time.deltaTime * -Input.GetAxis ("Mouse Y");
        _yRotation += cameraTurningSpeed * Time.deltaTime * Input.GetAxis ("Mouse X");

        if (_xRotation > _xRotationLimitUpper)
            _xRotation = _xRotationLimitUpper;
        if (_xRotation < _xRotationLimitLower)
            _xRotation = _xRotationLimitLower;
        
        
        _characterController.Move(move * speed * Time.deltaTime * 10);
        transform.rotation = Quaternion.Euler(0, _yRotation, 0);

        _camera.transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
    }
}
