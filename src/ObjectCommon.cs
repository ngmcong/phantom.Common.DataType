using System.Linq;
using System;

namespace phantom
{
    public static class ObjectExtention
    {
        /// <summary>Clone một đối tượng.</summary>
        /// Changes:
        /// - 210120: 1.0.0.0 [CNM]: Khởi tạo.
        /// <param name="input">Đối tượng cần clone.</param>
        /// <returns>Đối tượng mới clone.</returns>
        public static T DeepCopy<T>(this T input) where T : class
        {
            T outObj = Activator.CreateInstance<T>();
            var properties = typeof(T).GetProperties().Where(x => x.CanWrite);
            foreach (var property in properties)
            {
                property.SetValue(outObj, property.GetValue(input, null), null);
            }
            return outObj;
        }
    }
}