using UnityEngine;
using XLua;



namespace ZCFrame
{
    /// <summary>
    /// Lua窗口
    /// </summary>
    [LuaCallCSharp]
    public class LuaForm : UIFormBase
    {
        [CSharpCallLua]
        public delegate void OnInitHandler(Transform transform, System.Action closeAction);
        OnInitHandler onInit;

        [CSharpCallLua]
        public delegate void OnOpenHandler(object userData);
        OnOpenHandler onOpen;

        [CSharpCallLua]
        public delegate void OnCloseHandler();
        OnCloseHandler onClose;

        [CSharpCallLua]
        public delegate void OnBeforeHandler();
        OnBeforeHandler onBefore;

        private LuaTable scriptEnv;
        private LuaEnv luaEnv;


        protected override void OnInit()
        {
            base.OnInit();

            luaEnv = LuaManager.luaEnv;//从LuaManager上获取 全局只有一个
            if (luaEnv == null) return;

            scriptEnv = luaEnv.NewTable();

            LuaTable meta = luaEnv.NewTable();
            meta.Set("__index", luaEnv.Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();

            string name = "UI_TaskView";

            onInit = scriptEnv.GetInPath<OnInitHandler>(name + ".OnInit");
            onOpen = scriptEnv.GetInPath<OnOpenHandler>(name + ".OnOpen");
            onClose = scriptEnv.GetInPath<OnCloseHandler>(name+ ".OnClose");
            onBefore = scriptEnv.GetInPath<OnBeforeHandler>(name+".OnBefore");
            scriptEnv.Set("self", this);
            onInit?.Invoke(UIFormTransform, Close);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            onOpen?.Invoke(userData);
        }

        protected override void OnClose()
        {
            base.OnClose();
            onClose?.Invoke();
        }

        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            onBefore?.Invoke();

            onInit = null;
            onOpen = null;
            onClose = null;
            onBefore = null;
        }
    }
}


