using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO.Objects
{
    public class MMOAnimHandler : MonoBehaviour
    {
        public MMOFakePlayer fakePlayer;

        public void AnimComplete()
        {
            if (fakePlayer && fakePlayer.isServer)
            {
                fakePlayer.AnimComplete();
            }
        }
    }
}