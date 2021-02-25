using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarApi : MonoBehaviour
{
    public Image BossHealthBar;
    public Image CharacterHealthBar;

    private float lerpSpeed;

    public HealthInfo boss_health;
    public HealthInfo character_health; 
    
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
        
        BossHealthBar.fillAmount = Mathf.Lerp(BossHealthBar.fillAmount, boss_health.curr_health / boss_health.max_health, lerpSpeed);
        CharacterHealthBar.fillAmount = Mathf.Lerp(CharacterHealthBar.fillAmount,  character_health.curr_health / character_health.max_health, lerpSpeed);
    }

    void ColorChanger()
    {
        Color bosshealthColor = Color.Lerp(Color.red, Color.green,  boss_health.curr_health / boss_health.max_health);
        Color charhealthColor = Color.Lerp(Color.red, Color.green, character_health.curr_health / character_health.max_health);
        BossHealthBar.color = bosshealthColor;
        CharacterHealthBar.color = charhealthColor;
    }


}
