using System;
using System.Collections.Generic;
using System.Reflection;

namespace HEAL.Attic.Tests {
  public static class TypeManipulation {
    // the following are necessary to test backwards compatibility test cases
    private static Dictionary<Guid, Type> guid2Type;
    private static Dictionary<Type, Guid> type2Guid;
    private static Dictionary<Type, TypeInfo> typeInfos;
    private static PropertyInfo guidPropertyInfo = typeof(StorableTypeAttribute).GetProperty("Guid");

    static TypeManipulation() {
      var guid2TypeFieldInfo = typeof(StaticCache).GetField("guid2Type", BindingFlags.NonPublic | BindingFlags.Instance);
      var type2GuidFieldInfo = typeof(StaticCache).GetField("type2Guid", BindingFlags.NonPublic | BindingFlags.Instance);
      var typeInfosFieldInfo = typeof(StaticCache).GetField("typeInfos", BindingFlags.NonPublic | BindingFlags.Instance);

      guid2Type = (Dictionary<Guid, Type>)guid2TypeFieldInfo.GetValue(Mapper.StaticCache);
      type2Guid = (Dictionary<Type, Guid>)type2GuidFieldInfo.GetValue(Mapper.StaticCache);
      typeInfos = (Dictionary<Type, TypeInfo>)typeInfosFieldInfo.GetValue(Mapper.StaticCache);
    }

    public static void RegisterType(Guid guid, Type type) {
      Mapper.StaticCache.RegisterType(guid, type);
    }

    public static void DeregisterType(Guid guid) {
      if (!guid2Type.ContainsKey(guid)) return;
      type2Guid.Remove(guid2Type[guid]);
      guid2Type.Remove(guid);
    }

    public static Type GetType(Guid guid) {
      return guid2Type[guid];
    }

    public static TypeInfo GetTypeInfo(Type type) {
      return Mapper.StaticCache.GetTypeInfo(type);
    }

    public static void ReplaceTypeImplementation(Type old, Guid oldGuid, Type @new) {
      DeregisterType(oldGuid);
      DeregisterType(StorableTypeAttribute.GetStorableTypeAttribute(@new).Guid);

      RegisterType(oldGuid, @new);
      SetTypeGuid(@new, oldGuid);
      typeInfos.Remove(old);
    }

    public static void SetTypeGuid(Type type, Guid guid) {
      var typeInfo = GetTypeInfo(type);
      guidPropertyInfo.SetValue(typeInfo.StorableTypeAttribute, guid);
      var reflectMethod = typeInfo.GetType().GetMethod("Reflect", BindingFlags.NonPublic | BindingFlags.Instance);
      reflectMethod.Invoke(typeInfo, new object[0]);
    }
  }
}
