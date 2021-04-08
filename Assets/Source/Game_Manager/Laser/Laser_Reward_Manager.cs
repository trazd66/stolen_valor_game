using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game_Control;

public class Laser_Reward_Manager : MonoBehaviour
{
    // Start is called before the first frame update

    public ComboInfo combo_info;

    public Pause_Manager pause_manager;
    public Player_controller player_controller;
    public GameObject combo_meter;

    private Collider col;
    private bool active = true;

    private Vector3 position = new Vector3(0f, 2f, -0.558f);

    void Start()
    {
        col = gameObject.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("PlayerHitbox"));

            if (cols.Length > 0)
            {
                combo_info.unlockLaser();
                pause_manager.ShowLaserTutorial();
                pause_manager.PauseLaser();
                Time.timeScale = 0;
                combo_meter.SetActive(true);
                player_controller.cannon.SetActive(true);
                Destroy(gameObject);
            }
        }
    }

    public void placeReward()
    {
        gameObject.transform.position = position;
        active = true;
    }
}
