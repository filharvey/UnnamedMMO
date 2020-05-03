using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO.Objects
{
    public class MMOBuilding : MMOObject
    {
        public List<Collider> colliders;

        public void enableColliders()
        {
            for (var a = 0; a < colliders.Count; a++)
            {
                colliders[a].enabled = true;
            }
        }

        public void disableColliders()
        {
            for (var a = 0; a < colliders.Count; a++)
            {
                colliders[a].enabled = false;
            }
        }
    }

}