using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LuckRoll : MonoBehaviour {

    //����ת��
    private Transform mLuckBackGround;

    private Button btnLuck;
    //��ʼ��ת�ٶ�
    private float mInitSpeed;
    //�ٶȱ仯ֵ
    private float mDelta = 3f;

    //ת���Ƿ���ͣ
    private bool isPause = true;

    //��ʼ��ת�ٶ�
    private float mReward;
    void Start()
    {
        mLuckBackGround = GameObject.Find("BackGround/ZhuanPan").transform;
        btnLuck = GameObject.Find("BtnLuck").GetComponent<Button>();
        btnLuck.onClick.AddListener(delegate () {
            OnClick();
        });
        
    }

    //��ʼ�齱
    public void OnClick()
    {
        if (isPause)
        {
            mInitSpeed = 1000;
            //�������һ����ʼ�ٶ�
            mReward = Random.Range(100, 500);
            //��ʼ��ת
            isPause = false;
        }
    }

    void Update()
    {
        if (!isPause)
        {

            //ת��ת��(-1Ϊ˳ʱ��,1Ϊ��ʱ��)
            mLuckBackGround.Rotate(new Vector3(0, 0, -1) * mInitSpeed * Time.deltaTime);
            //��ת�����ٶȻ�������
            mInitSpeed -= mDelta;
            //��ת�����ٶ�Ϊ0ʱת��ֹͣת��
            if (mInitSpeed <= 0)
            {
                //ת��ֹͣ
                isPause = true;
            }
        }
    }
}
