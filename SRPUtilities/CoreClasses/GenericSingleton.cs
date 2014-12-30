using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace STG.CMS.Portal.Core
{
    internal class GenericSingleton<T> where T : class, new()
    {
        private static T _instance;

        public static T GetInstance()
        {
            //lock (typeof(T))
            //{
            //    return _instance ?? (_instance = new T());
            //}

            return _instance ?? (_instance = typeof(T).InvokeMember(typeof(T).Name,
                                BindingFlags.CreateInstance |
                                BindingFlags.Instance |
                                BindingFlags.Public |
                                BindingFlags.NonPublic,
                                null, null, null) as T);
        }
    }
}
