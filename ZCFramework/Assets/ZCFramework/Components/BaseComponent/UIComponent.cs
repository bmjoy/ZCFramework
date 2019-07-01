using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZCFrame
{
    
    /// <summary>
    /// UI组件
    /// </summary>
    public class UIComponent : ZCBaseComponent, IUpdateComponent
    {
       
        
        protected override void OnAwake()
        {
            base.OnAwake();
            GameEntry.RegisterUpdateComponent(this);
        }
        
        public void OnUpdate()
        {
            
        }
        
        public override void Shutdown()
        {
            
        }
    }
}


