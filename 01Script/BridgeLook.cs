using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeLook : MonoBehaviour
{
    public GameObject P1, P2;
    float distance;
    // Update is called once per frame
    public void SettingP(GameObject p1, GameObject p2)
    { P1 = p1; P2 = p2; }
    void Update()
    {
        this.transform.LookAt(P2.transform.position);
        this.transform.position = P1.transform.position + (P2.transform.position - P1.transform.position) * 0.5f;
        distance = Vector3.Distance(P1.transform.position, P2.transform.position);
        this.transform.localScale = new Vector3(0.25f, 0.2f, distance * 0.8f);

    }
}
