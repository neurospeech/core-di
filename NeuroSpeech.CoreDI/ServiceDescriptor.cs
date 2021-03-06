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
    public class ServiceDescriptor
    {


        /// <summary>
        /// 
        /// </summary>
        public Type BaseType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Type Implementor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LifeTime LifeTime { get; set; }


        private Func<DIScope, object> factory = null;
        public Func<DIScope, object> Factory {
            get {
                if (factory == null)
                {
                    switch (LifeTime)
                    {
                        case LifeTime.AlwaysNew:
                            factory = s => DI.New(s, Implementor);
                            break;
                        case LifeTime.Scoped:
                            factory = s => {
                                if (s == null)
                                    throw new ArgumentNullException($"scope cannot be null");
                                return s.Get(Implementor);
                            };
                            break;
                        case LifeTime.Global:
                            factory = s => DI.Global.Get(Implementor);
                            break;
                    }
                }
                return factory;
            }
        }
        public Object Get(DIScope scope)
        {
            return Factory(scope);
        }


    }

}