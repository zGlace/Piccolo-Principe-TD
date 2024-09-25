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
    private LoseMenu lose;
    // private Animator animator;
    // private SpriteRenderer spriteRenderer;


    private void Start()
    {
        baseSpeed = moveSpeed;
        target = LevelManager.main.path[pathIndex];
        // spriteRenderer = GetComponent<SpriteRenderer>();
        // animator = GetComponent<Animator>(); 
    }

    private void Update()
    {
        UpdatePath();
    }

    private void UpdatePath()
    {
        if (!isCollidingWithCheckpoint()) return;

        pathIndex++;

        if (isPathEnded())
        {
            EndOfPath();
            return;
        }

        target = LevelManager.main.path[pathIndex];
    }

    private void EndOfPath()
    {
        if (CompareTag("BossEnemy"))
        {
            Debug.Log("The boss has reached the end.");
            lose.GameLost();
        }
        else
        {
            LevelManager.onEnemyReachedEnd.Invoke();
            LevelManager.onEnemyDestroy.Invoke();
        }
        Destroy(gameObject);
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

        /*
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                if (gameObject.CompareTag("BossEnemy"))
                {
                    animator.Play("KingWalkingRightLeft"); 
                    spriteRenderer.flipX = false;
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 0, 90); 
                }
            }
            else
            {
                if (gameObject.CompareTag("BossEnemy"))
                {
                    animator.Play("KingWalkingRightLeft"); 
                    spriteRenderer.flipX = true;
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 0, -90); 
                }
            }
        }
        else
        {
            if (direction.y > 0)
            {
                if (gameObject.CompareTag("BossEnemy"))
                {
                    animator.Play("KingWalkingNorth");
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                }
            }
            else
            {
                if (gameObject.CompareTag("BossEnemy"))
                {
                    animator.Play("KingWalkingStraight");
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0); 
                }
            }
        }
        */
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
