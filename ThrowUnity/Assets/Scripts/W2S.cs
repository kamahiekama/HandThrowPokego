using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// World Position と Screen Position の確認用スクリプト
/// </summary>
public class W2S : MonoBehaviour
{
    public Transform t;
    public RectTransform rt;

    private Matrix4x4 matWorld2Screen;
    private float sw, sh;

    // Start is called before the first frame update
    void Start()
    {
        matWorld2Screen = Camera.main.projectionMatrix * Camera.main.worldToCameraMatrix;
        sw = Screen.width;
        sh = Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        //rt.position = Camera.main.WorldToScreenPoint(t.position);

        Vector4 p1 = matWorld2Screen.MultiplyPoint(t.position);
        p1 = new Vector3(p1.x + 1f, p1.y + 1f, 0) / 2f;
        p1 = new Vector3(p1.x * Screen.width, p1.y * Screen.height, 0);
        var unityScreenPos = new Vector2(p1.x, p1.y);
        rt.position = unityScreenPos;     
    }
}
