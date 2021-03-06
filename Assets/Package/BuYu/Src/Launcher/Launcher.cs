﻿using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GF;
using GF.UI;
using Lobby;
using UnityEngine.UI;
namespace BuYu
{


    internal enum LaunchFailedType
    {
        LFT_OK,
        LFT_CD,
        LFT_INVALID,
        LFT_ENERGY,
    };

    internal enum SkillFailedType
    {
        SFT_OK,
        SFT_CD,         
        SFT_INVALID,
        SFT_COUNT,
        SFT_UNLOCK,
        SFT_BIND
    };

    internal struct LockedFishEft
    {
        public GameObject m_Obj;
        public float m_LifeTime;
    }

    public class Launcher
    {
        private byte m_Type;
        private byte m_RateIndx; //倍率索引
        private byte m_Seat;
        private bool m_LaunchValid; //当前炮是否加锁
        private bool m_RateValid; //当前倍率是否有并行
        private bool m_bMyself;
        private float m_Angle;
        private float m_LauncherInterval; //发射频率
        private float m_LauncherTime;
        private bool m_bLauncherXPSkill; //是否正在发大招
        private GameObject m_ObjectHandle;
        private Transform m_TransformHandle;
        private Text m_LabelScore = null; //玩家成绩
        private Text m_LabelDiamond = null; //玩家钻石
        private Image m_UIGoldBg;
        private Transform m_GlodTransform; //金币transform
        private Vector2 m_Direction = new Vector2(); //发射方向
        private Vector2 m_GunPivot = new Vector2(); //炮管中心轴点的位置
        private Vector2 m_LauncherPos = new Vector2(); //炮台位置
        private GameObject[] m_ObjectBtn = new GameObject[2];
        private LauncherData m_LauncherSetting;
        private GunBarrel m_GunBarrel = new GunBarrel();
        private EnergyPool[] m_EnergyPoolLogic = new EnergyPool[ConstValue.MAX_LAUNCHER_NUM];
        private Image m_EnergyPoolUI; //能量槽UI
        private GameObject m_LockUI; //锁
        private Image m_FaceTexture; //头像
        private GameObject m_VipFunctionObj; //切炮和自动开炮UI
        //private TweenScale m_VipFunctionScaleAnim;
        private bool m_bShowVipFunction = false; //是否显示VIP功能区
        private GameObject m_BankruptcyObj; //破产
        private GameObject m_AutoShotCancel;
        private GameObject m_IsLotteryTips;
        private bool m_bFirstUpdatePos = true;
        private float m_PayTipsDelayed = 0; //那个
        private LockedFishEft m_LockedFishEft = new LockedFishEft();
        private Image m_VipLevelIcon;

        private bool m_isPress = false;

        public Launcher(byte launcherType, bool valid, byte seat, byte rateIndx)
        {
            m_Seat = seat;
            m_Type = launcherType;
            m_RateIndx = rateIndx;
            m_LaunchValid = valid;
            m_RateValid = true;
            m_Seat = seat;
            m_LauncherSetting = LauncherSetting.LauncherDataList[launcherType];

            if (SceneRuntime.PlayerMgr.MyClientSeat == m_Seat)
                m_bMyself = true;
            else
                m_bMyself = false;

            m_LauncherInterval = m_LauncherSetting.Interval;
            m_LauncherTime = 0;

            for (byte i = 0; i < ConstValue.MAX_LAUNCHER_NUM; ++i)
            {
                m_EnergyPoolLogic[i] = new EnergyPool();
                m_EnergyPoolLogic[i].InitEnergy(i);
            }
            m_bLauncherXPSkill = false;
        }

