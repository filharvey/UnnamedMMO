﻿using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO
{
    public class MMOFakePlayer : NetworkBehaviour
    {
        [Header("Components")]
        public GameObject owner;
        public Rigidbody rigidBody;
        public GameObject mesh;
        public Animator animator;

        [Header("Params")]
        [SyncVar]
        public string head;
        [SyncVar]
        public string torso;
        [SyncVar]
        public string bottom;
        [SyncVar]
        public string feet;
        [SyncVar]
        public string hand;
        [SyncVar]
        public string belt;

        [SyncVar]
        public Weapon weapon;
        [SyncVar]
        public float health;

        [SyncVar]
        public bool isFiring = false;
        [SyncVar]
        public bool isMoving = false;

        [SyncVar]
        public float lastFire = 0f;
        [SyncVar]
        public float moveHoriz = 0f;
        [SyncVar]
        public float moveVert = 0f;
        [SyncVar]
        public float lookAngle = 0f;

        public float cameraRotation = 0;

        public float speed = 300;

        [SyncVar]
        public uint netIdOwner;

        public Dictionary<string, Transform> weapons = new Dictionary<string, Transform>();

        public MMOPlayer player;

        public override void OnStartServer()
        {
            base.OnStartServer();

            rigidBody.isKinematic = false;

            Debug.Log("OnStartServer," + torso + "," + head);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            // find parent object
            Debug.Log("OnStartClient," + torso + "," + this.head);

            for (var a = 0; a < mesh.transform.childCount; a++)
            {
                var child = mesh.transform.GetChild(a);

                if (child.gameObject.name.Contains("Torso") ||
                    child.gameObject.name.Contains("Bottom") ||
                    child.gameObject.name.Contains("Feet") ||
                    child.gameObject.name.Contains("Hand") ||
                    child.gameObject.name.Contains("Belt"))
                {
                    child.gameObject.SetActive(false);
                }
            }

            Transform head = mesh.transform.Find(this.head);
            Transform body = mesh.transform.Find(this.torso);
            Transform bottom = mesh.transform.Find(this.bottom);
            Transform feet = mesh.transform.Find(this.feet);
            Transform hands = mesh.transform.Find(this.hand);
            Transform belt = mesh.transform.Find(this.belt);

            weapons.Add("pickaxe", mesh.transform.Find("RigPelvis/RigSpine1/RigSpine2/RigSpine3/RigRibcage/RigLCollarbone/RigLUpperarm/RigLForearm/RigLPalm/Pick Axe 01"));
            weapons.Add("axe", mesh.transform.Find("RigPelvis/RigSpine1/RigSpine2/RigSpine3/RigRibcage/RigLCollarbone/RigLUpperarm/RigLForearm/RigLPalm/Axe 01"));
            weapons.Add("hammer", mesh.transform.Find("RigPelvis/RigSpine1/RigSpine2/RigSpine3/RigRibcage/RigLCollarbone/RigLUpperarm/RigLForearm/RigLPalm/Hammer 01"));
            weapons.Add("sword", mesh.transform.Find("RigPelvis/RigSpine1/RigSpine2/RigSpine3/RigRibcage/RigLCollarbone/RigLUpperarm/RigLForearm/RigLPalm/Sword 01"));
            weapons.Add("spade", mesh.transform.Find("RigPelvis/RigSpine1/RigSpine2/RigSpine3/RigRibcage/RigLCollarbone/RigLUpperarm/RigLForearm/RigLPalm/Spade 01"));

            foreach (var weapon in weapons)
            {
                weapon.Value.gameObject.SetActive(false);
            }

            if (body)
            {
                body.gameObject.SetActive(true);
            }

            if (bottom)
            {
                bottom.gameObject.SetActive(true);
            }

            if (feet)
            {
                feet.gameObject.SetActive(true);
            }

            if (hands)
            {
                hands.gameObject.SetActive(true);
            }

            if (belt)
            {
                belt.gameObject.SetActive(true);
            }
        }

        void FixedUpdate()
        {
            if (isClient)
            {
                // make sure localPlayer is set
                if (MMOPlayer.localPlayer.serverobj == null &&
                    MMOPlayer.localPlayer.netId == netIdOwner)
                {
                    MMOPlayer.localPlayer.serverobj = this;

                    UILogin.instance.gameObject.SetActive(false);
                }
            }

            if (isServer)
            {
                movePlayer(Time.fixedDeltaTime);
            }
        }

        public void movePlayer(float dt)
        {
            Quaternion quatUp = new Quaternion();
            Quaternion quatLeft = new Quaternion();
            quatUp.eulerAngles = new Vector3(0, cameraRotation - 90, 0);
            quatLeft.eulerAngles = new Vector3(0, cameraRotation, 0);

            Vector3 up = quatUp * Vector3.forward;
            Vector3 left = quatLeft * Vector3.forward;

            Vector3 move = (up * moveVert) + (left * moveHoriz);
            move.Normalize();
            move *= (speed * Time.fixedDeltaTime);

            bool newMove = (move != Vector3.zero);
            rigidBody.velocity = move;

            // send move flag
            if (isMoving != newMove)
            {
                isMoving = newMove;
                animator.SetBool("Walk", isMoving);
            }

            var rotation = transform.rotation;
            if (isMoving)
            {
                // UP = 90
                // Right = 180
                // Down = 270
                // Left = 0

                float angle = Mathf.Atan2(-moveHoriz, -moveVert) * 180 / Mathf.PI;
                rotation.eulerAngles = new Vector3(0, angle + 90, 0);

                Debug.Log(angle);

/*                if (moveVert < 0)
                {
                    if (moveHoriz > 0)
                        rotation.eulerAngles = new Vector3(0, 45, 0);
                    else if (moveHoriz < 0)
                        rotation.eulerAngles = new Vector3(0, 135, 0);
                    else
                        rotation.eulerAngles = new Vector3(0, 90, 0);
                }
                else if (moveVert > 0)
                {
                    if (moveHoriz > 0)
                        rotation.eulerAngles = new Vector3(0, 315, 0);
                    else if (moveHoriz < 0)
                        rotation.eulerAngles = new Vector3(0, 225, 0);
                    else
                        rotation.eulerAngles = new Vector3(0, 270, 0);
                }
                else
                {
                    if (moveHoriz > 0)
                        rotation.eulerAngles = new Vector3(0, 0, 0);
                    else if (moveHoriz < 0)
                        rotation.eulerAngles = new Vector3(0, 180, 0);
                }*/
            }

            rigidBody.angularVelocity = Vector3.zero;
            transform.rotation = rotation;
        }

        [ServerCallback]
        void OnTriggerEnter(Collider co)
        {
            health -= 10;
        }

        public void AnimComplete()
        {
            Debug.Log("AnimComplete");
            if (isServer)
            {
                if (player)
                {
                    player.AnimComplete();
                }
            }
        }
    }
}