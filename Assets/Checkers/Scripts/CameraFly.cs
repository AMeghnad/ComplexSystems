using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Plugins.Curves;

namespace Checkers
{
    public class CameraFly : MonoBehaviour
    {
        public Transform lookTarget;
        public Vector3 offset = new Vector3(-10f, 0, 0);
        public BezierSpline spline;
        public float duration = 1f;
        public SplineWalkerMode mode;

        private float progress;

        // Debugging
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.1f);
        }

        // Update is called once per frame
        void Update()
        {
            // Increment the progress via duration
            progress += Time.deltaTime / duration;
            if (progress > 1f)
            {
                switch (mode)
                {
                    case SplineWalkerMode.Once:
                        //Cap the progress
                        progress = 1f;
                        break;
                    case SplineWalkerMode.Loop:
                        // Start back at zero
                        progress -= 1f;
                        break;
                    case SplineWalkerMode.PingPong:
                        // If all else, do ping pong
                        progress = 2f - progress;
                        break;
                    default:
                        break;
                }
            }
            // Get the current splinepoint based on progress
            transform.localPosition = spline.GetPoint(progress);
        }

        // LateUpdate is called every frame, if the Behaviour is enabled
        private void LateUpdate()
        {
            // If there is a set look target
            if (lookTarget)
            {
                // Look at that transform (rotate to transform)
                transform.LookAt(lookTarget);
                // Then apply offset only on y
                transform.rotation *= Quaternion.AngleAxis(offset.y, Vector3.up);
            }
        }
    }
}

