using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("目标设置")] 
    public Transform target;

    public Vector3 offset = new Vector3(0, 1.5f, 0);

    [Header("相机设置")] 
    public float distance = 5.0f;
    public float minDistance = 1.0f;
    public float maxDistance = 15.0f;
    public float height = 2.0f;
    public float cameraSoothTime = 0.1f;

    [Header("旋转设置")] 
    public float rotationSpeed = 120.0f;
    public float minVerticalAngle = -30.0f;
    public float maxVerticalAngle = 70.0f;

    [Header("碰撞检测")] 
    public LayerMask collisionLayers;
    public float cameraRadius = 0.2f;
    public float collisionOffset = 0.1f;

    private float currentRotationX;
    private float currentRotationY;
    private Vector3 cameraVelocity;
    private Vector3 desiredPosition;
    

}
