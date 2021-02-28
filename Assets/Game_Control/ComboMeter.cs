using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboMeter : MonoBehaviour
{
    public Image ComboRing;
    public Text ComboCount;

    public ComboInfo combo_info;

    public float MaxComboNum;


    private float lerpSpeed;
    private float textLerpSpeed;
    // Start is called before the first frame update
    void Start()
    {
        MaxComboNum = 2000;
        ComboCount.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        lerpSpeed = 3f * Time.deltaTime;
        
        ComboRingFiller();

        TextDisplay();
        ColorChanger();
        
    }

    void ComboRingFiller()
    {

        ComboRing.fillAmount = Mathf.Lerp(ComboRing.fillAmount, combo_info.combo_points / MaxComboNum, lerpSpeed);
        
    }

    void TextDisplay()
    {
        if(textLerpSpeed<1.0f)
        {
            textLerpSpeed += Time.deltaTime*0.25f;
            float count_temp = float.Parse(ComboCount.text);
            ComboCount.text = (Mathf.Lerp(count_temp, combo_info.combo_points, textLerpSpeed)).ToString("0.");
        }
        
    }

    void ColorChanger()
    {
        Color bosshealthColor = Color.Lerp(Color.red, Color.green, combo_info.combo_points / MaxComboNum);

        ComboRing.color = bosshealthColor;
        
    }

    void Combo_increase( int inc_num)
    {
        
        if (combo_info.combo_points + inc_num> MaxComboNum)
        {
            combo_info.combo_points = MaxComboNum;
        }
        else
        {
            combo_info.combo_points += inc_num;
        }
        textLerpSpeed = 0f;
    }

    void Combo_decrease(int dec_num)
    {
        if (combo_info.combo_points - dec_num < 0)
        {
            combo_info.combo_points = 0f;
        }
        else
        {
            combo_info.combo_points -= dec_num;
        }
        textLerpSpeed = 0f;
    }
    
}
