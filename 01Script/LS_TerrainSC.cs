using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEngine.TerrainUtils;

public class LS_TerrainSC : MonoBehaviour
{
    Terrain ter;

    public GameObject centerm1;

    public Transform c1; // 정점 1
    public Transform c2; // 정점 2
    public Transform c3;

    public GameObject ground;

    private int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        ter = this.GetComponent<Terrain>();
        ground = this.transform.parent.gameObject;
        ter.enabled = false;
    }

    private void Update()
    {
        if (GameManager.Instance.distanceEP != 0)
        {
            ter.enabled = true;
            centerm1.transform.position = new Vector3((c1.transform.position.x + c2.transform.position.x) / 2, (c1.transform.position.y + c2.transform.position.y) / 2 - 3, (c1.transform.position.z + c2.transform.position.z) / 2); // 중점
            ground.transform.position = centerm1.transform.position;
            ter.terrainData.size = new Vector3(GameManager.Instance.distanceEP *3f, 50, GameManager.Instance.distanceEP * 3f);
            this.transform.localPosition = new Vector3(ter.terrainData.size.x * -0.5f, 0, ter.terrainData.size.z * -0.5f);
            ground.transform.rotation = Quaternion.Euler(PlaneNVec());
        }
    }

    public Vector3 PlaneNVec()
    {
        Vector3[] c = new Vector3[3];

        for (int i = 0; i < 3; i++)
        {
            if (GameManager.Instance.marker.markerExist[i])
                c[i] = GameManager.Instance.marker.markerObj[i].transform.position;
        }

        // vector 1
        Vector3 a1 = new Vector3((c[2].x - c[0].x),
                                 (c[2].y - c[0].y),
                                 (c[2].z - c[0].z));
        // vector2
        Vector3 a2 = new Vector3((c[1].x - c[0].x),
                                 (c[1].y - c[0].y),
                                 (c[1].z - c[0].z));

        // vector3 a1과 a2를 외적한 a3
        Vector3 a3 = Vector3.Cross(a1, a2).normalized;
        return a3;
    }
}
