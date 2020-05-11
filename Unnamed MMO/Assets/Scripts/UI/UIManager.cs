using Acemobe.MMO.Data;
using Acemobe.MMO.UI.UIItems;
using Mirror;
using UnityEngine;
using ZXing;
using ZXing.QrCode;

namespace Acemobe.MMO.UI
{
    public class UIManager : MonoBehaviour
    {
        static UIManager _instance;

        public UIActionBar actionBarUI;

        public UIInventory inventoryUI;

        public UILogin loginUI;

        public UIGame gameUI;

        public UICraft craftUI;

        public UIChat chatUI;

        public UICustomize customizeUI;

        public GameObject mobileUI;

        public GameObject dragLayer;

        public TMPro.TMP_Text ping;

        public static UIManager instance
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

        // Start is called before the first frame update
        void Start()
        {
            _instance = this;

            actionBarUI.gameObject.SetActive(false);
            gameUI.gameObject.SetActive(false);
            mobileUI.SetActive(false);

            hideAllUIPanels();

            if (MMONetworkManager.isHeadless)
                loginUI.gameObject.SetActive(false);
            else
                loginUI.gameObject.SetActive(true);
        }

        private void Update()
        {
            ping.text = "Ping: " + (int)(NetworkTime.rtt * 1000);
        }

        public void showItemGain (MMOInventoryItem item)
        {
            gameUI.addNotification(item);
        }

        public void hideAllUIPanels ()
        {
            UIManager.instance.customizeUI.gameObject.SetActive(false);
            UIManager.instance.craftUI.gameObject.SetActive(false);
            UIManager.instance.chatUI.gameObject.SetActive(false);
            UIManager.instance.inventoryUI.gameObject.SetActive(false);
        }

        // QR code generator
        // https://github.com/nenuadrian/qr-code-unity-3d-read-generate
        // https://medium.com/@adrian.n/reading-and-generating-qr-codes-with-c-in-unity-3d-the-easy-way-a25e1d85ba51
        //
        private static Color32[] Encode(string textForEncoding, int width, int height)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width
                }
            };
            return writer.Write(textForEncoding);
        }

        public Texture2D generateQR(string text)
        {
            var encoded = new Texture2D(256, 256);
            var color32 = Encode(text, encoded.width, encoded.height);
            encoded.SetPixels32(color32);
            encoded.Apply();
            return encoded;
        }

        void create ()
        {
            Texture2D myQR = generateQR("test");
        }
    }
}