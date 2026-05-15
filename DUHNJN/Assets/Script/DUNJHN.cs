using Rewired;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// SUHMIF DUNJHN Gameplay Script

public class DUNJHN : MonoBehaviour
{
    [Header("Rewired")]
    private Player player;

    [Header("Health")]
    // Health integer variable, ui bar & healing with amount
    private int minimumHealth = 10; // because instantly killing them would be too harsh
    private int maximumHealth = 100;
    private int currentHealth;
    public Slider healthSlider;

    [Header("Potion")]
    private int minimumPotion = 0;
    private int maximumPotion = 14; // Limit amount player can have at once
    private int currentPotion = 0;
    public Text potionAmountText;

    [Header("Canvas'")]
    public GameObject playerHUD; 
    public GameObject scoreScreen;

    [Header("Informing Player")]
    public Text updateTextHealth; 

    [Header("Score Screen")]
    private int defeatedCounter;
    private float elapsedTime = 0f;
    private bool timerRunning;
    public Text defeatedText;
    public Text timerText;
    public GameObject scoreScreenDeathTitle; //circumstantial/ responsive title
    public GameObject scoreScreenAliveTitle;

    [Header("Enemy")]
    private double spawnChance = 0.6; // Chance of enemy appearing
    private int enemyCurrentHealth;
    private int currentDamage;
    public GameObject enemy;
    public Slider enemyHealthBar;
    public Text enemyText;
    public Animator enemyHitAnimation;

    private void Awake()
    {
        currentHealth = 100;

        timerRunning = true;

        player = ReInput.players.GetPlayer(0); // Gets Scene's Controls
    }

    //-----------------------------------Start is called once upon creation-------------------------
    private void Start()
    {
        UpdateHealthBar();

        currentPotion = Random.Range(minimumPotion, maximumPotion);
        UpdatePotionAmountText();

        enemyCurrentHealth = Random.Range(minimumHealth, maximumHealth);

        enemyHealthBar.value = enemyCurrentHealth;

        enemy.SetActive(true);

    }

    //-----------------------------------Update is called once per frame----------------------------
    void Update()
    {
        UpdateHealthBar();
        UpdatePotionAmountText();
        checkCurrentHealth();

        if (player.GetButtonDown("End")) // This is set as the E key for now, to Win
        {
            if (!scoreScreen.activeSelf)
            {
                playerHUD.SetActive(false); // Hides hud

                scoreScreen.SetActive(true);

                scoreScreenAliveTitle.SetActive(true); // Changes title, default is off for both titles

                StartCoroutine(pauseReset()); // Begins the game again so player doesn't have to refresh page
            }
        }

        if (timerRunning == true) 
        {
            elapsedTime += Time.deltaTime; // Just a normal timer for the scorescreen until it's made false
        }

        if (scoreScreen.activeInHierarchy) // Score Screen stops time count and displays
        {
            timerRunning = false; // Stops Timer

            if (timerRunning == false)
            {
                int minutes = Mathf.FloorToInt(elapsedTime / 60f);
                int seconds = Mathf.FloorToInt(elapsedTime % 60f);
                timerText.text = $"{minutes:00}:{seconds:00}"; // Displays minutes and seconds instead of just say the time in only seconds, appears like normal time
            }

            return;
        }
    }

    //-----------------------------------Healing-------------------------
    public void Potion()
    {
        if (currentPotion > minimumPotion && currentHealth < maximumHealth)
        {
            int healAmount = Random.Range(1, 20);
            currentHealth = Mathf.Min(currentHealth + healAmount, maximumHealth);
            currentPotion -= 1;
            updateTextHealth.text = "+" + healAmount.ToString() + " Vitality";
            UpdateHealthBar();
            UpdatePotionAmountText();
        }
    }


    //-----------------------------------Attacking----------------------------
    public void playerAttack()
    {
        if (!(enemy?.activeInHierarchy ?? false))
        {
            return; // Stops player from attacking if an enemy is not on screen
        }

        if (enemy.activeInHierarchy)
        {
            enemyText.gameObject.SetActive(true);
            currentDamage = Random.Range(2, 18);
            enemyCurrentHealth -= currentDamage;
            enemyText.text = "Thou Attacked For " + currentDamage.ToString() + " Damage";
            enemyHealthBar.value = enemyCurrentHealth;

            checkCurrentHealthEnemy();
            enemyHitAnimation.SetTrigger("Hit"); // Small Animation to visually represent Attack
        }

        enemyAttack();
    }

    //-----------------------------------Player's Death----------------------------
    void checkCurrentHealth()
    {
        if (currentHealth <= 0)
        {
            playerHUD.SetActive(false); // Hides hud

            scoreScreen.SetActive(true);

            scoreScreenDeathTitle.SetActive(true); // Changes title, default is off for both titles
        }
    }

    //-----------------------------------Enemy Health----------------------------

    void checkCurrentHealthEnemy() // Hides sprite to visually represent defeating enemy
    {
        if (enemyCurrentHealth <= 0)
        {
            enemy.SetActive(false);
            enemyText.gameObject.SetActive(true);
            defeatedCounter += 1; // Counter for the score screen
            defeatedText.text = defeatedCounter.ToString();

            currentPotion += Random.Range(1, 6); // Rewards player with potion/s

            enemyText.text = "Enemy Slain";

            StartCoroutine(pauseCoroutine());
        }
    }

    //-----------------------------------Enemy Attack----------------------------
    void enemyAttack()
    {
        currentDamage = Random.Range(2, 18); // Picks a number in-between for the damage amount 

        currentHealth -= currentDamage; // Simply takes the damage amount off the player's health counter

        updateTextHealth.text = currentDamage.ToString() + " Damage taken"; // Informs player how much damage they've taken
    }

    //-----------------------------------Keeps Health and Potion Correctly Displayed-------------------------
    void UpdateHealthBar()
    {
        healthSlider.value = currentHealth;
    }

    void UpdatePotionAmountText()
    {
        potionAmountText.text = currentPotion.ToString(); // int to string
    }


    //-----------------------------------Delay-------------------------

    IEnumerator pauseCoroutine()
    {
        yield return new WaitForSeconds (2);
        enemy.SetActive(true);
        enemyCurrentHealth = Random.Range(minimumHealth, maximumHealth);

        enemyHealthBar.value = enemyCurrentHealth;
    }
    IEnumerator pauseReset()
    {
        yield return new WaitForSeconds (2);
        SceneManager.LoadScene(0);
    }

}