using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer healingIndicator;

    [Header("Attributes")]
    [SerializeField] private float healingRadius = 5f; // Healing range
    [SerializeField] private float healAmount = 1f; // Amount of health restored per second
    [SerializeField] private float healInterval = 2.5f;  // How often healing happens (in seconds)
    [SerializeField] private LayerMask enemyLayerMask; // To identify what counts as an enemy
    
    private Collider2D[] enemiesInRange; // To store enemies within the range

    private void Start()
    {
        // Get the size of the sprite in world units
        float spriteRadius = healingIndicator.bounds.extents.x; // Assuming the sprite is circular or uniform

        // Calculate the scale factor needed to match the desired healing radius
        float scaleFactor = (healingRadius * 2) / (spriteRadius * 2); // We multiply by 2 to get the diameter
        
        // Apply the scale factor
        healingIndicator.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);

        // Start the healing coroutine
        StartCoroutine(HealNearbyEnemies());
    }

    private IEnumerator HealNearbyEnemies()
    {
        while (true)
        {
            HealEnemiesInRange();
            yield return new WaitForSeconds(healInterval); // Heal every healInterval seconds
        }
    }

    private void HealEnemiesInRange()
    {
        // Get all enemies within the healing range
        enemiesInRange = Physics2D.OverlapCircleAll(transform.position, healingRadius, enemyLayerMask);

        foreach (var enemyCollider in enemiesInRange)
        {
            EnemyHealth enemyHealth = enemyCollider.GetComponent<EnemyHealth>();
            if (enemyHealth != null && enemyHealth.currentHitPoints < enemyHealth.maxHitPoints)
            {
                enemyHealth.Heal(healAmount);  // Heal the enemy
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a wireframe of the healing area for visual aid in the editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, healingRadius);
    }
}
