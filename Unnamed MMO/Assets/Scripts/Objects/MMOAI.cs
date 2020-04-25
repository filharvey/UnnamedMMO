using UnityEngine;

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

        void doIdleState ()
        {
            stateTimer -= Time.deltaTime;

            // do something
            if (stateTimer < 0)
            {
                float rand = Random.Range(0, 100);

                // walk to new location around the spawn point
                if (rand < 30)
                {

                }
                else
                {
                    // remain idle
                }
            }
        }
    }
}