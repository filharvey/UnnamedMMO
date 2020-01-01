using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO
{
    public class MMOGameManager : NetworkBehaviour
    {
        static MMOGameManager _instance;

        public Dictionary<NetworkConnection, MMOPlayer> players = new Dictionary<NetworkConnection, MMOPlayer>();
        public Dictionary<uint, MMOPlayer> clientPlayers = new Dictionary<uint, MMOPlayer>();

        public static MMOGameManager instance
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

        public override void OnStartServer()
        {
            _instance = this;
        }

        void FixedUpdate()
        {
            if (isServer)
            {

            }
        }

        // server only functions for adding players
        public void addPlayer (MMOPlayer player, NetworkConnection conn)
        {
            if (isServer)
            {
                players.Add(conn, player);
            }
        }

        public MMOPlayer getPlayer(NetworkConnection conn)
        {
            if (isServer)
            {
                return players[conn];
            }

            return null;
        }

        public void removePlayer(NetworkConnection conn)
        {
            if (isServer)
            {
                players.Remove(conn);
            }
        }

        // example to spawn an object
        void spawnObject(Vector3 position, Vector3 speed)
        {
            // Set up coin on server
            var coin = MMOProjectileManager.instance.GetFromPool(position);
            var rigidbody = coin.GetComponent<Rigidbody>();

            if (rigidbody)
            {
                rigidbody.velocity = speed;
            }

            // spawn coin on client, custom spawn handler is called
            NetworkServer.Spawn(coin, MMOProjectileManager.instance.assetId);

            // when the coin is destroyed on the server, it is automatically destroyed on clients
            StartCoroutine(Destroy(coin, 2.0f));
        }

        public IEnumerator Destroy(GameObject go, float timer)
        {
            yield return new WaitForSeconds(timer);
            MMOProjectileManager.instance.UnSpawnObject(go);
            NetworkServer.UnSpawn(go);
        }
    }
}