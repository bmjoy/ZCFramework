using System;


namespace ZCFrame
{
    
    /// <summary>
    /// 变量泛型基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Variable<T> : VariableBase
    {

        /// <summary>
        /// 当前存储的真实值
        /// </summary>
        public T Value;
        
        /// <summary>
        /// 变量类型
        /// </summary>
        public override Type Type
        {
            get { return typeof(T); }
        }
        
        public static Variable<T> Alloc()
        {
            //要从对象池获取
            Variable<T> var = GameEntry.Pool.DequeueClassObject<Variable<T>>();
            var.Value = default(T);
            var.Retain();
            return var;
        }

        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Variable<T> Alloc(T value) 
        {
            //要从对象池获取
            Variable<T> var = Alloc();
            var.Value = value;
            return var;
        }

        public static implicit operator T (Variable<T> variable)
        {
            return variable.Value;
        }
    }

}


