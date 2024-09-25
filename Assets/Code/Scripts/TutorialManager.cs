using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] popUps;
    private int popUpIndex;
    public GameObject spawner;
    public float waitTime = 10f;

    void Update()
    {
        for (int i = 0; i < popUps.Length; i++)
        {
            if (i == popUpIndex)
            {
                popUps[popUpIndex].SetActive(true);
            }
            else
            {
                popUps[popUpIndex].SetActive(false);
            }
        }

        if (popUpIndex == 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                popUpIndex++;
            }
            else if (popUpIndex == 1)
            {
                if (waitTime <= 0)
                {
                    spawner.SetActive(true);
                }
                else
                {
                    waitTime -= Time.deltaTime;
                }
            }
        }
    }
}

public class Spawner : MonoBehaviour
{
    public GameObject tutorialEnemy;

    public float startTimeBtwEnemy;
    private float timeBtwEnemy;

    public int numberOfEnemies;

    void Update()
    {
        if (timeBtwEnemy <= 0 && numberOfEnemies > 0)
        {
            Instantiate(tutorialEnemy, LevelManager.main.startPoint.position, Quaternion.identity);
            timeBtwEnemy = startTimeBtwEnemy;
            numberOfEnemies++;
        }
        else
        {
            timeBtwEnemy -= Time.deltaTime;
        }
    }
}