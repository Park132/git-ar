using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS_MapMake : MonoBehaviour
{
    public GameObject wall; // ������ ������ ������Ʈ, ������ �迭�� ����� ����
    public GameObject wall_marker1;
    public GameObject wall_marker2;

    public Transform c1; // ���� 1
    public Transform c2; // ���� 2
    // public Transform center;
    public int a = 7; // a�� ����ġ x��
    public int b = 4; // b�� ����ġ z��


    // Start is called before the first frame update
    void Start()
    {
        Vector3 center_pos = new Vector3((c2.position.x + c1.position.x) / 2, 0, (c2.position.z + c1.position.z) / 2); // Ÿ���� ����

        Debug.Log("center : " + center_pos);


        int x_value_int = (int)center_pos.z; // �׷����� ġ�� x�� ����

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

}
