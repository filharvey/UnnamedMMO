using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Acemobe.MMO.Data.ScriptableObjects;
using Acemobe.MMO.UI.UIItems;
using Acemobe.MMO.Objects;

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
        }

        // Update is called once per frame
        void Update()
        {
            updateRecipie();

            // need to move
            if (MMOPlayer.localPlayer.player.GetButtonDown("Escape"))
            {
                onClose();
            }
        }

        public void onShow ()
        {
            if (UIManager.instance.craftUI.gameObject.activeInHierarchy)
            {
                onClose();
            }
            else
            {
                UIManager.instance.hideAllUIPanels();
                gameObject.SetActive(true);

                clearRecipies();
                updateAvaibleRecipies();
                updateRecipie();
            }
        }

        public void onClose()
        {
            clearRecipies();
            gameObject.SetActive(false);
        }

        public void craftItemSelected (Recipies recipies)
        {
            curRecipies = recipies;

            updateRecipie();
        }

        void clearRecipies ()
        {
            while (availableRecipies.Count > 0)
            {
                GameObject obj = availableRecipies[0];
                obj.transform.SetParent(null);
                Destroy(obj);

                availableRecipies.RemoveAt(0);
            }
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

                for (var a = 0; a < curRecipies.materials.Count; a++)
                {
                    RecipieMaterial mat = curRecipies.materials[a];
                    UICraftMaterialItem matInfo = materials[a];

                    matInfo.gameObject.SetActive(true);

                    matInfo.itemName.text = mat.material.name;
                    matInfo.itemImage.sprite = mat.material.icon;

                    // check that player has the correct count of an item
                    int count = MMOPlayer.localPlayer.inventory.getItemCount (mat.material.itemType);
                    if (MMOPlayer.localPlayer.inventory.hasItemCount(mat.material.itemType, mat.count))
                    {
                        matInfo.itemCount.text = count + "/" + mat.count;
                        matInfo.itemCount.color = Color.black;
                    }
                    else
                    {
                        matInfo.itemCount.color = Color.red;
                        matInfo.itemCount.text = count + "/" + mat.count;
                    }
                }

                for (var a = 0; a < 3; a++)
                {
                    UICraftMaterialItem matInfo = materials[2];
                    matInfo.gameObject.SetActive(false);
                }
            }
        }

        public void startCraft ()
        {
            // if we have enough inventory
            if (MMOPlayer.localPlayer.inventory.checkRecipe(curRecipies))
                MMOPlayer.localPlayer.startCraft(curRecipies);
            else
            {
                // show dialog
            }
        }
    }
}
