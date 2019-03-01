#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace HEAL.Attic {
  internal abstract class BoxTransformer<T> : Transformer {
    public override bool CanTransformType(Type type) {
      return type == typeof(T);
    }

    public override Box CreateBox(object o, Mapper mapper) {
      var box = new Box {
        TypeMsgId = mapper.TypeToTypeMessageId(o.GetType(), out TypeMessage typeMsg)
      };

      typeMsg.TransformerId = mapper.GetTransformerId(this);

      return box;
    }

    public override void FillBox(Box box, object o, Mapper mapper) {
      Populate(box, (T)o, mapper);
    }

    public override object ToObject(Box box, Mapper mapper) {
      var type = mapper.TypeMessageToType(mapper.GetTypeMessage(box.TypeMsgId));
      return type == null ? default(T) : Extract(box, type, mapper);
    }

    protected abstract void Populate(Box box, T value, Mapper mapper);
    protected abstract T Extract(Box box, Type type, Mapper mapper);
  }

  // [Transformer("854156DA-2A37-450F-92ED-355FBBD8D131", 50)]
  // internal sealed class TypeTransformer : Transformer {
  //   public override bool CanTransformType(Type type) {
  //     return typeof(Type).IsAssignableFrom(type);
  //   }
  // 
  //   public override Box CreateBox(object o, Mapper mapper) {
  //     var box = new Box { };
  //     Populate(box, o, mapper);
  //     return box;
  //   }
  // 
  //   public override void FillBox(Box box, object o, Mapper mapper) {
  //     // already filled in CreateBox
  //     // nothing to do
  //   }
  // 
  //   private void Populate(Box box, object value, Mapper mapper) {
  // 
  //   }
  // 
  //   public override object ToObject(Box box, Mapper mapper) {
  // 
  //   }
  // 
  //   private object Extract(Box box, Type type, Mapper mapper) {
  // 
  //   }
  // }

  [Transformer("4C610596-5234-4C49-998E-30007D64492E", 100)]
  internal sealed class StringBoxTransformer : BoxTransformer<string> {
    protected override void Populate(Box box, string value, Mapper mapper) { var b = new ScalarValueBox(); b.ULong = mapper.GetStringId(value); box.Value = b; }
    protected override string Extract(Box box, Type type, Mapper mapper) { return mapper.GetString((uint)box.Value.ULong); }
  }

  [Transformer("58E69402-2533-426A-B9B5-9F2EB5241560", 101)]
  internal sealed class BoolBoxTransformer : BoxTransformer<bool> {
    protected override void Populate(Box box, bool value, Mapper mapper) { var b = new ScalarValueBox(); b.ULong = Convert.ToUInt64(value); box.Value = b; }
    protected override bool Extract(Box box, Type type, Mapper mapper) { return Convert.ToBoolean(box.Value.ULong); }
  }

  [Transformer("D78F3391-3CAE-4376-9348-7FB38A4DE0EB", 102)]
  internal sealed class IntBoxTransformer : BoxTransformer<int> {
    protected override void Populate(Box box, int value, Mapper mapper) { var b = new ScalarValueBox(); b.SLong = value; box.Value = b; }
    protected override int Extract(Box box, Type type, Mapper mapper) { return (int)box.Value.SLong; }
  }

  [Transformer("25881263-F452-492E-9FD1-24E1938B048B", 103)]
  internal sealed class UIntBoxTransformer : BoxTransformer<uint> {
    protected override void Populate(Box box, uint value, Mapper mapper) { var b = new ScalarValueBox(); b.ULong = value; box.Value = b; }
    protected override uint Extract(Box box, Type type, Mapper mapper) { return (uint)box.Value.ULong; }
  }

  [Transformer("F4175165-382B-4B03-921E-5F923510FB1E", 104)]
  internal sealed class LongBoxTransformer : BoxTransformer<long> {
    protected override void Populate(Box box, long value, Mapper mapper) { var b = new ScalarValueBox(); b.SLong = value; box.Value = b; }
    protected override long Extract(Box box, Type type, Mapper mapper) { return box.Value.SLong; }
  }

  [Transformer("E8F63973-3C0C-4FA9-B068-40EF4463B30B", 105)]
  internal sealed class ULongBoxTransformer : BoxTransformer<ulong> {
    protected override void Populate(Box box, ulong value, Mapper mapper) { var b = new ScalarValueBox(); b.ULong = value; box.Value = b; }
    protected override ulong Extract(Box box, Type type, Mapper mapper) { return box.Value.ULong; }
  }

  [Transformer("15489146-EA11-4B90-8020-AF5C10A2531C", 106)]
  internal sealed class FloatBoxTransformer : BoxTransformer<float> {
    protected override void Populate(Box box, float value, Mapper mapper) { var b = new ScalarValueBox(); b.Float = value; box.Value = b; }
    protected override float Extract(Box box, Type type, Mapper mapper) { return box.Value.Float; }
  }

  [Transformer("91FD51F3-9C47-4944-AC85-273ED0561E87", 107)]
  internal sealed class DoubleBoxTransformer : BoxTransformer<double> {
    protected override void Populate(Box box, double value, Mapper mapper) { var b = new ScalarValueBox(); b.Double = value; box.Value = b; }
    protected override double Extract(Box box, Type type, Mapper mapper) { return box.Value.Double; }
  }

  [Transformer("BCB087EA-E477-47EB-9BCE-8C64BAC2F288", 108)]
  internal sealed class ByteBoxTransformer : BoxTransformer<byte> {
    protected override void Populate(Box box, byte value, Mapper mapper) { var b = new ScalarValueBox(); b.ULong = value; box.Value = b; }
    protected override byte Extract(Box box, Type type, Mapper mapper) { return (byte)box.Value.ULong; }
  }

  [Transformer("B90F61D9-75D0-4CAC-AF93-B8C6AB68F642", 109)]
  internal sealed class SByteBoxTransformer : BoxTransformer<sbyte> {
    protected override void Populate(Box box, sbyte value, Mapper mapper) { var b = new ScalarValueBox(); b.SLong = value; box.Value = b; }
    protected override sbyte Extract(Box box, Type type, Mapper mapper) { return (sbyte)box.Value.SLong; }
  }

  [Transformer("95EB44A4-EADD-4DA9-B60F-3262FAD6134B", 110)]
  internal sealed class ShortBoxTransformer : BoxTransformer<short> {
    protected override void Populate(Box box, short value, Mapper mapper) { var b = new ScalarValueBox(); b.SLong = value; box.Value = b; }
    protected override short Extract(Box box, Type type, Mapper mapper) { return (short)box.Value.SLong; }
  }

  [Transformer("E3A33614-9120-400E-BAD9-2594F6804DA8", 111)]
  internal sealed class UShortBoxTransformer : BoxTransformer<ushort> {
    protected override void Populate(Box box, ushort value, Mapper mapper) { var b = new ScalarValueBox(); b.ULong = value; box.Value = b; }
    protected override ushort Extract(Box box, Type type, Mapper mapper) { return (ushort)box.Value.ULong; }
  }

  [Transformer("C64EA534-E2E1-48F0-86C5-648AA02117BC", 112)]
  internal sealed class CharBoxTransformer : BoxTransformer<char> {
    protected override void Populate(Box box, char value, Mapper mapper) { var b = new ScalarValueBox(); b.ULong = value; box.Value = b; }
    protected override char Extract(Box box, Type type, Mapper mapper) { return (char)box.Value.ULong; }
  }

  [Transformer("93FF076B-BC4B-4C39-8C40-15E004468C98", 219)]
  internal sealed class EnumTransformer : Transformer {
    public override bool CanTransformType(Type type) {
      return typeof(Enum).IsAssignableFrom(type);
    }

    public override Box CreateBox(object o, Mapper mapper) {
      var type = o.GetType();
      var box = new Box {
        TypeMsgId = mapper.TypeToTypeMessageId(type, out TypeMessage typeMsg),
      };
      typeMsg.TransformerId = mapper.GetTransformerId(this);
      return box;
    }

    public override void FillBox(Box box, object o, Mapper mapper) {
      box.Value = new ScalarValueBox();
      box.Value.ULong = mapper.GetStringId(Enum.Format(o.GetType(), o, "G")); // TODO: introduce old names for enum values to enable refactoring
    }

    public override object ToObject(Box box, Mapper mapper) {
      var type = mapper.TypeMessageToType(mapper.GetTypeMessage(box.TypeMsgId));
      return type == null ? null : Enum.Parse(type, mapper.GetString((uint)box.Value.ULong));
    }
  }

  [Transformer("90F9F16D-9F94-491B-AC3B-E1C6F3432127", 400)]
  internal sealed class DecimalBoxTransformer : BoxTransformer<decimal> {
    protected override decimal Extract(Box box, Type type, Mapper mapper) {
      return new decimal(box.Values.Longs.Values.Select(l => (int)l).ToArray());
    }

    protected override void Populate(Box box, decimal value, Mapper mapper) {
      box.Values = new RepeatedValueBox();
      box.Values.Longs = new RepeatedLongBox();
      box.Values.Longs.Values.AddRange(decimal.GetBits(value).Select(i => (long)i));
    }
  }

  [Transformer("1C6C350F-B2C8-40F0-A964-54DDB0D087A3", 401)]
  internal sealed class DateTimeBoxTransformer : BoxTransformer<DateTime> {
    protected override void Populate(Box box, DateTime value, Mapper mapper) { var b = new ScalarValueBox(); b.Long = value.Ticks; box.Value = b; }
    protected override DateTime Extract(Box box, Type type, Mapper mapper) { return new DateTime(box.Value.Long); }
  }

  [Transformer("964074C9-4B82-4725-97AF-612A193EA5C6", 402)]
  internal sealed class TimeSpanBoxTransformer : BoxTransformer<TimeSpan> {
    protected override void Populate(Box box, TimeSpan value, Mapper mapper) { var b = new ScalarValueBox(); b.Long = value.Ticks; box.Value = b; }
    protected override TimeSpan Extract(Box box, Type type, Mapper mapper) { return new TimeSpan(box.Value.Long); }
  }

  [Transformer("B0C0165B-6279-4CC3-8DB7-D36898BFBC38", 403)]
  internal sealed class PointTransformer : BoxTransformer<Point> {
    protected override void Populate(Box box, Point value, Mapper mapper) {
      var b = new RepeatedValueBox();
      b.Longs = new RepeatedLongBox();
      box.Values = b;
      b.Longs.Values.Add(value.X);
      b.Longs.Values.Add(value.Y);
    }

    protected override Point Extract(Box box, Type type, Mapper mapper) {
      var longs = box.Values.Longs.Values;
      return new Point((int)longs[0], (int)longs[1]);
    }
  }

  [Transformer("D912D573-CE41-40B8-8F95-646C183662F6", 406)]
  internal sealed class KeyValuePairBoxTransformer : BoxTransformer<object> {
    public override bool CanTransformType(Type type) {
      return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>));
    }

    protected override void Populate(Box box, object value, Mapper mapper) {
      var b = new RepeatedValueBox();
      b.Kvps = new RepeatedKeyValuePairsBox();
      box.Values = b;

      var type = value.GetType();

      b.Kvps.Keys.Add(
        mapper.GetBoxId(type.GetProperty("Key").GetValue(value)));
      b.Kvps.Values.Add(
        mapper.GetBoxId(type.GetProperty("Value").GetValue(value)));
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var key = mapper.GetObject(box.Values.Kvps.Keys.First());
      var val = mapper.GetObject(box.Values.Kvps.Values.First());
      var type = obj.GetType();
      type.GetField("key", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, key);
      type.GetField("value", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, val);
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      return Activator.CreateInstance(type);
    }
  }

  [Transformer("BBA08D33-FDA4-4EB6-8D94-9BF3D30C3E11", 407)]
  internal sealed class TupleBoxTransformer : BoxTransformer<object> {
    private static readonly HashSet<Type> supportedTypes = new HashSet<Type> {
      typeof(Tuple<>), typeof(Tuple<,>), typeof(Tuple<,,>), typeof(Tuple<,,,>),
      typeof(Tuple<,,,,>), typeof(Tuple<,,,,,>), typeof(Tuple<,,,,,,>), typeof(Tuple<,,,,,,,>)
    };

    public override bool CanTransformType(Type type) {
      return type.IsGenericType && supportedTypes.Contains(type.GetGenericTypeDefinition());
    }

    protected override void Populate(Box box, object value, Mapper mapper) {
      var type = value.GetType();
      var b = new RepeatedValueBox();
      b.ULongs = new RepeatedULongBox();
      box.Values = b;
      var ulongs = b.ULongs.Values;
      for (int i = 1; i <= type.GetGenericArguments().Length; i++) {
        string name = string.Format("Item{0}", i);
        ulongs.Add(mapper.GetBoxId(type.GetProperty(name).GetValue(value)));
      }
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      var defaultValues = type.GetGenericArguments().Select(x =>
        x.IsValueType ? Activator.CreateInstance(x) : null
      ).ToArray();
      return Activator.CreateInstance(type, defaultValues);
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var ulongs = box.Values.ULongs.Values;
      var elements = ulongs.Select(i => mapper.GetObject((uint)i)).ToArray();
      var type = obj.GetType();
      for (int i = 1; i <= elements.Length; i++) {
        string name = string.Format("m_Item{0}", i);
        var fieldInfo = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        fieldInfo.SetValue(obj, elements[i - 1]);
      }
    }
  }

  [Transformer("731F9A18-6BF4-43BE-95CF-8205552C9B70", 500)]
  internal sealed class StructBoxTransformer : BoxTransformer<object> {
    public override bool CanTransformType(Type type) {
      return type.IsValueType && !type.IsPrimitive && !type.IsEnum && type.IsSealed;
    }

    protected override void Populate(Box box, object value, Mapper mapper) {
      var type = value.GetType();

      box.Values = new RepeatedValueBox();
      box.Values.Kvps = new RepeatedKeyValuePairsBox();
      var kvpBox = box.Values.Kvps;

      foreach (var fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
        kvpBox.Keys.Add(mapper.GetStringId(fieldInfo.Name));
        kvpBox.Values.Add(mapper.GetBoxId(fieldInfo.GetValue(value)));
      }
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      var obj = Activator.CreateInstance(type);

      for (int i = 0; i < box.Values.Kvps.Keys.Count; i++) {
        var key = box.Values.Kvps.Keys[i];
        var value = box.Values.Kvps.Values[i];
        string name = mapper.GetString(key);
        MemberInfo[] mis = type.GetMember(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (mis.Length != 1)
          throw new Exception("ambiguous struct member name " + name);
        MemberInfo mi = mis[0];
        if (StorableAttribute.IsStorable(mi))
          throw new PersistenceException("Don't use storable attributes for structs as all fields are serialized automatically.");
        if (mi.MemberType == MemberTypes.Field)
          ((FieldInfo)mi).SetValue(obj, mapper.GetObject(value));
        else
          throw new Exception("invalid struct member type " + mi.MemberType.ToString());
      }

      return obj;
    }
  }

  // transforms objects (e.g. [Storable] object o = new object())
  [Transformer("268617FE-3F0F-4029-8248-EDA420901FB6", 10000)]
  internal sealed class ObjectBoxTransformer : BoxTransformer<object> {
    protected override void Populate(Box box, object value, Mapper mapper) { }
    protected override object Extract(Box box, Type type, Mapper mapper) { return new object(); }
  }
}
