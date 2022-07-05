using System;
using System.Collections;
using System.Collections.Generic;
using TofAr.V0.Hand;
using UnityEngine;
using UnityEngine.UI;

public class TouchDetector : MonoBehaviour
{
    public TouchClient client;

    private Matrix4x4 matWorld2Screen;
    private float sw, sh;

    private Vector3 center = Vector3.zero;
    private Vector3 current = new Vector3(-100, -100, 0);

    private float distance = 0.1f;
    private float threshould = 0.05f;
    private bool touch = false;

    public Image im;

    // Start is called before the first frame update
    void Start()
    {
        // コールバック関数の登録
        TofArHandManager.OnFrameArrived += FrameArrived;

        matWorld2Screen = Camera.main.projectionMatrix * Camera.main.worldToCameraMatrix;
        sw = Screen.width;
        sh = Screen.height;
    }

    private Vector3 World2Screen(Vector3 xyz)
    {
        Vector4 p1 = matWorld2Screen.MultiplyPoint(xyz);
        p1 = new Vector3(p1.x + 1f, p1.y + 1f, 0) / 2f;
        p1 = new Vector3(p1.x * Screen.width, p1.y * Screen.height, 0);
        var unityScreenPos = new Vector3(p1.x, p1.y, 0);
        return unityScreenPos;
    }

    private void FrameArrived(object sender)
    {
        var manager = sender as TofArHandManager;
        var handData = manager.HandData.Data;

        if (handData.handStatus == HandStatus.BothHands || handData.handStatus == HandStatus.LeftHand)
        {
            Vector3[] leftPoints = handData.featurePointsLeft;
            Vector3 tips = averageV(leftPoints);

            if (current.x == -100)
            {
                current = World2Screen(tips);
            }
            else
            {
                current = (current + World2Screen(tips)) / 2;
            }

            Vector3 vi = leftPoints[(int)HandPointIndex.IndexTip];
            Vector3 vt = leftPoints[(int)HandPointIndex.ThumbTip];

            float dis = Vector3.Distance(vi, vt);
            distance = (distance + dis) / 2;
            bool touch0 = distance < threshould;

            string action = "";
            if (touch0 != touch)
            {
                touch = touch0;
                if (touch)
                {
                    // Down
                    action = "d";
                }
                else
                {
                    // Up
                    action = "u";
                }
            }
            else if (touch)
            {
                action = "m";
            }

            if (action != "" && center != Vector3.zero)
            {
                Vector3 xy = (current - center);
                int x = (int)xy.x + (int)sw / 2;
                int y = -(int)xy.y + (int)sh / 2;
                client.Send(action + " " + x + " " + y + "\n");
            }
        }
    }

    public void calibrate()
    {
        center = current;
    }

    /// <summary>
    /// 手先の平均座標を算出する
    /// </summary>
    /// <param name="handPoints"></param>
    /// <returns></returns>
    private Vector3 averageV(Vector3[] handPoints)
    {
        Vector3 v = Vector3.zero;
        int c = 0;
        for (int i = 0; i <= (int)HandPointIndex.ThumbRootWrist; i++)
        {
            if (handPoints[i].z > 0)
            {
                c++;
                v += handPoints[i];
            }
        }

        v = v / c;
        // screen rotation
        Vector3 vv = new Vector3(v.y, -v.x, v.z);
        
        return vv;
    }

    // Update is called once per frame
    void Update()
    {
        im.rectTransform.position = current;
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        // Debug
        string txt = "center: " + center + "\r\n";
        txt += "current: " + current + "\r\n";
        txt += "touch: " + touch + "\r\n";
        GUI.Box(new Rect(10, 10, 200, 23*10), txt);
    }
#endif
}
