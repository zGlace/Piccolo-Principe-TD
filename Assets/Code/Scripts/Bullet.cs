using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Bullet : MonoBehaviour
{

    public AudioClip hitSound;  // L'effetto sonoro dell'impatto
    public AudioClip spawnSound;  // L'effetto sonoro dell'impatto
    private AudioSource audioSource;
    public LayerMask enemyLayer;


    [Header("References")]
    [SerializeField] private Rigidbody2D rb; 
    
    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float bulletDamage = 1;

    private Transform target;



    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(spawnSound);
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    private void FixedUpdate()
    {
        if (!target) return;

        
        Vector2 direction = (target.position - transform.position).normalized; 

        
        rb.velocity = direction * bulletSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle); 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        collision.gameObject.GetComponent<EnemyHealth>().EnemyTakeDamage(bulletDamage);
        audioSource.PlayOneShot(hitSound);

        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().sprite = null;

        if (collision.gameObject.layer == enemyLayer)
        {

            // Logica per danni e distruzione del nemico
            Destroy(collision.gameObject);  // Distruggi il nemico
        }
        Destroy(gameObject, hitSound.length);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}