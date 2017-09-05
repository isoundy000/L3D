/***********************************************
	FileName: ShopItem.cs	    
	Creation: 2017-09-05
	Author：East.Su
	Version：V1.0.0
	Desc: 
**********************************************/

using System.Collections;
using System.Collections.Generic;
using GF;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public enum PayType
    {
        Diamond = 0,
        Gold,
        ITEM,
    }


    public class ShopItem
    {
        public GameObject m_BaseWndObject;
        public Transform m_BaseTrans;

        Button btnBuy;          //购买按钮
        Text m_DesItemSum;      //购买数量
        Text m_ItemCurPrice;         //货物现价
        Text m_ItemOldPrice;         //货物原价
        Text m_ItemName;             //货物名称
        Image m_ItemIcon;           //目标货物图
        Image m_ItemPriceIcon;     //需要消耗货币图
        Image m_DesItemIcon;       //目标货币数量图
        Image m_DisCountIcon;     //折扣图标
        Image m_OverLine;         //下划线
        PayType m_Paytype;
        uint m_ItemNum;
        uint m_ItemID;

        public void Init(GameObject go)
        {
            m_BaseWndObject = GameObject.Instantiate(go) as GameObject;
            m_BaseTrans = m_BaseWndObject.transform;
            if (m_BaseWndObject.activeSelf != true)
                m_BaseWndObject.SetActive(true);

            btnBuy = m_BaseTrans.GetComponent<Button>();
            m_ItemIcon = m_BaseTrans.Find("BigIcon").GetComponent<Image>();
            m_DesItemIcon = m_BaseTrans.Find("Num").GetComponent<Image>();
            m_DesItemSum = m_DesItemIcon.transform.Find("Value").GetComponent<Text>();
            m_DisCountIcon = m_BaseTrans.transform.Find("Discount").GetComponent<Image>();
            m_ItemOldPrice = m_BaseTrans.transform.Find("Price/OldPrice").GetComponent<Text>();
            m_OverLine = m_ItemOldPrice.transform.Find("Line").GetComponent<Image>();
            m_ItemCurPrice = m_BaseTrans.transform.Find("Price/CurParice").GetComponent<Text>();
            m_ItemName = m_BaseTrans.transform.Find("Title").GetComponent<Text>();
            
            btnBuy.onClick.AddListener(OnClickBuy);
        }

        public void ShowGoodsInfo(uint ItemID, tagFishRechargeInfo payInfo, PayType type)
        {
            m_Paytype = type;
            m_ItemID = ItemID;
            m_ItemIcon.sprite = ResManager.Instance.LoadSprite("BuYu/Texture/GoodsIcon/" + payInfo.Icon);
            m_ItemName.text = payInfo.Name;
            if (type == PayType.Diamond)
            {
                
            }
            else if (type == PayType.Gold)
            {
                
            }
            m_DesItemSum.text = payInfo.AddMoney.ToString();
            m_DisCountIcon.sprite = ResManager.Instance.LoadSprite("Lobby/Achieve/" + payInfo.sDisCountPicName);
            
            //不打折扣
            if (payInfo.dDisCountPrice == payInfo.dPreDisCountPrice)
            {
                m_ItemOldPrice.text = payInfo.dDisCountPrice.ToString();
                m_ItemCurPrice.text = payInfo.dDisCountPrice.ToString();
                m_ItemCurPrice.gameObject.SetActive(false);
                m_OverLine.gameObject.SetActive(false);
            }
            else
            {
                m_ItemOldPrice.text = payInfo.dPreDisCountPrice.ToString();
                m_ItemCurPrice.text = payInfo.dDisCountPrice.ToString();
                m_OverLine.gameObject.SetActive(true);

            }

        }

        public void ResetLocalScale()
        {
            m_BaseTrans.localScale = Vector3.one;
        }
        void OnClickBuy()
        {
           Debug.Log("OnClickBuy");

        }

    }


}

