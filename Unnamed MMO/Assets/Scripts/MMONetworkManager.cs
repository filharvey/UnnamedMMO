using UnityEngine;
using Mirror;
using BestHTTP;
using System;
using System.Collections;
using Acemobe.MMO.UI;

namespace Acemobe.MMO
{
    public enum Weapon
    {
        None,
        Pistol,
        Rifle
    }

    public class MMOCharacterCreateMessage : MessageBase
    {
        public string name;
        public string head;
        public string torso;
        public string bottom;
        public string feet;
        public string hand;
        public string belt;
        public Weapon weapon;
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
            base.OnClientConnect(conn);

            // get user info
            new HTTPRequest(new Uri("http://gnash.io"), (request, response) =>
            {
                if (response.IsSuccess)
                {
                    Debug.Log("Request Finished! Text received: " + response.DataAsText);

                    // you can send the message here, or wherever else you want
                    MMOCharacterCreateMessage characterMessage = new MMOCharacterCreateMessage
                    {
                        name = "Phil",
                        head = "01 Head 01",
                        torso = "02 Torso 02",
                        bottom = "03 Bottom 02",
                        feet = "04 Feet 01",
                        hand = "05 Hand 01",
                        belt = "06 Belt 01",
                        weapon = Weapon.Rifle
                    };

                    conn.Send(characterMessage);
                }
            }).Send();
            /*
                        // you can send the message here, or wherever else you want
                        MMOCharacterCreateMessage characterMessage = new MMOCharacterCreateMessage
                        {
                            name = "Phil",
                            head = "01 Head 01",
                            torso = "02 Torso 02",
                            bottom = "03 Bottom 02",
                            feet = "04 Feet 01",
                            hand = "05 Hand 01",
                            belt = "06 Belt 01",
                            weapon = Weapon.Rifle
                        };

                        conn.Send(characterMessage);
            */
        }

        // server handler
        void OnCreateCharacter(NetworkConnection conn, MMOCharacterCreateMessage message)
        {
            Debug.Log("OnCreateCharacter");

            Transform startPos = GetStartPosition();
            float dist = 10;

            // get random position around the world
            float x = UnityEngine.Random.Range(-dist, dist);
            float z = UnityEngine.Random.Range(-dist, dist);

            Vector3 pos = startPos.position;
            Quaternion rotation = startPos.rotation;

            pos.x = x;
            pos.y = 0.1f;
            pos.z = z;

            GameObject gameobject = Instantiate(playerPrefab, pos, startPos.rotation);
            MMOPlayer player = gameobject.GetComponent<MMOPlayer>();
            player.name = message.name;
            player.torso = message.torso;
            player.bottom = message.bottom;
            player.hand = message.hand;
            player.feet = message.feet;
            player.belt = message.belt;
            player.head = message.head;
            player.weapon = message.weapon;
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

