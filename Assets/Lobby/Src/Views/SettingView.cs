using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GF;
using GF.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lobby
{
    public class SettingView : AppView, IPointerClickHandler
    {
        private Animation animation;

        protected override void OnStart()
        {
            Transform background = transform.Find("BackGround").transform;
            Tweener tweener = background.DOLocalMoveX(450, 0.3f);
            tweener.SetUpdate(true);
            tweener.SetEase(Ease.OutBack);
            //tweener.OnComplete(OnComplete);

            GridLayoutGroup grid = background.Find("DiamondPanel/Grid").gameObject.GetComponent<GridLayoutGroup>();
            var btn = grid.transform.Find("SwitchAccount").gameObject.GetComponent<Button>();
            btn.onClick.AddListener(delegate ()
            {
                UIManager.Instance.HideView<SettingView>();
                UIManager.Instance.HideView<UserInfoView>();
                UIManager.Instance.HideView<MainMenuView>();
                UIManager.Instance.ShowView<LoginView>();
            });
            btn = grid.transform.Find("About").gameObject.GetComponent<Button>();
            btn.onClick.AddListener(delegate ()
            {

            });
        }

        public void OnComplete()
        {
            UIManager.Instance.HideView<SettingView>();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            Transform background = transform.Find("BackGround").transform;
            Tweener tweener = background.DOLocalMoveX(1665, 0.4f);
            tweener.SetUpdate(true);
            tweener.SetEase(Ease.InBack);
            tweener.OnComplete(OnComplete);
            //StartCoroutine(OnClose());
        }
    }

}

