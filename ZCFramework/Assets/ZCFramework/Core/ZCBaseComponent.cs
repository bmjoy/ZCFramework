using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZCFrame
{
    
    
    /// <summary>
    /// 基础组件基类
    /// </summary>
    public abstract class ZCBaseComponent : ZCComponent
    {
     
        protected override void OnAwake()
        {
            base.OnAwake();
            //把自己加入基础组件列表
            GameEntry.RegisterComponent(this);
        }

        public abstract void Shutdown();

    }

}


