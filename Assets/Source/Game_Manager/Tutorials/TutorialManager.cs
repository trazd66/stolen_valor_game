using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public GameObject page0;
    public GameObject page1;
    public GameObject page2;
    public GameObject page3;
    public GameObject page4;
    public GameObject page5;
    private int page_num = 0;
    // Start is called before the first frame update
    void Start()
    {
        page0.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && page_num == 0)
        {
            //SceneManager.LoadScene("arena+platform_test_scene");
            page0.SetActive(false);
            page1.SetActive(true);
            page_num = 1;
        }

        else if (Input.GetButtonDown("Jump") && page_num == 1)
        {
            page1.SetActive(false);
            page2.SetActive(true);
            page_num = 2;
        }

        else if (Input.GetButtonDown("Jump") && page_num == 2)
        {
            page2.SetActive(false);
            page3.SetActive(true);
            page_num = 3;
        }
        else if (Input.GetButtonDown("Jump") && page_num == 3)
        {
            page3.SetActive(false);
            page4.SetActive(true);
            page_num = 4;
        }

        else if (Input.GetButtonDown("Jump") && page_num == 4)
        {
            page4.SetActive(false);
            page5.SetActive(true);
            page_num = 5;
        }

        else if (Input.GetButtonDown("Jump") && page_num == 5)
        {
            page5.SetActive(false);
            page_num = 0;
            SceneManager.LoadScene("arena+platform_test_scene");
        }

        else if (Input.GetButtonDown("Attack") && page_num == 1)
        {
            page1.SetActive(false);
            page0.SetActive(true);
            page_num = 0;
        }

        else if (Input.GetButtonDown("Attack") && page_num == 2)
        {
            page2.SetActive(false);
            page1.SetActive(true);
            page_num = 1;
        }

        else if (Input.GetButtonDown("Attack") && page_num == 3)
        {
            page3.SetActive(false);
            page2.SetActive(true);
            page_num = 2;
        }

        else if (Input.GetButtonDown("Attack") && page_num == 4)
        {
            page4.SetActive(false);
            page3.SetActive(true);
            page_num = 3;
        }

        else if (Input.GetButtonDown("Attack") && page_num == 5)
        {
            page5.SetActive(false);
            page4.SetActive(true);
            page_num = 4;
        }
    }
}
