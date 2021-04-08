using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonSelect : MonoBehaviour
{
    // Start is called before the first frame update
    
    public Button start;
    public Button back;
    private bool is_in_play;
    private bool m_inuse, c_inuse;
    void Start()
    {
        start.Select();
        is_in_play = true;
        m_inuse = false;
        c_inuse = false;
    }

    void Update()
    {
        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && (c_inuse==false))
        {
            
            Debug.Log("123123");
            if (is_in_play)
            {
                start.Select();
            }
            else
            {
                back.Select();
            }

            c_inuse = true;
            m_inuse = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            c_inuse = false;
            m_inuse = true;
        }
    }

    public void select_star()
    {
        start.Select();
    }

    public void select_back()
    {
        back.Select();
    }

    public void inplay()
    {
        is_in_play = true;
    }

    public void inop()
    {
        is_in_play = false;
    }
}
