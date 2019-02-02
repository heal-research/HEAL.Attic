#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HEAL.Attic {
  public sealed class TypeInfo {
    private ConstructorInfo storableConstructor;
    private ConstructorInfo defaultConstructor;

    public Type Type { get; private set; }
    public ITransformer Transformer { get; private set; }
    public StorableTypeAttribute StorableTypeAttribute { get; private set; }
    public string StorableTypeAttributeGuid { get; private set; }
    public IEnumerable<ComponentInfo<FieldInfo>> Fields { get; private set; }
    // public IEnumerable<ComponentInfo<PropertyInfo>> Properties { get; private set; }
    public IEnumerable<ComponentInfo<PropertyInfo>> WriteableProperties { get; private set; }
    public IEnumerable<ComponentInfo<PropertyInfo>> ReadableProperties { get; private set; }
    public IEnumerable<MethodInfo> BeforeSerializationHooks { get; private set; }
    public IEnumerable<MethodInfo> AfterDeserializationHooks { get; private set; }
    public long Used { get; set; }

    public TypeInfo(Type type) {
      Type = type;
      StorableTypeAttribute = StorableTypeAttribute.GetStorableTypeAttribute(type);
      StorableTypeAttributeGuid = string.Empty;
      Fields = Enumerable.Empty<ComponentInfo<FieldInfo>>();
      WriteableProperties = Enumerable.Empty<ComponentInfo<PropertyInfo>>();
      ReadableProperties = Enumerable.Empty<ComponentInfo<PropertyInfo>>();
      BeforeSerializationHooks = Enumerable.Empty<MethodInfo>();
      AfterDeserializationHooks = Enumerable.Empty<MethodInfo>();
      Used = 0;
      Reflect();
    }
    public TypeInfo(Type type, ITransformer transformer)
      : this(type) {
      Transformer = transformer;
    }

    private void Reflect() {
      var type = Type;
      if (StorableTypeAttribute != null) {
        StorableTypeAttributeGuid = StorableTypeAttribute.Guid.ToString().ToUpperInvariant();
        string guidPrefix = StorableTypeAttributeGuid;
        // check constructors
        if (!type.IsValueType && !type.IsEnum && !type.IsInterface &&
          GetStorableConstructor() == null && GetDefaultConstructor() == null)
          throw new PersistenceException("No storable constructor or parameterless constructor found.");

        var fields = new List<ComponentInfo<FieldInfo>>();
        var properties = new List<ComponentInfo<PropertyInfo>>();
        var beforeSerializationHooks = new List<MethodInfo>();
        var afterDeserializationHooks = new List<MethodInfo>();

        if (StorableTypeAttribute.MemberSelection != StorableMemberSelection.AllProperties) {
          // TODO: improved performance for static fields
          var fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic)
                               .Where(x => !x.Name.StartsWith("<") && !x.Name.EndsWith("k__BackingField")); // exclude backing fields

          if (StorableTypeAttribute.MemberSelection == StorableMemberSelection.MarkedOnly)
            fieldInfos = fieldInfos.Where(StorableAttribute.IsStorable).ToArray();

          foreach (var field in fieldInfos) {
            var name = field.Name;
            var attrib = StorableAttribute.GetStorableAttribute(field);

            if (attrib != null) {
              if (!string.IsNullOrEmpty(attrib.Name) && !string.IsNullOrEmpty(attrib.OldName))
                throw new PersistenceException("Cannot use Name and OldName at the same time.");

              if (!string.IsNullOrEmpty(attrib.Name)) name = attrib.Name;
              else if (!string.IsNullOrEmpty(attrib.OldName)) name = attrib.OldName;
            }

            var nameParts = name.Split('.').ToArray();
            var sourceType = type;
            var tmpGuid = Guid.Empty;

            for (int i = 0; i < nameParts.Length; i++) {
              var part = nameParts[i];
              if (part == "base") sourceType = sourceType.BaseType;
              else if (Guid.TryParse(part, out tmpGuid)) {
                if (i != 0 || nameParts.Length != 2) throw new PersistenceException("Invalid field path specified.");
                guidPrefix = tmpGuid.ToString().ToUpper();
                break;
              } else if (i != nameParts.Length - 1)
                throw new PersistenceException("Invalid field path specified.");
              else break;
            }

            if (sourceType != type) {
              name = nameParts[nameParts.Length - 1];
              guidPrefix = StorableTypeAttribute.GetStorableTypeAttribute(sourceType).Guid.ToString().ToUpperInvariant();
            } else if (tmpGuid != Guid.Empty) {
              name = nameParts[nameParts.Length - 1];
            }

            fields.Add(new ComponentInfo<FieldInfo>(name, guidPrefix + "." + name, field, type, attrib, true, true));
          }
        }

        if (StorableTypeAttribute.MemberSelection != StorableMemberSelection.AllFields) {
          // TODO: improved performance for static properties
          var propertyInfos = type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic)
                                  .Where(x => !x.GetIndexParameters().Any());  // exclude indexed properties

          if (StorableTypeAttribute.MemberSelection == StorableMemberSelection.MarkedOnly)
            propertyInfos = propertyInfos.Where(StorableAttribute.IsStorable).ToArray();

          foreach (var property in propertyInfos) {
            var name = property.Name;
            var attrib = StorableAttribute.GetStorableAttribute(property);

            if (attrib != null) {
              if (!string.IsNullOrEmpty(attrib.Name) && !string.IsNullOrEmpty(attrib.OldName))
                throw new PersistenceException("Cannot use Name and OldName at the same time.");

              if (attrib.AllowOneWay && !string.IsNullOrEmpty(attrib.OldName))
                throw new PersistenceException("Cannot use AllowOneWay and OldName at the same time.");

              if (!string.IsNullOrEmpty(attrib.Name)) name = attrib.Name;
              else if (!string.IsNullOrEmpty(attrib.OldName)) name = attrib.OldName;
            }

            if ((!property.CanRead || !property.CanWrite) && (attrib == null || !attrib.AllowOneWay && string.IsNullOrEmpty(attrib.OldName)))
              throw new PersistenceException("Properties must be readable and writable or have one way serialization explicitly enabled or use OldName.");

            var nameParts = name.Split('.').ToArray();
            var sourceType = type;
            var tmpGuid = Guid.Empty;

            for (int i = 0; i < nameParts.Length; i++) {
              var part = nameParts[i];
              if (part == "base") sourceType = sourceType.BaseType;
              else if (Guid.TryParse(part, out tmpGuid)) {
                if (i != 0 || nameParts.Length != 2) throw new PersistenceException("Invalid field path specified.");
                guidPrefix = tmpGuid.ToString().ToUpper();
                break;
              } else if (i != nameParts.Length - 1)
                throw new PersistenceException("Invalid field path specified.");
              else break;
            }

            if (sourceType != type) {
              name = nameParts[nameParts.Length - 1];
              guidPrefix = StorableTypeAttribute.GetStorableTypeAttribute(sourceType).Guid.ToString().ToUpper();
            } else if (tmpGuid != Guid.Empty) {
              name = nameParts[nameParts.Length - 1];
            }

            var declaringType = GetPropertyDeclaringBaseType(property);
            properties.Add(new ComponentInfo<PropertyInfo>(name, guidPrefix + "." + name, property, declaringType, attrib, property.CanRead, property.CanWrite));
          }
        }

        var methodInfos = type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic)
                              .Where(StorableHookAttribute.IsStorableHook)
                              .Where(x => x.ReturnType == typeof(void) && !x.GetParameters().Any());

        foreach (var method in methodInfos) {
          foreach (var attrib in StorableHookAttribute.GetStorableHookAttributes(method)) {
            if (attrib.HookType == HookType.BeforeSerialization)
              beforeSerializationHooks.Add(method);
            if (attrib.HookType == HookType.AfterDeserialization)
              afterDeserializationHooks.Add(method);
          }
        }

        Fields = fields;
        WriteableProperties = properties.Where(p => p.Writeable).ToArray();
        ReadableProperties = properties.Where(p => p.Readable).ToArray();
        BeforeSerializationHooks = beforeSerializationHooks;
        AfterDeserializationHooks = afterDeserializationHooks;
      }
    }

    private readonly object[] defaultConstructorParams = new object[] { StorableConstructorFlag.Default };
    private readonly object[] emptyConstructorParams = new object[0];
    public Func<object> GetConstructor() {
      if (storableConstructor != null) return () => storableConstructor.Invoke(defaultConstructorParams);
      else if (defaultConstructor != null) return () => defaultConstructor.Invoke(emptyConstructorParams);

      // get storable constructor
      var ctor = GetStorableConstructor();
      if (ctor != null) {
        storableConstructor = ctor;
        return () => storableConstructor.Invoke(defaultConstructorParams);
      }
        
      // get default constructor
      ctor = GetDefaultConstructor();
      if (ctor != null) {
        defaultConstructor = ctor;
        return () => defaultConstructor.Invoke(emptyConstructorParams);
      }

      throw new PersistenceException("No storable constructor or parameterless constructor found.");
    }

    private ConstructorInfo GetStorableConstructor() {
      return (from ctor in Type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
              let parameters = ctor.GetParameters()
              where StorableConstructorAttribute.IsStorableConstructor(ctor)
                 && parameters.Length == 1
                 && parameters[0].ParameterType == typeof(StorableConstructorFlag)
              select ctor).FirstOrDefault();
    }

    private ConstructorInfo GetDefaultConstructor() {
      return Type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
    }

    private static Type GetPropertyDeclaringBaseType(MemberInfo mi) {
      PropertyInfo pi = mi as PropertyInfo;
      if (pi == null)
        throw new PersistenceException("fields don't have a declaring base type, directly use FullyQualifiedMemberName instead");
      if (pi.CanRead)
        return pi.GetGetMethod(true).GetBaseDefinition().DeclaringType;
      if (pi.CanWrite)
        return pi.GetSetMethod(true).GetBaseDefinition().DeclaringType;
      throw new InvalidOperationException("property has neigher a getter nor a setter.");
    }
  }
}
