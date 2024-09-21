using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float moveSpeed = 2f;

    private Transform target;
    private int pathIndex = 0;
    private float pathTargetRange = 0.1f;
    private float baseSpeed;
    public Player player;

    private void Start()
    {
        baseSpeed = moveSpeed;
        target = LevelManager.main.path[pathIndex];
    }

    private void Update()
    {
        UpdatePath();
    }

    private void UpdatePath()
    {

        if (!isCollidingWithCheckpoint())
            return;

        pathIndex++;

        if (isPathEnded())
        {
            if (CompareTag("BossEnemy"))
            {
                // Trigger instant game over when the boss reaches the end
                Debug.Log("The boss has reached the end. Game Over!");
                player.TakeDamage(player.maxHealth); // Instantly deplete player health
            }
            else
            {
                EnemySpawner.onEnemyReachedEnd.Invoke(); // Regular enemy reaches the end
            }

            EnemySpawner.onEnemyDestroy.Invoke();
            Destroy(gameObject);
            return;
        }

        target = LevelManager.main.path[pathIndex];
    }

    private bool isCollidingWithCheckpoint()
    {
        return Vector2.Distance(target.position, transform.position) <= pathTargetRange;
    }

    private bool isPathEnded()
    {
        return pathIndex >= LevelManager.main.path.Length;
    }

    public int GetPathIndex()
    {
        return pathIndex;
    }

    private void FixedUpdate()
    {
        Vector2 direction = (target.position - transform.position).normalized; // "normalized" makes it so that the direction only goes between 0 and 1

        rb.velocity = direction * moveSpeed;
    }

    public void UpdateSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    public void ResetSpeed()
    {
        moveSpeed = baseSpeed;
    }
}
