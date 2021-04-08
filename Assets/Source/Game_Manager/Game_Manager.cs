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
    void Start()
    {   
        instance = this;     
        manager_enabled = true;
    }
    

    // Update is called once per frame
    void Update()
    {
        if(!manager_enabled) return;

        prev_scene_name = SceneManager.GetActiveScene().name;

        if (game_state == 0 && SceneManager.GetActiveScene().name != tutorial1_scene_name){
            SceneManager.LoadScene(tutorial1_scene_name);
            manager_enabled = false;            
        }

        if(game_state == 1 && SceneManager.GetActiveScene().name != tutorial2_scene_name){
            SceneManager.LoadScene(tutorial2_scene_name);
            manager_enabled = false;            
    }
        if(game_state == 2 && SceneManager.GetActiveScene().name != tutorial3_scene_name){
            SceneManager.LoadScene(tutorial3_scene_name);
            manager_enabled = false;            
        }
        if(game_state == 3 && SceneManager.GetActiveScene().name != tutorial4_scene_name){
            SceneManager.LoadScene(tutorial4_scene_name);
            manager_enabled = false;            
    }
        if(game_state == 4 && SceneManager.GetActiveScene().name != main_game_scene_name ){
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
            AudioManager.instance.SetLoop("leveltheme1_v2", true);
            AudioManager.instance.Play("leveltheme1_v2");
            manager_enabled = false;
        }
        else if (prev_scene_name != "Game_Init")
        {
            AudioManager.instance.Stop("leveltheme1_v2");
            manager_enabled = false;
        }

        if (SceneManager.GetActiveScene().name == menu_scene_name && player_controller != null)
        {
            player_controller.enable_gameplay();
            AudioManager.instance.SetLoop("leveltheme1_v2", true);
            AudioManager.instance.Play("leveltheme1_v2");
            manager_enabled = false;
        }
        else if (prev_scene_name != "Game_Init")
        {
            AudioManager.instance.Stop("leveltheme1_v2");
            manager_enabled = false;
        }

        if (AudioManager.instance != null){
            //AudioManager.instance.SetThemeVolume(game_volume_theme);
            //AudioManager.instance.SetThemeVolume(game_volume_SFX);
        }
    }


    public void setState(int state){
        manager_enabled = true;
        game_state = state;
    }
}
