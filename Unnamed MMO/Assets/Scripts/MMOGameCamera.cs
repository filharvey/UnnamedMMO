using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Acemobe.MMO
{
    public class MMOGameCamera : MonoBehaviour
    {
        static MMOGameCamera _instance;

        public float cameraRotation = 15;
        public Vector3 cameraOffset = new Vector3(5, 12, 0);

//        Camera camera;

        public static MMOGameCamera instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        void Start()
        {
            _instance = this;

//            camera = GetComponent<Camera>();
        }

        public float smoothTime = 0.3F;
        private Vector3 velocity = Vector3.zero;

        void LateUpdate()
        {
            float dt = Time.deltaTime;

            if (MMOPlayer.localPlayer && MMOPlayer.localPlayer.serverobj)
            {
                Quaternion quat = new Quaternion();
                quat.eulerAngles = new Vector3 (0, cameraRotation, 0);
                Vector3 off = quat * cameraOffset;

                Vector3 targetPosition = MMOPlayer.localPlayer.serverobj.transform.localPosition + off;

                transform.localPosition = targetPosition;
                transform.LookAt (MMOPlayer.localPlayer.serverobj.transform);

                if (Input.GetMouseButtonDown(0))
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        int layerMask = 1 << 8;

                        RaycastHit hit;
                        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

                        // Does the ray intersect any objects excluding the player layer
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                        {
                            int x = (int)Mathf.Floor(hit.point.x + 0.5f);
                            int z = (int)Mathf.Floor(hit.point.z + 0.5f);

                            // send to server command mouse down
                            MMOPlayer.localPlayer.mouseDown(x, z);
                        }
                    }
                }
                else if (Input.GetMouseButton (0))
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        int layerMask = 1 << 8;

                        RaycastHit hit;
                        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

                        // Does the ray intersect any objects excluding the player layer
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                        {
                            int x = (int)Mathf.Floor(hit.point.x + 0.5f);
                            int z = (int)Mathf.Floor(hit.point.z + 0.5f);

                            // send to server command mouse down
                            MMOPlayer.localPlayer.CmdMouseUpdate(x, z);
                        }
                    }
                }
                else if (!Input.GetMouseButton(0))
                {
                    if (MMOPlayer.localPlayer.isMouseDown)
                    {
                        MMOPlayer.localPlayer.CmdMouseUp();
                    }
                }

                if (Input.GetKeyDown (KeyCode.I))
                {
                    UIManager.instance.inventory.gameObject.SetActive (true);
                    UIManager.instance.inventory.updateInventory();
                }
            }
        }
    }
}