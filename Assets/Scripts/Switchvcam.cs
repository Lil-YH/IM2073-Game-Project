using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class Switchvcam : MonoBehaviour
{

    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private int priorityBoostAmount = 10;
    [SerializeField]
    private Canvas aimCanvas;
    [SerializeField]
    private Canvas thirdPersonCanvas;
    public PlayerController controller;
    public CinemachineVirtualCamera sprintCam;

    private CinemachineVirtualCamera aimCamera;
    private InputAction aimAction;
    private InputAction sprintAction;
    private bool isSprint;
    

    private void Awake()
    {
        aimCamera = GetComponent<CinemachineVirtualCamera>();
        aimAction = playerInput.actions["Aim"];
        sprintAction = playerInput.actions["Sprint"];
        aimCanvas.enabled = false;
        thirdPersonCanvas.enabled = true;
        isSprint = false;
    }
    private void OnEnable()
    {
        aimAction.performed += _ => StartAim();
        aimAction.canceled += _ => CancelAim();
        sprintAction.performed += _ => StartSprint();
        sprintAction.canceled += _ => CancelSprint();
    }
    private void OnDisable()
    {
        aimAction.performed -= _ => StartAim();
        aimAction.canceled -= _ => CancelAim();
        sprintAction.performed += _ => StartSprint();
        sprintAction.canceled += _ => CancelSprint();
    }
    private void StartSprint()
    {
        isSprint = true;
        if (aimCamera.Priority == 19)
        {
            aimCanvas.enabled = false;
            aimCamera.Priority -= priorityBoostAmount;
        }
        sprintCam.Priority += priorityBoostAmount;
        aimCanvas.enabled = false;
        thirdPersonCanvas.enabled = true;
        
    }
    private void CancelSprint()
    {
        isSprint = false;
        sprintCam.Priority -= priorityBoostAmount;
        
    }
    private void StartAim()
    {
        if (sprintCam.Priority != 19)
        {
            controller.playerSpeed = controller.aimSpeed;
            aimCamera.Priority += priorityBoostAmount;
            aimCanvas.enabled = true;
            thirdPersonCanvas.enabled = false;
        }
    }
    private void CancelAim()
    {
        if (sprintCam.Priority != 19 && aimCamera.Priority == 19)
        {
            controller.playerSpeed = controller.baseSpeed;
            thirdPersonCanvas.enabled = true;
            aimCanvas.enabled = false;
            aimCamera.Priority -= priorityBoostAmount;
        }
    }
}
