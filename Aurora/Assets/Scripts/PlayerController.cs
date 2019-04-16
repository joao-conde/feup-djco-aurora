﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody rb;
    private float speed = 5.0f;

    void Start() {
        this.rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        this.rb.MovePosition(transform.position + direction * Time.deltaTime * this.speed);
        
        if (direction != Vector3.zero)
            this.rb.MoveRotation(Quaternion.LookRotation(direction));
    }
}