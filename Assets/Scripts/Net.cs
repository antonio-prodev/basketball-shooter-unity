using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Net : MonoBehaviour
{
    private GameManager gameManager;
    private Cloth netCloth;
    private List<GameObject> pooledBalls;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        netCloth = GetComponent<Cloth>();
        var list = ObjectPooler.SharedInstance.pooledObjects;
        var pairs = new ClothSphereColliderPair[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            var col = list[i].GetComponent<SphereCollider>();
        
            var pair = new ClothSphereColliderPair();
            pair.first = col;
            pairs[i] = pair;
        }
        netCloth.sphereColliders = pairs;

        for (int i = 0; i < list.Count; i++)
        {
            list[i].SetActive(false); // Deactivate the ball after adding its collider to the cloth
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
