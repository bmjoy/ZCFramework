using System;
using System.Collections.Generic;
using UnityEngine;



namespace ZCFrame
{

    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIManager : ManagerBase , IDisposable
    {

        private readonly Dictionary<int, UIFormBase> uiFormDic = null;
        private LinkedList<UIFormBase> closeUIFormList;

        public UIManager()
        {
            uiFormDic = new Dictionary<int, UIFormBase>();
            closeUIFormList = new LinkedList<UIFormBase>();
        }


        /// <summary>
        /// 打开UI窗体
        /// </summary>
        /// <typeparam name="T">窗体类型</typeparam>
        /// <param name="uiFormId">窗体编号</param>
        internal void OpenUIForm<T>(int uiFormId) where T : UIFormBase, new()
        {
            GetUIFormById<T>(uiFormId).Open();
        }

        /// <summary>
        /// 打开UI窗体
        /// </summary>
        /// <typeparam name="T"><窗体类型/typeparam>
        /// <typeparam name="T1">参数1类型</typeparam>
        /// <param name="uiFormId">窗体编号</param>
        /// <param name="arg1">参数1</param>
        internal void OpenUIForm<T, T1>(int uiFormId, T1 arg1) where T : UIFormBase, new()
        {
             GetUIFormById<T>(uiFormId).Open(arg1);

        }

        internal void OpenUIForm<T, T1, T2>(int uiFormId, T1 arg1, T2 arg2) where T : UIFormBase, new()
        {
            GetUIFormById<T>(uiFormId).Open(arg1, arg2);
        }

        internal void OpenUIForm<T, T1, T2, T3>(int uiFormId, T1 arg1, T2 arg2, T3 arg3) where T : UIFormBase, new()
        {
            GetUIFormById<T>(uiFormId).Open(arg1, arg2, arg3);
        }


        /// <summary>
        /// 关闭UI窗体(根据UIFormId)
        /// </summary>
        internal void CloseUIForm(int uiFormId)
        {
            if (!uiFormDic.TryGetValue(uiFormId, out UIFormBase uiForm))
            {
                Debug.LogWarning(string.Format("窗口 {0:d} 未实例化,关闭失败", uiFormId));
                return;
            }

            CloseUIForm(uiForm);
        }

        /// <summary>
        /// 关闭UI窗体(根据UIFormBase)
        /// </summary>
        /// <param name="formBase"></param>
        internal void CloseUIForm(UIFormBase formBase)
        {
            formBase.ToClose();
            AddFormOnCloseList(formBase);
        }

        /// <summary>
        /// 根据UIFormId 获取 UIForm
        /// </summary>
        private UIFormBase GetUIFormById<T>(int uiFormId) where T : UIFormBase, new()
        {
            lock (uiFormDic)
            {
                if (!uiFormDic.TryGetValue(uiFormId, out UIFormBase uiForm))
                {
                    uiForm = CreateNewUIForm<T>(uiFormId);
                    uiFormDic.Add(uiFormId, uiForm);
                }

                RemoveFormOnCloseList(uiForm);
                return uiForm;
            }
        }

        /// <summary>
        /// 创建新窗口
        /// </summary>
        /// <typeparam name="T">UIForm类型</typeparam>
        /// <param name="uiFormId"></param>
        /// <returns></returns>
        private UIFormBase CreateNewUIForm<T>(int uiFormId) where T : UIFormBase, new()
        {
            T uiForm = new T();
            GameObject uiFormObject = LoadUIFormObject();
            uiFormObject.SetActive(false);
            uiForm.Init(uiFormId, 2, false, uiFormObject);
            return uiForm;
        }

        /// <summary>
        /// 加载UI窗体对象
        /// </summary>
        /// <returns></returns>
        private GameObject LoadUIFormObject()
        {
            GameObject go = new GameObject();
            return go;
        }

        private void RemoveFormOnCloseList(UIFormBase formBase)
        {
            if (closeUIFormList.Contains(formBase))
                closeUIFormList.Remove(formBase);
        }

        private void AddFormOnCloseList(UIFormBase formBase)
        {
            if (!closeUIFormList.Contains(formBase) && !formBase.IsLook)
            closeUIFormList.AddLast(formBase);
        }


        /// <summary>
        /// 检查是否可以释放闲置的UI
        /// </summary>
        internal void CheckClear()
        {

            for (LinkedListNode<UIFormBase> curr = closeUIFormList.First; curr != null;)
            {
                if ( Time.time > curr.Value.CloseTime + GameEntry.UI.UIExpire)
                {
                    //手动释放UI
                    lock (uiFormDic)
                    {
                        uiFormDic[curr.Value.UIFormId] = null;
                        uiFormDic.Remove(curr.Value.UIFormId);
                        curr.Value.Dispose();
                    }

                    LinkedListNode<UIFormBase> next = curr.Next;
                    closeUIFormList.Remove(curr.Value);
                    curr.Value = null;
                    curr = next;
                }
                else
                {
                    curr = curr.Next;
                }
            }
        }

        public void Dispose()
        {
            closeUIFormList.Clear();

            foreach (UIFormBase uiForm in uiFormDic.Values)
            {
                uiForm.Dispose();
            }

            uiFormDic.Clear();

        }
    }
}


