using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayerInput : NetworkBehaviour
{
    public PlayerController controller;
    public Orbit cam;
    // Use this for initialization
    void Start()
    {
        if (!isLocalPlayer)
        {
            cam.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            float inputH = Input.GetAxis("Horizontal");
            float inputV = Input.GetAxis("Vertical");
            controller.Move(inputH, inputV);
        }
    }
}
