#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;

namespace HEAL.Attic.Transformers {
  [Transformer("78556C88-0FEE-4602-95C7-A469B2DDB468", 600)]
  internal sealed class StorableTypeBoxTransformer : BoxTransformer<object> {
    public override bool CanTransformType(Type type) {
      return StorableTypeAttribute.IsStorableType(type) && !type.IsValueType && !type.IsEnum || // don't transform structs or enums
        type.BaseType != null && CanTransformType(type.BaseType);
    }

    protected override void Populate(Box box, object value, Mapper mapper) {
      if (mapper.CancellationToken.IsCancellationRequested) return;
      var emptyArgs = new object[0];
      var type = value.GetType();

      // traverse type hierarchy
      do {
        var typeInfo = Mapper.StaticCache.GetTypeInfo(type);

        foreach (var hook in typeInfo.BeforeSerializationHooks) {
          try {
            hook.Invoke(value, emptyArgs);
          } catch (TargetInvocationException t) {
            throw t.InnerException;
          }
        }

        type = type.BaseType;
      } while (StorableTypeAttribute.IsStorableType(type) && !mapper.CancellationToken.IsCancellationRequested);

      type = value.GetType();

      var set = new HashSet<Tuple<Type, string>>();

      // traverse type hierarchy
      var membersBox = new StorableTypeMembersBox();
      box.Members = membersBox;
      do {
        var typeInfo = Mapper.StaticCache.GetTypeInfo(type);
        membersBox.TypeId = mapper.GetStringId(typeInfo.StorableTypeAttributeGuid);

        foreach (var componentInfo in typeInfo.Fields) {
          membersBox.Keys.Add(mapper.GetStringId(componentInfo.Name));
          membersBox.Values.Add(mapper.GetBoxId(componentInfo.MemberInfo.GetValue(value)));
        }

        foreach (var componentInfo in typeInfo.ReadableProperties) {
          var declaringType = componentInfo.DeclaringType;

          if (!set.Add(Tuple.Create(declaringType, componentInfo.Name))) continue;

          membersBox.Keys.Add(mapper.GetStringId(componentInfo.Name));
          membersBox.Values.Add(mapper.GetBoxId(componentInfo.MemberInfo.GetValue(value, null)));
        }

        type = type.BaseType;
        membersBox.Parent = new StorableTypeMembersBox();
        membersBox = membersBox.Parent;
      } while (StorableTypeAttribute.IsStorableType(type) && !mapper.CancellationToken.IsCancellationRequested);
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      return mapper.CreateInstance(type);
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var dict = new Dictionary<string, object>();
      var members = box.Members;
      while (members != null) {
        for (int i = 0; i < members.Keys.Count; i++) {
          string key = mapper.GetComponentInfoKey(members.TypeId, members.Keys[i]);
          object value = mapper.GetObject(members.Values[i]);
          dict.Add(key, value);
        }
        members = members.Parent;
      }

      var type = (Type)mapper.GetObject(box.TypeBoxId);
      var typeInfo = Mapper.StaticCache.GetTypeInfo(type);
      var typeStack = new Stack<Tuple<Type, TypeInfo>>();

      do {
        typeInfo = Mapper.StaticCache.GetTypeInfo(type);
        typeStack.Push(Tuple.Create(type, typeInfo));
        type = type.BaseType;
      } while (StorableTypeAttribute.IsStorableType(type));

      foreach (var frame in typeStack) {
        type = frame.Item1;
        typeInfo = frame.Item2;

        // set stored or default values for all fields and properties
        foreach (var componentInfo in typeInfo.Fields) {
          var attrib = componentInfo.StorableAttribute;
          var fieldInfo = componentInfo.MemberInfo;

          if (dict.TryGetValue(componentInfo.FullName, out object value)) {
            fieldInfo.SetValue(obj, value);
          } else if (attrib != null && attrib.DefaultValue != null) {
            fieldInfo.SetValue(obj, attrib.DefaultValue);
          }
        }

        foreach (var componentInfo in typeInfo.WriteableProperties) {
          var attrib = componentInfo.StorableAttribute;
          var propertyInfo = componentInfo.MemberInfo;

          if (dict.TryGetValue(componentInfo.FullName, out object value)) {
            propertyInfo.SetValue(obj, value, null);
          } else if (attrib != null && attrib.DefaultValue != null) {
            propertyInfo.SetValue(obj, attrib.DefaultValue, null);
          }
        }
      }
    }
  }
}
