using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS_SceneProp : MonoBehaviour
{
    public GameObject[] creating_pos;

    private float timer = 1.0f;
    private float world_timer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        world_timer += Time.deltaTime;

        int index = Random.Range(0, 4);
        
        if(world_timer>= timer)
        {
            float x = Random.Range(0.0f, 1.0f);
            float y = Random.Range(0.0f, 1.0f);
            GameObject prop_prefabs = Instantiate(creating_pos[index], this.transform.position, this.transform.rotation);
            prop_prefabs.GetComponent<Rigidbody>().AddForce(new Vector3(x, y, 0).normalized * 500f);
            world_timer = 0;
        }
    }

}
