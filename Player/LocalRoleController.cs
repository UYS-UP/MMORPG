using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using Unity.VisualScripting;
using UnityEngine;

public struct PlayerInputState
{
    public long Timestamp;
    public float MoveDirection;
    public float Yaw;
    public Vector3 Position;
}

public class LocalRoleController
{
    private NetworkEntity entity;
    private CharacterController characterController;
    private Animator animator;

    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float predictionErrorTolerance = 0.5f;
    [SerializeField] private bool isProcessingCorrection;
    
    private readonly Queue<PlayerInputState> inputQueue = new Queue<PlayerInputState>();
    private float sendInterval = 0.1f;
    private float currentMoveDirection;
    private float currentYaw;
    private float lastSendTime;

    private void Awake()
    {
        entity = GetComponent<NetworkEntity>();
        characterController = GetComponent<CharacterController>();
        currentYaw = transform.eulerAngles.y;
    }

    private void Update()
    {
        ProcessPlayerInput();
        ApplyLocalPrediction();
        SendMovementToServer();
    }


    private void ProcessPlayerInput()
    {
        float forwardInput = Input.GetKey(KeyCode.W) ? 1f : 0f;
        float backwardInput = Input.GetKey(KeyCode.S) ? -1f : 0f;
        float leftInput = Input.GetKey(KeyCode.A) ? -0.7f : 0f;
        float rightInput = Input.GetKey(KeyCode.D) ? 0.7f : 0f;
        
        currentMoveDirection = forwardInput + backwardInput;
        if (Mathf.Approximately(currentMoveDirection, 0)) {
            currentMoveDirection = leftInput + rightInput;
        }
        
    }
}
