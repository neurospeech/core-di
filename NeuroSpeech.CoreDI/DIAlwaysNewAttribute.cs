using System;

namespace NeuroSpeech.CoreDI
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DIAlwaysNewAttribute : DIAttribute {

        /// <summary>
        /// 
        /// </summary>
        public DIAlwaysNewAttribute()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public DIAlwaysNewAttribute(Type type):base(type)
        {

        }

    }
}
