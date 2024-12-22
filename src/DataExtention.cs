using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace phantom
{
    public static class DataExtention
    {
        public static object ConvertDBIfNull(this object input)
        {
            if (input == null)
            {
                return DBNull.Value;
            }
            if (input.GetType() == typeof(DateTime) && Convert.ToDateTime(input) == DateTime.MinValue)
            {
                return DBNull.Value;
            }
            return input;
        }
        public static object DefaultValueIfNull(this object input, object defaultValue)
        {
            if (input == null) return defaultValue;
            if (input.GetType() == typeof(DateTime) && Convert.ToDateTime(input) == DateTime.MinValue)
            {
                return defaultValue;
            }
            if (input.GetType() == typeof(string) && string.IsNullOrEmpty(input.ToString()))
            {
                return defaultValue;
            }
            return input;
        }
        public static object GetDefault(this Type type)
        {
            if (type == null) return null;
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
        public static string ToXmlString<T>(this List<T> input)
        {
            if (input == null || input.Count == 0) return null;
            var mXDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));
            XElement mRoot = new XElement("root");
            var mPropertyCollection = typeof(T).GetProperties();
            foreach (var item in input)
            {
                XElement mItem = new XElement(typeof(T).Name);
                foreach (var aProperty in mPropertyCollection)
                {
                    mItem.Add(new XElement(aProperty.Name, aProperty.GetValue(item, null) ?? Nullable.GetUnderlyingType(aProperty.PropertyType).GetDefault()));
                }
                mRoot.Add(mItem);
            }
            mXDocument.Add(mRoot);
            return mXDocument.ToString();
        }
        public static IEnumerable<T> CollectionDeepCopy<T>(this IEnumerable<T> input)
        {
            if (input == null) return null;
            return input.Select(x => x.DeepCopy());
        }
        public static T DeepCopy<T>(this T input)
        {
            if (input == null)
            {
                return default(T);
            }
            T OutObject = Activator.CreateInstance<T>();
            //if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(ObservableCollection<>))
            //{
            //    Type ItemType = typeof(ObservableCollection<>).MakeGenericType(typeof(T));
            //    var CollectionObject = (IList)Activator.CreateInstance(ItemType);
            //}
            var propertyCollection = OutObject.GetType().GetProperties();
            if (propertyCollection == null || propertyCollection.Length == 0
                || !propertyCollection.Any(x => x.CanWrite))
            {
                return OutObject;
            }
            foreach (var CurrentProperty in propertyCollection.Where(x => x.CanWrite))
            {
                CurrentProperty.SetValue(OutObject, CurrentProperty.GetValue(input, null), null);
            }
            return OutObject;
        }
        public static void CopyValues<T>(this T aInput, T aObject)
        {
            var mListProperties = typeof(T).GetProperties();
            foreach (var mProperty in mListProperties.Where(x => x.CanWrite).ToList())
            {
                mProperty.SetValue(aInput, mProperty.GetValue(aObject, null), null);
            }
        }
        public static void FromBase<T>(this T input, object baseObj)
        {
            if (!typeof(T).BaseType.Equals(baseObj.GetType()))
            {
                return;
            }
            var mPropertyCollection = typeof(T).BaseType.GetProperties();
            if (mPropertyCollection == null || mPropertyCollection.Length == 0
                || !mPropertyCollection.Any(x => x.CanWrite))
            {
                return;
            }
            foreach (var aProperty in mPropertyCollection.Where(x => x.CanWrite))
            {
                aProperty.SetValue(input, aProperty.GetValue(baseObj, null), null);
            }
        }
        public static T ConvertInheritToObject<T>(this object baseObj)
        {
            var NewObject = Activator.CreateInstance<T>();
            if (!typeof(T).BaseType.Equals(baseObj.GetType()))
            {
                return default(T);
            }
            var mPropertyCollection = typeof(T).BaseType.GetProperties();
            if (mPropertyCollection == null || mPropertyCollection.Length == 0
                || !mPropertyCollection.Any(x => x.CanWrite))
            {
                return default(T);
            }
            foreach (var aProperty in mPropertyCollection.Where(x => x.CanWrite))
            {
                aProperty.SetValue(NewObject, aProperty.GetValue(baseObj, null), null);
            }
            return NewObject;
        }
        public static void ChangeTypeCurrentPropertyValue<T>(this PropertyInfo inputObjProperty, object inputObj, object inputValue)
        {
            if (inputObjProperty.PropertyType.IsGenericType && inputObjProperty.PropertyType.GetGenericTypeDefinition() == typeof(ObservableCollection<>))
            {
                inputObjProperty.SetValue(inputObj, new ObservableCollection<T>((IEnumerable<T>)inputValue), null);
            }
            else if (inputObjProperty.PropertyType.IsEnum)
            {
                if (inputValue.ToString().Length == 1 && inputValue.GetType() == typeof(string))
                {
                    inputObjProperty.SetValue(inputObj, inputValue == DBNull.Value ? null : Convert.ChangeType(Enum.ToObject(inputObjProperty.PropertyType, inputValue.ToString()[0]), inputObjProperty.PropertyType), null);
                }
                else
                {
                    inputObjProperty.SetValue(inputObj, inputValue == DBNull.Value ? null : Convert.ChangeType(Enum.Parse(inputObjProperty.PropertyType, inputValue.ToString()), inputObjProperty.PropertyType), null);
                }
            }
            else
            {
                inputObjProperty.SetValue(inputObj, inputValue == DBNull.Value ? null : Convert.ChangeType(inputValue, Nullable.GetUnderlyingType(inputObjProperty.PropertyType) ?? inputObjProperty.PropertyType), null);
            }
        }
        public static T CopyValueTo<T>(this object input)
        {
            if (input == null) return default(T);
            var properties = input.GetType().GetProperties();
            T outObject = Activator.CreateInstance<T>();
            var propertyCollection = outObject.GetType().GetProperties();
            if (propertyCollection == null || propertyCollection.Length == 0 || !propertyCollection.Any(x => x.CanWrite))
            {
                return outObject;
            }
            foreach (var currentProperty in propertyCollection.Where(x => x.CanWrite))
            {
                var inputProperty = properties?.FirstOrDefault(x => x.Name == currentProperty.Name);
                if (inputProperty == null) continue;
                currentProperty.SetValue(outObject, inputProperty.GetValue(input, null), null);
            }
            return outObject;
        }
    }
}