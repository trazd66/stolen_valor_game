using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game_Control;
using UnityEngine.SceneManagement;

public class Game_Manager : MonoBehaviour
{

    public string menu_scene_name;
    public string tutorial1_scene_name;
    public string tutorial2_scene_name;
    public string tutorial3_scene_name;
    public string tutorial4_scene_name;
    public string main_game_scene_name;


    public float game_volume_theme = 0.1f;
    public float game_volume_SFX = 0.3f;

    public static Game_Manager instance;

    public Player_controller player_controller;
    public int game_state;

    private bool manager_enabled;
    private string prev_scene_name;

    private bool on_fighting_scene;
    private bool game_over_screen_shown;
    
    void Start()
    {
        instance = this;
        manager_enabled = true;
        on_fighting_scene = false;
        if (SceneManager.GetActiveScene().name == main_game_scene_name)
        {
            on_fighting_scene = true;
        }
        game_over_screen_shown = false;
    }


    // Update is called once per frame
    void Update()
    {

        if(on_fighting_scene){
            if(player_controller.player_health_info.is_dead){
                if(!game_over_screen_shown){
                    game_over_screen_shown = true;
                    StartCoroutine(delayedDeath());
                }
                if (Input.GetButtonDown("Jump"))
                {
                    game_over_screen_shown = false;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }

        }

        if (!manager_enabled) return;

        prev_scene_name = SceneManager.GetActiveScene().name;

        if (game_state == 0 && SceneManager.GetActiveScene().name != tutorial1_scene_name){
            SceneManager.LoadScene(tutorial1_scene_name);
            if (AudioManager.instance != null)
            {
                AudioManager.instance.SetLoop("tutorial theme 2", true);
                AudioManager.instance.Play("tutorial theme 2");
            }
            manager_enabled = false;
        }

        if (game_state == 1 && SceneManager.GetActiveScene().name != tutorial2_scene_name)
        {
            SceneManager.LoadScene(tutorial2_scene_name);
            manager_enabled = false;
        }
        if (game_state == 2 && SceneManager.GetActiveScene().name != tutorial3_scene_name)
        {
            SceneManager.LoadScene(tutorial3_scene_name);
            manager_enabled = false;
        }
        if (game_state == 3 && SceneManager.GetActiveScene().name != tutorial4_scene_name)
        {
            SceneManager.LoadScene(tutorial4_scene_name);
            manager_enabled = false;
        }
        if (game_state == 4 && SceneManager.GetActiveScene().name != main_game_scene_name)
        {
            SceneManager.LoadScene(main_game_scene_name);
            manager_enabled = false;

        }

        if(game_state == 5 && SceneManager.GetActiveScene().name != menu_scene_name)
        {
            SceneManager.LoadScene(menu_scene_name);
            manager_enabled = false;
        }

        if (SceneManager.GetActiveScene().name == main_game_scene_name && player_controller != null)
        {
            player_controller.enable_gameplay();
            AudioManager.instance.SetLoop("leveltheme1_v3", true);
            AudioManager.instance.Stop("tutorial theme 2");
            AudioManager.instance.Play("leveltheme1_v3");
            manager_enabled = false;
        }
        else if (prev_scene_name != "Game_Init")
        {
            AudioManager.instance.Stop("leveltheme1_v3");
            AudioManager.instance.Stop("leveltheme_phase2");
            manager_enabled = false;
        }

        if (SceneManager.GetActiveScene().name == menu_scene_name)
        {
            AudioManager.instance.SetLoop("leveltheme_menu", true);
            AudioManager.instance.Play("leveltheme_menu");
            manager_enabled = false;
        }
        else if (prev_scene_name != "Game_Init")
        {
            AudioManager.instance.Stop("leveltheme_menu");
            manager_enabled = false;
        }

        if(SceneManager.GetActiveScene().name == tutorial1_scene_name)
        {
            AudioManager.instance.SetLoop("leveltheme_tutorial", true);
            AudioManager.instance.Play("leveltheme_tutorial");
            manager_enabled = false;
        }
        else if (!SceneManager.GetActiveScene().name.Contains("tutorial") && prev_scene_name != "Game_Init")
        {
            AudioManager.instance.Stop("leveltheme_tutorial");
            manager_enabled = false;
        }

    }



        //private void reload_scene_if_death()
        //{
            //reset scene if player dies
            //if (player_controller.player_health_info.is_dead)
            //{
                
                //if (Input.GetButtonDown("Jump"))
                //{
                    //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                //}
            //}
        //}

        public IEnumerator delayedDeath(){

            Time.timeScale = 0.2f;
            yield return new WaitForSeconds(0.3f);
            Time.timeScale = 1;
            player_controller.game_over.SetActive(true);
 
        }

    public void setState(int state)
    {
        manager_enabled = true;
        game_state = state;
    }
}
