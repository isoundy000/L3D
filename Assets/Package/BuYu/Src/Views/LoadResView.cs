/***********************************************
	FileName: LoadResView.cs	    
	Creation: 2017-08-08
	Author：East.Su
	Version：V1.0.0
	Desc: 
**********************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using GF;
using GF.UI;
using Lobby;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace BuYu
{
    public class LoadResView : AppView
    {
        public Image ProgressBar;
        private float speed = 0.2f;
        private SceneModel sceneModel;
        void Start()
        {
            ProgressBar.fillAmount = 0;
            sceneModel = ModelManager.Instance.Get<SceneModel>();
            NetManager.Instance.CanProcessCmd = false;
            StartCoroutine(LoadRes());
            
        }


        IEnumerator LoadRes()
        {
            yield return StartCoroutine(ServerSetting.OnNewInit(null));

            Object obj = ResManager.Instance.LoadObject("FishConfig", "BuYu/GlobalRes/ServerSetting/", ResType.GlobalRes, typeof(TextAsset));

            Object objErrorStr = ResManager.Instance.LoadObject("ErrorString", "BuYu/GlobalRes/ServerSetting/", ResType.GlobalRes, typeof(TextAsset));


            yield return StartCoroutine(FishConfig.Instance.LoadFishConfig(obj, objErrorStr));

            NetManager.Instance.CanProcessCmd = true; yield return new WaitForEndOfFrame();
            //JoinRoomData 
            yield return StartCoroutine(sceneModel.MainInitProcedure());

            FishManager.Instance.Init(); yield return new WaitForEndOfFrame();
            BulletManager.Instance.Init(); yield return new WaitForEndOfFrame();
            SkillManager.Instance.Init();yield return new WaitForEndOfFrame();
            PlayerManager.Instance.Init(); yield return new WaitForEndOfFrame();


            sceneModel.ResetScene(true); yield return new WaitForEndOfFrame();
            
        }

        protected override void OnUpdate(float time)
        {
            if (ProgressBar.fillAmount < 1)
            {
                ProgressBar.fillAmount += speed * Time.deltaTime;
            }
            else
            {
                UIManager.Instance.ShowView<GameView>();
                UIManager.Instance.HideView<LoadResView>();
            }
        }
        

    }


}