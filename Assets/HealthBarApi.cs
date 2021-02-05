using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarApi : MonoBehaviour
{
    public Image BossHealthBar;
    public Image CharacterHealthBar;

    public float Boss_health, Boss_maxHealth = 100;
    public float Character_health, Character_maxHealth = 100;

    private float lerpSpeed;
    // Start is called before the first frame update
    void Start()
    {
        Boss_health = Boss_maxHealth;
        Character_health = Character_maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        lerpSpeed = 3f * Time.deltaTime;
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
}
