using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS_MapMake : MonoBehaviour
{
    public GameObject wall; // ������ ������ ������Ʈ, ������ �迭�� ����� ����
    public GameObject centerm1;
    public GameObject wall_marker1;
    public GameObject wall_marker2;
    public GameObject wall_marker3;

    public Transform c1; // ���� 1
    public Transform c2; // ���� 2
    // public Transform center;
    public int a = 7; // a�� ����ġ x��
    public int b = 4; // b�� ����ġ z��


    // Start is called before the first frame update
    void Start()
    {
        MapInit3(c1, c2, 10, 7);
    }

    void MapInit(Transform v1, Transform v2)
    {
        centerm1.transform.position = new Vector3((v2.position.x + v1.position.x) / 2, (v2.position.y + v1.position.y) / 2, (v2.position.z + v1.position.z) / 2);
        Debug.Log("Position of Center : " + centerm1.transform.position);

        //=========================================================================================
        /*                                                                                        =
         Ÿ���� ������                                                                            =
        (x-x0)^2 / a^2 + (y-y0)^2 / b^2 = 1                                                       =
         */                                                                                       
        //=========================================================================================

        int A1 = (int)(-a + centerm1.transform.position.x); // ���� ���� ����
        int A2 = (int)(a + centerm1.transform.position.x); // ���� ������ ����

        int B1 = (int)(-b + centerm1.transform.position.z); // ���� �Ʒ��� ����
        int B2 = (int)(b + centerm1.transform.position.z); // ���� ���� ����

        //=========================================================================================
        // A1���� A2������ x������ �ݺ������� ������.
        // �̶� x���� ���� ����� y���� 2��(���, ����)�� ���� ���̰� �װ��� ������Ʈ�� ������ ����
        //=========================================================================================

        // y���� ���س��� ������ 1
        for(int i = A1; i<= A2; i++)
        {
            float y1 = Mathf.Sqrt((b * b) - ((b * b) / (a * a) + (b * b) % (a * a)) * (i * i));
            Debug.Log("i : " + i);
            Debug.Log("y1 : " + y1);
            Debug.Log("y2 : " + -y1);
        }
    } // 1��

    void MapInit2()
    {
        // �� ����� Ÿ������ ���� �׷��� ��ó�� �����ִ� �����
        // Ÿ�� �������� ������ ���� ������, Ÿ���������� �̿��� ��ó�� ������
        // �� ���� �׳� �׷��� �������� ��ó�� �ܸӸ� �����ڱ��� ����
        
        // ������ ����Ʈ�� ��� ȸ���ϸ鼭 ��ġ�� ����, ȸ���� ������ �ٸ� ȸ������ �ٸ� ���̸� �����ν� Ÿ���� �׷����� �ϱ�
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
    } // 2��

    void MapInit3(Transform o1, Transform o2, float a, float b) // 3��
    {
        // ��ü1�� ��ġ, ��ü2�� ��ġ, x�� ����ġ, z�� ����ġ
        // �ﰢ�Լ��� �����س��� ���
        
        centerm1.transform.position = new Vector3((o1.transform.position.x + o2.transform.position.x) / 2, 0, (o1.transform.position.z + o2.transform.position.z) / 2); // ����


        for(int i = 0; i<= 360; i+=20) //19�� ����
        {
            wall_marker1.transform.position = new Vector3(centerm1.transform.position.x + a * Mathf.Cos(i), 0, centerm1.transform.position.z + b*Mathf.Sin(i));
            Instantiate(wall, wall_marker1.transform.position, wall_marker1.transform.rotation);
        }
    }

    

    //-------------------------
    // https://m.blog.naver.com/PostView.naver?isHttpsRedirect=true&blogId=whitejopd&logNo=70167518161
    // https://m.blog.naver.com/rk9034/221449408888
    // https://namu.wiki/w/%ED%83%80%EC%9B%90

    // 1��
    // �ð� ������ x�� ������ �̿�
    // �� �� Ÿ���� �����Ŀ� ���� ��µǴ� y�� ����
    // ��ü�� �����ǰ� �ϴ� ���� ���?
    // �� x0 �Ǵ� y0�� 0���� ���� ��� �� ������ ���� ����

    // 2��
    // A'�� A�� �Ÿ��� ���س� ���� �ݺ������� �����鼭
    // ������� y�� ���� ������ ������Ʈ ����
    // 1���� �������� ��ĥ �� ���� �� ����
    // 
    //----------------------2 ��

    // 3��
    // �ﰢ�Լ� �̿�

}
