using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebApplication1
{
    public static class IQueryableExtensions
    {
        private static Dictionary<Type, Func<object, object>> getDelegates = new Dictionary<Type, Func<object, object>>();

        public static T FirstOrDefault<T>(this IQueryable queryable)
        {
            var enumerator = queryable.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return default(T);
            }

            var current = enumerator.Current;

            if(current is T obj)
            {
                return obj;
            }

            var type = current.GetType();
            if(type.Name == "SelectAllAndExpand`1")
            {
                if(!getDelegates.TryGetValue(type, out var getDelegate))
                {
                    var entityProperty = type.GetProperty("Instance");
                    getDelegate = CreateGetDelegateInternal(type, entityProperty.GetGetMethod());
                    getDelegates.Add(type, getDelegate);
                }
                return (T)getDelegate(current);
            }

            if(current is ISelectExpandWrapper edmEntityObject)
            {
                return GetObject<T>(edmEntityObject.ToDictionary());
            }

            return default(T);
        }

        public static IEnumerable<T> ToEnumerable<T>(this IQueryable queryable)
        {
            var enumerator = queryable.GetEnumerator();

            if (enumerator.MoveNext())
            {
                var current = enumerator.Current;

                //It is the wanted class directly, just return all of them
                if(current is T obj)
                {
                    yield return obj;

                    while (enumerator.MoveNext())
                    {
                        yield return (T)enumerator.Current;
                    }
                }
                
                //Check if it is a selectAllAndExpand
                var type = current.GetType();
                if (type.Name == "SelectAllAndExpand`1")
                {
                    //Get the delegate to get the instance
                    if (!getDelegates.TryGetValue(type, out var getDelegate))
                    {
                        var entityProperty = type.GetProperty("Instance");
                        getDelegate = CreateGetDelegateInternal(type, entityProperty.GetGetMethod());
                        getDelegates.Add(type, getDelegate);
                    }
                    yield return (T)getDelegate(current);

                    //Loop through the remaining elements just with the delegate
                    while (enumerator.MoveNext())
                    {
                        yield return (T)getDelegate(enumerator.Current);
                    }
                }

                //As a backup, check if it is a selectexpandwrapper
                if (current is ISelectExpandWrapper edmEntityObject)
                {
                    yield return GetObject<T>(edmEntityObject.ToDictionary());

                    while (enumerator.MoveNext())
                    {
                        yield return GetObject<T>(((ISelectExpandWrapper)enumerator.Current).ToDictionary());
                    }
                }
            }
        }

        private static T GetObject<T>(IDictionary<string, object> dict)
        {
            Type type = typeof(T);
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                type.GetProperty(kv.Key).SetValue(obj, kv.Value);
            }
            return (T)obj;
        }


        static Func<object, object> CreateGetDelegateInternal(Type objectType, MethodInfo method)
        {
            // First fetch the generic form
#pragma warning disable S3011 // Make sure that this accessibility bypass is safe here.
            MethodInfo genericHelper = typeof(IQueryableExtensions).GetMethod("CreateGetDelegateHelper",
                BindingFlags.Static | BindingFlags.NonPublic);
#pragma warning restore S3011

            // Now supply the type arguments
            MethodInfo constructedHelper = genericHelper.MakeGenericMethod
                (objectType, method.ReturnType);

            // Now call it. The null argument is because it's a static method.
            object ret = constructedHelper.Invoke(null, new object[] { method });

            // Cast the result to the right kind of delegate and return it
            return (Func<object, object>)ret;
        }

#pragma warning disable S1144 // Unused private types or members should be removed
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used through reflection")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "Assignment is used")]
        static Func<object, object> CreateGetDelegateHelper<TTarget, TReturn>(MethodInfo method)
            where TTarget : class
        {
            // Convert the slow MethodInfo into a fast, strongly typed, open delegate
            Func<TTarget, TReturn> func = (Func<TTarget, TReturn>)Delegate.CreateDelegate(typeof(Func<TTarget, TReturn>), method);

            // Now create a more weakly typed delegate which will call the strongly typed one
            object ret(object target) => func((TTarget)target);
            return ret;
        }
#pragma warning restore S1144 // Unused private types or members should be removed
    }
}
