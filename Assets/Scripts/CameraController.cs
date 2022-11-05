using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float moveSmoothTime = 1f;
    [SerializeField] private float heightSmoothTime = 1f;
    [SerializeField] private float followDist = 5f;
    [SerializeField] private float height = 6f;

    private Vector3 lastPlayerPos;
    private Vector3 moveCurrentVelocity;
    private float heightCurrentVelocity;

    void Start()
    {
        lastPlayerPos = player.transform.position;
    }

    void FixedUpdate()
    {
        lastPlayerPos = player.transform.position;
        Vector3 smoothMovement = Vector3.SmoothDamp(transform.position, lastPlayerPos - Vector3.back * -followDist, ref moveCurrentVelocity, moveSmoothTime);
        float smoothHeight = Mathf.SmoothDamp(transform.position.y, height, ref heightCurrentVelocity, heightSmoothTime);
        transform.position = new Vector3(smoothMovement.x, smoothHeight, smoothMovement.z);
    }
}
