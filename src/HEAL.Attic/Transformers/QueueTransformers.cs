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
using HEAL.Attic;

namespace HEAL.Attic.Transformers {
  [Transformer("F8A2F91F-3FCE-4514-A9E6-3B376BD3F379", 200)]
  internal abstract class QueueTransformer<T> : BoxTransformer<object> {
    public override bool CanTransformType(Type type) {
      return
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Queue<>) && typeof(T).IsAssignableFrom(type.GetGenericArguments()[0]);
    }

    protected abstract void AddRange(IEnumerable values, RepeatedValueBox repValueBox, Mapper mapper);
    protected abstract IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper);

    protected override void Populate(Box box, object value, Mapper mapper) {
      box.Values = new RepeatedValueBox();
      AddRange((IEnumerable)value, box.Values, mapper);
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      if (type.GetGenericArguments()[0].IsPrimitive) {
        return Activator.CreateInstance(type, new object[] {
          ExtractValues(box.Values, mapper)
        });
      } else {
        return Activator.CreateInstance(type, new object[] { box.Values.UInts.Values.Count });  // init with correct capacity
      }
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var t = obj.GetType();
      if(t.GetGenericArguments()[0].IsPrimitive) return;
      var _arrayFieldInfo = t.GetField("_array", BindingFlags.NonPublic | BindingFlags.Instance);
      var _array = (Array)_arrayFieldInfo.GetValue(obj);
      var _headFieldInfo = t.GetField("_head", BindingFlags.NonPublic | BindingFlags.Instance);
      _headFieldInfo.SetValue(obj, 0);
      var _tailFieldInfo = t.GetField("_tail", BindingFlags.NonPublic | BindingFlags.Instance);
      _tailFieldInfo.SetValue(obj, _array.Length);
      var _sizeFieldInfo = t.GetField("_size", BindingFlags.NonPublic | BindingFlags.Instance);
      _sizeFieldInfo.SetValue(obj, _array.Length);
      int i = 0;
      foreach (var e in ExtractValues(box.Values, mapper)) {
        _array.SetValue(e, i++);
      }
    }
  }

  [Transformer("60AD45C3-E315-4AD4-8A86-69E6AA59A3BB", 200)]
  internal sealed class QueueTransformer : BoxTransformer<object> {
    public override bool CanTransformType(Type type) {
      return
        type == typeof(Queue);
    }

    protected override void Populate(Box box, object value, Mapper mapper) {
      box.Values = new RepeatedValueBox();
      box.Values.UInts = new RepeatedUIntBox();

      foreach (var e in ((Queue)value)) {
        box.Values.UInts.Values.Add(mapper.GetBoxId(e));
      }
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      return Activator.CreateInstance(type, new object[] { box.Values.UInts.Values.Count });  // init with correct capacity
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var values = box.Values.UInts.Values;
      var q = (Queue)obj;
      for (int i = 0; i < values.Count; ++i) {
        q.Enqueue(mapper.GetObject(values[i]));
      }
    }
  }

  [Transformer("25A7EFC7-00D3-426D-A087-689D699B49F6", 200)]
  internal sealed class StringQueueTransformer : QueueTransformer<string> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      foreach (string val in values) box.UInts.Values.Add(mapper.GetStringId(val));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values.Select(l => mapper.GetString(l));
    }
  }

  [Transformer("AEDA8E1C-BD5B-4D52-B2A0-8DF8444F7905", 200)]
  internal sealed class BoolQueueTransformer : QueueTransformer<bool> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Bools = new RepeatedBoolBox();
      box.Bools.Values.AddRange((IEnumerable<bool>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Bools.Values;
    }
  }

  [Transformer("0ACA47B1-62EA-416E-A8BC-0A5D0C99C6D2", 200)]
  internal sealed class IntQueueTransformer : QueueTransformer<int> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Ints = new RepeatedIntBox();
      box.Ints.Values.AddRange((IEnumerable<int>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Ints.Values;
    }
  }

  [Transformer("644ED81E-0EA8-4813-A076-3C9A1096176A", 200)]
  internal sealed class UnsignedIntQueueTransformer : QueueTransformer<uint> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange((IEnumerable<uint>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values;
    }
  }

  [Transformer("2DCB37FF-560C-4DFE-8078-00BB7A65EAD0", 200)]
  internal sealed class LongQueueTransformer : QueueTransformer<long> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Longs = new RepeatedLongBox();
      box.Longs.Values.AddRange((IEnumerable<long>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Longs.Values;
    }
  }

  [Transformer("A9FE2D4D-073E-45BA-9775-595E9D8B03AE", 200)]
  internal sealed class UnsignedLongQueueTransformer : QueueTransformer<ulong> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.ULongs = new RepeatedULongBox();
      box.ULongs.Values.AddRange((IEnumerable<ulong>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.ULongs.Values;
    }
  }

  [Transformer("AF2D4F4A-9179-4795-BB8B-F3A22E112E4A", 200)]
  internal sealed class FloatQueueTransformer : QueueTransformer<float> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Floats = new RepeatedFloatBox();
      box.Floats.Values.AddRange((IEnumerable<float>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Floats.Values;
    }
  }

  [Transformer("5FE4F361-BFA3-48BE-932C-F382DA38AD21", 200)]
  internal sealed class DoubleQueueTransformer : QueueTransformer<double> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Doubles = new RepeatedDoubleBox();
      box.Doubles.Values.AddRange((IEnumerable<double>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Doubles.Values;
    }
  }

  [Transformer("5DFFB2A9-95A5-4469-B401-C77E02B66AC7", 200)]
  internal sealed class ByteQueueTransformer : QueueTransformer<byte> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange(((IEnumerable<byte>)values).Select(v => (uint)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values;
    }
  }

  [Transformer("16D2A2A6-07B4-4E70-9675-5956C8568241", 200)]
  internal sealed class SByteQueueTransformer : QueueTransformer<sbyte> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.SInts = new RepeatedSIntBox();
      box.SInts.Values.AddRange(((IEnumerable<sbyte>)values).Select(v => (int)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.SInts.Values;
    }
  }

  [Transformer("87565238-8A71-42E9-807F-5C962D295DCC", 200)]
  internal sealed class ShortQueueTransformer : QueueTransformer<short> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Ints = new RepeatedIntBox();
      box.Ints.Values.AddRange(((IEnumerable<short>)values).Select(v => (int)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Ints.Values;
    }
  }

  [Transformer("8C83C1FF-2FBB-4549-AB3A-116E8246E484", 200)]
  internal sealed class UShortQueueTransformer : QueueTransformer<ushort> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange(((IEnumerable<ushort>)values).Select(v => (uint)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values;
    }
  }

  [Transformer("59EAF093-3936-4194-A8E7-E522359FC7C5", 200)]
  internal sealed class CharQueueTransformer : QueueTransformer<char> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange(((IEnumerable<char>)values).Select(v => (uint)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values;
    }
  }

  [Transformer("BFF47262-56C8-4002-8CA4-799D01E428D6", 200)]
  internal sealed class DecimalQueueTransformer : QueueTransformer<decimal> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Ints = new RepeatedIntBox();
      box.Ints.Values.AddRange(((IEnumerable<decimal>)values).SelectMany(decimal.GetBits));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      int[] bits = new int[4];
      int i = 0;
      while (i <= box.Ints.Values.Count - 4) {
        for (int j = 0; j < 4; j++) {
          bits[j] = box.Ints.Values[i++];
        }
        yield return new decimal(bits);
      }
    }
  }

  [Transformer("7A55726C-3138-4A20-B2FA-8F0049306059", 200)]
  internal sealed class ObjectQueueTransformer : QueueTransformer<object> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox repValueBox, Mapper mapper) {
      repValueBox.UInts = new RepeatedUIntBox();
      repValueBox.UInts.Values.AddRange(((IEnumerable<object>)values).Select(mapper.GetBoxId));
    }

    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values.Select(mapper.GetObject);
    }
  }
}
