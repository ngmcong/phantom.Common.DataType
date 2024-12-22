using System;

namespace phantom
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class PrimaryKey : Attribute
    {
    }
}