/***********************************************
	FileName: MoveImage.cs	    
	Creation: 2017-07-21
	Author：East.Su
	Version：V1.0.0
	Desc: 
**********************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuYu
{
    public class MoveImage : MonoBehaviour
    {

        public float MoveSpeedInGame = 0.001f;

        private Transform bg1;
        private Transform bg2;

        int leftBgNum = 1;
        int rightBgNum = 2;
        // Use this for initialization
        void Start()
        {
            bg1 = GetTransform(this.gameObject.transform, "Bg1");
            bg2 = GetTransform(this.gameObject.transform, "Bg2");
        }

        // Update is called once per frame
        void Update()
        {
            if (GetBgByNum(rightBgNum).localPosition.x <= 4f)
            {
                Vector3 leftPos = GetBgByNum(leftBgNum).localPosition;
                leftPos.x = 1920;
                GetBgByNum(leftBgNum).localPosition = leftPos;
                ResetLeftRight();
            }
            else
            {
                Vector3 bg1Pos = bg1.localPosition;
                Vector3 bg2Pos = bg2.localPosition;
                bg1Pos.x -= MoveSpeedInGame;
                bg2Pos.x -= MoveSpeedInGame;
                bg1.localPosition = bg1Pos;
                bg2.localPosition = bg2Pos;
            }
        }
        private Transform GetBgByNum(int num)
        {
            if (num == 1)
                return bg1;
            else return bg2;
        }
        private void ResetLeftRight()
        {
            int oldLeft = leftBgNum;
            leftBgNum = rightBgNum;
            rightBgNum = oldLeft;
        }
        Transform GetTransform(Transform check, string name)
        {
            foreach (Transform t in check.GetComponentsInChildren<Transform>())
            {
                if (t.name == name) { return t; }
            }
            return null;
        }
    }

}
