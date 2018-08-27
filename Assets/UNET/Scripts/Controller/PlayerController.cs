using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 50f;
    public Rigidbody rigid;
    public GameObject cam;

    // Update is called once per frame
    public void Move(float inputH, float inputV)
    {
        Vector3 force = new Vector3(inputH, 0, inputV);
        force = transform.TransformDirection(force);
        rigid.AddForce(force * speed);
    }
}
