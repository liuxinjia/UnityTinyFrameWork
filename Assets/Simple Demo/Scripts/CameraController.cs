using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Configurable Properties")]
    [Tooltip("This is the Y offset of our focal point. 0 Means we're looking at the ground.")]
    public float LookOffset = 1f;
    [Tooltip("The angle that we want the camera to be at.")]
    public float CameraAngle = 45;
    [Tooltip("The default amount the player is zoomed into the game world.")]
    public float DefaultZoom = 5;
    [Tooltip("The most a player can zoom in to the game world.")]
    public float ZoomMax = 2;
    [Tooltip("The furthest point a player can zoom back from the game world.")]
    public float ZoomMin = 10;
    [Tooltip("How fast the camera rotates")]
    public float RotationSpeed = 2;


    #region Private variables
    #region Camera specific variables
    private Camera _actualCamera;
    private Vector3 _cameraPositionTarget;

    #endregion

    #region Movement variables
    private const float InternalMoveTargetSpeed = 8;
    private const float InternalMoveSpped = 4;
    private Vector3 _moveTarget;
    private Vector3 _moveDirection;

    #endregion

    #endregion

    #region MonoBehaviour
    private void Start()
    {
        // Store a reference to the actual camera right now
        _actualCamera = GetComponent<Camera>();

        // Set the rotation of the camera based on the CameraAngle property 
        _actualCamera.transform.rotation = Quaternion.AngleAxis(CameraAngle, Vector3.right);

        // Set the position of the camera based on the look offset, angle and default zoom porperties.
        // This will make sure we're focusing on the right focal point
        _cameraPositionTarget = (Vector3.up * LookOffset) +
          _actualCamera.transform.rotation * Vector3.back * DefaultZoom;

        _actualCamera.transform.position = _cameraPositionTarget;

    }

    private void OnEnable()
    {
        InputManager.Instance.StartMoveEvent += OnMove;
    }

    private void OnDisable()
    {
        InputManager.Instance.StartMoveEvent -= OnMove;
    }

    private void lateUpdate()
    {
        // Lerp the cameraa to a new move target position
        // transform.position = Vector3.Lerp(transform.position, _moveTarget, Time.deltaTime * InternalMoveSpped);
    }

    #endregion

    #region Private Methods
    /// <summary>
    /// Sets the direction of movement based on the input provided by the player
    /// </summary>
    private void OnMove(InputAction.CallbackContext ctx)
    {
        // Read the input value that is being sent by the Input System
        var value = ctx.ReadValue<Vector2>();

        //Strore the value as Vector3, making sure to move the Y input on the Z axis
        _moveDirection = new Vector3(value.x, 0, value.y);

        //Increment the new move Target position of the camera
        _moveTarget += (transform.forward * _moveDirection.z + transform.right * _moveDirection.x)
            * Time.deltaTime * InternalMoveTargetSpeed;
    }
    #endregion
}
