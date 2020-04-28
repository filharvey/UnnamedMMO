using UnityEngine;
using Mirror;
using BestHTTP;
using Acemobe.MMO.UI;
using Acemobe.MMO.Objects;
using SimpleJSON;
using System.Collections.Generic;

namespace Acemobe.MMO
{
    public class MMOCharacterCreateMessage : MessageBase
    {
        public string username;
        public string hash;
    }

    public class MMONetworkManager : NetworkManager
    {
        public override void OnStartServer()
        {
            base.OnStartServer();

            NetworkServer.RegisterHandler<MMOCharacterCreateMessage>(OnCreateCharacter);
        }

        public override void OnStartClient ()
        {
            base.OnStartClient();

            UIManager.instance.gameUI.gameObject.SetActive(true);

            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                UIManager.instance.mobileUI.SetActive(true);
            }
            else
            {
                UIManager.instance.mobileUI.SetActive(false);
            }
        }

        // client
        public override void OnClientConnect(NetworkConnection conn)
        {
            Debug.Log("OnClientConnect");
            base.OnClientConnect(conn);

            MMOCharacterCreateMessage characterMessage = new MMOCharacterCreateMessage
            {
                username = MMOPlayer.userName,
                hash = MMOPlayer.userHash
            };

            conn.Send(characterMessage);
        }

        // server handler
        void OnCreateCharacter(NetworkConnection conn, MMOCharacterCreateMessage message)
        {
            Debug.Log("OnCreateCharacter");

            Transform startPos = GetStartPosition();
            float dist = 10;

            // get random position around the world
            float x = Random.Range(-dist, dist);
            float z = Random.Range(-dist, dist);

            Vector3 pos = startPos.position;
            Quaternion rotation = startPos.rotation;

            GameObject gameobject = Instantiate(playerPrefab, new Vector3(x, 0.1f, z), startPos.rotation);
            MMOPlayer player = gameobject.GetComponent<MMOPlayer>();
            player.name = message.username;
            player.characterInfo = MMOGameManager.instance.userData[message.username];
            player.characterInfo.setPlayer(player, message.username, message.hash);

            player.torso = player.characterInfo.torso;
            player.bottom = player.characterInfo.bottom;
            player.hand = player.characterInfo.hand;
            player.feet = player.characterInfo.feet;
            player.belt = player.characterInfo.belt;
            player.head = player.characterInfo.head;
            player.health = 100;

            // call this to use this gameobject as the primary controller
            NetworkServer.AddPlayerForConnection(conn, player.gameObject);
            MMOGameManager.instance.addPlayer(player, conn);
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            Transform startPos = GetStartPosition();
            float dist = 20;

            // get random position around the world
            float x = UnityEngine.Random.Range(-dist, dist);
            float z = UnityEngine.Random.Range(-dist, dist);

            Vector3 pos = startPos.position;
            Quaternion rotation = startPos.rotation;

            pos.x = x;
            pos.y = 1.6f;
            pos.z = z;

            // get starting spawn point
            GameObject player = Instantiate(playerPrefab, pos, startPos.rotation);
            NetworkServer.AddPlayerForConnection(conn, player);
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            MMOPlayer player = MMOGameManager.instance.getPlayer(conn);

            if (player)
            {
                NetworkServer.Destroy (player.serverobj.gameObject);
            }

            MMOGameManager.instance.removePlayer(conn);

            // call base functionality (actually destroys the player)
            base.OnServerDisconnect(conn);
        }

        public void Update()
        {
            
        }
    }
}

