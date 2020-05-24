using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSetPlayer : MonoBehaviour
{
    public TurnTheGameOn.NPCChat.NPCChat chatReference;
    public string playerNameToFind;
    public GameObject playerObject;
    void Update()
    {
        if (chatReference.player == null)
        {
            playerObject = GameObject.Find (playerNameToFind);
            {
                if (playerObject != null)
                {
                    chatReference.player = playerObject.transform;
                    enabled = false;
                }
            }
        }
    }
}
