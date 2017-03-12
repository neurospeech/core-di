using System;
using System.Collections.Generic;
using System.Text;

namespace NeuroSpeech.CoreDI
{

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DIGlobalAttribute: DIAttribute
    {

        /// <summary>
        /// 
        /// </summary>
        public DIGlobalAttribute()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public DIGlobalAttribute(Type type):base(type)
        {

        }

    }
}
