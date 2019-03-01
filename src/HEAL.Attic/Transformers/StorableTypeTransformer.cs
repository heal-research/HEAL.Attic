#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;

namespace HEAL.Attic {
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

      var typeInfo = Mapper.StaticCache.GetTypeInfo(type);
      do {
        foreach (var hook in typeInfo.BeforeSerializationHooks) {
          try {
            hook.Invoke(value, emptyArgs);
          } catch (TargetInvocationException t) {
            throw t.InnerException;
          }
        }

        type = type.BaseType;
        typeInfo = Mapper.StaticCache.GetTypeInfo(type);
      } while (StorableTypeAttribute.IsStorableType(type) && !mapper.CancellationToken.IsCancellationRequested);


      var set = new HashSet<Tuple<Type, string>>();

      // traverse type hierarchy
      var membersBox = new StorableTypeMembersBox();
      box.Members = membersBox;

      type = value.GetType();
      typeInfo = Mapper.StaticCache.GetTypeInfo(type);

      membersBox.StorableTypeLayoutId = mapper.GetStorableTypeLayoutIds(typeInfo.StorableTypeAttributeGuid);
      var layout = mapper.GetStorableTypeLayout(membersBox.StorableTypeLayoutId);

      while (StorableTypeAttribute.IsStorableType(type) && !mapper.CancellationToken.IsCancellationRequested) {

        foreach (var componentInfo in typeInfo.Fields) {
          if (!layout.IsPopulated) layout.MemberNames.Add(componentInfo.Name);
          membersBox.ValueBoxId.Add(mapper.GetBoxId(componentInfo.MemberInfo.GetValue(value)));
        }

        foreach (var componentInfo in typeInfo.ReadableProperties) {
          var declaringType = componentInfo.DeclaringType;

          if (!set.Add(Tuple.Create(declaringType, componentInfo.Name))) continue;

          if (!layout.IsPopulated) layout.MemberNames.Add(componentInfo.Name);
          membersBox.ValueBoxId.Add(mapper.GetBoxId(componentInfo.MemberInfo.GetValue(value, null)));
        }

        layout.IsPopulated = true;

        // prepare for next iteration
        type = type.BaseType;
        if (StorableTypeAttribute.IsStorableType(type)) {
          typeInfo = Mapper.StaticCache.GetTypeInfo(type);
          layout.ParentLayoutId = mapper.GetStorableTypeLayoutIds(typeInfo.StorableTypeAttributeGuid);
          layout = mapper.GetStorableTypeLayout(layout.ParentLayoutId);
        }
      }
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      return mapper.CreateInstance(type);
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var dict = new Dictionary<string, object>();
      var members = box.Members;
      var layout = mapper.GetStorableTypeLayout(members.StorableTypeLayoutId);

      var valueIdx = 0;
      while (layout != null) {
        for (int j = 0; j < layout.MemberNames.Count; j++) {
          string key = mapper.GetComponentInfoKey(layout.TypeGuid, layout.MemberNames[j]);
          object value = mapper.GetObject(members.ValueBoxId[valueIdx++]);
          dict.Add(key, value);
        }
        layout = mapper.GetStorableTypeLayout(layout.ParentLayoutId);
      }

      var type = mapper.TypeMessageToType(mapper.GetTypeMessage(box.TypeMsgId));
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
