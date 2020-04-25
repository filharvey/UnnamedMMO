using Acemobe.MMO;
using Acemobe.MMO.Objects;
using Acemobe.MMO.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO.UI
{
    public class UIChat : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (MMOPlayer.localPlayer.player.GetButtonDown("Escape"))
            {
                onClose();
            }
        }

        public void onShow()
        {
            if (gameObject.activeInHierarchy == true)
            {
                onClose();
            }
            else
            {
                UIManager.instance.hideAllUIPanels();

                gameObject.SetActive(true);
            }
        }

        public void onClose()
        {
            gameObject.SetActive(false);
        }
    }
}