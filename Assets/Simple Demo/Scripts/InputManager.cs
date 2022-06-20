using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviourSingleton<InputManager>
{
    public delegate void StartEvent(InputAction.CallbackContext ctx);
    public delegate void PerformEvent(InputAction.CallbackContext ctx);
    public delegate void CancelEvent(InputAction.CallbackContext ctx);

    public event CancelEvent EndMoveEvent;
    public event PerformEvent PerformMoveEvent;

    public event StartEvent StartMoveEvent;


    private SimpleControls simpleControl;

    private void Awake()
    {
        simpleControl = new SimpleControls();
    }

    private void OnEnable()
    {
        simpleControl.Enable();
        TouchSimulation.Enable();
    }

    private void OnDisable()
    {
        simpleControl.Disable();
        TouchSimulation.Disable();
    }

    private void Start()
    {
        simpleControl.gameplay.Camera_Rotate.started += RotateCamera;

        simpleControl.gameplay.move.started += OnStartMove;
        simpleControl.gameplay.move.performed += OnPerformMove;
        simpleControl.gameplay.move.canceled += OnEndMove;
    }

    private void OnEndMove(InputAction.CallbackContext ctx)
    {
        if (EndMoveEvent != null) EndMoveEvent(ctx);
    }

    private void OnPerformMove(InputAction.CallbackContext ctx)
    {
        if (PerformMoveEvent != null) PerformMoveEvent(ctx);
    }

    private void OnStartMove(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (StartMoveEvent != null) StartMoveEvent(ctx);
    }

    private void RotateCamera(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        // if(OnStartTouch!=null)OnstartTouch(ctx);
    }
}
