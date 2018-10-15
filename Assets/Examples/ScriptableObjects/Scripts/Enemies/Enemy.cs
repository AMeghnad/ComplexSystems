using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Examples.ScriptableObjects
{
    public class Enemy : MonoBehaviour
    {
        public int damage = 10;
        public int health = 100;
        public float attackRate = 1f;

        public Transform target;
        public NavMeshAgent agent;

        // Use this for initialization
        void Start()
        {
            StartCoroutine(AttackDelay());
        }

        // Update is called once per frame
        void Update()
        {

        }

        private IEnumerator AttackDelay()
        {
            Attack();
            yield return new WaitForSeconds(attackRate);
            StartCoroutine(AttackDelay());
        }
        public virtual void Attack()
        {
            // Gets called every attackRate
            // Override this function to change what attack does
            Debug.Log("This does NOTHING");
        }
    }
}


