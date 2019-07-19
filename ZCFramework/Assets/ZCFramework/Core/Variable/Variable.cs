using System;


namespace ZCFrame
{

    /// <summary>
    /// 变量泛型基类（Int,Float, Bool,String, Long,Byte,Color）
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
            //设置为默认值 (如果为引用类型 会为Null,需要new 赋值,违背了对象池的初衷,暂时未想到好的优化方式)
            var.Value = default;
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


