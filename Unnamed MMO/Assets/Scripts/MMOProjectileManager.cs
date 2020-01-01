using Mirror;
using UnityEngine;

namespace Acemobe.MMO
{
    public class MMOProjectileManager : MonoBehaviour
    {
        static MMOProjectileManager _instance;

        public int m_ObjectPoolSize = 5;
        public GameObject m_Prefab;
        public GameObject[] m_Pool;

        public System.Guid assetId
        {
            get; set;
        }

        public static MMOProjectileManager instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public delegate GameObject SpawnDelegate(Vector3 position, System.Guid assetId);
        public delegate void UnSpawnDelegate(GameObject spawned);

        void Start()
        {
            /*            assetId = m_Prefab.GetComponent<NetworkIdentity>().assetId;
                        m_Pool = new GameObject[m_ObjectPoolSize];
                        for (int i = 0; i < m_ObjectPoolSize; ++i)
                        {
                            m_Pool[i] = Instantiate(m_Prefab, Vector3.zero, Quaternion.identity);
                            m_Pool[i].name = "PoolObject" + i;
                            m_Pool[i].SetActive(false);
                        }

                        ClientScene.RegisterSpawnHandler(assetId, SpawnObject, UnSpawnObject);
            */
        }

        public GameObject GetFromPool(Vector3 position)
        {
            foreach (var obj in m_Pool)
            {
                if (!obj.activeInHierarchy)
                {
                    Debug.Log("Activating GameObject " + obj.name + " at " + position);
                    obj.transform.position = position;
                    obj.SetActive(true);
                    return obj;
                }
            }
            Debug.LogError("Could not grab game object from pool, nothing available");
            return null;
        }

        public GameObject SpawnObject(Vector3 position, System.Guid assetId)
        {
            return GetFromPool(position);
        }

        public void UnSpawnObject(GameObject spawned)
        {
            Debug.Log("Re-pooling game object " + spawned.name);
            spawned.SetActive(false);
        }
    }
}