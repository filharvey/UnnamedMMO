using Acemobe.MMO.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles chat between a player and AI
namespace Acemobe.MMO.AI
{

    public class MMOChat : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void startChat ()
        {
            // show chat popup
            UIManager.instance.chatUI.onShow();
        }
    }
}