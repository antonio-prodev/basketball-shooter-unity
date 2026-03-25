using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler SharedInstance;
    public GameObject objectToPool;
    public int amountToPool;

    public List<GameObject> pooledObjects;

    private GameManager gameManager;

    private void Awake() {
        SharedInstance = this;
        pooledObjects = new List<GameObject>(amountToPool);
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        for (int i = 0; i < amountToPool; i++){
            GameObject obj = Instantiate(objectToPool);
            obj.SetActive(true); // Set active to true to ensure colliders are registered with the Cloth component
            obj.transform.SetParent(transform);
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject(){
        for (int i = 0; i < pooledObjects.Count; i++){
            if (!pooledObjects[i].activeInHierarchy){
                return pooledObjects[i];
            }
        }
        return null;
    }
}
