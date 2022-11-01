using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCtrl : MonoBehaviour
{
    public GameObject point1;
    public GameObject point2;

    // Start is called before the first frame update
    void Start()
    {
        point1 = GameObject.FindGameObjectWithTag("Player");
        point2 = GameObject.FindGameObjectWithTag("Enemy");

        InitiatingMap();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitiatingMap()
    {
        Vector3 center = new Vector3((point2.transform.position.x + point1.transform.position.x) / 2, 0, (point2.transform.position.z + point1.transform.position.z) / 2);
        Debug.Log(center);


    }
}
