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
using HEAL.Attic;

namespace HEAL.Attic {
  internal abstract class ListTransformer<T> : BoxTransformer<object> {
    public override bool CanTransformType(Type type) {
      return
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) && typeof(T).IsAssignableFrom(type.GetGenericArguments()[0]);
    }

    protected abstract void AddRange(IEnumerable values, RepeatedValueBox repValueBox, Mapper mapper);
    protected abstract IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper);

    protected override void Populate(Box box, object value, Mapper mapper) {
      box.Values = new RepeatedValueBox();
      AddRange((IEnumerable)value, box.Values, mapper);
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      if (type.GetGenericArguments()[0].IsPrimitive) {
        return Activator.CreateInstance(type, new object[] { ExtractValues(box.Values, mapper) });
      } else {
        return Activator.CreateInstance(type, new object[] { box.Values.UInts.Values.Count });  // init with correct capacity
      }
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var l = (IList)obj;
      if (l.GetType().GetGenericArguments()[0].IsPrimitive) return;
      foreach (var v in ExtractValues(box.Values, mapper)) l.Add(v);
    }
  }

  [Transformer("15474F9E-8AA9-4C63-B385-4D2BACAF3850", 200)]
  internal sealed class StringListTransformer : ListTransformer<string> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      foreach (string val in values) box.UInts.Values.Add(mapper.GetStringId(val));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values.Select(l => mapper.GetString(l));
    }
  }

  [Transformer("D4087458-A314-471A-965F-9EF8170222F5", 200)]

  internal sealed class BoolListTransformer : ListTransformer<bool> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Bools = new RepeatedBoolBox();
      box.Bools.Values.AddRange((IEnumerable<bool>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Bools.Values;
    }
  }

  [Transformer("822A6420-C50F-4DEA-9A4C-D2C1E9E353F1", 200)]

  internal sealed class IntListTransformer : ListTransformer<int> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Ints = new RepeatedIntBox();
      box.Ints.Values.AddRange((IEnumerable<int>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Ints.Values;
    }
  }

  [Transformer("5A72B879-7934-4D58-85CF-CDBC43B2507D", 200)]

  internal sealed class UnsignedIntListTransformer : ListTransformer<uint> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange((IEnumerable<uint>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values;
    }
  }

  [Transformer("2A7AAE3A-6047-4D25-BFF3-1B8A74A4384F", 200)]

  internal sealed class LongListTransformer : ListTransformer<long> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Longs = new RepeatedLongBox();
      box.Longs.Values.AddRange((IEnumerable<long>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Longs.Values;
    }
  }

  [Transformer("CFC89684-417F-4AC6-9C67-96A267867F2E", 200)]

  internal sealed class UnsignedLongListTransformer : ListTransformer<ulong> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.ULongs = new RepeatedULongBox();
      box.ULongs.Values.AddRange((IEnumerable<ulong>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.ULongs.Values;
    }
  }

  [Transformer("B052F3B4-EA8A-4D6B-8197-2EC982FF1A89", 200)]

  internal sealed class FloatListTransformer : ListTransformer<float> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Floats = new RepeatedFloatBox();
      box.Floats.Values.AddRange((IEnumerable<float>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Floats.Values;
    }
  }

  [Transformer("C1CDA825-93F6-49A8-A638-0607ACCA4383", 200)]

  internal sealed class DoubleListTransformer : ListTransformer<double> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Doubles = new RepeatedDoubleBox();
      box.Doubles.Values.AddRange((IEnumerable<double>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Doubles.Values;
    }
  }

  [Transformer("503FED10-5C8E-40A5-9A03-015282AE067D", 200)]

  internal sealed class ByteListTransformer : ListTransformer<byte> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange(((IEnumerable<byte>)values).Select(v => (uint)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values;
    }
  }

  [Transformer("E56D6436-D8F2-421E-8460-15B34E3A92A4", 200)]

  internal sealed class SByteListTransformer : ListTransformer<sbyte> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.SInts = new RepeatedSIntBox();
      box.SInts.Values.AddRange(((IEnumerable<sbyte>)values).Select(v => (int)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.SInts.Values;
    }
  }

  [Transformer("8C2B7D7C-2C79-4802-8F7F-4296C9B240DD", 200)]

  internal sealed class ShortListTransformer : ListTransformer<short> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Ints = new RepeatedIntBox();
      box.Ints.Values.AddRange(((IEnumerable<short>)values).Select(v => (int)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Ints.Values;
    }
  }

  [Transformer("933F7F05-E75F-4FE0-9988-69F206E0C477", 200)]

  internal sealed class UShortListTransformer : ListTransformer<ushort> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange(((IEnumerable<ushort>)values).Select(v => (uint)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values;
    }
  }

  [Transformer("270A5877-CC29-473D-81BF-2B42D4F938B7", 200)]

  internal sealed class CharListTransformer : ListTransformer<char> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange(((IEnumerable<char>)values).Select(v => (uint)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values;
    }
  }

  [Transformer("5E6B5B8F-367E-410D-BC75-13E200B16BD0", 200)]

  internal sealed class DecimalListTransformer : ListTransformer<decimal> {
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

  [Transformer("4D09074F-1AF7-4E56-AD49-EB5F33F17008", 200)]

  internal sealed class ObjectListTransformer : ListTransformer<object> {
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
