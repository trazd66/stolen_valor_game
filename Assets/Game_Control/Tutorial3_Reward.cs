using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Tutorial3_Reward : MonoBehaviour
{
    private Collider col;
    private bool active = false;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
        col = gameObject.GetComponent<Collider>();
    }

    void Update()
    {
        if (active)
        {
            Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("PlayerHitbox"));

            if (cols.Length > 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }


    public void placeReward(Vector3 position)
    {
        gameObject.transform.position = position;
        active = true;
    }
}
