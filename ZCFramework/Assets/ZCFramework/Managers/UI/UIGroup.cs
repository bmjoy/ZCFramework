using UnityEngine;
using System;


namespace ZCFrame
{

    /// <summary>
    /// UI分组
    /// </summary>
    [Serializable]
    public class UIGroup 
    {

        [SerializeField, Header("分组号")]
        private byte m_Id = 0;

        [SerializeField, Header("分组对象")]
        private Transform m_Group = null;

        /// <summary>
        /// 分组编号
        /// </summary>
        public byte Id{ get { return m_Id;} }
        public Transform Group { get { return m_Group; } }

       
    }
}


