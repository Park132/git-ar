using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS_MapMake : MonoBehaviour
{
    public GameObject[] wall; // 벽으로 생성될 오브젝트, 추후의 배열로 선언될 예정
    public GameObject centerm1;

    public GameObject wall_marker1;

    public Transform c1; // 정점 1
    public Transform c2; // 정점 2

    public GameObject ground_prefab;

    private int i = 0;


    // Start is called before the first frame update
    void Start()
    {
        //MapInit3(c1, c2, 10, 7);
    }

    private void Update()
    {
        if(GameManager.Instance.distanceEP != 0)
        {
            while ( i<1)
            {
                MapInit3(c1, c2, 16, 12);
                i++;
            }
        }

    }

    public void MapInit3(Transform o1, Transform o2, float a, float b) // 3안
    {
        // 물체1의 위치, 물체2의 위치, x축 가중치, z축 가중치
        // 삼각함수로 구현해내는 방안

        centerm1.transform.position = new Vector3((o1.transform.position.x + o2.transform.position.x) / 2, 0, (o1.transform.position.z + o2.transform.position.z) / 2); // 중점
        Instantiate(ground_prefab, centerm1.transform.position, centerm1.transform.rotation);

        for (int i = 0; i <= 360; i += 20) //19개 생성
        {
            int index = Random.Range(0, 26);
            wall_marker1.transform.position = new Vector3(centerm1.transform.position.x + a * Mathf.Cos(i), 0, centerm1.transform.position.z + b * Mathf.Sin(i));
            Instantiate(wall[index], wall_marker1.transform.position, Quaternion.Euler(Random.Range(1f, 179f), Random.Range(1f, 179f), Random.Range(1f, 179f)));
        }
    }
}
