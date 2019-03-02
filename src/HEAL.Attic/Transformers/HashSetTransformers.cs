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

namespace HEAL.Attic {
  internal abstract class HashSetTransformer<T> : BoxTransformer<object> {
    public override bool CanTransformType(Type type) {
      return
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>) && typeof(T).IsAssignableFrom(type.GetGenericArguments()[0]);
    }

    protected abstract void AddRange(IEnumerable values, RepeatedValueBox repValueBox, Mapper mapper);
    protected abstract IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper);

    protected override void Populate(Box box, object value, Mapper mapper) {
      box.Values = new RepeatedValueBox();
      var type = value.GetType();
      var propertyInfo = type.GetProperty("Comparer");
      var comparer = propertyInfo.GetValue(value);

      var comparerType = comparer.GetType();
      if (StorableTypeAttribute.IsStorableType(comparerType))
        box.Values.ComparerId = mapper.GetBoxId(comparer);
      else if (comparerType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).Any())
        throw new NotSupportedException("Cannot serialize non-storable equality comparers with fields");
      else
        box.Values.ComparerTypeId = mapper.GetTypeMessageId(comparerType, transformer: null); // there is no transformer for the comparer type
      AddRange((IEnumerable)value, box.Values, mapper);
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      object comparer;
      if (box.Values.ComparerId != 0) {
        comparer = mapper.GetObject(box.Values.ComparerId);
      } else {
        comparer = Activator.CreateInstance(mapper.TypeMessageToType(mapper.GetTypeMessage(box.Values.ComparerTypeId)));
      }
      if (type.GetGenericArguments()[0].IsPrimitive) {
        return Activator.CreateInstance(type, new object[] {
          ExtractValues(box.Values, mapper), comparer
        });
      } else {
        return Activator.CreateInstance(type, new object[] { box.Values.UInts.Values.Count, comparer });  // init with correct capacity
      }
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var t = obj.GetType();
      if (t.GetGenericArguments()[0].IsPrimitive) return;
      var addMethodInfo = t.GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
      object[] p = new object[1];
      foreach (var val in ExtractValues(box.Values, mapper)) {
        p[0] = val;
        addMethodInfo.Invoke(obj, p);
      }
    }
  }

  [Transformer("81E4E0D1-3EF2-4D38-A6AE-CC2F89EDCDB0", 200)]
  internal sealed class StringHashSetTransformer : HashSetTransformer<string> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      foreach (string val in values) box.UInts.Values.Add(mapper.GetStringId(val));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values.Select(l => mapper.GetString(l));
    }
  }

  [Transformer("3E5AFA38-4B76-4E37-A8BE-7A83C073F3AA", 200)]
  internal sealed class BoolHashSetTransformer : HashSetTransformer<bool> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Bools = new RepeatedBoolBox();
      box.Bools.Values.AddRange((IEnumerable<bool>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Bools.Values;
    }
  }

  [Transformer("D12D09F0-2E9F-402E-BC97-BA858AB1F8FA", 200)]
  internal sealed class IntHashSetTransformer : HashSetTransformer<int> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Ints = new RepeatedIntBox();
      box.Ints.Values.AddRange((IEnumerable<int>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Ints.Values;
    }
  }

  [Transformer("86F16004-D591-40A5-9B57-683A2B2D31D4", 200)]
  internal sealed class UnsignedIntHashSetTransformer : HashSetTransformer<uint> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange((IEnumerable<uint>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values;
    }
  }

  [Transformer("CBC3CF4B-EE9E-4D41-88F1-53B0D5701EA5", 200)]
  internal sealed class LongHashSetTransformer : HashSetTransformer<long> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Longs = new RepeatedLongBox();
      box.Longs.Values.AddRange((IEnumerable<long>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Longs.Values;
    }
  }

  [Transformer("30F54768-B1FE-4B84-BB4A-66FD07897B61", 200)]
  internal sealed class UnsignedLongHashSetTransformer : HashSetTransformer<ulong> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.ULongs = new RepeatedULongBox();
      box.ULongs.Values.AddRange((IEnumerable<ulong>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.ULongs.Values;
    }
  }

  [Transformer("9FC14EFC-D74C-49AF-B2B1-0905C57C2D0A", 200)]
  internal sealed class FloatHashSetTransformer : HashSetTransformer<float> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Floats = new RepeatedFloatBox();
      box.Floats.Values.AddRange((IEnumerable<float>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Floats.Values;
    }
  }

  [Transformer("03EDA1D8-4BB9-48D0-8915-DC0CAAEC07B8", 200)]
  internal sealed class DoubleHashSetTransformer : HashSetTransformer<double> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Doubles = new RepeatedDoubleBox();
      box.Doubles.Values.AddRange((IEnumerable<double>)values);
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Doubles.Values;
    }
  }

  [Transformer("A463637A-CFBB-49C7-9F54-79FFB0C69013", 200)]
  internal sealed class ByteHashSetTransformer : HashSetTransformer<byte> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange(((IEnumerable<byte>)values).Select(v => (uint)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values;
    }
  }

  [Transformer("E1562FBE-EF22-4BEB-B4A7-B7BAE98C00E0", 200)]
  internal sealed class SByteHashSetTransformer : HashSetTransformer<sbyte> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.SInts = new RepeatedSIntBox();
      box.SInts.Values.AddRange(((IEnumerable<int>)values).Select(v => (int)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.SInts.Values;
    }
  }

  [Transformer("2D657E39-7E97-4AAE-B25B-632AAC26C2AC", 200)]
  internal sealed class ShortHashSetTransformer : HashSetTransformer<short> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.Ints = new RepeatedIntBox();
      box.Ints.Values.AddRange(((IEnumerable<short>)values).Select(v => (int)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Ints.Values;
    }
  }

  [Transformer("62B6A3A7-C31E-4C78-B6BF-BFA477D8C299", 200)]
  internal sealed class UShortHashSetTransformer : HashSetTransformer<ushort> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange(((IEnumerable<ushort>)values).Select(v => (uint)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values;
    }
  }

  [Transformer("8886CAB5-AC7F-40C4-A939-84DCA580FB96", 200)]
  internal sealed class CharHashSetTransformer : HashSetTransformer<char> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange(((IEnumerable<char>)values).Select(v => (uint)v));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values;
    }
  }

  [Transformer("343CFC09-8BE6-4219-890C-0D42AE6C5B68", 200)]
  internal sealed class DecimalHashSetTransformer : HashSetTransformer<decimal> {
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

  [Transformer("A8799BD8-43E5-484F-BCA0-C89A3BC1A89A", 200)]
  internal sealed class ObjectHashSetTransformer : HashSetTransformer<object> {
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
