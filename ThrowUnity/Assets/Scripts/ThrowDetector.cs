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
        // �R�[���o�b�N�֐��̓o�^
        TofArHandManager.OnGestureEstimated += GestureEstimated;
    }

    /// <summary>
    /// �W�F�X�`�����o���ꂽ�Ƃ��ɌĂяo�����B
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="result"></param>
    private void GestureEstimated(object sender, GestureResultProperty result)
    {
        // �W�F�X�`���̎��
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

        // �W�F�X�`�����o�� 1 �b�ԕ\��
        float alpha = Mathf.Max(0, 1.0f - (current - last));
        gestureText.color = new Color(0, 0, 0, alpha);
    }
}
