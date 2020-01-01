using Mirror;
using UnityEngine;

namespace Acemobe.MMO
{
    public class MMOProjectile : NetworkBehaviour
    {
        public float destroyAfter = 5;
        public Rigidbody rigidBody;
        public float force = 1000;
        
        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), destroyAfter);

            rigidBody.isKinematic = false;
        }

        // set velocity for server and client. this way we don't have to sync the
        // position, because both the server and the client simulate it.
        void Start()
        {
            rigidBody.AddForce(transform.forward * force);
        }

        void FixedUpdate()
        {
            Debug.Log(rigidBody.velocity);
        }

        // destroy for everyone on the server
        [Server]
        void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        // ServerCallback because we don't want a warning if OnTriggerEnter is
        // called on the client
        [ServerCallback]
        void OnTriggerEnter(Collider co)
        {
            NetworkServer.Destroy(gameObject);
        }

        public override void OnNetworkDestroy ()
        {
            base.OnNetworkDestroy();
        }
    }
}