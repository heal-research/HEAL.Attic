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

namespace HEAL.Attic {
  internal abstract class StackTransformer<T> : BoxTransformer<object> {
    public override bool CanTransformType(Type type) {
      return
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Stack<>) && typeof(T).IsAssignableFrom(type.GetGenericArguments()[0]);
    }

    protected abstract void AddRange(IEnumerable values, RepeatedValueBox repValueBox, Mapper mapper);
    protected abstract IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper);

    protected override void Populate(Box box, object value, Mapper mapper) {
      box.Values = new RepeatedValueBox();
      AddRange((IEnumerable)value, box.Values, mapper);
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      if (type.GetGenericArguments()[0].IsPrimitive) {
        var s = Activator.CreateInstance(type, new object[] {
          ExtractValues(box.Values, mapper)
        });
        var fi = type.GetField("_array", BindingFlags.NonPublic | BindingFlags.Instance);
        var _array = (Array)fi.GetValue(s);
        Array.Reverse(_array);
        return s;
      } else {
        return Activator.CreateInstance(type, new object[] { box.Values.UInts.Values.Count });  // init with correct capacity
      }
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var t = obj.GetType();
      if (t.GetGenericArguments()[0].IsPrimitive) return;
      var arrfi = t.GetField("_array", BindingFlags.NonPublic | BindingFlags.Instance);
      var _array = (Array)arrfi.GetValue(obj);
      var sizeFi = t.GetField("_size", BindingFlags.NonPublic | BindingFlags.Instance);
      sizeFi.SetValue(obj, _array.Length);
      int i = _array.Length;
      foreach (var val in ExtractValues(box.Values, mapper)) {
        i--;
        _array.SetValue(val, i);
      }
    }
  }

  [Transformer("58A7B527-C041-492F-8131-6F5667777CD4", 200)]
  internal sealed class StackTransformer : BoxTransformer<object> {
    public override bool CanTransformType(Type type) {
      return
        type == typeof(Stack);
    }

    protected override void Populate(Box box, object value, Mapper mapper) {
      box.Values = new RepeatedValueBox();
      box.Values.UInts = new RepeatedUIntBox();

      foreach (var e in ((Stack)value)) {
        box.Values.UInts.Values.Add(mapper.GetBoxId(e));
      }
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      return Activator.CreateInstance(type, new object[] { box.Values.UInts.Values.Count });  // init with correct capacity
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var values = box.Values.UInts.Values;
      var s = (Stack)obj;
      for (int i = values.Count - 1; i >= 0; --i) {
        s.Push(mapper.GetObject(values[i]));
      }
    }
  }

  [Transformer("5D6B5405-BC2E-454A-973B-03FCAC6917ED", 200)]
  internal sealed class StringStackTransformer : StackTransformer<string> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      foreach (string val in values) box.UInts.Values.Add(mapper.GetStringId(val));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values.Select(l => mapper.GetString(l));
    }
  }

  [Transformer("E587A15D-426B-4A0C-968A-EC8F5470EACF", 200)]
  internal sealed class BoolStackTransformer : StackTransformer<bool> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Bools = new RepeatedBoolBox();
      box.Bools.Values.AddRange((IEnumerable<bool>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Bools.Values;
    }
  }

  [Transformer("414CA897-0A33-4DD2-8661-3A31800454B8", 200)]
  internal sealed class IntStackTransformer : StackTransformer<int> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Ints = new RepeatedIntBox();
      box.Ints.Values.AddRange((IEnumerable<int>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Ints.Values;
    }
  }

  [Transformer("2F019915-541F-4923-A0AF-52421B522BC6", 200)]
  internal sealed class UnsignedIntStackTransformer : StackTransformer<uint> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange((IEnumerable<uint>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values;
    }
  }

  [Transformer("1ADD4F64-8981-4456-9374-B73F23771397", 200)]
  internal sealed class LongStackTransformer : StackTransformer<long> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Longs = new RepeatedLongBox();
      box.Longs.Values.AddRange((IEnumerable<long>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Longs.Values;
    }
  }

  [Transformer("11C9DA82-5D39-4139-AC12-791E68DA9AAD", 200)]
  internal sealed class UnsignedLongStackTransformer : StackTransformer<ulong> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.ULongs = new RepeatedULongBox();
      box.ULongs.Values.AddRange((IEnumerable<ulong>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.ULongs.Values;
    }
  }

  [Transformer("C806DA65-0DBD-4E2F-A2C0-49244EFEE4AD", 200)]
  internal sealed class FloatStackTransformer : StackTransformer<float> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Floats = new RepeatedFloatBox();
      box.Floats.Values.AddRange((IEnumerable<float>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Floats.Values;
    }
  }

  [Transformer("CD66A60C-2D3E-4D8A-B24D-AFB01B10B397", 200)]
  internal sealed class DoubleStackTransformer : StackTransformer<double> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Doubles = new RepeatedDoubleBox();
      box.Doubles.Values.AddRange((IEnumerable<double>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Doubles.Values;
    }
  }

  [Transformer("8E3C741B-5115-4760-80A6-B79642EE5D91", 200)]
  internal sealed class ByteStackTransformer : StackTransformer<byte> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange(((IEnumerable<byte>)values).Select(v => (uint)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values;
    }
  }

  [Transformer("F362082C-65CA-48AA-BB2C-83388AE02227", 200)]
  internal sealed class SByteStackTransformer : StackTransformer<sbyte> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.SInts = new RepeatedSIntBox();
      box.SInts.Values.AddRange(((IEnumerable<int>)values).Select(v => (int)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.SInts.Values;
    }
  }

  [Transformer("41303582-CB4F-4898-9EAB-31ABC108A955", 200)]
  internal sealed class ShortStackTransformer : StackTransformer<short> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Ints = new RepeatedIntBox();
      box.Ints.Values.AddRange(((IEnumerable<short>)values).Select(v => (int)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Ints.Values;
    }
  }

  [Transformer("C21CD2E3-0160-40CC-A755-018D07ECB58A", 200)]
  internal sealed class UShortStackTransformer : StackTransformer<ushort> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange(((IEnumerable<ushort>)values).Select(v => (uint)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values;
    }
  }

  [Transformer("3705ADBC-C352-49D9-9197-BECF92172C65", 200)]
  internal sealed class CharStackTransformer : StackTransformer<char> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange(((IEnumerable<char>)values).Select(v => (uint)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values;
    }
  }

  [Transformer("69662FC1-0274-40C3-A3A0-84177F5D7AD2", 200)]
  internal sealed class DecimalStackTransformer : StackTransformer<decimal> {
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

  [Transformer("64688EEB-9A19-423F-9999-0C1671A90AC2", 200)]
  internal sealed class ObjectStackTransformer : StackTransformer<object> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox repValueBox, Mapper mapper) {
      repValueBox.UInts = new RepeatedUIntBox();
      foreach (var v in values) {
        repValueBox.UInts.Values.Add(mapper.GetBoxId(v));
      }
    }

    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values.Select(mapper.GetObject);
    }
  }
}
