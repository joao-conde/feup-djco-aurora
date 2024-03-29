﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float sensitivity;               //input sensitivity multiplier
    
    private GameObject target;
    private Vector3 offset;                         //camera-target distance to be kept
    private float pitch = 0f;                       //rotation around X axis
    private float yaw = 0f;                         //rotation around Y axis
    private float minPitch = -5f, maxPitch = 85f;
    private float cameraRotationSpeed = 4f;         //how fast camera rotates, the slower the smoother

    private PlayerController controller;

    void Start() {
        target = GameObject.FindWithTag("Player");
        this.controller = this.target.GetComponent<PlayerController>();
        offset = target.transform.position - transform.position;
    }

    void Update() {
        if (!this.controller.IsDead()) {
            yaw += sensitivity * (Input.GetAxis("Mouse X") + Input.GetAxis("Joystick X"));
            pitch += sensitivity * (Input.GetAxis("Mouse Y") + Input.GetAxis("Joystick Y"));
            UpdateCameraTransform();
        }
    }

    void UpdateCameraTransform(){
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch); 
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(pitch, yaw, 0f)), cameraRotationSpeed * Time.deltaTime);
        transform.position = target.transform.position - offset.magnitude * transform.forward - Vector3.up * offset.y;
    }

    void Recenter(){
        pitch = target.transform.localEulerAngles.x;
        yaw = target.transform.localEulerAngles.y;
    }
}