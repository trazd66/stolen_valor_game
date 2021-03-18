using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_system_controller : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] particle_system_prefabs;

    private Dictionary<string,GameObject> prefab_dict;
    // private Dictionary<string,Stack<GameObject>> active_pool;
    private Dictionary<string,Stack<GameObject>> disabled_pool;
    public static Particle_system_controller Instance;

    void Start()
    {
        Instance = this;
        prefab_dict = new Dictionary<string, GameObject>();
        disabled_pool = new Dictionary<string, Stack<GameObject>>();
        foreach (GameObject prefab in particle_system_prefabs){
            prefab_dict.Add(prefab.name,prefab); 
        }

        foreach(GameObject prefab in particle_system_prefabs){
            // active_pool.Add(prefab.name, new Stack<GameObject>());
            disabled_pool.Add(prefab.name, new Stack<GameObject>());
        }       
    }


    private void instantiate_prefab_into_pool(string prefab_name)
    {   var obj = GameObject.Instantiate(prefab_dict[prefab_name]);
        obj.SetActive(false);
        disabled_pool[prefab_name].Push(obj);
    }

    public void set_particle(string name, Vector3 location, float duration){
        
        if(disabled_pool[name].Count == 0){
            instantiate_prefab_into_pool(name);
        }
        var obj = disabled_pool[name].Pop();
        obj.SetActive(true);
        obj.transform.position = location;
        StartCoroutine(_disable_obj_after_duration(name,obj, duration));
    }

    private IEnumerator _disable_obj_after_duration(string name, GameObject gameObject, float duration)
     {        
         //Wait for 14 secs.
         yield return new WaitForSeconds(duration);
         //Turn My game object that is set to false(off) to True(on).
         gameObject.SetActive(false);
         disabled_pool[name].Push(gameObject);
     }

    

}
