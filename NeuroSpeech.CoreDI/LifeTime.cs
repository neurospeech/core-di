using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSpeech.CoreDI
{

    /// <summary>
    /// 
    /// </summary>
    public enum LifeTime : int
    {
        /// <summary>
        /// New instance always...
        /// </summary>
        AlwaysNew = 2,

        /// <summary>
        /// Singleton for current scope
        /// </summary>
        Scoped = 1,

        /// <summary>
        /// Singleton for entire application
        /// </summary>
        Global = 0
    }

}