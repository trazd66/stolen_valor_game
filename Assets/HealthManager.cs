using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour

{
    public GameObject playerScoreObj;
    public GameObject enemyScoreObj;

    private Text _playerScoreText;
    private Text _enemyScoreText;

    public int playerHealth = 300;
    public int enemyHealth = 300;

    // Start is called before the first frame update
    void Start()
    {
        _playerScoreText = playerScoreObj.GetComponent<Text>();
        _enemyScoreText = enemyScoreObj.GetComponent<Text>();

        _playerScoreText.text = "Player: " + playerHealth;
        _enemyScoreText.text = "Enemy: " + enemyHealth;

    }

    public void UpdatePlayerScoreText(int damage)
    {
        playerHealth = playerHealth - damage;
        _playerScoreText.text = "Player: " + playerHealth;
    }

    public void UpdateEnemyScoreText(int damage)
    {
        enemyHealth = enemyHealth - damage;
        _enemyScoreText.text = "Enemy: " + enemyHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
