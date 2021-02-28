using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboMeter : MonoBehaviour
{
    public Image ComboRing;
    public Text ComboCount;

    public float Player_combo_count;
    public float MaxComboNum;
    private float displayNum;

    private float lerpSpeed;
    private float textLerpSpeed;
    // Start is called before the first frame update
    void Start()
    {
        Player_combo_count = 0;
        MaxComboNum = 2000;
        displayNum = Player_combo_count;
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

        ComboRing.fillAmount = Mathf.Lerp(ComboRing.fillAmount, Player_combo_count / MaxComboNum, lerpSpeed);
        
    }

    void TextDisplay()
    {
        
        if(Player_combo_count!= displayNum)
        {
            if(displayNum<Player_combo_count)
            {
                displayNum += (4f * Time.deltaTime) * (Player_combo_count - displayNum);
                if(displayNum>= Player_combo_count)
                {
                    displayNum = Player_combo_count;
                }
            }
            else
            {
                displayNum -= (4f * Time.deltaTime) * (displayNum-Player_combo_count);
                if (displayNum <= Player_combo_count)
                {
                    displayNum = Player_combo_count;
                }
            }
            ComboCount.text = displayNum.ToString("0.");
        }
        
        
    }

    void ColorChanger()
    {
        Color bosshealthColor = Color.Lerp(Color.red, Color.green, Player_combo_count / MaxComboNum);

        ComboRing.color = bosshealthColor;
        
    }

    void Combo_increase( int inc_num)
    {
        
        if (Player_combo_count + inc_num> MaxComboNum)
        {
            Player_combo_count = MaxComboNum;
        }
        else
        {
            Player_combo_count += Player_combo_count;
        }
        textLerpSpeed = 0f;
    }

    void Combo_decrease(int dec_num)
    {
        if (Player_combo_count - dec_num <0)
        {
            Player_combo_count = 0f;
        }
        else
        {
            Player_combo_count -= Player_combo_count;
        }
        textLerpSpeed = 0f;
    }
    
}
