using Acemobe.MMO;
using Acemobe.MMO.Data;
using Acemobe.MMO.MMOObjects;
using Acemobe.MMO.UI;
using Acemobe.MMO.UI.UIItems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIElementDragger : MonoBehaviour
{
    public const string DRAGGABLE_TAG = "UIDraggable";

    bool isDragging = false;

    Vector3 originalPos;

    Transform dragObj;
    Transform parent;
    UIItemInfo draggedItemInfo;

    List<RaycastResult> hitObjects = new List<RaycastResult>();

    private void Update()
    {
        if (Input.GetMouseButtonDown (0)){
            dragObj = GetDraggableTransformUnderMouse();

            if (dragObj != null)
            {
                isDragging = true;

                draggedItemInfo = dragObj.GetComponent<UIItemInfo>();

                if (draggedItemInfo.invetoryItem != null)
                {
                    // hide count
                    draggedItemInfo.count.gameObject.SetActive(false);

                    // set as 
                    originalPos = dragObj.position;
                    parent = dragObj.parent;

                    dragObj = draggedItemInfo.item.transform;
                    dragObj.localScale = Vector3.one * 1.2f;
                    dragObj.SetParent(UIManager.instance.dragLayer.transform);
                }
            }
        }

        if (isDragging && dragObj)
        {
            dragObj.position = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp (0))
        {
            if (dragObj)
            {
                Transform objectToReplace = GetDraggableTransformUnderMouse();

                if (objectToReplace != null)
                {
                    // swap item info
                    dragObj.position = originalPos;

                    UIItemInfo targetItemInfo = objectToReplace.GetComponent<UIItemInfo>();

                    MMOPlayer.localPlayer.moveInventory(
                        draggedItemInfo.invetoryItem.idx,
                        draggedItemInfo.invetoryItem.action,
                        targetItemInfo.invetoryItem.idx,
                        targetItemInfo.invetoryItem.action
                    );

                    if (targetItemInfo.invetoryItem.type == MMOItemType.None)
                    {
                        targetItemInfo.invetoryItem.type = draggedItemInfo.invetoryItem.type;
                        targetItemInfo.invetoryItem.amount = draggedItemInfo.invetoryItem.amount;

                        draggedItemInfo.invetoryItem.type = MMOItemType.None;
                        draggedItemInfo.invetoryItem.amount = 0;
                    }
                    else
                    {
                        var temp = targetItemInfo.invetoryItem.type;
                        var count = targetItemInfo.invetoryItem.amount;

                        targetItemInfo.invetoryItem.type = draggedItemInfo.invetoryItem.type;
                        targetItemInfo.invetoryItem.amount = draggedItemInfo.invetoryItem.amount;

                        draggedItemInfo.invetoryItem.type = temp;
                        draggedItemInfo.invetoryItem.amount = count;
                    }
                }
                else
                {
                    dragObj.position = originalPos;
                }

                dragObj.SetParent(draggedItemInfo.transform);
                // set after background
                dragObj.SetSiblingIndex(1);

                dragObj.localScale = Vector3.one;
                draggedItemInfo.count.gameObject.SetActive(true);
            }

            isDragging = false;
            dragObj = null;
        }
    }

    GameObject GetObjectUnderMouse ()
    {
        var pointer = new PointerEventData(EventSystem.current);

        pointer.position = Input.mousePosition;

        EventSystem.current.RaycastAll(pointer, hitObjects);

        if (hitObjects.Count <= 0)
            return null;

        return hitObjects[0].gameObject;
    }

    Transform GetDraggableTransformUnderMouse ()
    {
        GameObject clicked = GetObjectUnderMouse();

        if (clicked != null && clicked.tag == DRAGGABLE_TAG)
        {
            return clicked.transform;
        }

        return null;
    }
}
