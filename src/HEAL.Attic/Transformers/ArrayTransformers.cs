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

namespace HEAL.Attic {

  internal abstract class MultiDimStructArrayTransformer<T> : BoxTransformer<object> where T : struct {
    public override bool CanTransformType(Type type) {
      return
        type.IsArray && typeof(T) == type.GetElementType();    // T is struct -> access T[], T[,], T[,,], without boxing
    }

    protected abstract void AddRange(IEnumerable<T> values, RepeatedValueBox repValueBox, Mapper mapper);
    protected abstract IEnumerable<T> ExtractValues(RepeatedValueBox box, Mapper mapper);

    protected override void Populate(Box box, object value, Mapper mapper) {
      var a = (Array)value;

      box.Values = new RepeatedValueBox();
      var arrInfo = new ArrayInfo();
      arrInfo.Rank = a.Rank;
      for (int d = 0; d < a.Rank; d++) {
        arrInfo.Lengths.Add(a.GetLength(d));
        arrInfo.LowerBounds.Add(a.GetLowerBound(d));
      }
      box.Values.ArrayInfoId = mapper.GetArrayInfoId(arrInfo);
      AddRange(new ArrayWrapper<T>(a), box.Values, mapper); // RepeatedField.AddRange expects IEnumerable<T> and is efficient for ICollection
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      var arrInfo = mapper.GetArrayInfo(box.Values.ArrayInfoId);
      var lower = arrInfo.LowerBounds.ToArray();
      var lenghts = arrInfo.Lengths.ToArray();
      return Array.CreateInstance(type.GetElementType(), lenghts, lower);
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var a = (Array)obj;
      var values = ExtractValues(box.Values, mapper);
      if (a.Rank == 1)
        if (values is ICollection collection) collection.CopyTo(a, a.GetLowerBound(0));
        else ArrayWrapper<T>.CopyTo1d(values, (T[])a);
      else if (a.Rank == 2) ArrayWrapper<T>.CopyTo2d(values, (T[,])a);
      else if (a.Rank == 3) ArrayWrapper<T>.CopyTo3d(values, (T[,,])a);
      else ArrayWrapper<T>.CopyTo(values, a);
    }
  }

  internal abstract class MultiDimArrayTransformer<T> : BoxTransformer<object> {
    public override bool CanTransformType(Type type) {
      return
        type.IsArray && typeof(T).IsAssignableFrom(type.GetElementType());
    }

    protected abstract void AddRange(IEnumerable values, RepeatedValueBox repValueBox, Mapper mapper);
    protected abstract IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper);

