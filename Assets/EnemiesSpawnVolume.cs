using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesSpawnVolume : MonoBehaviour
{

    public LayerMask playerLayer;

    public GameObject triggerArrow;

    public List<Enemy> enemies;
    public List<Enemy> leapEnemies;
    public List<Vector3> leapPosition;

    public bool releaseCameraAfterEvent = true;

    public List<GameObject> activateAfterEvent;

    public bool dontInitializeMembers = false;

    bool eventStarted = false;
    bool eventFinished = false;

    CameraBehaviour cameraBehaviour;
    UIManager uiManager;
    SpawnFood spawner;

    int remainingEnemies;

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        spawner = GetComponentInChildren<SpawnFood>();

        foreach (Enemy enemy in enemies)
        {
            enemy.IgnorePlayer();
            remainingEnemies++;
        }

        foreach (Enemy enemy in leapEnemies)
        {
            enemy.gameObject.SetActive(false);
            remainingEnemies++;
        }

        spawner.gameObject.SetActive(false);
        cameraBehaviour = FindObjectOfType<CameraBehaviour>();
    }

    void Update()
    {
        if (eventStarted && !eventFinished && remainingEnemies <= 0)
        {
            // SUCCESS
            if (releaseCameraAfterEvent)
            {
                cameraBehaviour.Release();
                cameraBehaviour.FollowTarget();
            }
            uiManager.ShowArrow();
            triggerArrow.SetActive(true);

            foreach (SpawnFood spawn in FindObjectsOfType<SpawnFood>())
            {
                spawn.gameObject.SetActive(false);
            }
            spawner.gameObject.SetActive(false);

            foreach (GameObject go in activateAfterEvent)
            {
                go.SetActive(true);
            }

            eventFinished = true;
        }

        if (!eventStarted)
        {
            if (Mathf.Abs(cameraBehaviour.transform.position.x - transform.position.x) < 15f)
            {
                PrepareArena();
            }
        }
    }

    private void PrepareArena()
    {
        cameraBehaviour.Lock();
        cameraBehaviour.transform.position = new Vector3(
            transform.position.x,
            cameraBehaviour.transform.position.y,
            cameraBehaviour.transform.position.z);

        cameraBehaviour.StopFollowingTarget();

        Camera camera = Camera.main;

        float aspectRatio = camera.aspect;
        float height = 2 * camera.orthographicSize;
        float width = height * aspectRatio;

        Vector3 cameraPos = camera.transform.position;

        // LEFT
        GameObject gLeft = new GameObject("Left wall");
        gLeft.transform.SetParent(transform, true);
        BoxCollider2D bcLeft = gLeft.AddComponent<BoxCollider2D>();
        bcLeft.gameObject.layer = LayerMask.NameToLayer("Obstacle");
        bcLeft.offset = new Vector2(cameraPos.x - width / 2, cameraPos.y);
        bcLeft.size = new Vector2(width / 10, height);

        // RIGHT
        GameObject gRight = new GameObject("Right wall");
        gRight.transform.SetParent(transform, true);
        BoxCollider2D bcRight = gRight.AddComponent<BoxCollider2D>();
        bcRight.gameObject.layer = LayerMask.NameToLayer("Obstacle");
        bcRight.offset = new Vector2(cameraPos.x + width / 2, cameraPos.y);
        bcRight.size = new Vector2(width / 10, height);

        // TOP
        GameObject gUp = new GameObject("Upper wall");
        gUp.transform.SetParent(transform, true);
        BoxCollider2D bcUp = gUp.AddComponent<BoxCollider2D>();
        bcUp.gameObject.layer = LayerMask.NameToLayer("Obstacle");
        bcUp.offset = new Vector2(cameraPos.x, cameraPos.y + height / 2);
        bcUp.size = new Vector2(width, height / 10);

        // BOTTOM
        GameObject gDown = new GameObject("Bottom wall");
        gDown.transform.SetParent(transform, true);
        BoxCollider2D bcDown = gDown.AddComponent<BoxCollider2D>();
        bcDown.gameObject.layer = LayerMask.NameToLayer("Obstacle");
        bcDown.offset = new Vector2(cameraPos.x, cameraPos.y - height / 2);
        bcDown.size = new Vector2(width, height / 10);

        if (!dontInitializeMembers)
        {

            foreach (Enemy enemy in enemies)
            {
                enemy.ChasePlayer();
            }

            for (int i = 0; i < leapEnemies.Count; i++)
            {
                Enemy leapEnemy = leapEnemies[i];
                leapEnemy.gameObject.SetActive(true);
                leapEnemy.IgnorePlayer();
                Vector3 target = leapPosition[i];
                leapEnemy.MoveTo(transform.position + target);
            }
        }

        spawner.gameObject.SetActive(true);
        eventStarted = true;
    }

    private void GoonDied()
    {
        remainingEnemies--;
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < leapEnemies.Count; i++)
        {
            Enemy enemy = leapEnemies[i];
            if (enemy != null)
            {
                Vector3 target = leapPosition[i];
                Gizmos.DrawLine(enemy.transform.position, transform.position + target);
            }
        }

        Camera camera = Camera.main;
        float aspectRatio = camera.aspect;
        float height = 2f * camera.orthographicSize;
        float width = height * aspectRatio;

        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 1f));
    }
}
