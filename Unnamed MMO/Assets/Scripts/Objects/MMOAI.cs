using Acemobe.MMO.Data;
using Acemobe.MMO.Objects.Tools;
using UnityEngine;
using UnityEngine.AI;

namespace Acemobe.MMO.Objects
{
    public class MMOAI : MMOObject
    {
        enum AI_STATE
        {
            NONE = 0,
            IDLE,
            WALK,
            ATTACK
        }



        public Animator animator;

        public Collider collider;

        public NavMeshObstacle navMeshObs;

        Vector3 spawnLocation;

        Vector3 moveTarget;

        Vector3 attackTarget;

        AI_STATE state;

        float stateTimer;

        public override void OnStartServer()
        {
            base.OnStartServer();

            state = AI_STATE.NONE;
            stateTimer = 0;

        }

        public override void OnStartClient()
        {
            base.OnStartClient();
        }

        void FixedUpdate()
        {
            if (isServer)
            {
                switch (state)
                {
                    case AI_STATE.NONE:
                        state = AI_STATE.IDLE;
                        break;

                    case AI_STATE.IDLE:
                        doIdleState();
                        break;

                    case AI_STATE.WALK:
                        break;

                    case AI_STATE.ATTACK:
                        break;
                }
            }
        }

        void doIdleState ()
        {
            stateTimer -= Time.deltaTime;

            // do something
            if (stateTimer < 0)
            {
                float rand = Random.Range(0, 100);

                if (rand < 30)
                {

                }
                else
                {
                    // remain idle
                }
            }
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (isServer)
            {
                Debug.Log("Hit");
                MMOHeldItem item = collider.gameObject.GetComponent<MMOHeldItem>();

                if (item)
                {
                    if (item.fakePlayer.player.curAction == MMOResourceAction.Attack)
                    {
                        if (this.health > 0 && !animator.GetBool("Hit"))
                        {
                            this.health -= 5;

                            if (this.health > 0)
                                animator.SetBool("Hit", true);
                            else
                            {
                                animator.SetBool("Dead", true);

                                if (collider)
                                    collider.enabled = false;

                                if (navMeshObs)
                                    navMeshObs.enabled = false;
                            }
                        }
                    }
                }
            }
        }

        public override void AnimComplete ()
        {
            animator.SetBool("Hit", false);
        }
    }
}