using System;

namespace NeuroSpeech.CoreDI
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DIScopedAttribute : DIAttribute {


        /// <summary>
        /// 
        /// </summary>
        public DIScopedAttribute()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public DIScopedAttribute(Type type): base(type)
        {

        }

    }
}
