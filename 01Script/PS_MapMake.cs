using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS_MapMake : MonoBehaviour
{
    public GameObject wall; // ������ ������ ������Ʈ, ������ �迭�� ����� ����
    public GameObject centerm1;

    public GameObject wall_marker1;

    public Transform c1; // ���� 1
    public Transform c2; // ���� 2


    // Start is called before the first frame update
    void Start()
    {
        MapInit3(c1, c2, 10, 7);
    }

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