        public void Init()
        {
            byte indx = SceneRuntime.LauncherPrefabIndx(m_Seat, m_bMyself);
            m_ObjectHandle = GameObject.Instantiate(SceneRuntime.PlayerMgr.LauncherObject(indx)) as GameObject;
            m_ObjectHandle.SetActive(true);
            m_ObjectHandle.transform.SetParent(SceneBoot.Instance.UIPanelTransform, false);
            m_TransformHandle = m_ObjectHandle.transform;

            m_LabelScore = m_TransformHandle.GetChild(0).gameObject.GetComponent<Text>();
            //m_GlodTransform = m_TransformHandle.GetChild(0).GetChild(0);
            //m_UIGoldBg = m_GlodTransform.GetComponent<Image>();
            CreatGunBarrel(LauncherType);
            if (m_bMyself)
            {
                for (byte i = 0; i < 2; ++i)
                {
                    /*m_ObjectBtn[i] = m_TransformHandle.GetChild(i + 2).gameObject;
                    UIEventListener.Get(m_ObjectBtn[i]).onPress = OnButtonPressMsg;
                    switch (i)
                    {
                        case 0: //加炮台
                            UIEventListener.Get(m_ObjectBtn[i]).onClick = OnAddButtonMessage;
                            break;
                        case 1: //减炮台
                            UIEventListener.Get(m_ObjectBtn[i]).onClick = OnSubtractButtonMessage;
                            break;
                    }*/
                }
                //m_LabelDiamond = m_TransformHandle.GetChild(1).gameObject.GetComponent<Text>();
                //m_EnergyPoolUI = m_TransformHandle.GetChild(4).gameObject.GetComponent<Image>();
                //m_LockUI = m_TransformHandle.GetChild(5).gameObject;
                // SceneRuntime.LauncherEftMgr.PlayDiamondEft(m_TransformHandle.GetChild(1).GetChild(0));
                //  UpdateUserGold(m_Seat);
                /*m_VipFunctionObj = m_TransformHandle.GetChild(6).gameObject;
                UIEventListener.Get(m_TransformHandle.GetChild(8).gameObject).onClick = OnClickLaunch;
                UIEventListener.Get(m_TransformHandle.GetChild(8).gameObject).onPress = OnButtonPressMsg;
                //m_VipFunctionScaleAnim = m_VipFunctionObj.GetComponent<TweenScale>();
                for (byte k = 0; k < 2; ++k)
                {
                    UIEventListener.Get(m_TransformHandle.GetChild(6).GetChild(k + 1).gameObject).onPress =
                        OnButtonPressMsg;
                    if (k == 0)
                        UIEventListener.Get(m_TransformHandle.GetChild(6).GetChild(k + 1).gameObject).onClick =
                            OnClickChnageLaunch;
                    else
                        UIEventListener.Get(m_TransformHandle.GetChild(6).GetChild(k + 1).gameObject).onClick =
                            OnClickAutoLaunch;
                }
                m_BankruptcyObj = m_TransformHandle.GetChild(7).gameObject;
                m_AutoShotCancel = m_TransformHandle.GetChild(9).gameObject;
                UIEventListener.Get(m_AutoShotCancel).onClick = OnClickAutoShotCancel;
                UIEventListener.Get(m_AutoShotCancel).onPress = OnButtonPressMsg;*/

                /*Button btn = m_GunBarrel.HandleObj.GetComponentInChildren<Button>();
                btn.onClick.AddListener(delegate ()
                {
                    OnClickLaunch(btn.gameObject);
                });

                btn = m_TransformHandle.FindChild("CanonOption/BtnChangeCanon").GetComponentInChildren<Button>();
                btn.onClick.AddListener(delegate ()
                {
                    OnClickChnageLaunch(btn.gameObject);
                });

                m_VipFunctionObj = m_TransformHandle.FindChild("CanonOption").gameObject;
                m_VipFunctionObj.SetActive(false);*/
            }
            else
            {
               /* m_FaceTexture = m_TransformHandle.GetChild(1).GetChild(0).GetComponent<Image>();
                UIEventListener.Get(m_TransformHandle.GetChild(1).gameObject).onClick = OnClickAvatarMsg;
                UIEventListener.Get(m_TransformHandle.GetChild(1).gameObject).onPress = OnButtonPressMsg;
                m_LockUI = m_TransformHandle.GetChild(2).gameObject;
                m_BankruptcyObj = m_TransformHandle.GetChild(5).gameObject;
                m_IsLotteryTips = m_TransformHandle.GetChild(6).gameObject;
                m_LockedFishEft.m_Obj = m_TransformHandle.GetChild(7).gameObject;
                m_VipLevelIcon = m_TransformHandle.GetChild(8).GetComponent<UISprite>();*/
                //  UpdateUserGold(m_Seat);
            }

            
            //SceneRuntime.LauncherEftMgr.PlayGlodLightEft(m_GlodTransform);
            //是否显示锁
            /*if (m_RateValid && m_LaunchValid)
                m_LockUI.SetActive(false);*/

            UpdateRootPos();

            /*if (PlayerRole.Instance.RoleInfo.RoleMe.GetMonthID() != 0)
            {
                SetMatchGoldBg();
            }*/
        }

        private void CreatGunBarrel(byte type)
        {
            int Idx = m_Seat < 2 ? (type*2) : (type*2 + 1);
            m_GunBarrel.HandleObj =
                GameObject.Instantiate(SceneRuntime.PlayerMgr.GetGunBarrelObj((byte) Idx)) as GameObject;
            m_GunBarrel.HandleObj.transform.SetParent(m_TransformHandle, false);
            m_GunBarrel.Reset(type, m_Seat);
            m_GunBarrel.BulletConsume = BulletSetting.BulletRate[m_RateIndx];
            m_GunBarrel.Init();
            
            //SceneRuntime.LauncherEftMgr.PlayGaiEffect(m_GunBarrel.GaiTransfom, type);
            //SceneRuntime.LauncherEftMgr.PlayMoveLight(m_GunBarrel.BaseTransform, type, m_Seat);

            Button btn = m_GunBarrel.HandleObj.GetComponentInChildren<Button>();
            btn.onClick.AddListener(delegate ()
            {
                OnClickLaunch(btn.gameObject);
            });

            btn = m_TransformHandle.FindChild("CanonOption/BtnChangeCanon").GetComponentInChildren<Button>();
            btn.onClick.AddListener(delegate ()
            {
                OnClickChnageLaunch(btn.gameObject);
            });

            m_VipFunctionObj = m_TransformHandle.FindChild("CanonOption").gameObject;
            m_VipFunctionObj.SetActive(false);

            m_LabelScore = m_TransformHandle.FindChild("UserScore/text").GetComponent<Text>();
            m_LabelDiamond = m_TransformHandle.FindChild("Diamod/text").GetComponent<Text>();
            m_BankruptcyObj = m_TransformHandle.FindChild("Bankruptcy").gameObject;
            m_BankruptcyObj.transform.SetSiblingIndex(20);
        }

