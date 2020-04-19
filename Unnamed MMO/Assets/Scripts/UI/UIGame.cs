using Acemobe.MMO;
using Acemobe.MMO.Data;
using Acemobe.MMO.Data.ScriptableObjects;
using Acemobe.MMO.UI.UIItems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO.UI
{
    public class UIGame : MonoBehaviour
    {
        public GameObject notificationPanel;

        List<GameObject> notificationList = new List<GameObject>();

        [Header("Prefabs")]
        public GameObject notificationPrefab;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void addNotification(MMOInventoryItem item)
        {
            GameItem gameItem = MMOResourceManager.instance.getItemByType(item.type);

            GameObject obj = Instantiate<GameObject>(notificationPrefab);
            UINotificatoin notification = obj.GetComponent<UINotificatoin>();

            notification.text.text = "+" + item.amount + " " + gameItem.name;

            obj.transform.SetParent(notificationPanel.transform);
            notificationList.Insert(0, obj);

            updateNotifications();
        }

        public void removeNotification(UINotificatoin notification)
        {
            notificationList.Remove(notification.gameObject);

            Destroy(notification.gameObject);
        }

        public void updateNotifications()
        {
            for (int a = 0; a < notificationList.Count; a++)
            {
                GameObject obj = notificationList[a];
                obj.transform.localPosition = new Vector3(0, -a * 60, 0);
                obj.transform.localScale = Vector3.one;
            }
        }
    }
}