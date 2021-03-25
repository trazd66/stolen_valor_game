using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial1_Reward : MonoBehaviour
{
    private Collider col;

    public Tutorial1_Manager tutorial1_Manager;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
        col = gameObject.GetComponent<Collider>();
    }

    void Update()
    {
        Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("PlayerHitbox"));

        if (cols.Length > 0)
        {
            tutorial1_Manager.tutorial_finished = true;
        }
    }

}
