using System;
using UnityEngine;


namespace ZCFrame
{
    public abstract class UIFormBase : IDisposable
    {
        /// <summary>
        /// UI窗体编号
        /// </summary>
        public int UIFormId { get; private set; }

        /// <summary>
        /// UI分组Id
        /// </summary>
        public byte GroupId { get; private set; }

        /// <summary>
        /// UI窗体是否激活
        /// </summary>
        public bool UIFormActiveSelf { get; private set; }

        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool IsLook { get; private set; }

        /// <summary>
        /// 关闭时间
        /// </summary>
        public float CloseTime { get; private set; }
    
        /// <summary>
        /// 窗体对象
        /// </summary>
        public Transform UIFormTransform { get; private set; }

        /// <summary>
        /// 接受参数委托
        /// </summary>
        private Delegate ParameterDelegate;


        internal void Init(int uiFormId, byte groupId, bool isLook, GameObject uiFormObject)
        {
            UIFormId = uiFormId;
            GroupId = groupId;
            IsLook = isLook;

            UIFormTransform = uiFormObject.transform;
            UIFormTransform.SetParent(GameEntry.UI.GetUIGroup(groupId).Group, false);

            SetUIFormActive(false);

            OnInit(); 
        }


        /// <summary>
        /// 添加参数获取监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        protected void AddParameterListener<T>(Action<T> callback){ ParameterDelegate = callback; }
        protected void AddParameterListener<T1, T2>(Action<T1, T2> callback)  { ParameterDelegate = callback; }
        protected void AddParameterListener<T1, T2, T3>(Action<T1, T2> callback) { ParameterDelegate = callback; }


        internal void Open()
        {
            SetFormToTheTop();
            SetUIFormActive(true);
            OnOpen();
        }

 
        internal void Open<T>(T arg)
        {
            ((Action<T>)ParameterDelegate)?.Invoke(arg);
            SetFormToTheTop();
            SetUIFormActive(true);
            OnOpen();
        }


        internal void Open<T1,T2>(T1 arg1, T2 arg2)
        {
            ((Action<T1, T2>)ParameterDelegate)?.Invoke(arg1, arg2);
            SetFormToTheTop();
            SetUIFormActive(true);
            OnOpen();
        }


        internal void Open<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        {
            ((Action<T1, T2, T3>)ParameterDelegate)?.Invoke(arg1, arg2, arg3);
            SetFormToTheTop();
            SetUIFormActive(true);
            OnOpen();
        }


        /// <summary>
        /// 置顶显示
        /// </summary>
        private void SetFormToTheTop()
        {
            //设置层级
            UIFormTransform.SetSiblingIndex(UIFormTransform.parent.childCount);
        }


        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
           GameEntry.UI.CloseUIForm(this);
        }


        public void ToClose()
        {
            if (!UIFormActiveSelf) return;

            SetUIFormActive(false);
            OnClose();
            CloseTime = Time.time;
        }


        /// <summary>
        /// 设置UI窗体激活状态
        /// </summary>
        /// <param name="value"></param>
        private void SetUIFormActive(bool value)
        {
            UIFormActiveSelf = value;
            UIFormTransform.gameObject.SetActive(value);
        }


        protected virtual void OnInit() { }
        protected virtual void OnOpen()  { }
        protected virtual void OnClose() { }
        protected virtual void OnBeforeDestroy() { }


        public void Dispose()
        {
            ParameterDelegate = null;
            UnityEngine.Object.Destroy(UIFormTransform.gameObject);
            OnBeforeDestroy();
        }
    }
}


