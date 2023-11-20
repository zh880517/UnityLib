using System;
/*Sdp代码生成时的分类，新建分类继承 SdpLiteCatalog
    * 一个类或者结构体可以支持多个分类
    * 代码生成时同一分类的会成成在一个文件里面
    * 每个分类可以指定目录
    * 多个分类的会生成在多多份
    * {SdpLiteCatalog Type名}Packer : SdpLitePacker
    * {SdpLiteCatalog Type名}UnPacker : SdpLiteUnPacker
    */
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited= true, AllowMultiple = true)]
public abstract class SdpLiteCatalogAttribute : Attribute
{
    //是否生成序列化和反序列化接口
    public bool SerializeFunction { get; private set; }
    public SdpLiteCatalogAttribute(bool serializeFunction)
    {
        SerializeFunction = serializeFunction;
    }
}