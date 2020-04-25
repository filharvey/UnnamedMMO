using Acemobe.MMO.Objects;
using UnityEngine;

public class Faceplayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (MMOPlayer.localPlayer)
        {
            Vector3 pos = Camera.main.transform.position;
            pos.y = transform.position.y;

            transform.LookAt(pos, Vector3.up);
        }
    }
}
