using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS_MapMake : MonoBehaviour
{
    public GameObject wall; // 벽으로 생성될 오브젝트, 추후의 배열로 선언될 예정
    public GameObject centerm1;
    public GameObject wall_marker1;
    public GameObject wall_marker2;
    public GameObject wall_marker3;

    public Transform c1; // 정점 1
    public Transform c2; // 정점 2
    // public Transform center;
    public int a = 7; // a의 가중치 x축
    public int b = 4; // b의 가중치 z축


    // Start is called before the first frame update
    void Start()
    {
        //MapInit(c1, c2);
        MapInit2();
        /*
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
        */
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
    void MapInit(Transform v1, Transform v2)
    {
        centerm1.transform.position = new Vector3((v2.position.x + v1.position.x) / 2, (v2.position.y + v1.position.y) / 2, (v2.position.z + v1.position.z) / 2);
        Debug.Log("Position of Center : " + centerm1.transform.position);

        //=========================================================================================
        /*                                                                                        =
         타원의 방정식                                                                            =
        (x-x0)^2 / a^2 + (y-y0)^2 / b^2 = 1                                                       =
         */                                                                                       
        //=========================================================================================

        int A1 = (int)(-a + centerm1.transform.position.x); // 제일 왼쪽 정점
        int A2 = (int)(a + centerm1.transform.position.x); // 제일 오른쪽 정점

        int B1 = (int)(-b + centerm1.transform.position.z); // 제일 아래쪽 정점
        int B2 = (int)(b + centerm1.transform.position.z); // 제일 위쪽 정점

        //=========================================================================================
        // A1에서 A2까지의 x값들을 반복문으로 돌린다.
        // 이때 x값에 의한 결과인 y값이 2개(양수, 음수)가 나올 것이고 그곳에 오브젝트를 생성할 것임
        //=========================================================================================

        // y값을 구해내는 방정식 1
        for(int i = A1; i<= A2; i++)
        {
            float y1 = Mathf.Sqrt((b * b) - ((b * b) / (a * a) + (b * b) % (a * a)) * (i * i));
            Debug.Log("i : " + i);
            Debug.Log("y1 : " + y1);
            Debug.Log("y2 : " + -y1);
        }
    }

    void MapInit2()
    {
        // 이 방법은 타원으로 맵이 그려진 것처럼 보여주는 방법임
        // 타원 방정식은 실제로 쓰지 않지만, 타원방정식을 이용한 것처럼 보여줌
        // 아 ㅋㅋ 그냥 그렇게 보여지는 것처럼 잔머리 굴리자구요 ㅋㅋ
        
        // 중점을 포인트로 잡고 회전하면서 위치를 리턴, 회전할 때마다 다른 회전각과 다른 길이를 줌으로써 타원을 그려보게 하기
        for(int i = -1; i <= 1; i+=2)
        {
            for (int j = -1; j <= 1; j += 2)
            {
                wall_marker1.transform.position = new Vector3(i * 2, 0, j * 2 * Mathf.Sqrt(3));
                wall_marker2.transform.position = new Vector3(i * (5 / 2 + 5 % 2), 0, j * (5 / 2 + 5 % 2) * Mathf.Sqrt(2));
                wall_marker3.transform.position = new Vector3(i * 3 * Mathf.Sqrt(3), 0, j * 3);

                Instantiate(wall, wall_marker1.transform.position, wall_marker1.transform.rotation);
                Instantiate(wall, wall_marker2.transform.position, wall_marker2.transform.rotation);
                Instantiate(wall, wall_marker3.transform.position, wall_marker3.transform.rotation);
            }
        }
    }
}
