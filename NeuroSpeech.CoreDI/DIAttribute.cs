using System;

namespace NeuroSpeech.CoreDI
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DIAttribute : Attribute {

        /// <summary>
        /// 
        /// </summary>
        public DIAttribute()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public DIAttribute(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; private set; }
    }
}