        public void Update(float delta)
        {

            m_GunBarrel.Update(delta);
            if (m_LockedFishEft.m_LifeTime > 0)
            {
                m_LockedFishEft.m_LifeTime -= delta;
                if (m_LockedFishEft.m_LifeTime <= 0)
                    m_LockedFishEft.m_Obj.SetActive(false);
            }
            //处理能量槽回滚
            if (m_EnergyPoolLogic[LauncherType].Rollback)
            {
                m_EnergyPoolLogic[LauncherType].PlayRollbackPoolUI(delta);
                m_EnergyPoolUI.fillAmount = m_EnergyPoolLogic[LauncherType].FillAmount;
            }
            //处于大招CD状态
            if (m_EnergyPoolLogic[LauncherType].LaserCDState)
            {
                m_EnergyPoolLogic[LauncherType].PlayCD(delta);
                return;
            }
            //大招的硬直状态
            if (m_EnergyPoolLogic[LauncherType].HitRecoverState)
            {
                m_EnergyPoolLogic[LauncherType].Update(delta);
                return;
            }

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            UpdateRootPos(); //更新炮台最新位置
#else
        if (m_bFirstUpdatePos)
        {
            UpdateRootPos();        //更新炮台最新位置
            m_bFirstUpdatePos = false;
        }
#endif
            ushort lockfishID = SceneRuntime.PlayerMgr.LockedFishID;
            if (lockfishID != 0)
            {
                Fish fish = SceneRuntime.FishMgr.FindFishByID(lockfishID);
                if (fish != null)
                {
                    UpdateLaunchByLockFish(fish.ScreenPos);
                }
            }
            else
            {
                //UpdateLaunchAngle(); //更新炮台角度
            }
            //m_GunBarrel.Update(delta);
            CheckIsBankruptcy();

            m_LauncherTime += delta;
            if (CheckXPSkill())
            {
                //向服务器发送大招请求
                short angle = Utility.FloatToShort(m_Angle);
                angle = SceneRuntime.AngleInversion(angle);
                /*if (SceneRuntime.SceneModel.UseLaser(angle))
                {
                    m_bLauncherXPSkill = true;
                    m_EnergyPoolLogic[LauncherType].bSendXPSkill = false;
                }*/
            }
            if (CheckLaunch())
            {
                if (m_LauncherTime >= m_LauncherInterval)
                {
                    m_LauncherTime = 0;
                    //检测是否钱够
                   
                    /*if (PlayerRole.Instance.GetPlayerGlobelBySeat(m_Seat) <
                        (BulletSetting.BulletRate[m_RateIndx]*m_LauncherSetting.Consume))
                    {
                        if (GetMaxRate())
                            return;
                        SceneRuntime.PlayerMgr.StopAutoShotAndLocked();
                        if (m_AutoShotCancel.activeSelf)
                            m_AutoShotCancel.SetActive(false);
                        // GlobalHallUIMgr.Instance.ShowSystemTipsUI(StringTable.GetString("GoldNotEnough"), 1, false);
                        if (PlayerRole.Instance.RoleInfo.RoleMe.GetMonthID() == 0)
                        {
                            UIManager.Instance.ShowMessage("金币不足", MessageBoxEnum.Style.Ok, null);
                            /*GlobalEffectMgr.Instance.ShutDownMsgBox();
                            GlobalHallUIMgr.Instance.ShowPayWnd(PayType.Gold);#1#
                        }
                        else
                            UIManager.Instance.ShowMessage("金币不足", MessageBoxEnum.Style.Ok, null);
                        /*GlobalHallUIMgr.Instance.ShowMatchMsgBox("",
                            PlayerRole.Instance.RoleInfo.RoleMe.GetMonthID(), MatchMsgBoxType.Match_BuyGold);#1#
                        return;
                    }*/
                    UpdateLaunchAngle();
                    short angle = Utility.FloatToShort(m_Angle);
                    angle = SceneRuntime.AngleInversion(angle);
                    SceneRuntime.SceneModel.LaunchBullet(angle);
                }
            }
        }

