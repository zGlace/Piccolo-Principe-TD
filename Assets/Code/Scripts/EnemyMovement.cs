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
    private Animator animator;
    private SpriteRenderer spriteRenderer;


    private void Start()
    {
        baseSpeed = moveSpeed;
        target = LevelManager.main.path[pathIndex];
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
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

        UpdateAnimation(direction);
    }

    private void UpdateAnimation(Vector2 direction)
    {
        // Ottieni il tag del personaggio per determinare quale animazione giocare
        string characterTag = gameObject.tag;

        // Determina quale asse è dominante (orizzontale o verticale)
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Movimenti a destra o a sinistra
            if (direction.x > 0)
            {
                PlayAnimation(characterTag, "RightLeft", false); // Verso destra
            }
            else
            {
                PlayAnimation(characterTag, "RightLeft", true); // Verso sinistra con flip
            }
        }
        else
        {
            // Movimenti su o giù
            if (direction.y > 0)
            {
                PlayAnimation(characterTag, "North", false); // Verso l'alto
            }
            else
            {
                PlayAnimation(characterTag, "Straight", false); // Verso il basso
            }
        }
    }

    private void PlayAnimation(string characterTag, string direction, bool flipX)
    {
        // Usa il tag per determinare l'animazione
        if (characterTag == "BossEnemy")
        {
            animator.Play("KingWalking" + direction);
        }
        else if (characterTag == "SpeedEnemy")
        {
            animator.Play("WitchWalking" + direction);
        }
        else if (characterTag == "Enemy")
        {
            animator.Play("SnakeCrawling" + direction); 
        }
        else if (characterTag == "Healer")
        {
            animator.Play("HealerWalking" + direction);
        }
         else if (characterTag == "TankEnemy")
        {
            animator.Play("ScorpionWalking" + direction);
        }
        
        spriteRenderer.flipX = flipX;
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
