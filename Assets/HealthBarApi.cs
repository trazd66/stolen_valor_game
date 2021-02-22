using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarApi : MonoBehaviour
{
    public Image BossHealthBar;
    public Image CharacterHealthBar;

    public float Boss_health, Boss_maxHealth;
    public float Character_health, Character_maxHealth;

    public GameObject Boss;
    public GameObject Player;

    private bool player_invincible = false;
    private bool enemy_invincible = false;

    private float lerpSpeed;
    // Start is called before the first frame update
    void Start()
    {
        Boss_health = Boss.GetComponent<HealthInfo>().max_health;
        Character_health = Player.GetComponent<HealthInfo>().max_health;
    }

    // Update is called once per frame
    void Update()
    {
        lerpSpeed = 3f * Time.deltaTime;
        //Boss_health = Boss.GetComponent<HealthInfo>().curr_health;
        //Character_health = Player.GetComponent<HealthInfo>().curr_health;
        HealthBarFiller();
        ColorChanger();
    }

    void HealthBarFiller()
    {   
        
        BossHealthBar.fillAmount = Mathf.Lerp(BossHealthBar.fillAmount, Boss_health / Boss_maxHealth, lerpSpeed);
        CharacterHealthBar.fillAmount = Mathf.Lerp(CharacterHealthBar.fillAmount, Character_health / Character_maxHealth, lerpSpeed);
    }

    void ColorChanger()
    {
        Color bosshealthColor = Color.Lerp(Color.red, Color.green, (Boss_health / Boss_maxHealth));
        Color charhealthColor = Color.Lerp(Color.red, Color.green, (Character_health / Character_maxHealth));
        BossHealthBar.color = bosshealthColor;
        CharacterHealthBar.color = charhealthColor;
    }



    /// <summary>
    /// enter the damage deal to the boss
    /// </summary>
    /// <param name="damagePoint"></param>
    public void BossDamage(float damagePoint)
    {
        if (Boss_health>0)
        {
            Boss_health -= damagePoint;
        }
    }

    /// <summary>
    /// enter the heal point you want to heal the boss
    /// </summary>
    /// <param name="healPoint"></param>
    public void BossHeal(float healPoint)
    {
        if (Boss_health < Boss_maxHealth)
        {
            Boss_health += healPoint;
        }
    }
    /// <summary>
    /// enter the damage deal to the character
    /// </summary>
    /// <param name="damagePoint"></param>
    public void CharDamage(float damagePoint)
    {
        if (Character_health > 0)
        {
            Character_health -= damagePoint;
        }
    }

    /// <summary>
    /// enter the heal point you want to heal the character
    /// </summary>
    /// <param name="healPoint"></param>
    public void CharHeal(float healPoint)
    {
        if (Character_health < Character_maxHealth)
        {
            Character_health += healPoint;
        }
    }

    public void set_character_Max(float max_heal)
    {
        Character_maxHealth = max_heal;
    }

    public void set_boss_Max(float max_heal)
    {
        Boss_maxHealth = max_heal;
    }

    public bool get_player_invincible()
    {
        return player_invincible;
    }

    public bool get_enemy_invincible()
    {
        return enemy_invincible;
    }

    public void set_player_invincible(bool new_invincible)
    {
        player_invincible = new_invincible;
    }

    public void set_enemy_invincible(bool new_invincible)
    {
        enemy_invincible = new_invincible;
    }
}
