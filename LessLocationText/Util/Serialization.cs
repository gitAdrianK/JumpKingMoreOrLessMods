namespace LessLocationText.Util
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml.Linq;

    public static class Serialization
    {
        public static void SaveToFile<T>(T obj, string path) where T : new()
        {
            var document = new XDocument(Serialize(obj));
            document.Save(path);
        }

        public static T ReadFromFile<T>(string path) where T : new()
        {
            if (!File.Exists(path))
            {
                return new T();
            }

            var document = XDocument.Load(path);
            return Deserialize<T>(document.Root);
        }

        private static XElement Serialize<T>(T obj, string rootName = null) =>
            SerializeObject(obj, rootName ?? typeof(T).Name);

        private static T Deserialize<T>(XElement element) where T : new() => (T)DeserializeObject(element, typeof(T));

        private static XElement SerializeObject(object obj, string name)
        {
            if (obj == null)
            {
                return new XElement(name);
            }

            var type = obj.GetType();
            var element = new XElement(name);

            if (type.IsPrimitive || obj is string || obj is decimal)
            {
                element.Value = obj.ToString();
                return element;
            }

            if (obj is IEnumerable list)
            {
                foreach (var item in list)
                {
                    element.Add(SerializeObject(item, item?.GetType().Name ?? "Item"));
                }

                return element;
            }

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = prop.GetValue(obj);
                element.Add(SerializeObject(value, prop.Name));
            }

            return element;
        }


        private static object DeserializeObject(XElement element, Type type)
        {
            if (type == typeof(string))
            {
                return element.Value;
            }

            if (type.IsPrimitive || type == typeof(decimal))
            {
                return Convert.ChangeType(element.Value, type);
            }

            if (typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
            {
                var itemType = type.GetGenericArguments()[0];
                var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
                foreach (var itemElem in element.Elements())
                {
                    _ = list.Add(DeserializeObject(itemElem, itemType));
                }

                return list;
            }

            var obj = Activator.CreateInstance(type);

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var propElem = element.Element(prop.Name);
                if (propElem == null)
                {
                    continue;
                }

                var value = DeserializeObject(propElem, prop.PropertyType);
                prop.SetValue(obj, value);
            }

            return obj;
        }
    }
}
