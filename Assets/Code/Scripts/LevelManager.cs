using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Transform startPoint;
    [SerializeField] public Transform[] path;

    [Header("Attributes")]
    [SerializeField] private int startingCurrency = 200;
    [SerializeField] public int currency;

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();
    public static UnityEvent onEnemyReachedEnd = new UnityEvent();
    public static UnityEvent onBossDefeated = new UnityEvent();

    public static LevelManager main;
    
    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        currency = startingCurrency;
    }

    public void IncreaseCurrency(int amount)
    {
        currency += amount;
    }

    public bool SpendCurrency(int amount)
    {
        if (amount <= currency)
        {
            currency -= amount;
            return true;
        }
        return false;
    }
}
