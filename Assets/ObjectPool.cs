using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }
    public GameObject pointPrefab;
    private List<GameObject> pooledObjects = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetPooledObject()
    {
        // Search for an inactive object in the pool and return it
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        // If no inactive object found, create a new one and add it to the pool
        GameObject newObj = Instantiate(pointPrefab);
        newObj.SetActive(false);
        pooledObjects.Add(newObj);
        return newObj;
    }
}
