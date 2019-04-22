﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject target;

    private Vector3 offset;
    private float pitch = 0f;   //up/down
    private float yaw = 0f;     //left/right
    private float minPitch = -40f, maxPitch = 60f;
    private float sensitivity = 2f;
    private float timeCount = 0.0f;


    void Start() {
        this.offset = target.transform.position - transform.position;
    }

    void Update() {
        Vector3 directedOffset = offset.magnitude * transform.forward;
        directedOffset.y = offset.y;
        transform.position = target.transform.position - directedOffset;

        float axisXInp = Input.GetAxis("Mouse X");
        float axisYInp = Input.GetAxis("Mouse Y");

        if(axisXInp == 0 && axisYInp == 0){ //Stick released, recenter on player's back
            transform.rotation = Quaternion.Slerp(transform.rotation, target.transform.rotation, timeCount);
            timeCount += Time.deltaTime;
        }
        else{
            timeCount = 0f;
            yaw += sensitivity * axisXInp;
            pitch += sensitivity * axisYInp;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            transform.localEulerAngles = new Vector3(pitch, yaw, 0f);
        }
    }
}
