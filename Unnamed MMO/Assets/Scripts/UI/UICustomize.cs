using Acemobe.MMO.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Acemobe.MMO.UI
{
    public class UICustomize : MonoBehaviour
    {
        public Image shirtImage;

        public Texture2D texture;

        public Sprite textureSprite;

        Color color = Color.red;
        int colorIdx = 0;

        // Start is called before the first frame update
        void Start()
        {
            if (!texture)
            {
                texture = new Texture2D(27, 32, TextureFormat.ARGB32, false);
                texture.filterMode = FilterMode.Point;

                for (int x = 0; x < 27; x++)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        texture.SetPixel(x, y, Color.white);
                    }
                }

                texture.Apply();
            }

            textureSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            shirtImage.sprite = textureSprite;
        }

        List<RaycastResult> hitObjects = new List<RaycastResult>();

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                var pointer = new PointerEventData(EventSystem.current);

                pointer.position = Input.mousePosition;

                EventSystem.current.RaycastAll(pointer, hitObjects);

                if (hitObjects.Count > 0)
                {
                    RaycastResult hitObj = hitObjects[0];

                    if (hitObj.gameObject == shirtImage.gameObject)
                    {
                        var worldCorners = new Vector3[4];
                        var pos = shirtImage.gameObject.GetComponent<RectTransform>();
                        pos.GetWorldCorners(worldCorners);

                        var w = worldCorners[2].x - worldCorners[0].x;
                        var h = worldCorners[2].y - worldCorners[0].y;

                        var x = Mathf.FloorToInt((hitObj.screenPosition.x - worldCorners[0].x) * 270 / w);
                        var y = Mathf.FloorToInt((hitObj.screenPosition.y - worldCorners[0].y) * 320 / h);

                        x = Mathf.FloorToInt(x / 10);
                        y = Mathf.FloorToInt(y / 10);

                        if (x >= 0 && x < 27 &&
                            y >= 0 && y < 32)
                        {
                            texture.SetPixel(x, y, color);
                            texture.Apply();

                            MMOPlayer.localPlayer.updateShirtColor(x, 31 - y, color, colorIdx);
                        }
                    }
                }
            }
        }

        public void onShow()
        {
            if (UIManager.instance.craftUI.gameObject.activeInHierarchy)
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

        public void onColor (int index)
        {
            colorIdx = index;

            switch (index)
            {
                case 0:
                    color = Color.red;
                    break;
                case 1:
                    color = Color.blue;
                    break;
                case 2:
                    color = Color.yellow;
                    break;
                case 3:
                    color = Color.green;
                    break;
                case 4:
                    color = Color.cyan;
                    break;
                case 5:
                    color = Color.white;
                    break;
                case 6:
                    color = Color.black;
                    break;
                case 7:
                    color = Color.grey;
                    break;

            }
        }
    }
}