        //根据鱼的屏幕坐标更新炮台位置。
        public void UpdateLaunchByLockFish(Vector3 screePos)
        {
            bool vaild = m_LaunchValid & m_RateValid;
            if (!m_bMyself || !vaild || m_bLauncherXPSkill)
                return;
            Vector2 UpDir = new Vector2(0, m_Seat > 1 ? -1 : 1);
            Vector2 pos = new Vector2(screePos.x, screePos.y);
            m_Direction = (pos - m_GunPivot);
            if (m_Seat <= 1 && m_Direction.y < 0.1f)
                m_Direction.y = 0.1f;
            else if (m_Seat > 1 && m_Direction.y > -0.1f)
                m_Direction.y = 0.1f;
            if (m_Direction.x < 0.1f && m_Direction.x > 0.0f)
                m_Direction.x = 0.1f;
            else if (m_Direction.x > -0.1f && m_Direction.x < 0.0f)
                m_Direction.x = -0.1f;
            m_Direction.Normalize();
            float dot = Vector2.Dot(UpDir, m_Direction);
            m_Angle = Mathf.Acos(Mathf.Clamp(dot, 0.0f, 1.0f))*Mathf.Rad2Deg;
            //第一三象和第二四象角度相反
            if (m_Direction.x >= 0)
                m_Angle = -m_Angle;
            m_Angle = Mathf.Clamp(m_Angle, -85, 85);
            m_GunBarrel.UpdateAngle(m_Angle);
        }

