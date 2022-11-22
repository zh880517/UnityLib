using System;
namespace FrameLine
{
    //类型标识符：字符串，通过Guid.NewGuid().ToString()生成
    //主要用来防止类重名后丢失已经序列化的类型信息
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TypeIdAttribute : Attribute
    {
        public string GUID { get; private set; }
        public TypeIdAttribute(string guid)
        {
            GUID = guid;
        }
    }
}