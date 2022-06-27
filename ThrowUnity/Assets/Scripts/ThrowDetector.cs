using System;
using System.Collections;
using System.Collections.Generic;
using TofAr.V0.Hand;
using UnityEngine;
using UnityEngine.UI;

public class ThrowDetector : MonoBehaviour
{
    public TouchClient client;
    public Text gestureText;

    private GestureIndex detectedGesture = GestureIndex.None;
    private float last = 0;

    void Start()
    {
        // コールバック関数の登録
        TofArHandManager.OnGestureEstimated += GestureEstimated;
    }

    /// <summary>
    /// ジェスチャ検出されたときに呼び出される。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="result"></param>
    private void GestureEstimated(object sender, GestureResultProperty result)
    {
        // ジェスチャの種類
        GestureIndex gi = result.gestureIndex;
        detectedGesture = gi;
    }

    void Update()
    {
        float current = Time.realtimeSinceStartup;

        if (detectedGesture != GestureIndex.None)
        {
            bool throwBall = 
                (detectedGesture == GestureIndex.HandThrow
                    || detectedGesture == GestureIndex.FingerThrow);

            detectedGesture = GestureIndex.None;
            last = current;
            gestureText.text = "" + detectedGesture;

            if (throwBall)
            {
                client.ThrowBall();
            }
        }

        // ジェスチャ検出後 1 秒間表示
        float alpha = Mathf.Max(0, 1.0f - (current - last));
        gestureText.color = new Color(0, 0, 0, alpha);
    }
}
