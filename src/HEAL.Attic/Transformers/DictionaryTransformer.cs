#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HEAL.Attic.Transformers {
  [Transformer("C47A62F5-F113-4A43-A8EE-CF817EC799A2", 303)]
  internal sealed class DictionaryTransformer : BoxTransformer<object> {   // TODO dictionaries of value types can be stored more efficiently?
    public override bool CanTransformType(Type type) {
      return type.IsGenericType && typeof(Dictionary<,>) == type.GetGenericTypeDefinition();
    }

    protected override void Populate(Box box, object value, Mapper mapper) {
      var kvpBox = new RepeatedValueBox();
      kvpBox.Kvps = new RepeatedKeyValuePairsBox();
      box.Values = kvpBox;

      var keys = kvpBox.Kvps.Keys;
      var values = kvpBox.Kvps.Values;
      foreach (DictionaryEntry item in (IDictionary)value) {
        if (mapper.CancellationToken.IsCancellationRequested) return;
        keys.Add(mapper.GetBoxId(item.Key));
        values.Add(mapper.GetBoxId(item.Value));
      }

      var type = value.GetType();
      var propertyInfo = type.GetProperty("Comparer");
      var comparer = propertyInfo.GetValue(value);

      var comparerType = comparer.GetType();
      if (StorableTypeAttribute.IsStorableType(comparerType))
        kvpBox.ComparerId = mapper.GetBoxId(comparer);
      else if (comparerType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).Any())
        throw new NotSupportedException("Cannot serialize non-storable equality comparers with fields");
      else
        kvpBox.ComparerId = mapper.GetBoxId(comparerType);
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      var comparerObj = mapper.GetObject(box.Values.ComparerId);
      var comparer = comparerObj is Type ? Activator.CreateInstance((Type)comparerObj) : comparerObj;

      return Activator.CreateInstance(type, box.Values.Kvps.Keys.Count, comparer);
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var type = obj.GetType();
      var dict = (IDictionary)obj;

      var addMethod = type.GetMethod("Add");
      var kvpBox = box.Values;
      var keys = kvpBox.Kvps.Keys;
      var values = kvpBox.Kvps.Values;
      for (int i = 0; i < kvpBox.Kvps.Keys.Count; i++) {
        var key = mapper.GetObject(keys[i]);
        var value = mapper.GetObject(values[i]);
        dict.Add(key, value);
      }
    }
  }
}
