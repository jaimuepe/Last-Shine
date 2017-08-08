using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteSpawner : MonoBehaviour
{

    public GameObject enemyPrefab;

    public int maxConcurrentEnemies = 10;
    public List<GameObject> spawnPosition;
    public List<GameObject> spawnDestination;

    public float spawnDelay;

    void Start()
    {
        StartCoroutine(Spawn());
    }

    private void OnEnable()
    {
        StartCoroutine(Spawn());
    }

    void Update()
    {

    }

    IEnumerator Spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay * Random.Range(0.8f, 1.2f));

            if (FindObjectsOfType<Enemy>().Length > maxConcurrentEnemies)
            {
                continue;
            }

            GameObject e = Instantiate(enemyPrefab);
            Enemy en = e.GetComponent<Enemy>();

            int idx = Random.Range(0, spawnPosition.Count);

            e.transform.position = spawnPosition[idx].transform.position;
            en.MoveTo(spawnDestination[idx].transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < spawnPosition.Count; i++)
        {
            Gizmos.DrawLine(spawnPosition[i].transform.position, spawnDestination[i].transform.position);

        }
    }
}