    protected override void Populate(Box box, object value, Mapper mapper) {
      var a = (Array)value;

      box.Values = new RepeatedValueBox();
      var arrInfo = new ArrayInfo();
      arrInfo.Rank = a.Rank;
      for (int d = 0; d < a.Rank; d++) {
        arrInfo.Lengths.Add(a.GetLength(d));
        arrInfo.LowerBounds.Add(a.GetLowerBound(d));
      }
      box.Values.ArrayInfoId = mapper.GetArrayInfoId(arrInfo);
      AddRange(a, box.Values, mapper);
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      var arrInfoBox = mapper.GetArrayInfo(box.Values.ArrayInfoId);
      var lower = arrInfoBox.LowerBounds.ToArray();
      var lenghts = arrInfoBox.Lengths.ToArray();
      return Array.CreateInstance(type.GetElementType(), lenghts, lower);
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var a = (Array)obj;
      ArrayWrapper<object>.CopyTo(ExtractValues(box.Values, mapper), a);
    }
  }

  [Transformer("C9CFA67B-DF13-4125-B781-98EC8F5E390F", 201)]
  internal sealed class BoolArrayTransformer : MultiDimStructArrayTransformer<bool> {
    protected override void AddRange(IEnumerable<bool> values, RepeatedValueBox box, Mapper mapper) {
      box.Bools = new RepeatedBoolBox();
      box.Bools.Values.AddRange(values);
    }
    protected override IEnumerable<bool> ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Bools.Values;
    }
  }

  [Transformer("BA2E18F6-5C17-40CA-A5B8-5690C5EFE872", 202)]
  internal sealed class IntArrayTransformer : MultiDimStructArrayTransformer<int> {
    protected override void AddRange(IEnumerable<int> values, RepeatedValueBox box, Mapper mapper) {
      box.Ints = new RepeatedIntBox();
      box.Ints.Values.AddRange(values);
    }
    protected override IEnumerable<int> ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Ints.Values;
    }
  }

  [Transformer("EEE7710D-86DE-47E1-887D-BDA2996B141E", 203)]
  internal sealed class UnsignedIntArrayTransformer : MultiDimStructArrayTransformer<uint> {
    protected override void AddRange(IEnumerable<uint> values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange(values);
    }
    protected override IEnumerable<uint> ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values;
    }
  }

  [Transformer("557932AA-F023-477F-AAD0-5098E8B8CD56", 204)]
  internal sealed class LongArrayTransformer : MultiDimStructArrayTransformer<long> {
    protected override void AddRange(IEnumerable<long> values, RepeatedValueBox box, Mapper mapper) {
      box.Longs = new RepeatedLongBox();
      box.Longs.Values.AddRange(values);
    }
    protected override IEnumerable<long> ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Longs.Values;
    }
  }

  [Transformer("CFF20DEE-2A55-4D04-B543-A4C7E0A8F7BF", 205)]
  internal sealed class UnsignedLongArrayTransformer : MultiDimStructArrayTransformer<ulong> {
    protected override void AddRange(IEnumerable<ulong> values, RepeatedValueBox box, Mapper mapper) {
      box.ULongs = new RepeatedULongBox();
      box.ULongs.Values.AddRange(values);
    }
    protected override IEnumerable<ulong> ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.ULongs.Values;
    }
  }

  [Transformer("2A0F766D-FE71-4415-A75F-A32FB8BB9E2D", 206)]
  internal sealed class FloatArrayTransformer : MultiDimStructArrayTransformer<float> {
    protected override void AddRange(IEnumerable<float> values, RepeatedValueBox box, Mapper mapper) {
      box.Floats = new RepeatedFloatBox();
      box.Floats.Values.AddRange(values);
    }
    protected override IEnumerable<float> ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Floats.Values;
    }
  }


  [Transformer("192A8F7D-84C7-44BF-B0CA-AD387A241AAD", 207)]
  internal sealed class DoubleArrayTransformer : MultiDimStructArrayTransformer<double> {
    protected override void AddRange(IEnumerable<double> values, RepeatedValueBox box, Mapper mapper) {
      box.Doubles = new RepeatedDoubleBox();
      box.Doubles.Values.AddRange(values);
    }
    protected override IEnumerable<double> ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Doubles.Values;
    }
  }

  [Transformer("3A35CECE-9953-4C29-A796-A56C02D80A05", 208)]
  internal sealed class ByteArrayTransformer : MultiDimStructArrayTransformer<byte> {
    protected override void AddRange(IEnumerable<byte> values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange(values.Select(v => (uint)v));
    }
    protected override IEnumerable<byte> ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values.Select(i => (byte)i);
    }
  }

  [Transformer("880D4A63-6C77-4F9F-8F7C-2D365F0AE829", 209)]
  internal sealed class SByteArrayTransformer : MultiDimStructArrayTransformer<sbyte> {
    protected override void AddRange(IEnumerable<sbyte> values, RepeatedValueBox box, Mapper mapper) {
      box.SInts = new RepeatedSIntBox();
      box.SInts.Values.AddRange(values.Select(v => (int)v));
    }
    protected override IEnumerable<sbyte> ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.SInts.Values.Select(i => (sbyte)i);
    }
  }

  [Transformer("9786E711-7C1D-4761-BD6B-445793834264", 210)]
  internal sealed class ShortArrayTransformer : MultiDimStructArrayTransformer<short> {
    protected override void AddRange(IEnumerable<short> values, RepeatedValueBox box, Mapper mapper) {
      box.Ints = new RepeatedIntBox();
      box.Ints.Values.AddRange(values.Select(v => (int)v));
    }
    protected override IEnumerable<short> ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.Ints.Values.Select(i => (short)i);
    }
  }

  [Transformer("1AAC2625-356C-40BC-8CB4-15CB3D047EB8", 211)]
  internal sealed class UShortArrayTransformer : MultiDimStructArrayTransformer<ushort> {
    protected override void AddRange(IEnumerable<ushort> values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange(values.Select(v => (uint)v));
    }
    protected override IEnumerable<ushort> ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values.Select(i => (ushort)i);
    }
  }

  [Transformer("12F19098-5D49-4C23-8897-69087F1C146D", 212)]

  internal sealed class CharArrayTransformer : MultiDimStructArrayTransformer<char> {
    protected override void AddRange(IEnumerable<char> values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      box.UInts.Values.AddRange(values.Select(v => (uint)v));
    }
    protected override IEnumerable<char> ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values.Select(i => (char)i);
    }
  }

  [Transformer("C199BE18-6454-4EB7-AE25-DCFCABD54FF4", 213)]

  internal sealed class DecimalArrayTransformer : MultiDimStructArrayTransformer<decimal> {
    protected override void AddRange(IEnumerable<decimal> values, RepeatedValueBox box, Mapper mapper) {
      box.Ints = new RepeatedIntBox();
      box.Ints.Values.AddRange(values.SelectMany(decimal.GetBits));
    }
    protected override IEnumerable<decimal> ExtractValues(RepeatedValueBox box, Mapper mapper) {
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

  [Transformer("D50C4782-7211-4476-B50F-7D3378EE3E53", 200)]
  internal sealed class StringArrayTransformer : MultiDimArrayTransformer<string> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox box, Mapper mapper) {
      box.UInts = new RepeatedUIntBox();
      foreach (string val in values) box.UInts.Values.Add(mapper.GetStringId(val));
    }
    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values.Select(l => mapper.GetString(l));
    }
  }

  [Transformer("05AE4C5D-4D0C-47C7-B6D5-F04230C6F565", 301)]
  internal sealed class ObjectArrayTransformer : MultiDimArrayTransformer<object> {
    protected override void AddRange(IEnumerable values, RepeatedValueBox repValueBox, Mapper mapper) {
      repValueBox.UInts = new RepeatedUIntBox();
      foreach (object val in values) repValueBox.UInts.Values.Add(mapper.GetBoxId(val));
    }

    protected override IEnumerable ExtractValues(RepeatedValueBox box, Mapper mapper) {
      return box.UInts.Values.Select(mapper.GetObject);
    }
  }

  #region wrapper for array enumeration


  internal class ArrayWrapper<T> : IEnumerable<T>, ICollection {

    internal class TwoDimArrayEnumerator<A> : IEnumerator<A> {
      private A[,] a;
      private int i, j;
      private int u0, u1, l1;

      public TwoDimArrayEnumerator(A[,] a) {
        this.a = a;
        Reset();
      }

      public A Current {
        get => a[i, j];
        set => a[i, j] = value; // use with care!
      }

      object IEnumerator.Current => Current;

      public void Dispose() {
        // ignore
      }

      public bool MoveNext() {
        j++;
        if (j > u1) {
          j = l1;
          i++;
        }
        return i <= u0;
      }

      public void Reset() {
        i = a.GetLowerBound(0);
        j = a.GetLowerBound(1);
        l1 = j;
        u0 = a.GetUpperBound(0);
        u1 = a.GetUpperBound(1);
        j--; // simplify MoveNext
      }
    }
    internal class ThreeDimArrayEnumerator<A> : IEnumerator<A> {
      private A[,,] a;
      private int i, j, k;
      private int u0, u1, u2, l1, l2;

      public ThreeDimArrayEnumerator(A[,,] a) {
        this.a = a;
        Reset();
      }

      public A Current {
        get => a[i, j, k];
        set => a[i, j, k] = value; // use with care!
      }

      object IEnumerator.Current => Current;

      public void Dispose() {
        // ignore
      }

      public bool MoveNext() {
        k++;
        if (k >= u2) {
          k = l2;
          j++;
          if (j >= u1) {
            j = l1;
            i++;
          }
        }
        return i <= u0;
      }

      public void Reset() {
        i = a.GetLowerBound(0);
        j = a.GetLowerBound(1);
        k = a.GetLowerBound(2);
        l1 = j;
        l2 = k;
        u0 = a.GetUpperBound(0);
        u1 = a.GetUpperBound(1);
        u2 = a.GetUpperBound(2);
        k--; // simplify MoveNext
      }
    }
    internal class MultiDimArrayEnumerator<A> : IEnumerator<A> {
      private Array a;
      private int rank;
      private int[] idx;
      private int[] lower, length, upper;
      private A current;

      public MultiDimArrayEnumerator(Array a) {
        this.a = a;
        Reset();
      }

      public A Current {
        get => current;
        set => a.SetValue(value, idx); // use with care!
      }

      object IEnumerator.Current => current;

      public void Dispose() {
        // ignore
      }

      public bool MoveNext() {
        if (idx[0] >= upper[0]) return false;
        current = (A)a.GetValue(idx);
        idx[rank - 1]++;
        for (int i = rank - 1; i > 0; i--) {
          if (idx[i] >= upper[i]) {
            idx[i] = lower[i];
            idx[i - 1]++;
          } else {
            break;
          }
        }
        return true;
      }

      public void Reset() {
        rank = a.Rank;
        lower = new int[rank];
        length = new int[rank];
        upper = new int[rank];
        idx = new int[rank];
        for (int i = 0; i < rank; i++) {
          lower[i] = a.GetLowerBound(i);
          length[i] = a.GetLength(i);
          upper[i] = lower[i] + length[i];
          idx[i] = lower[i];
        }
      }
    }

    private readonly Array a;

    public ArrayWrapper(Array a) {
      this.a = a;
    }

    public int Count => a.Length;

    public bool IsSynchronized => false;

    public object SyncRoot => a;

    public void CopyTo(Array array, int index) {
      Buffer.BlockCopy(a, 0, array, index, Buffer.ByteLength(a));
    }

    public static void CopyTo1d(IEnumerable<T> src, T[] dest) {
      var v = src.GetEnumerator();
      v.MoveNext();
      for (int i = dest.GetLowerBound(0); i < dest.GetLength(0) + dest.GetLowerBound(0); i++) {
        dest[i] = v.Current;
        v.MoveNext();
      }
    }

    public static void CopyTo2d(IEnumerable<T> src, T[,] dest) {
      var v = src.GetEnumerator();
      v.MoveNext();
      for (int i = dest.GetLowerBound(0); i < dest.GetLength(0) + dest.GetLowerBound(0); i++) {
        for (int j = dest.GetLowerBound(1); j < dest.GetLength(1) + dest.GetLowerBound(1); j++) {
          dest[i, j] = v.Current;
          v.MoveNext();
        }
      }
    }

    public static void CopyTo3d(IEnumerable<T> src, T[,,] dest) {
      var v = src.GetEnumerator();
      v.MoveNext();
      for (int i = dest.GetLowerBound(0); i < dest.GetLength(0) + dest.GetLowerBound(0); i++) {
        for (int j = dest.GetLowerBound(1); j < dest.GetLength(1) + dest.GetLowerBound(1); j++) {
          for (int k = dest.GetLowerBound(2); k < dest.GetLength(2) + dest.GetLowerBound(2); k++) {
            dest[i, j, k] = v.Current;
            v.MoveNext();
          }
        }
      }
    }
    public static void CopyTo(IEnumerable src, Array dest) {
      var e = new MultiDimArrayEnumerator<object>(dest);
      var v = src.GetEnumerator();
      while (v.MoveNext()) {
        e.Current = v.Current;
        e.MoveNext();
      }
    }


    public IEnumerator<T> GetEnumerator() {
      if (a.Rank == 1) return ((T[])a).AsEnumerable().GetEnumerator();
      else if (a.Rank == 2) return new TwoDimArrayEnumerator<T>((T[,])a);
      else if (a.Rank == 3) return new ThreeDimArrayEnumerator<T>((T[,,])a);
      return new MultiDimArrayEnumerator<T>(a);
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }

  #endregion
}