        private void UpdateLaunchAngle()
        {
            bool vaild = m_LaunchValid & m_RateValid;

            if (!m_bMyself || !vaild || m_bLauncherXPSkill)
                return;
            if (m_BankruptcyObj && m_BankruptcyObj.activeSelf)
                return;
            Vector3 position;
            //if (Input.touchCount > 0)
            //{
            //    position = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, ConstValue.NEAR_Z);
            //}
            //else
            if (Input.GetMouseButton(0) && SceneRuntime.HandleClickEvent == false)
            {
                position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, ConstValue.NEAR_Z);
            }
            else
            {
#if (UNITY_ANDROID || UNITY_IOS)
           return;
#endif
                position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, ConstValue.NEAR_Z);
            }
            //position = new Vector3(1920/2,1080/2);
            Vector2 UpDir = new Vector2(0, m_Seat > 1 ? -1 : 1);
            Vector2 pos = new Vector2(position.x, position.y);
            m_Direction = (pos - m_GunPivot);
            if (m_Seat <= 1 && m_Direction.y < 0.1f)
                m_Direction.y = 0.1f;
            else if (m_Seat > 1 && m_Direction.y > -0.1f)
                m_Direction.y = 0.1f;
            if (m_Direction.x < 0.1f && m_Direction.x > 0.0f)
                m_Direction.x = 0.1f;
            else if (m_Direction.x > -0.1f && m_Direction.x < 0.0f)
                m_Direction.x = -0.1f;
            m_Direction.Normalize();
            float dot = Vector2.Dot(UpDir, m_Direction);
            m_Angle = Mathf.Acos(Mathf.Clamp(dot, 0.0f, 1.0f))*Mathf.Rad2Deg;
            //第一三象和第二四象角度相反
            if (m_Direction.x >= 0)
                m_Angle = -m_Angle;
            m_Angle = Mathf.Clamp(m_Angle, -85, 85);
            m_GunBarrel.UpdateAngle(m_Angle);

        }

        public void UpdateEnergy(int energy)
        {
            m_EnergyPoolLogic[m_Type].CurEnergy = energy; // BulletSetting.BulletRate[m_RateIndx];
            m_EnergyPoolLogic[m_Type].UpdateEnergyPool(m_RateIndx);
            //m_EnergyPoolUI.fillAmount = m_EnergyPoolLogic[LauncherType].FillAmount;

            //Debug.Log(m_EnergyPoolLogic[m_Type].CurEnergy + ", " + m_EnergyPoolLogic[m_Type].MaxEnergy);
        }

        //非自己炮台角度更新
        public void UpdatOtherAngle()
        {
            Vector2 UpDir = new Vector2(0, m_Seat > 1 ? -1 : 1);
            float dot = 0.0f;
            dot = Vector2.Dot(UpDir, m_Direction);
            m_Angle = Mathf.Acos(Mathf.Clamp(dot, 0.0f, 1.0f))*Mathf.Rad2Deg;
            if (m_Direction.x >= 0)
                m_Angle = -m_Angle;
            m_Angle = Mathf.Clamp(m_Angle, -85, 85);
            if (m_bMyself)
                return;
            m_GunBarrel.UpdateAngle(m_Seat > 1 ? -m_Angle : m_Angle);
        }

        public void SetMatchGoldBg()
        {
            m_UIGoldBg.name = "Match_Gold";
            //m_UIGoldBg.spriteName = "Match_Gold";
        }

        public void UpdateUserGold(byte clientSeat)
        {
            if (m_bMyself)
            {
                m_LabelDiamond.text = PlayerRole.Instance.RoleInfo.RoleMe.GetCurrency().ToString();
            }
            else
            {
                if (SceneRuntime.PlayerMgr.GetPlayer(clientSeat) == null)
                    return;
            }
            m_LabelScore.text = PlayerRole.Instance.GetPlayerGlobelBySeat(clientSeat).ToString();
        }

        public void UpdateVipLevelData(uint UserID)
        {
            //if (PlayerRole.Instance.TableManager.GetTableRole(UserID).GetVipLevel() > 0)
            //{
            //    m_VipLevelIcon.gameObject.SetActive(true);
            //    m_VipLevelIcon.spriteName = string.Format("Vip_Icon_0{0}", PlayerRole.Instance.TableManager.GetTableRole(UserID).GetVipLevel());
            //}

        }

        private void CheckIsBankruptcy()
        {
            byte minRate =
                FishConfig.Instance.m_TableInfo.m_TableConfig[PlayerRole.Instance.RoleInfo.RoleMe.GetTableTypeID()]
                    .MinRate;
            if (PlayerRole.Instance.GetPlayerGlobelBySeat(m_Seat) <
                BulletSetting.BulletRate[minRate]*m_LauncherSetting.Consume
                && SceneRuntime.SceneModel.BulletMgr.HaveBullet(m_Seat) == false)
                m_BankruptcyObj.SetActive(true);
            else
            {
                if (m_BankruptcyObj.activeSelf)
                {
                    //TODO 
                    //SceneRuntime.LogicUI.RevertLockedFish();
                }
                m_BankruptcyObj.SetActive(false);
            }
        }

        private void UpdateRootPos()
        {
            Vector3 pos;
            Vector3 scrPos = new Vector3(Screen.width, Screen.height, 0);
            Vector3 worldPos = scrPos; //SceneBoot.Instance.UICamera.ScreenToWorldPoint(scrPos);
            float x = Screen.width/4;
            float x1 = Screen.width/3;

            switch (m_Seat)
            {
                case 0:
                    //pos = new Vector3(x, Camera.main.rect.yMin*Screen.height, 0);
                    pos = new Vector3(x, 0, 0);
                    break;
                case 1:
                    pos = new Vector3(x1*2, Camera.main.rect.yMin*Screen.height, 0);
                    break;
                case 2:
                    pos = new Vector3(x1*2, Camera.main.rect.yMax*Screen.height, 0);
                    break;
                case 3:
                    pos = new Vector3(x, Camera.main.rect.yMax*Screen.height, 0);
                    break;
                default:
                    pos = Vector3.zero;
                    break;
            }

            m_TransformHandle.position = pos;//SceneObjMgr.Instance.UICamera.ScreenToWorldPoint(pos);
                // new Vector3(-1, 0, 0);
            LauncherPos = new Vector2(m_TransformHandle.position.x, m_TransformHandle.position.y);
            //Transform goldIcon = m_TransformHandle.FindChild("UserScore/Image");
            Transform goldIcon = m_TransformHandle.FindChild("UserScore/Image");
            SceneRuntime.InitGoldPosMapping(m_Seat, goldIcon.position   );
            // m_GlodPos = m_GlodTransform.position;
            //m_GunPivot = Camera.main.ScreenToWorldPoint( m_GunBarrel.GunPivot);
            m_GunPivot = m_TransformHandle.position;//m_GunBarrel.GunPivot;
        }

        private bool CheckLaunch()
        {
            bool bauto = SceneRuntime.PlayerMgr.AutoShotOrLocked | IsPress;
            bool btop = !WndManager.Instance.HasTopWnd | SceneRuntime.PlayerMgr.AutoShotOrLocked;
            bool Vaild = m_RateValid & m_LaunchValid;
            if (bauto && SceneRuntime.HandleClickEvent == false && btop)
            {
                //场景按纽界面特殊处理
                /*if (m_bShowVipFunction && !SceneRuntime.PlayerMgr.AutoShotOrLocked)
                {
                    SceneRuntime.SceneModel.BtnsMgr.BaseBtnHide();
                    OnClickLaunch(null);
                }
                if (m_bLauncherXPSkill || !m_bMyself) // 炮为锁状态
                    return false;
                if (!Vaild && PlayerRole.Instance.RoleLauncher.IsCanUseLauncher(LauncherType) == false)
                {
                    // GlobalHallUIMgr.Instance.ShowVipWnd();
                    SceneRuntime.SceneModel.ChangeDestLauncher(0);
                    return false;
                }
                else if (!Vaild && PlayerRole.Instance.RoleLauncher.IsCanUseLauncher(LauncherType))
                {
                    return false;
                }*/
                //解除自动射击
                //if (SceneRuntime.PlayerMgr.AutoShot && Input.GetMouseButton(0))
                //    SceneRuntime.PlayerMgr.SetAutoShot(false);
                return true;
            }
            return false;
        }

        private bool CheckXPSkill()
        {
            /*if (!m_bMyself)
                return false;
            if (m_bLauncherXPSkill || SceneRuntime.HandleClickEvent || WndManager.Instance.HasTopWnd)
                return false;
            bool bauto = SceneRuntime.PlayerMgr.AutoShotOrLocked | Input.GetMouseButton(0);
            bool Vaild = m_LaunchValid & m_RateValid;
            if (bauto && m_EnergyPoolLogic[LauncherType].bSendXPSkill && Vaild)
            {
                return true;
            }*/
            return false;
        }

        //玩家钱不够主动帮玩家切最合适的倍率
        private bool GetMaxRate()
        {

           /* byte minRate =
                FishConfig.Instance.m_TableInfo.m_TableConfig[PlayerRole.Instance.RoleInfo.RoleMe.GetTableTypeID()]
                    .MinRate;
            if (m_RateIndx == minRate)
                return false;

            for (int i = m_RateIndx - 1; i >= minRate; --i)
            {
                if (PlayerRole.Instance.GetPlayerGlobelBySeat(m_Seat) <
                    (BulletSetting.BulletRate[i]*m_LauncherSetting.Consume))
                    continue;
                SceneRuntime.SceneModel.ChangeDestRate((byte) i);
                return true;
            }*/
            return false;
        }

        private void OnSubtractButtonMessage(GameObject button)
        {
            //游戏中有窗口打开游戏暂时处理假暂停状态
            /*if (m_EnergyPoolLogic[LauncherType].LaserCDState || m_bLauncherXPSkill)
                return;

            GlobalAudioMgr.Instance.PlayOrdianryMusic(Audio.OrdianryMusic.m_BtnMusic);

            SceneRuntime.SceneModel.ChangeRate(false);*/

            // SceneRuntime.SceneModel.ChangeLauncher(1);
        }

        private void OnAddButtonMessage(GameObject button)
        {
            //游戏中有窗口打开游戏暂时处理假暂停状态

            /*if (m_EnergyPoolLogic[LauncherType].LaserCDState || m_bLauncherXPSkill)
                return;
            GlobalAudioMgr.Instance.PlayOrdianryMusic(Audio.OrdianryMusic.m_BtnMusic);

            SceneRuntime.SceneModel.ChangeRate(true);*/
            // SceneRuntime.SceneModel.ChangeLauncher(0);
        }

        private void OnClickChnageLaunch(GameObject go)
        {
            UIManager.Instance.ShowView<ChangeCanonView>();
            //SceneRuntime.LogicUI.ShowChangeLauncherWnd();
        }

        private void OnClickAutoLaunch(GameObject go)
        {
            /*if (PlayerRole.Instance.RoleMonthCard.IsCanAutoFire())
            {
                SceneRuntime.PlayerMgr.SetAutoShot(true);
                m_VipFunctionObj.SetActive(false);
                m_bShowVipFunction = false;
                m_AutoShotCancel.SetActive(true);
            }
            else
            {
                GlobalHallUIMgr.Instance.ShowMonthCardWnd();
            }*/
        }

        private void OnRatioButtonMessage(GameObject button)
        {
            //游戏中有窗口打开游戏暂时处理假暂停状态

            /*if (m_EnergyPoolLogic[LauncherType].LaserCDState || m_bLauncherXPSkill)
                return;
            GlobalAudioMgr.Instance.PlayOrdianryMusic(Audio.OrdianryMusic.m_BtnMusic);*/

            //SceneRuntime.SceneModel.ChangeRate(m_RateIndx);
        }

        public void ChangeLauncher(byte type, bool valid)
        {
            m_Type = type;

            m_GunBarrel.ShutDown();
            //SceneRuntime.LauncherEftMgr.PlayChangeLauncherEft(LauncherPos);
            //SceneRuntime.LauncherEftMgr.RemoveAtEffect();
            m_LauncherSetting = LauncherSetting.LauncherDataList[type];
            CreatGunBarrel(m_Type);
            m_GunBarrel.UpdateAngle(0);
            m_Angle = 0;
            m_LaunchValid = valid;
            if (m_RateIndx != PlayerRole.Instance.RoleInfo.GetRoleUnLockRateIndex())
            {
                bool Valid = m_RateValid & m_LaunchValid;
                //是否显示锁
                /*if (m_LockUI.activeSelf == valid)
                    m_LockUI.SetActive(!valid);*/
            }
            //切换能量槽
            if (m_bMyself)
            {
                ClearXPSkillEft();
                /*m_EnergyPoolLogic[LauncherType].UpdateEnergyPool(m_RateIndx);
                m_EnergyPoolUI.fillAmount = m_EnergyPoolLogic[LauncherType].FillAmount;
                if (m_EnergyPoolLogic[LauncherType].Full)
                    PlayXPSkillEft();*/
            }
        }

        public void UpdateLauncherChange()
        {
            /*if (PlayerRole.Instance.RoleLauncher.IsCanUseLauncher(LauncherType))
            {
                if (m_RateIndx != PlayerRole.Instance.RoleInfo.GetRoleUnLockRateIndex())
                {
                    m_LaunchValid = true;
                    bool Valid = m_RateValid & m_LaunchValid;
                    if (Valid)
                        m_LockUI.SetActive(false);
                }
            }
            else
            {
                m_LaunchValid = false;
                bool Valid = m_RateValid & m_LaunchValid;
                if (!Valid)
                    m_LockUI.SetActive(true);
                m_GunBarrel.UpdateAngle(0);
            }*/

        }

        public void ChangeRate(byte rateIndx, bool bvaild)
        {
            /*if (m_EnergyPoolLogic[LauncherType] == null)
                return;
            m_RateIndx = rateIndx;

            //ShowLauncherRateInfo();
            SceneRuntime.LauncherEftMgr.PlayChangeLauncherEft(LauncherPos);
            m_GunBarrel.BulletConsume = BulletSetting.BulletRate[m_RateIndx];
            m_GunBarrel.ShowBulletConsume();
            if (bvaild && PlayerRole.Instance.RoleLauncher.IsCanUseLauncher(LauncherType))
            {
                m_RateValid = bvaild;
                bool Valid = m_RateValid & m_LaunchValid;
                if (Valid)
                    m_LockUI.SetActive(false);
            }
            else
            {
                m_RateValid = bvaild;
                bool Valid = m_RateValid & m_LaunchValid;
                if (!Valid)
                    m_LockUI.SetActive(true);
            }
            m_GunBarrel.UpdateAngle(0);
            m_Angle = 0;

            //切换能量槽
            if (m_bMyself)
            {
                ClearXPSkillEft();
                m_EnergyPoolLogic[LauncherType].UpdateEnergyPool(m_RateIndx);
                m_EnergyPoolUI.fillAmount = m_EnergyPoolLogic[LauncherType].FillAmount;
                if (m_EnergyPoolLogic[LauncherType].Full)
                    PlayXPSkillEft();
            }*/
        }

        public void LauncherBullet(uint Energy)
        {
            if (m_EnergyPoolLogic[LauncherType] == null)
                return;
            m_GunBarrel.PlayAnimation();
            SceneRuntime.LauncherEftMgr.PlayGunBarrelFire(m_GunBarrel.BaseTransform, LauncherType);
            m_LauncherTime = 0;

            //蓄能
            if (m_bMyself)
            {
                if (!m_EnergyPoolLogic[LauncherType].Full)
                {
                    m_EnergyPoolLogic[LauncherType].UpdateEnergyPool(LauncherType, m_RateIndx, Energy);
                    // m_EnergyPoolLogic[LauncherType].UpdateEnergyPool(LauncherType, m_RateIndx);
                    /*m_EnergyPoolUI.fillAmount = m_EnergyPoolLogic[LauncherType].FillAmount;
                    if (m_EnergyPoolLogic[LauncherType].Full)
                        PlayXPSkillEft();*/
                }
            }

        }

        public void PlayCD(float CDTime)
        {
            // if (m_bMyself)
            if (m_EnergyPoolLogic[LauncherType].Full)
            {
                m_EnergyPoolLogic[LauncherType].LaserCDState = true;
                m_EnergyPoolLogic[LauncherType].LaserCDTime = CDTime;

            }

        }

        public void LaunchLaser(byte clientSeat, short degree)
        {

            if (m_bMyself)
            {
                m_bLauncherXPSkill = false;
                ClearXPSkillEft();
                m_EnergyPoolLogic[LauncherType].LaserCDState = true;
                m_EnergyPoolLogic[LauncherType].Rollback = true;
                m_EnergyPoolLogic[LauncherType].PlayRollbackPoolUI(1000);
                m_EnergyPoolUI.fillAmount = m_EnergyPoolLogic[LauncherType].FillAmount;
            }

            Vector3 startPos;
            Vector3 dir;
            degree = SceneRuntime.AngleInversion(degree);
            SceneRuntime.GetBulletPosAndDir(clientSeat, degree, out dir, out startPos);
            if (!m_bMyself)
            {
                Vector2 direction = new Vector2(dir.x, dir.y);
                Direction = direction;
                UpdatOtherAngle();
            }
            SceneRuntime.LauncherEftMgr.PlayXPSkillMuzzleEft(m_GunBarrel.BaseTransform, LauncherType, startPos,
                m_LauncherSetting.LaserCDTime);
            SceneRuntime.LauncherEftMgr.PlayLaserEft(m_GunBarrel.BaseTransform, LauncherType, clientSeat, Direction,
                startPos, m_Angle);
            // SceneRuntime.LauncherEftMgr.RemoveAtEffect();
        }

        public void LaunchLaserFailed(NetCmdLaunchFailed lf)
        {
            if (m_bLauncherXPSkill)
            {
                if (lf.FailedType == (byte) LaunchFailedType.LFT_CD)
                {
                    m_bLauncherXPSkill = false;
                    m_EnergyPoolLogic[LauncherType].bSendXPSkill = true;
                }
                else
                {
                    m_bLauncherXPSkill = false;
                    m_EnergyPoolLogic[LauncherType].bSendXPSkill = false;
                    m_EnergyPoolLogic[LauncherType].CurEnergy = lf.Energy;
                }
            }
        }

        private void PlayXPSkillEft()
        {
            //   byte [] type = {20,20,21,22,22};
            /*if (m_bMyself)
                GlobalAudioMgr.Instance.PlayOrdianryMusic( /*(Audio.OrdianryMusic)type[(int)LauncherType]#1#
                    Audio.OrdianryMusic.m_FullEnergy, true);
            else
                GlobalAudioMgr.Instance.PlayOrdianryMusic( /*(Audio.OrdianryMusic)type[(int)LauncherType]#1#
                    Audio.OrdianryMusic.m_FullEnergy, true, false, 0.3f);
            SceneRuntime.LauncherEftMgr.PlayXPSKillActiveEft(SceneObjMgr.Instance.UIPanelTransform, LauncherPos);
            SceneRuntime.LauncherEftMgr.PlayXPSkillLiveEft(m_GunBarrel.BaseTransform, LauncherType, LauncherPos);*/
        }

        private void ClearXPSkillEft()
        {
            SceneRuntime.LauncherEftMgr.StopXPSkillActiveEft();
            SceneRuntime.LauncherEftMgr.StopXPSKillLiveEft();
        }

        private void OnClickAvatarMsg(GameObject go)
        {

            /*GlobalHallUIMgr.Instance.ShowNameCardsUI(NameCardsUIType.NameCards_UI, CardsParentUIType.Scene_Head,
                SceneRuntime.PlayerMgr.GetPlayer(m_Seat).Player.playerData.ID, (Texture2D) m_FaceTexture.mainTexture);*/
        }

        private void OnClickLaunch(GameObject go)
        {
            
            /*if (m_BankruptcyObj.activeSelf)
                return;
            if (m_AutoShotCancel.activeSelf)
                return;*/
            //显示
            if (m_bShowVipFunction)
                m_VipFunctionObj.SetActive(false);
            else
                m_VipFunctionObj.SetActive(true);

            /*m_VipFunctionScaleAnim.ResetToBeginning();
            m_VipFunctionScaleAnim.PlayForward();*/
            m_bShowVipFunction = !m_bShowVipFunction;
        }

        public void OnClickAutoShotCancel(GameObject go)
        {
            SceneRuntime.PlayerMgr.SetAutoShot(false);
            //m_AutoShotCancel.SetActive(false);
        }

        public void SetPlayerImg(Texture2D img, uint imgCrc)
        {
            /*if (img == null)
            {
                if (imgCrc < ConstValue.CUSTOM_HEADER)
                    m_FaceTexture.mainTexture = GlobalHallUIMgr.Instance.m_HeadTextureUI[imgCrc];
                else
                    m_FaceTexture.mainTexture = GlobalHallUIMgr.Instance.m_HeadTextureUI[5];
            }
            else
                m_FaceTexture.mainTexture = img;*/
        }

        //玩家是否破产
        public bool IsBankruptcy()
        {
            //TODO
            return m_BankruptcyObj.activeSelf;
        }

        public void PlaySkillAvatarEft()
        {
            if (m_bMyself)
                return;

            SceneRuntime.LauncherEftMgr.PlayAvatarEft(m_FaceTexture.transform);
        }

        public void ShowLotteryState(bool bShow)
        {
            m_IsLotteryTips.SetActive(bShow);
            if (bShow && m_LockedFishEft.m_LifeTime > 0)
            {
                m_LockedFishEft.m_Obj.SetActive(false);
                return;
            }
            if (!bShow && m_LockedFishEft.m_LifeTime > 0)
                m_LockedFishEft.m_Obj.SetActive(true);
        }

        public void ShowOtherUserLocked()
        {
            m_LockedFishEft.m_Obj.SetActive(true);
            m_LockedFishEft.m_LifeTime = SkillSetting.SkillDataList[(byte) SkillType.SKILL_LOCK].CDTime;
        }

        private void OnButtonPressMsg(GameObject button, bool state)
        {
            if (state)
            {

            }
            else
            {
                //SceneRuntime.SceneModel.BtnsMgr.BaseBtnHide();
            }
        }

        public void Shutdown()
        {
            m_GunBarrel.ShutDown();
            if (m_ObjectHandle != null)
            {
                if (m_bMyself)
                {
                    SceneRuntime.LauncherEftMgr.StopXPSkillActiveEft();
                    SceneRuntime.LauncherEftMgr.StopXPSKillLiveEft();
                }

                GameObject.Destroy(m_ObjectHandle);
                m_ObjectHandle = null;
            }

        }

        public bool Myself
        {
            get { return m_bMyself; }
            set { m_bMyself = value; }
        }

        public byte LauncherType
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        public Vector2 Direction
        {
            get { return m_Direction; }
            set { m_Direction = value; }
        }

        public Vector2 LauncherPos
        {
            get { return m_LauncherPos; }
            set { m_LauncherPos = value; }
        }

        public LauncherData Setting
        {
            get { return m_LauncherSetting; }
            set { m_LauncherSetting = value; }
        }

        public bool IsPress
        {
            get{ return m_isPress;}
            set { m_isPress = value; }
        }
    }
}

