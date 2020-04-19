using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Acemobe.MMO.Data.ScriptableObjects;

namespace Acemobe.MMO.UI.UIItems
{
    public class UICraftItemList : MonoBehaviour
    {
        public Recipies recipe;
        public TMP_Text itemName;

        // Start is called before the first frame update
        void Start()
        {
            itemName.text = recipe.name;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void onClick ()
        {
            UIManager.instance.craftUI.craftItemSelected(recipe);
        }
    }
}