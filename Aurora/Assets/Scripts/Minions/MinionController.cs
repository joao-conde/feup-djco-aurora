﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MinionController : MonoBehaviour {
    public float lookRadius = 10f;

    public float speed = 10f;
    private NavMeshAgent agent;
    private Animator anim;

    private Vector3 target;

    [SerializeField]
    private int health = 100;

        

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.speed =  speed;
        agent.acceleration = speed;
    }

    void Update() {
        if (health <= 0) {
            // On death, spawn a spirit in its place.
            GameObject spirit = ObjectPooler.SharedInstance.GetPooledObject("Spirit");
            spirit.GetComponent<SpiritController>().PositionSelf(transform);

            // Despawn minion.
            ObjectPooler.SharedInstance.FreePooledObject(gameObject);
        }
    }

    private void OnDisable() {
        // Reset health so new minions won't have zero health.
        this.health = 100;
    }

    public void ReceiveDamage(int damage) =>
        this.health -= damage;

    public Collider checkForPlayer(){
        Collider[] minionRadius = Physics.OverlapSphere(transform.position, lookRadius);
        foreach (Collider col in minionRadius) {
            if (col.tag == "Player") return col;
        }
        return null;
    }
   
    public void goToPosition(Vector3 pos){
        agent.SetDestination(pos);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,lookRadius);
    }
}
