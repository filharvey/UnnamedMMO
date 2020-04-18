using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Acemobe.MMO;

public class UINotificatoin : MonoBehaviour
{
    public TMP_Text text;

    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer > 4)
        {
            UIManager.instance.gameUI.removeNotification(this);
            UIManager.instance.gameUI.updateNotifications();
        }
    }
}
