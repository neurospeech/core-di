using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSpeech.CoreDI
{

    /// <summary>
    /// 
    /// </summary>
    public class DI
    {

        private static Dictionary<Type, ServiceDescriptor> descriptors = new Dictionary<Type, ServiceDescriptor>();
        internal static ConcurrentDictionary<Type, object> globalInstances = new ConcurrentDictionary<Type, object>();


        /// <summary>
        /// Clears everything...
        /// Only for testing...
        /// </summary>
        public static void Clear() {
            descriptors.Clear();
            globalInstances.Clear();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAbstract"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <param name="data"></param>
        public static void ReplaceGlobal<TAbstract, TImpl>(TAbstract data)
        {
            object v;
            globalInstances.TryRemove(typeof(TImpl), out v);
            globalInstances[typeof(TImpl)] = data;
            descriptors[typeof(TAbstract)] = new CoreDI.ServiceDescriptor
            {
                BaseType = typeof(TAbstract),
                Implementor = typeof(TImpl),
                LifeTime = LifeTime.Global
            };
        }


        /// <summary>
        /// Register all types declared in given assembly with DIGlobal, DIScoped and DIAlwaysNew attributes
        /// </summary>
        /// <param name="assembly"></param>
        public static void Register(Assembly assembly) {

            foreach (var type in assembly.DefinedTypes
                .Select(x => new
                {
                    Type = x.AsType(),
                    DIAttribute = x.GetCustomAttribute<DIAttribute>()
                }).Where(x => x.DIAttribute != null)) {
                switch (type.DIAttribute) {
                    case DIGlobalAttribute d:
                        Register(d.Type ?? type.Type, type.Type, LifeTime.Global);
                        break;
                    case DIScopedAttribute d:
                        Register(d.Type ?? type.Type, type.Type, LifeTime.Scoped);
                        break;
                    case DIAlwaysNewAttribute d:
                        Register(d.Type ?? type.Type, type.Type, LifeTime.AlwaysNew);
                        break;
                }
            }

        }



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RegisterGlobal<T>() {
            RegisterGlobal<T, T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RegisterScoped<T>()
        {
            RegisterScoped<T, T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RegisterAlwaysNew<T>()
        {
            RegisterAlwaysNew<T, T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAbstract"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        public static void RegisterGlobal<TAbstract, TImpl>()
        {
            Register(typeof(TAbstract), typeof(TImpl), LifeTime.Global);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        public static void GlobalOverride<T>(Func<T> factory) {
            globalInstances[typeof(T)] = factory();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAbstract"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        public static void RegisterScoped<TAbstract, TImpl>()
        {
            Register(typeof(TAbstract), typeof(TImpl), LifeTime.Scoped);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAbstract"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        public static void RegisterAlwaysNew<TAbstract, TImpl>()
        {
            Register(typeof(TAbstract), typeof(TImpl), LifeTime.AlwaysNew);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static DIScope NewScope(DIScope parent = null)
        {
            return new CoreDI.DIScope(parent);
        }

        private static void Register(Type baseType, Type typeImpl, LifeTime lifeTime)
        {

            if (typeImpl != baseType &&
                !baseType.GetTypeInfo().IsAssignableFrom(typeImpl.GetTypeInfo()))
                throw new ArgumentException($"Type {typeImpl.FullName} must inherit or implement {baseType.FullName}");

            if (lifeTime == LifeTime.Global)
            {
                if (globalInstances.ContainsKey(typeImpl))
                    throw new ArgumentException($"Global instance of {typeImpl.FullName} already exists, cannot register again");
            }

            descriptors[baseType] = new ServiceDescriptor
            {
                BaseType = baseType,
                Implementor = typeImpl,
                LifeTime = lifeTime
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static T Get<T>(DIScope scope = null)
        {
            Type type = typeof(T);
            var sd = GetFactory(type);
            if (sd == null)
            {
                throw new KeyNotFoundException($"No implementation registration found for type {type.FullName}, did you forget to register?");
            }
            return (T)sd.Get(scope);
        }

        private static ServiceDescriptor GetFactory(Type type)
        {

            ServiceDescriptor sd;
            while (!descriptors.TryGetValue(type, out sd))
            {
                if (sd?.BaseType == type)
                {
                    break;
                }
                type = type?.GetTypeInfo()?.BaseType;
                if (type == null)
                    return null;
            }

            return sd;

        }

        private static ConcurrentDictionary<Type, Func<DIScope, object>> factories = new ConcurrentDictionary<Type, Func<DIScope, object>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="implementor"></param>
        /// <returns></returns>
        public static object New(DIScope scope, Type implementor)
        {
            var f = factories.GetOrAdd(implementor, i =>
            {

                ParameterExpression sp = Expression.Parameter(typeof(DIScope));

                // get public constructor...
                // should only be one...
                var first = implementor.GetTypeInfo().DeclaredConstructors.FirstOrDefault(x=>x.IsPublic);

                if (first == null)
                {
                    return Expression.Lambda<Func<DIScope, object>>(Expression.New(implementor), sp).Compile();
                }

                List<Expression> args = new List<Expression>();
                foreach (var p in first.GetParameters())
                {
                    var ptype = p.ParameterType;
                    if (ptype == typeof(DIScope))
                    {
                        args.Add(sp);
                    }
                    else
                    {
                        var sd = GetFactory(ptype);
                        if (sd == null)
                        {
                            args.Add(Expression.Constant(null));
                        }
                        else
                        {
                            args.Add(Expression.Constant(sd.Get(scope)));
                        }
                    }
                }

                Expression call = Expression.New(first, args);
                return Expression.Lambda<Func<DIScope, object>>(call, sp).Compile();
            });
            return f(scope);
        }
    }


}
