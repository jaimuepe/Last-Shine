using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour
{

    public Food foodPrefab;
    public float spawnDelay;
    public int maxConcurrentSpawns;

    public Rect spawnArea;

    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay + Random.Range(3f, 8f));

            int count = FindObjectsOfType<Food>().Length;

            if (count > maxConcurrentSpawns)
            {
                yield return null;
            }

            Camera camera = Camera.main;
            Vector3 cameraPos = camera.transform.position;

            Food food = Instantiate(foodPrefab);
            food.readyToPickUp = false;
            BoxCollider2D boxCollider = food.GetComponent<BoxCollider2D>();

            float x = Random.Range(
                spawnArea.xMin + boxCollider.size.x / 2,
                spawnArea.xMax - boxCollider.size.x / 2);

            float y = Random.Range(
                spawnArea.yMin + boxCollider.size.y / 2,
                spawnArea.yMax - boxCollider.size.y / 2);

            food.transform.position = new Vector3(
                x - spawnArea.width / 2,
                y - spawnArea.height / 2,
                food.transform.position.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position +
            new Vector3(
                spawnArea.position.x,
                spawnArea.position.y,
                0f),
                spawnArea.size);
    }
}
