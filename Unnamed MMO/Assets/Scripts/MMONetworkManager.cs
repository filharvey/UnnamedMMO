using UnityEngine;
using Mirror;
using Acemobe.MMO.UI;
using Acemobe.MMO.Objects;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Acemobe.MMO
{
    public class MMOCharacterCreateMessage : MessageBase
    {
        public string username;
        public string hash;
    }

    public class MMONetworkManager : NetworkManager
    {
        public GameObject smallMap;

        public MMOTerrainManager mainIsland;

        public override void OnStartServer()
        {
            base.OnStartServer();

            NetworkServer.RegisterHandler<MMOCharacterCreateMessage>(OnCreateCharacter);

            mainIsland.gameObject.SetActive(true);
            mainIsland.createTerrain();
            mainIsland.islandMap.owner = "mainIsland";

            // load an island
//            loadIsland(smallMap, 500, 0);
        }

        public override void OnStartClient ()
        {
            base.OnStartClient();

            mainIsland.gameObject.SetActive(true);

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
//            Transform startPos = GetStartPosition();
            GameObject gameobject = Instantiate(playerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
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

        public void loadIsland (GameObject map, int x, int z)
        {
            GameObject gameobject = Instantiate(map, new Vector3(x, 0f, z), Quaternion.identity);
            MMOMap island = gameobject.GetComponent<MMOMap>();

            island.loadMap (x, z);
        }
    }
}

