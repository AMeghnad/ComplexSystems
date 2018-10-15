using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.ScriptableObjects
{
    public class Cannon : Enemy
    {
        public float range = 10f;
        public Transform spawnPoint;
        public GameObject projectilePrefab;

        public override void Attack()
        {
            GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);

        }
    }
}
