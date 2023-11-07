using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    [Header("References")]
    [SerializeField] ScoreUI scoreUI;

    [Header("Scores")]
    public float damageDone = 0; // Done
    public float damageTaken = 0; // Done
    public int enemiesKilled = 0; // Done

    public int goldSpent = 0; // Done
    public int goldEarned = 0; // Done

    public int turrets = 0; // Done
    public int upgrades = 0; // Done

    protected override void Awake()
    {
        base.Awake();

        scoreUI.gameObject.SetActive(false);
    }

    void HandleEnemyDeath(Target enemy)
    {
        enemiesKilled += 1;
    }

    public string GetScoresAsText()
    {
        string scores = "Damage Done: " + damageDone + "\n";
        scores += "Damage Taken: " + damageTaken + "\n";
        scores += "Enemies Killed: " + enemiesKilled + "\n";
        scores += "Gold Spent: " + goldSpent + "\n";
        scores += "Gold Earned: " + goldEarned + "\n";
        scores += "Turrets: " + turrets + "\n";
        scores += "Upgrades: " + upgrades + "\n";
        return scores;
    }

    void HandleGameEnd()
    {
        scoreUI.gameObject.SetActive(true);
        scoreUI.SetScores(GetScoresAsText());
    }

    void OnEnable()
    {
        Enemy.OnDeath += HandleEnemyDeath;
        GameController.OnGameEnd += HandleGameEnd;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Enemy.OnDeath -= HandleEnemyDeath;
        GameController.OnGameEnd -= HandleGameEnd;
    }
}
