using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Acemobe.MMO.Data.ScriptableObjects;
using Acemobe.MMO.UI.UIItems;

namespace Acemobe.MMO.UI
{
    public class UICraft : MonoBehaviour
    {
        public Recipies curRecipies;
        public TMP_Text itemName;
        public Image itemImage;
        public TMP_Text itemDescription;

        public List<UICraftMaterialItem> materials;

        public GameObject scrollContainer;

        public GameObject craftItemPrefab;

        List<GameObject> availableRecipies = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            updateRecipie();
            updateAvaibleRecipies();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void onShow ()
        {
            gameObject.SetActive(true);

            updateRecipie();
            updateAvaibleRecipies();
        }

        public void onClose()
        {
            gameObject.SetActive(false);
        }

        public void craftItemSelected (Recipies recipies)
        {
            curRecipies = recipies;

            updateRecipie();
        }

        void updateAvaibleRecipies()
        {
            if (!MMOResourceManager.instance)
                return;

            for (var a = 0; a < MMOResourceManager.instance.gameRecipies.Count; a++)
            {
                GameObject obj = Instantiate<GameObject>(craftItemPrefab);
                UICraftItemList craftableItem = obj.GetComponent<UICraftItemList>();

                craftableItem.recipe = MMOResourceManager.instance.gameRecipies[a];

                obj.transform.SetParent(scrollContainer.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;

                availableRecipies.Add(obj);
            }
        }

        void clearAvailableReceipes ()
        {
            while(availableRecipies.Count > 0)
            {
                GameObject obj = availableRecipies[0];
                availableRecipies.RemoveAt(0);

                Destroy(obj);
            }
        }

        void updateRecipie()
        {
            if (!MMOResourceManager.instance)
                return;

            if (!curRecipies)
            {
                // set to basic Axe
                curRecipies = MMOResourceManager.instance.gameRecipies[0];
            }

            if (curRecipies)
            {
                itemName.text = curRecipies.name;
                itemDescription.text = curRecipies.description;
                itemImage.sprite = curRecipies.icon;

                if (curRecipies.material1)
                {
                    UICraftMaterialItem matInfo = materials[0];
                    matInfo.gameObject.SetActive(true);

                    matInfo.itemName.text = curRecipies.material1.name;
                    matInfo.itemImage.sprite = curRecipies.material1.icon;

                    // check that player has the correct count of an item
                    matInfo.itemCount.text = curRecipies.count1 + "/" + curRecipies.count1;
                }

                if (curRecipies.material2)
                {
                    UICraftMaterialItem matInfo = materials[1];
                    matInfo.gameObject.SetActive(true);

                    matInfo.itemName.text = curRecipies.material2.name;
                    matInfo.itemImage.sprite = curRecipies.material2.icon;

                    // check that player has the correct count of an item
                    matInfo.itemCount.text = curRecipies.count2 + "/" + curRecipies.count2;
                }
                else
                {
                    UICraftMaterialItem matInfo = materials[1];
                    matInfo.gameObject.SetActive(false);
                }

                if (curRecipies.material3)
                {
                    UICraftMaterialItem matInfo = materials[2];
                    matInfo.gameObject.SetActive(true);

                    matInfo.itemName.text = curRecipies.material3.name;
                    matInfo.itemImage.sprite = curRecipies.material3.icon;

                    // check that player has the correct count of an item
                    matInfo.itemCount.text = curRecipies.count3 + "/" + curRecipies.count3;
                }
                else
                {
                    UICraftMaterialItem matInfo = materials[2];
                    matInfo.gameObject.SetActive(false);
                }
            }
        }

        public void startCraft ()
        {
            // if we have enough inventory
            MMOPlayer.localPlayer.startCraft(curRecipies);
        }
    }
}
