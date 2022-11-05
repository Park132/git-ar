using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS_MapMake : MonoBehaviour
{
    public GameObject wall; // 벽으로 생성될 오브젝트, 추후의 배열로 선언될 예정
    public GameObject centerm1;

    public GameObject wall_marker1;

    public Transform c1; // 정점 1
    public Transform c2; // 정점 2


    // Start is called before the first frame update
    void Start()
    {
        MapInit3(c1, c2, 10, 7);
    }

    void MapInit3(Transform o1, Transform o2, float a, float b) // 3안
    {
        // 물체1의 위치, 물체2의 위치, x축 가중치, z축 가중치
        // 삼각함수로 구현해내는 방안
        
        centerm1.transform.position = new Vector3((o1.transform.position.x + o2.transform.position.x) / 2, 0, (o1.transform.position.z + o2.transform.position.z) / 2); // 중점

        for(int i = 0; i<= 360; i+=20) //19개 생성
        {
            wall_marker1.transform.position = new Vector3(centerm1.transform.position.x + a * Mathf.Cos(i), 0, centerm1.transform.position.z + b*Mathf.Sin(i));
            Instantiate(wall, wall_marker1.transform.position, wall_marker1.transform.rotation);
        }
    }

    

    //-------------------------
    // https://m.blog.naver.com/PostView.naver?isHttpsRedirect=true&blogId=whitejopd&logNo=70167518161
    // https://m.blog.naver.com/rk9034/221449408888
    // https://namu.wiki/w/%ED%83%80%EC%9B%90

    // 1안
    // 시간 증가를 x축 증가로 이용
    // 그 때 타원의 방정식에 따라 출력되는 y값 마다
    // 물체가 생성되게 하는 것은 어떤지?
    // ㄴ x0 또는 y0이 0보다 작은 경우 맵 생성이 끊길 것임

    // 2안
    // A'와 A의 거리를 구해낸 다음 반복문으로 돌리면서
    // 결과값인 y가 나올 때마다 오브젝트 생성
    // 1안의 문제점을 고칠 수 있을 것 같음
    // 
    //----------------------2 안

    // 3안
    // 삼각함수 이용

}
