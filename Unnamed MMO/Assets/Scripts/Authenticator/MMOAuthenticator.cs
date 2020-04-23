﻿using UnityEngine;
using System.Collections;
using Mirror;
using BestHTTP;
using BestHTTP.JSON;
using System.Collections.Generic;
using Acemobe.MMO.UI;

namespace Acemobe.MMO
{
    public class MMOAuthenticator : NetworkAuthenticator
    {
        [Header("Custom Properties")]

        // set these in the inspector
        public string username;
        public string password;

        public class AuthRequestMessage : MessageBase
        {
            // use whatever credentials make sense for your game
            // for example, you might want to pass the accessToken if using oauth
            public string authUsername;
            public string authPassword;
        }

        public class AuthResponseMessage : MessageBase
        {
            public byte code;
            public string message;
        }

        public override void OnStartServer()
        {
            // register a handler for the authentication request we expect from client
            NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
        }

        public override void OnStartClient()
        {
            // register a handler for the authentication response we expect from server
            NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
        }

        public override void OnServerAuthenticate(NetworkConnection conn)
        {
            // do nothing...wait for AuthRequestMessage from client
        }

        public override void OnClientAuthenticate(NetworkConnection conn)
        {
            AuthRequestMessage authRequestMessage = new AuthRequestMessage
            {
                authUsername = username,
                authPassword = password
            };

            NetworkClient.Send(authRequestMessage);
        }

        public void OnAuthRequestMessage(NetworkConnection conn, AuthRequestMessage msg)
        {
            Debug.LogFormat("Authentication Request: {0} {1}", msg.authUsername, msg.authPassword);

/*            AuthResponseMessage authResponseMessage = new AuthResponseMessage
            {
                code = 100,
                message = "Success"
            };

            conn.Send(authResponseMessage);

            // Invoke the event to complete a successful authentication
            base.OnServerAuthenticated.Invoke(conn);
*/
            HTTPRequest request = new HTTPRequest(new System.Uri("http://157.245.226.33:3000/login"), HTTPMethods.Post, (request2, response) =>
            {
                if (response != null && response.IsSuccess)
                {
                    bool ok = false;
                    IDictionary<string, object> result = Json.Decode(response.DataAsText, ref ok) as IDictionary<string, object>;

                    Debug.Log("Request Finished! Text received: " + response.DataAsText);

                    if ((bool)(result["ok"]) == true)
                    {
                        Debug.Log("ok");

                        // create and send msg to client so it knows to proceed
                        AuthResponseMessage authResponseMessage = new AuthResponseMessage
                        {
                            code = 100,
                            message = "Success"
                        };

                        conn.Send(authResponseMessage);

                        // Invoke the event to complete a successful authentication
                        base.OnServerAuthenticated.Invoke(conn);

                        return;
                    }
                }

                {
                    // create and send msg to client so it knows to disconnect
                    AuthResponseMessage authResponseMessage = new AuthResponseMessage
                    {
                        code = 200,
                        message = "Invalid Credentials"
                    };

                    conn.Send(authResponseMessage);

                    // must set NetworkConnection isAuthenticated = false
                    conn.isAuthenticated = false;

                    // disconnect the client after 1 second so that response message gets delivered
                    Invoke(nameof(conn.Disconnect), 1);
                }
            });

            request.AddField("username", msg.authUsername);
            request.AddField("password", msg.authPassword);
            request.Send();
        }

        public void OnAuthResponseMessage(NetworkConnection conn, AuthResponseMessage msg)
        {
            if (msg.code == 100)
            {
                Debug.LogFormat("Authentication Response: {0}", msg.message);

                // Invoke the event to complete a successful authentication
                base.OnClientAuthenticated.Invoke(conn);
            }
            else
            {
                Debug.LogErrorFormat("Authentication Response: {0}", msg.message);

                // Set this on the client for local reference
                conn.isAuthenticated = false;

                // disconnect the client
                conn.Disconnect();
            }
        }
    }
}
