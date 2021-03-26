using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboMeter : MonoBehaviour
{
    public Image ComboRing;
    public Text ComboPointCount;
    public Text ComboCount;

    public ComboInfo combo_info;

    public float MaxComboNum;
    private float displayNum;


    private float lerpSpeed;
    private float textLerpSpeed;
    // Start is called before the first frame update
    void Start()
    {
        MaxComboNum = 500;
        ComboPointCount.text = "500";
        ComboCount.text = "500";
        displayNum = combo_info.getComboPoints();
    }

    // Update is called once per frame
    void Update()
    {
        lerpSpeed = 3f * Time.deltaTime;
        
        ComboRingFiller();

        TextDisplay();
        //ColorChanger();
        
    }

    void ComboRingFiller()
    {

        ComboRing.fillAmount = Mathf.Lerp(ComboRing.fillAmount, combo_info.getComboPoints() / MaxComboNum, lerpSpeed);
        
    }

    void TextDisplay()
    {

        if (combo_info.getComboPoints() != displayNum)
        {
            if (displayNum < combo_info.getComboPoints())
            {
                displayNum += (4f * Time.deltaTime) * (combo_info.getComboPoints() - displayNum);
                if (displayNum >= combo_info.getComboPoints())
                {
                    displayNum = combo_info.getComboPoints();
                }
            }
            else
            {
                displayNum -= (4f * Time.deltaTime) * (displayNum - combo_info.getComboPoints());
                if (displayNum <= combo_info.getComboPoints())
                {
                    displayNum = combo_info.getComboPoints();
                }
            }
            ComboPointCount.text = displayNum.ToString("0.");
        }

        ComboCount.text = combo_info.combo_counter.ToString();


    }

    void ColorChanger()
    {
        Color bosshealthColor = Color.Lerp(Color.red, Color.green, combo_info.getComboPoints() / MaxComboNum);

        ComboRing.color = bosshealthColor;
        
    }
    
}
