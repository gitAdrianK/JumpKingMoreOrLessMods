namespace MoreSaves.Util
{
    using System;
    using HarmonyLib;

    public static class ReflectionUtil
    {
        public static Func<T> CreateStaticPropertyGetter<T>(Type type, string propertyName)
        {
            var property = AccessTools.Property(type, propertyName)
                           ?? throw new MissingMemberException(type.FullName, propertyName);

            var getter = property.GetGetMethod(true)
                         ?? throw new MissingMethodException(type.FullName, $"get_{propertyName}");

            return (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), getter);
        }

        public static Action<T> CreateStaticPropertySetter<T>(Type type, string propertyName)
        {
            var property = AccessTools.Property(type, propertyName)
                           ?? throw new MissingMemberException(type.FullName, propertyName);

            var setter = property.GetSetMethod(true)
                         ?? throw new MissingMethodException(type.FullName, $"set_{propertyName}");

            return (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), setter);
        }
    }
}
