using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acemobe.MMO.Objects
{
    public class MMOBuilding : MMOObject
    {
        public List<Collider> colliders;
        public List<Texture> textures;
        public MeshRenderer meshRender;

        public int baseTexture = 0;

        private void Awake()
        {
            baseTexture = 0;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            updateMaterial();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            updateMaterial();
        }

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

        public void changeMaterial ()
        {
            if (textures.Count > 0)
            {
                baseTexture++;

                if (baseTexture >= textures.Count)
                    baseTexture = 0;

                updateMaterial();
            }
        }

        public void updateMaterial ()
        {
            if (!meshRender)
                return;

            Material materialInstance = meshRender.material;

            if (textures.Count > 0)
            {
                materialInstance.mainTexture = textures[baseTexture];
            }
        }

        public override JSONObject writeData()
        {
            JSONObject json = base.writeData();

            json["baseTexture"].AsInt = baseTexture;

            return json;
        }
        public override void readData(JSONObject json)
        {
            baseTexture = json["baseTexture"].AsInt;
        }

    }
}