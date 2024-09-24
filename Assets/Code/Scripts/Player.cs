using UnityEngine.Rendering;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public HealthBar healthBar;
    [SerializeField] public Volume globalVolume;

    [Header("Animation References")]
    [SerializeField] private Animator loseTextAnimator;
    [SerializeField] private string loseAnimation;

    [Header("Attributes")]
    [SerializeField] public int maxHealth = 5;
    [SerializeField] public int currentHealth;

    private VignetteController vignetteController;
    private LoseMenu lose;
    
    public void Start()
    {
        lose = FindObjectOfType<LoseMenu>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        vignetteController = GetComponent<VignetteController>();

        if (vignetteController != null && globalVolume != null)
        {
            vignetteController.SetGlobalVolume(globalVolume);
        }

        LevelManager.onEnemyReachedEnd.AddListener(OnEnemyReachedEnd);
    }

    public void OnDestroy()
    {
        LevelManager.onEnemyReachedEnd.RemoveListener(OnEnemyReachedEnd);
    }

    public void OnEnemyReachedEnd()
    {
        if (!LoseMenu.GameOver) // Only take damage if the game is not over
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (vignetteController != null)
        {
            vignetteController.ModifyVignette(Color.red, 0.3f, 0.15f, 0.6f, 0.7f);
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Prevent negative health

        Debug.Log($"Player takes {damage} damage. Current health: {currentHealth}");
        healthBar.SetHealth(currentHealth);

        if (currentHealth == 0 && !LoseMenu.GameOver)
        {
            lose.gameOverUI.SetActive(true);

            loseTextAnimator.updateMode = AnimatorUpdateMode.UnscaledTime; // Set Animator to Unscaled Time so animation plays even when time is frozen
            loseTextAnimator.Play(loseAnimation, 0, 0.0f);

            lose.GameLost();
            Time.timeScale = 0f;
        }
    }
}
