using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS_MapMake : MonoBehaviour
{
    public GameObject wall; // 벽으로 생성될 오브젝트, 추후의 배열로 선언될 예정
    public GameObject wall_marker1;
    public GameObject wall_marker2;

    public Transform c1; // 정점 1
    public Transform c2; // 정점 2
    // public Transform center;
    public int a = 7; // a의 가중치 x축
    public int b = 4; // b의 가중치 z축


    // Start is called before the first frame update
    void Start()
    {
        Vector3 center_pos = new Vector3((c2.position.x + c1.position.x) / 2, 0, (c2.position.z + c1.position.z) / 2); // 타원의 중점

        Debug.Log("center : " + center_pos);


        int x_value_int = (int)center_pos.z; // 그래프로 치면 x축 중점

        // center_pos.z = (int)center_pos.z;
        for (int i = -a + (int)center_pos.z; i <= a + (int)center_pos.z; i++)
        {
            float temp;
            float temp2;
            temp = Mathf.Sqrt((b * b) - ((b * b) / (a * a) + ((b * b) % (a * a))) * (i * i));
            temp2 = b * Mathf.Sqrt(1 - (i * i) / (a * a));
            Debug.Log("-a + x_value_int : " + (-a + x_value_int));
            Debug.Log("-a + x_value_int : " + (a + x_value_int));

            Debug.Log("i : " + i);
            Debug.Log("temp : " + temp);
            //Debug.Log("temp2 : " + temp2);

            // wall_marker1.transform.position = new Vector3(temp, 0, i);
            // wall_marker2.transform.position = new Vector3(-temp, 0, i);

            wall_marker1.transform.position = new Vector3(i, 0, temp);
            wall_marker2.transform.position = new Vector3(i, 0, -temp);

            Instantiate(wall, wall_marker1.transform.position, wall_marker1.transform.rotation);
            Instantiate(wall, wall_marker2.transform.position, wall_marker2.transform.rotation);
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

}
