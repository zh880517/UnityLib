using System;
using System.Collections.Generic;
using System.Linq;

public class SdpLiteCatalogCollector
{
    private readonly List<SdpLiteStructCatalog> catalogs = new List<SdpLiteStructCatalog>();

    public IReadOnlyList<SdpLiteStructCatalog> Catalogs => catalogs;
    public void CollectType()
    {
        foreach (var assemble in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assemble.GetTypes())
            {
                if (type.IsInterface || type.IsAbstract || type.IsValueType)
                    continue;
                if (!type.IsSubclassOf(typeof(SdpLiteCatalogAttribute)))
                    continue;
                AddType(type);
            }
        }

        foreach (var catalog in catalogs)
        {
            var type = catalog.CatalogType;
            var baseType = type.BaseType;
            while (baseType != null && baseType != typeof(SdpLiteCatalogAttribute))
            {
                if (baseType.IsAbstract)
                {
                    baseType = baseType.BaseType;
                    continue;
                }
                SdpLiteStructCatalog parent = catalogs.FirstOrDefault(it => it.CatalogType == baseType);
                catalog.SetParent(parent);
                break;
            }
        }

        foreach (var catalog in catalogs)
        {
            catalog.UpdateDynamicField();
        }
    }

    private SdpLiteStructCatalog AddType(Type type)
    {
        var catalog = catalogs.FirstOrDefault(it => it.CatalogType == type);
        if (catalog == null)
        {
            catalog = new SdpLiteStructCatalog(type);
            catalog.CollectType();
        }
        return catalog;
    }
}
