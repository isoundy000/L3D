/***********************************************
	FileName: LoginModel.cs	    
	Creation: 2017-07-21
	Author：East.Su
	Version：V1.0.0
	Desc: 
**********************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GF;
using GF.UI;
using GF.NET;
namespace Lobby
{
    public class LoginModel : AppModel
    {
        //登录状态
        LogonState m_State;
        UInt32 _mUserId;
        UInt32 _mOnlyId;

        public LoginModel()
        {
            Init();
        }

        public void Init()
        {
            SetState(LogonState.LOGON_INIT);
            RegisterEvent();
        }

        public void RegisterEvent()
        {
            NetManager.Instance.AddNetEventListener(NetCmdType.CMD_LC_AccountOnlyID, OnAccountOnlyId);
        }

        public void StartConnect()
        {
            
        }
        public void RegisterLogon(AccountInfo rd)
        {
            if (m_State != LogonState.LOGON_INIT)
            {
                return;
            }
            NormalAccountInfo ad = new NormalAccountInfo();
            ad.UID = rd.UID;            
            CL_Cmd_AccountRsg ncb = new CL_Cmd_AccountRsg();
            ncb.SetCmdType(NetCmdType.CMD_CL_AccountRsg);
            ncb.AccountName = rd.UID;
            if (!NativeInterface.ComputeCrc(ad.UID, rd.PWD, out ad.CRC1, out ad.CRC2, out ad.CRC3))
            {
                ncb.PasswordCrc1 = ad.CRC1;
                ncb.PasswordCrc2 = ad.CRC2;
                ncb.PasswordCrc3 = ad.CRC3;
            }            
            ncb.MacAddress = UIDevice.GetMacAddress();
            ncb.VersionID = ServerSetting.ClientVer;
            ncb.PlateFormID = (Byte)UIDevice.GetPlatformString();
            ncb.PathCrc = ServerSetting.RES_VERSION;

            //发送命令
            NetManager.Instance.Send<CL_Cmd_AccountRsg>(ncb);
        }


        public void Logon(AccountInfo rd)
        {            
            NormalAccountInfo ad = new NormalAccountInfo();
            ad.UID = rd.UID;
            CL_Cmd_AccountLogon ncb = new CL_Cmd_AccountLogon();
            ncb.SetCmdType(NetCmdType.CMD_CL_AccountLogon);
            ncb.AccountName = rd.UID;
            if (NativeInterface.ComputeCrc(ad.UID, rd.PWD, out ad.CRC1, out ad.CRC2, out ad.CRC3))
            {
                ncb.PasswordCrc1 = ad.CRC1;
                ncb.PasswordCrc2 = ad.CRC2;
                ncb.PasswordCrc3 = ad.CRC3;
            }

            ncb.VersionID = ServerSetting.ClientVer;
            ncb.PlateFormID = (Byte)UIDevice.GetPlatformString();
            ncb.PathCrc = ServerSetting.RES_VERSION;
            ncb.MacAddress = UIDevice.GetMacAddress();
            //发送命令
            NetManager.Instance.Send<CL_Cmd_AccountLogon>(ncb);
            return;
        }


        System.Collections.IEnumerator LogOnInitProcedure(object obj)
        {
            yield return new WaitForEndOfFrame();
            /*m_State = ServerSetting.Init();
            if (!m_State)
            {
                LogMgr.Log("资源加载失败:ServerSetting");
            }*/
        }

        public LogonState State
        {
            get { return m_State; }
            set { m_State = value; }
        }

        void SetState(LogonState state)
        {
            m_State = state;
            //m_LogicUI.OnStateChanged(state);
        }

        public void OnAccountOnlyId(IEvent iEvent)
        {
            NetCmdPack pack = (NetCmdPack)iEvent.parameter;
            //玩家登陆的结果 或者是注册的结果
            LC_Cmd_AccountOnlyID ncb = (LC_Cmd_AccountOnlyID)pack.cmd;
            switch (ncb.LogonTypeID)
            {
                case 1:
                    {
                        //正常登陆
                        Debug.Log("1");
                    }
                    break;
                case 2:
                    {
                        Debug.Log("2");
                    }
                    break;
            }

            //UIManager.Instance.ShowView<MainMenuView>();
            if (ncb.dwUserID == 0)
            {
                UIManager.Instance.ShowMessage("账号密码错误",MessageBoxEnum.Style.Ok, null);
            }
            else
            {
                Debug.Log(_mUserId);
                //将IP转化为String 
                _mUserId = ncb.dwUserID;
                _mOnlyId = ncb.dwOnlyID;
                ServerSetting.HallServerIP = Utility.IPToString(ncb.GateIp);
                ServerSetting.HallServerPort = ncb.GatePort;
                ServerSetting.NewIP = ncb.GameIp;
                ServerSetting.NewPort = ncb.GamePort;
                ConnectHall();//加入到大厅并且发送命令 设置好IP 和 Port 并且 设置好需要发送的命令的参赛
            }
        }

        public void ConnectHall()
        {
            Debug.Log("开始连接UDP");
            NetManager.Instance.Disconnect();
            SetState(LogonState.LOGON_CONNECT_HALL);
            NetManager.Instance.Connect(false, ServerSetting.HallServerIP, ServerSetting.HallServerPort);
            UIManager.Instance.ShowView<MainMenuView>();
            
        }



        public void SendLogonHallData()
        {
            if (false)
            {
               /* NetCmdLogonHall ncl = new NetCmdLogonHall();
                ncl.SetCmdType(NetCmdType.CMD_LOGON_HALL);
                ncl.CRC1 = GlobalLogon.Instance.AccountData.AccountInfo.CRC1;
                ncl.CRC2 = GlobalLogon.Instance.AccountData.AccountInfo.CRC2;
                ncl.CRC3 = GlobalLogon.Instance.AccountData.AccountInfo.CRC3;
                ncl.UID = GlobalLogon.Instance.AccountData.AccountInfo.UID;
                NetServices.Instance.Send<NetCmdLogonHall>(ncl);*/
            }
            else
            {
                CL_Cmd_AccountOnlyID msgHall = new CL_Cmd_AccountOnlyID();
                msgHall.SetCmdType(NetCmdType.CMD_CL_AccountOnlyID);
                msgHall.dwOnlyID = _mOnlyId;
                msgHall.dwUserID = _mUserId;
                msgHall.MacAddress = UIDevice.GetMacAddress();
                //携带数据
                msgHall.PlateFormID = (Byte)UIDevice.GetPlatformString();
                int Witdh = GF.Resolution.GetScreenWidth();
                msgHall.ScreenPoint = Convert.ToUInt32((Witdh << 16) + GF.Resolution.GetScreenHeight());

                NetManager.Instance.Send<CL_Cmd_AccountOnlyID>(msgHall);
            }
            SetState(LogonState.LOGON_WAIT_HALL_RESULT);
        }

    }
}
