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
            EnemySpawner.onEnemyReachedEnd.Invoke();
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

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player != null)
            {
                Debug.Log("Player collided with enemy");
                player.TakeDamage(1);
            }
            else
            {
                Debug.LogError("Player reference is null!");
            }
        }
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
