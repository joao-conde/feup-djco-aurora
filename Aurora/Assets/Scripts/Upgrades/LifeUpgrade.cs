using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeUpgrade : Upgrade {
    [Header("Specific")]
    public int healthGain;
    private int[] healthGainByLevel = {5, 10, 15, 20, 25};

    public GameObject shieldChargeObject;
    private List<GameObject> shields = new List<GameObject>();
    private bool canActivateShield = false;

    private void Start() {
        this.type = Type.Life;
    }

    private void Update() {
        if (!IsInvoking("Passive") && this.level > 0)
            InvokeRepeating("Passive", this.tick, this.tick);
    }


    public override void Active() {
        if (this.canActivateShield) {
            return;
        }

        for (int i = 0; i < 3; i++) {
            GameObject obj = Instantiate(shieldChargeObject, transform.position, Quaternion.identity);
            obj.transform.SetParent(transform.root);
            this.shields.Add(obj);
        }
        this.canActivateShield = true;
    }

    public override void Passive() {
        GetComponentInParent<PlayerController>().UpdateAttribute(GameManager.Attributes.Health, this.healthGain);

        foreach (ParticleSystem particle in GetComponentsInChildren<ParticleSystem>()) {
            particle.Play();
        }
    }

    public void RestoreActiveCharges() =>
        this.canActivateShield = false;

    public void BreakShield() {
        if (this.shields.Count > 0) {
            Destroy(this.shields[0]);
            this.shields.RemoveAt(0);
        } 
    }

    public bool HasShieldActive() =>
        this.shields.Count > 0;

    public override void LevelUp() {
        // Attempt to upgrade level and make every upgrade status change.
        if (this.UpgradeLevel()) {
            this.healthGain = this.healthGainByLevel[this.level];  // Gem specific logic.
        }
    } 
}