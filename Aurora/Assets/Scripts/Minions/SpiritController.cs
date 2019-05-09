﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiritController : MonoBehaviour {
    private Transform player;

    private float isCollectableCountdown = 2.0f;

    void Start() {
        this.player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnDisable() {
        this.isCollectableCountdown = 2.0f;  // Reset the collectable countdown because of object pool reutilization.
    }

    void Update() {
        if (this.isCollectableCountdown > 0.0f) {
            this.isCollectableCountdown -= Time.deltaTime;
        }

        if (this.isCollectableCountdown <= 0.0f) {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            // Collect spirit.
            if (distance >= 0.0f && distance < 0.50f) {
                GameObject.Find("HUDCanvas").GetComponent<HUDUpdater>().UpdateSlider("EssenceUI", 1);
                ObjectPooler.SharedInstance.FreePooledObject(this.gameObject);
            }
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * 100.0f * 1.5f / distance);
        }
    }

    public void PositionSelf(Transform minion) {
        this.transform.position = minion.position;
    }
}
