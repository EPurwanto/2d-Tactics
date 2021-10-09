using System;
using UnityEngine;

namespace Grid
{
    public class MouseController : MonoBehaviour
    {
        public Camera camera;

        private void Start()
        {
            if (!camera)
            {
                camera = Camera.main;
            }
        }

        private void Update()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                var mousePos = Input.mousePosition;
                var worldRay = camera.ScreenPointToRay(mousePos);
                var hit = Physics2D.Raycast(worldRay.origin, worldRay.direction);

                if (hit.collider != null)
                {
                    Debug.Log("Mouse Click hit");
                    hit.collider.SendMessage("HandleClick");
                }
                else
                {

                    Debug.Log("Mouse Click miss");
                }
            }
        }
    }
}
