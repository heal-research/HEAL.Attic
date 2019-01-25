#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Google.Protobuf;

namespace HEAL.Attic {
  [StorableType("8c7e99f5-092f-4cef-8b72-8afec1d10236")]
  public abstract class BoxTransformer<T> : Transformer {
    public override bool CanTransformType(Type type) {
      return type == typeof(T);
    }

    public override Box ToBox(object o, Mapper mapper) {
      var box = new Box {
        TransformerId = mapper.GetTransformerId(this),
        TypeBoxId = mapper.GetBoxId(o.GetType())
      };
      Populate(box, (T)o, mapper);
      return box;
    }

    public override object ToObject(Box box, Mapper mapper) {
      var type = (Type)mapper.GetObject(box.TypeBoxId);
      return type == null ? default(T) : Extract(box, type, mapper);
    }

    protected abstract void Populate(Box box, T value, Mapper mapper);
    protected abstract T Extract(Box box, Type type, Mapper mapper);
  }

  [Transformer("854156DA-2A37-450F-92ED-355FBBD8D131", 50)]
  [StorableType("D48991BB-1DDA-4D64-A2E6-5B5608F85F2A")]
  internal sealed class TypeTransformer : Transformer {
    public override bool CanTransformType(Type type) {
      return typeof(Type).IsAssignableFrom(type);
    }

    public override Box ToBox(object o, Mapper mapper) {
      var box = new Box { TransformerId = mapper.GetTransformerId(this) };
      Populate(box, o, mapper);
      return box;
    }

    private void Populate(Box box, object value, Mapper mapper) {
      var type = (Type)value;

      if (type.IsGenericType) {
        box.TypeId = mapper.GetTypeId(type.GetGenericTypeDefinition());
        box.GenericTypeBoxIds.AddRange(type.GetGenericArguments().Select(mapper.GetBoxId));
      } else if (type.IsArray) {
        box.TypeId = mapper.GetTypeId(typeof(Array));
        box.GenericTypeBoxIds.Add(mapper.GetBoxId(type.GetElementType()));
      } else {
        box.TypeId = mapper.GetTypeId(type);
      }
    }

    public override object ToObject(Box box, Mapper mapper) {
      return mapper.TryGetType(box.TypeId, out Type type) ? Extract(box, type, mapper) : null;
    }

    private object Extract(Box box, Type type, Mapper mapper) {
      if (type.IsGenericType) {
        var genericArgumentTypes = box.GenericTypeBoxIds.Select(x => (Type)mapper.GetObject(x)).ToArray();
        return genericArgumentTypes.Any(x => x == null) ? null : type.MakeGenericType(genericArgumentTypes);
      } else if (type == typeof(Array)) {
        var arrayType = (Type)mapper.GetObject(box.GenericTypeBoxIds[0]);
        return arrayType?.MakeArrayType();
      } else {
        return type;
      }
    }
  }

  [Transformer("4C610596-5234-4C49-998E-30007D64492E", 100)]
  [StorableType("69870239-00B7-4C9D-AF0B-14208069FAC8")]
  internal sealed class StringBoxTransformer : BoxTransformer<string> {
    protected override void Populate(Box box, string value, Mapper mapper) { box.UInt = mapper.GetStringId(value); }
    protected override string Extract(Box box, Type type, Mapper mapper) { return mapper.GetString(box.UInt); }
  }

  [Transformer("58E69402-2533-426A-B9B5-9F2EB5241560", 101)]
  [StorableType("667182BB-D2D5-46C6-97CB-593CE1B19CBC")]
  internal sealed class BoolBoxTransformer : BoxTransformer<bool> {
    protected override void Populate(Box box, bool value, Mapper mapper) { box.Bool = value; }
    protected override bool Extract(Box box, Type type, Mapper mapper) { return box.Bool; }
  }

  [Transformer("D78F3391-3CAE-4376-9348-7FB38A4DE0EB", 102)]
  [StorableType("1E75D1D8-3FAD-4D68-86BB-95DE981FDDD2")]
  internal sealed class IntBoxTransformer : BoxTransformer<int> {
    protected override void Populate(Box box, int value, Mapper mapper) { box.Int = value; }
    protected override int Extract(Box box, Type type, Mapper mapper) { return box.Int; }
  }

  [Transformer("25881263-F452-492E-9FD1-24E1938B048B", 103)]
  [StorableType("7742A4A3-31B4-4449-9657-0D9F50F2382F")]
  internal sealed class UIntBoxTransformer : BoxTransformer<uint> {
    protected override void Populate(Box box, uint value, Mapper mapper) { box.UInt = value; }
    protected override uint Extract(Box box, Type type, Mapper mapper) { return box.UInt; }
  }

  [Transformer("F4175165-382B-4B03-921E-5F923510FB1E", 104)]
  [StorableType("985427E0-E4F0-4182-8D30-63DA7FB69735")]
  internal sealed class LongBoxTransformer : BoxTransformer<long> {
    protected override void Populate(Box box, long value, Mapper mapper) { box.Long = value; }
    protected override long Extract(Box box, Type type, Mapper mapper) { return box.Long; }
  }

  [Transformer("E8F63973-3C0C-4FA9-B068-40EF4463B30B", 105)]
  [StorableType("C9F95D84-BCDC-498E-A9AE-7187E483BBBA")]
  internal sealed class ULongBoxTransformer : BoxTransformer<ulong> {
    protected override void Populate(Box box, ulong value, Mapper mapper) { box.ULong = value; }
    protected override ulong Extract(Box box, Type type, Mapper mapper) { return box.ULong; }
  }

  [Transformer("15489146-EA11-4B90-8020-AF5C10A2531C", 106)]
  [StorableType("AF1C5176-859C-423B-A2BF-2AEA4D0792C7")]
  internal sealed class FloatBoxTransformer : BoxTransformer<float> {
    protected override void Populate(Box box, float value, Mapper mapper) { box.Float = value; }
    protected override float Extract(Box box, Type type, Mapper mapper) { return box.Float; }
  }

  [Transformer("91FD51F3-9C47-4944-AC85-273ED0561E87", 107)]
  [StorableType("4AB25AE9-2600-4041-8F31-C02CCE9A3BEC")]
  internal sealed class DoubleBoxTransformer : BoxTransformer<double> {
    protected override void Populate(Box box, double value, Mapper mapper) { box.Double = value; }
    protected override double Extract(Box box, Type type, Mapper mapper) { return box.Double; }
  }

  [Transformer("BCB087EA-E477-47EB-9BCE-8C64BAC2F288", 108)]
  [StorableType("341C2F99-8849-408E-99AA-7700FE0FB789")]
  internal sealed class ByteBoxTransformer : BoxTransformer<byte> {
    protected override void Populate(Box box, byte value, Mapper mapper) { box.UInt = value; }
    protected override byte Extract(Box box, Type type, Mapper mapper) { return (byte)box.UInt; }
  }

  [Transformer("B90F61D9-75D0-4CAC-AF93-B8C6AB68F642", 109)]
  [StorableType("FEC52DD5-A422-45C4-995F-09F3B0DEC13F")]
  internal sealed class SByteBoxTransformer : BoxTransformer<sbyte> {
    protected override void Populate(Box box, sbyte value, Mapper mapper) { box.Int = value; }
    protected override sbyte Extract(Box box, Type type, Mapper mapper) { return (sbyte)box.Int; }
  }

  [Transformer("95EB44A4-EADD-4DA9-B60F-3262FAD6134B", 110)]
  [StorableType("48744968-15F2-4B22-AAF2-6C0910239384")]
  internal sealed class ShortBoxTransformer : BoxTransformer<short> {
    protected override void Populate(Box box, short value, Mapper mapper) { box.Int = value; }
    protected override short Extract(Box box, Type type, Mapper mapper) { return (short)box.Int; }
  }

  [Transformer("E3A33614-9120-400E-BAD9-2594F6804DA8", 111)]
  [StorableType("DCF05BA3-3C57-4DBB-96B4-B2CE31CA60C5")]
  internal sealed class UShortBoxTransformer : BoxTransformer<ushort> {
    protected override void Populate(Box box, ushort value, Mapper mapper) { box.Int = value; }
    protected override ushort Extract(Box box, Type type, Mapper mapper) { return (ushort)box.Int; }
  }

  [Transformer("C64EA534-E2E1-48F0-86C5-648AA02117BC", 112)]
  [StorableType("142980B3-A251-48D2-BF02-A66C483D6385")]
  internal sealed class CharBoxTransformer : BoxTransformer<char> {
    protected override void Populate(Box box, char value, Mapper mapper) { box.UInt = value; }
    protected override char Extract(Box box, Type type, Mapper mapper) { return (char)box.UInt; }
  }


  [StorableType("26CA3AAF-4266-49CE-B101-568326504CA2")]
  internal abstract class MultiDimValueArrayBoxTransformer<T> : BoxTransformer<object> {
    public override bool CanTransformType(Type type) {
      return
        type.IsArray && type.GetElementType().IsValueType && type.GetElementType() == typeof(T);
    }

    protected abstract void AddValueToBox(Box box, T val, Mapper mapper);
    protected abstract IEnumerable<T> ExtractValues(Box box, Mapper mapper);

    protected override void Populate(Box box, object value, Mapper mapper) {
      var a = (Array)value;
      box.ArrayMetaInfoBox = new ArrayMetaInfoBox();
      box.ArrayMetaInfoBox.Rank = a.Rank;
      for (int d = 0; d < a.Rank; d++) {
        box.ArrayMetaInfoBox.Lengths.Add(a.GetLength(d));
        box.ArrayMetaInfoBox.LowerBounds.Add(a.GetLowerBound(d));
      }
      if (a.Rank == 1)
        PopulateWithValues(box, (T[])a, mapper);
      else if (a.Rank == 2)
        PopulateWithValues(box, (T[,])a, mapper);
      else if (a.Rank == 3)
        PopulateWithValues(box, (T[,,])a, mapper);
      else
        PopulateWithValues(box, a, mapper);
    }

    private void PopulateWithValues(Box box, T[] value, Mapper mapper) {
      var lb = value.GetLowerBound(0);
      for (int i = 0; i < value.Length; i++) {
        AddValueToBox(box, value[i + lb], mapper);
      }
    }
    private void PopulateWithValues(Box box, T[,] value, Mapper mapper) {
      var lb0 = value.GetLowerBound(0);
      var lb1 = value.GetLowerBound(1);
      for (int i = 0; i < value.GetLength(0); i++) {
        for (int j = 0; j < value.GetLength(1); j++) {
          AddValueToBox(box, value[i + lb0, j + lb1], mapper);
        }
      }
    }
    private void PopulateWithValues(Box box, T[,,] value, Mapper mapper) {
      var lb0 = value.GetLowerBound(0);
      var lb1 = value.GetLowerBound(1);
      var lb2 = value.GetLowerBound(2);
      for (int i = 0; i < value.GetLength(0); i++) {
        for (int j = 0; j < value.GetLength(1); j++) {
          for (int k = 0; k < value.GetLength(2); k++) {
            AddValueToBox(box, value[i + lb0, j + lb1, k + lb2], mapper);
          }
        }
      }
    }

    private void PopulateWithValues(Box box, Array value, Mapper mapper) {
      var rank = value.Rank;
      var lowerBounds = new int[rank];
      var lengths = new int[rank];
      for (int d = 0; d < rank; d++) {
        lowerBounds[d] = value.GetLowerBound(d);
        lengths[d] = value.GetLength(d);
      }

      int[] positions = (int[])lowerBounds.Clone();
      while (positions[0] < lengths[0] + lowerBounds[0]) {
        AddValueToBox(box, (T)value.GetValue(positions), mapper);
        positions[rank - 1] += 1;
        for (int i = rank - 1; i > 0; i--) {
          if (positions[i] >= lowerBounds[i] + lengths[i]) {
            positions[i] = lowerBounds[i];
            positions[i - 1] += 1;
          } else {
            break;
          }
        }
      }
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      var rank = box.ArrayMetaInfoBox.Rank;
      var lenghts = new int[rank];
      var lowerBounds = new int[rank];
      for (int d = 0; d < rank; d++) {
        lenghts[d] = box.ArrayMetaInfoBox.Lengths[d];
        lowerBounds[d] = box.ArrayMetaInfoBox.LowerBounds[d];
      }

      var arr = Array.CreateInstance(typeof(T), lenghts, lowerBounds);

      PopulateArray(arr, lenghts, lowerBounds, ExtractValues(box, mapper));
      return arr;
    }

    private void PopulateArray(Array arr, int[] lengths, int[] lowerBounds, IEnumerable<T> values) {
      var rank = arr.Rank;
      int[] positions = (int[])lowerBounds.Clone();
      var e = values.GetEnumerator();
      while (e.MoveNext()) {
        int[] currentPositions = positions;
        arr.SetValue(e.Current, currentPositions);
        positions[positions.Length - 1] += 1;
        for (int i = rank - 1; i > 0; i--) {
          if (positions[i] >= lengths[i] + lowerBounds[i]) {
            positions[i] = lowerBounds[i];
            positions[i - 1] += 1;
          } else {
            break;
          }
        }
      }
    }
  }

  [Transformer("D50C4782-7211-4476-B50F-7D3378EE3E53", 200)]
  [StorableType("4397B775-7D48-4C2A-8D28-ACC8C196CF70")]
  internal sealed class StringArrayBoxTransformer : MultiDimValueArrayBoxTransformer<string> {
    protected override void AddValueToBox(Box box, string val, Mapper mapper) {
      box.UInts.Add(mapper.GetStringId(val));
    }
    protected override IEnumerable<string> ExtractValues(Box box, Mapper mapper) {
      return box.UInts.Select(mapper.GetString);
    }
  }

  [Transformer("C9CFA67B-DF13-4125-B781-98EC8F5E390F", 201)]
  [StorableType("9719DB59-C6BC-4788-BBB0-389A1B49CFEE")]
  internal sealed class BoolArrayBoxTransformer : MultiDimValueArrayBoxTransformer<bool> {
    protected override void AddValueToBox(Box box, bool val, Mapper mapper) {
      box.Bools.Add(val);
    }
    protected override IEnumerable<bool> ExtractValues(Box box, Mapper mapper) {
      return box.Bools;
    }
  }

  [Transformer("BA2E18F6-5C17-40CA-A5B8-5690C5EFE872", 202)]
  [StorableType("6548E9F0-621D-47BA-A605-8A47EF85C231")]
  internal sealed class IntArrayBoxTransformer : MultiDimValueArrayBoxTransformer<int> {
    protected override void AddValueToBox(Box box, int val, Mapper mapper) {
      box.Ints.Add(val);
    }
    protected override IEnumerable<int> ExtractValues(Box box, Mapper mapper) {
      return box.Ints;
    }
  }

  [Transformer("EEE7710D-86DE-47E1-887D-BDA2996B141E", 203)]
  [StorableType("4127B466-AFC0-4050-8C45-1376A0E3E016")]
  internal sealed class UnsignedIntArrayBoxTransformer : MultiDimValueArrayBoxTransformer<uint> {
    protected override void AddValueToBox(Box box, uint val, Mapper mapper) {
      box.UInts.Add(val);
    }
    protected override IEnumerable<uint> ExtractValues(Box box, Mapper mapper) {
      return box.UInts;
    }
  }

  [Transformer("557932AA-F023-477F-AAD0-5098E8B8CD56", 204)]
  [StorableType("C2ED50C8-C340-40C1-B00C-2F398EB709A0")]
  internal sealed class LongArrayBoxTransformer : MultiDimValueArrayBoxTransformer<long> {
    protected override void AddValueToBox(Box box, long val, Mapper mapper) {
      box.Longs.Add(val);
    }
    protected override IEnumerable<long> ExtractValues(Box box, Mapper mapper) {
      return box.Longs;
    }
  }

  [Transformer("CFF20DEE-2A55-4D04-B543-A4C7E0A8F7BF", 205)]
  [StorableType("641AE353-5373-4811-BACB-C13D3144809C")]
  internal sealed class UnsignedLongArrayBoxTransformer : MultiDimValueArrayBoxTransformer<ulong> {
    protected override void AddValueToBox(Box box, ulong val, Mapper mapper) {
      box.ULongs.Add(val);
    }
    protected override IEnumerable<ulong> ExtractValues(Box box, Mapper mapper) {
      return box.ULongs;
    }
  }

  [Transformer("2A0F766D-FE71-4415-A75F-A32FB8BB9E2D", 206)]
  [StorableType("AEE9384F-3857-4CE4-AE30-B99474F7A6C9")]
  internal sealed class FloatArrayBoxTransformer : MultiDimValueArrayBoxTransformer<float> {
    protected override void AddValueToBox(Box box, float val, Mapper mapper) {
      box.Floats.Add(val);
    }
    protected override IEnumerable<float> ExtractValues(Box box, Mapper mapper) {
      return box.Floats;
    }
  }


  [Transformer("192A8F7D-84C7-44BF-B0CA-AD387A241AAD", 207)]
  [StorableType("17D0BA74-CB84-405C-8DBB-D9E361274A0A")]
  internal sealed class DoubleArrayBoxTransformer : MultiDimValueArrayBoxTransformer<double> {
    protected override void AddValueToBox(Box box, double val, Mapper mapper) {
      box.Doubles.Add(val);
    }
    protected override IEnumerable<double> ExtractValues(Box box, Mapper mapper) {
      return box.Doubles;
    }
  }

  [Transformer("3A35CECE-9953-4C29-A796-A56C02D80A05", 208)]
  [StorableType("A076D11E-89AA-43C8-87F5-A0D0F52569EB")]
  internal sealed class ByteArrayBoxTransformer : MultiDimValueArrayBoxTransformer<byte> {
    protected override void AddValueToBox(Box box, byte val, Mapper mapper) {
      box.Ints.Add(val);
    }
    protected override IEnumerable<byte> ExtractValues(Box box, Mapper mapper) {
      return box.Ints.Select(i => (byte)i);
    }
  }

  [Transformer("880D4A63-6C77-4F9F-8F7C-2D365F0AE829", 209)]
  [StorableType("74F6FD4B-D7D7-43CD-B28B-3A775505FEE3")]
  internal sealed class SByteArrayBoxTransformer : MultiDimValueArrayBoxTransformer<sbyte> {
    protected override void AddValueToBox(Box box, sbyte val, Mapper mapper) {
      box.Ints.Add(val);
    }
    protected override IEnumerable<sbyte> ExtractValues(Box box, Mapper mapper) {
      return box.Ints.Select(i => (sbyte)i);
    }
  }

  [Transformer("9786E711-7C1D-4761-BD6B-445793834264", 210)]
  [StorableType("5F32480E-AACB-4DB3-ADE3-1CF36E33C037")]
  internal sealed class ShortArrayBoxTransformer : MultiDimValueArrayBoxTransformer<short> {
    protected override void AddValueToBox(Box box, short val, Mapper mapper) {
      box.Ints.Add(val);
    }
    protected override IEnumerable<short> ExtractValues(Box box, Mapper mapper) {
      return box.Ints.Select(i => (short)i);
    }
  }

  [Transformer("1AAC2625-356C-40BC-8CB4-15CB3D047EB8", 211)]
  [StorableType("C303B4CF-FFD0-47E2-9D94-07F2D558D17F")]
  internal sealed class UShortArrayTransformer : MultiDimValueArrayBoxTransformer<ushort> {
    protected override void AddValueToBox(Box box, ushort val, Mapper mapper) {
      box.Ints.Add(val);
    }
    protected override IEnumerable<ushort> ExtractValues(Box box, Mapper mapper) {
      return box.Ints.Select(i => (ushort)i);
    }
  }

  [Transformer("12F19098-5D49-4C23-8897-69087F1C146D", 212)]
  [StorableType("55F7C8B0-F2AA-4830-857D-6CE2807DA138")]
  internal sealed class CharArrayTransformer : MultiDimValueArrayBoxTransformer<char> {
    protected override void AddValueToBox(Box box, char val, Mapper mapper) {
      box.Ints.Add(val);
    }
    protected override IEnumerable<char> ExtractValues(Box box, Mapper mapper) {
      return box.Ints.Select(i => (char)i);
    }
  }

  [Transformer("05AE4C5D-4D0C-47C7-B6D5-F04230C6F565", 301)]
  [StorableType("A74820C8-F400-462A-913A-610BB588D04A")]
  internal sealed class ArrayBoxTransformer : BoxTransformer<object> {
    public override bool CanTransformType(Type type) {
      return type.IsArray;
    }

    protected override void Populate(Box box, object value, Mapper mapper) {
      var type = value.GetType();
      var array = (Array)value;
      var rank = array.Rank;

      box.ArrayMetaInfoBox = new ArrayMetaInfoBox();
      box.ArrayMetaInfoBox.Rank = rank;

      int[] lengths = new int[rank];
      int[] lowerBounds = new int[rank];
      for (int i = 0; i < rank; i++) {
        box.ArrayMetaInfoBox.Lengths.Add(lengths[i] = array.GetLength(i));
        box.ArrayMetaInfoBox.LowerBounds.Add(lowerBounds[i] = array.GetLowerBound(i));
      }

      int[] positions = (int[])lowerBounds.Clone();
      while (positions[0] < lengths[0] + lowerBounds[0]) {
        box.UInts.Add(mapper.GetBoxId(array.GetValue(positions)));
        positions[rank - 1] += 1;
        for (int i = rank - 1; i > 0; i--) {
          if (positions[i] >= lowerBounds[i] + lengths[i]) {
            positions[i] = lowerBounds[i];
            positions[i - 1] += 1;
          } else {
            break;
          }
        }
      }
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      var rank = box.ArrayMetaInfoBox.Rank;

      int[] lengths = new int[rank], lowerBounds = new int[rank];
      for (int i = 0; i < rank; i++) {
        lengths[i] = box.ArrayMetaInfoBox.Lengths[i];
        lowerBounds[i] = box.ArrayMetaInfoBox.LowerBounds[i];
      }

      return Array.CreateInstance(type.GetElementType(), lengths, lowerBounds);
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var array = (Array)obj;
      var uints = box.UInts;
      var rank = box.ArrayMetaInfoBox.Rank;

      int[] lengths = new int[rank], lowerBounds = new int[rank];
      for (int i = 0; i < rank; i++) {
        lengths[i] = box.ArrayMetaInfoBox.Lengths[i];
        lowerBounds[i] = box.ArrayMetaInfoBox.LowerBounds[i];
      }

      int[] positions = (int[])lowerBounds.Clone();
      var e = uints.GetEnumerator();
      while (e.MoveNext()) {
        int[] currentPositions = positions;
        array.SetValue(mapper.GetObject(e.Current), currentPositions);
        positions[rank - 1] += 1;
        for (int i = rank - 1; i > 0; i--) {
          if (positions[i] >= lengths[i] + lowerBounds[i]) {
            positions[i] = lowerBounds[i];
            positions[i - 1] += 1;
          } else {
            break;
          }
        }
      }
    }
  }

  [Transformer("26AD5F85-1D77-4579-BCB9-CD409B48AC7A", 302)]
  [StorableType("A96E2C1E-EBDF-4D81-A0ED-91677BC84FEE")]
  internal sealed class EnumerableBoxTransformer : BoxTransformer<object> {
    private static readonly HashSet<Type> supportedTypes = new HashSet<Type> {
      typeof(Stack<>), typeof(Stack),
      typeof(Queue<>), typeof(Queue),
      typeof(HashSet<>),
      typeof(List<>), typeof(ArrayList)
    };

    public override bool CanTransformType(Type type) {
      return type.IsGenericType && supportedTypes.Contains(type.GetGenericTypeDefinition()) || supportedTypes.Contains(type);
    }

    protected override void Populate(Box box, object value, Mapper mapper) {
      var type = value.GetType();
      var propertyInfo = type.GetProperty("Comparer");
      if (propertyInfo != null) {
        var comparer = propertyInfo.GetValue(value);
        var comparerType = comparer.GetType();
        if (StorableTypeAttribute.IsStorableType(comparerType))
          box.ComparerId = mapper.GetBoxId(comparer);
        else if (comparerType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).Any())
          throw new NotSupportedException("Cannot serialize non-storable equality comparers with fields");
        else
          box.ComparerId = mapper.GetBoxId(comparerType);
      }

      if (type.IsGenericType) {
        var genericType = type.GetGenericArguments().Single();
        if (genericType.IsPrimitive) {
          if (genericType == typeof(bool)) {
            box.Bools.AddRange((IEnumerable<bool>)value);
          } else if (genericType == typeof(int)) {
            box.Ints.AddRange((IEnumerable<int>)value);
          } else if (genericType == typeof(uint)) {
            box.UInts.AddRange((IEnumerable<uint>)value);
          } else if (genericType == typeof(long)) {
            box.Longs.AddRange((IEnumerable<long>)value);
          } else if (genericType == typeof(ulong)) {
            box.ULongs.AddRange((IEnumerable<ulong>)value);
          } else if (genericType == typeof(float)) {
            box.Floats.AddRange((IEnumerable<float>)value);
          } else if (genericType == typeof(double)) {
            box.Doubles.AddRange((IEnumerable<double>)value);
          } else if (genericType == typeof(byte)) {
            box.Bytes = ByteString.CopyFrom(((IEnumerable<byte>)value).ToArray());
          } else {
            AddItemsToUints(box, (IEnumerable)value, mapper);
          }
        } else {
          AddItemsToUints(box, (IEnumerable)value, mapper);
        }
      } else {
        AddItemsToUints(box, (IEnumerable)value, mapper);
      }
    }

    private static void AddItemsToUints(Box box, IEnumerable items, Mapper mapper) {
      foreach (var item in items) {
        if (mapper.CancellationToken.IsCancellationRequested) return;
        box.UInts.Add(mapper.GetBoxId(item));
      }
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      if (type.IsGenericType && type.GetGenericArguments().Single().IsPrimitive) {
        if (type.GetGenericTypeDefinition() == typeof(Queue<>) ||
                  type.GetGenericTypeDefinition() == typeof(List<>)) {
          // for primitive type arguments we can immediately populate the collection with the correct capacity
          return Activator.CreateInstance(type, GetPrimitiveElements(type, box));
        } else if (type.GetGenericTypeDefinition() == typeof(Stack<>)) {
          return Activator.CreateInstance(type, GetReversePrimitiveElements(type, box));
        } else if (type.GetGenericTypeDefinition() == typeof(HashSet<>)) {
          // for HashSets we need restore the comparer
          var comparerObj = mapper.GetObject(box.ComparerId);
          var comparer = comparerObj is Type ? Activator.CreateInstance((Type)comparerObj) : comparerObj;
          return Activator.CreateInstance(type, GetPrimitiveElements(type, box), comparer);
        } else throw new ArgumentException($"Unsupported type in transformer {type}");
      } else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>)) {
        // for HashSets we need restore the comparer
        var comparerObj = mapper.GetObject(box.ComparerId);
        var comparer = comparerObj is Type ? Activator.CreateInstance((Type)comparerObj) : comparerObj;
        return Activator.CreateInstance(type, comparer);
      } else {
        return Activator.CreateInstance(type);
      }
    }

    private IEnumerable GetPrimitiveElements(Type type, Box box) {
      var typeArg = type.GetGenericArguments().Single();
      if (typeArg == typeof(bool)) {
        return box.Bools;
      } else if (typeArg == typeof(int)) {
        return box.Ints;
      } else if (typeArg == typeof(uint)) {
        return box.UInts;
      } else if (typeArg == typeof(long)) {
        return box.Longs;
      } else if (typeArg == typeof(ulong)) {
        return box.ULongs;
      } else if (typeArg == typeof(float)) {
        return box.Floats;
      } else if (typeArg == typeof(double)) {
        return box.Doubles;
      } else if (typeArg == typeof(byte)) {
        return box.Bytes.ToByteArray();
      } else if (typeArg.IsPrimitive) {
        return box.UInts;
      } else throw new ArgumentException($"invalid generic type argument {typeArg}");
    }
    private IEnumerable GetReversePrimitiveElements(Type type, Box box) {
      var typeArg = type.GetGenericArguments().Single();
      if (typeArg == typeof(bool)) {
        return box.Bools.Reverse();
      } else if (typeArg == typeof(int)) {
        return box.Ints.Reverse();
      } else if (typeArg == typeof(uint)) {
        return box.UInts.Reverse();
      } else if (typeArg == typeof(long)) {
        return box.Longs.Reverse();
      } else if (typeArg == typeof(ulong)) {
        return box.ULongs.Reverse();
      } else if (typeArg == typeof(float)) {
        return box.Floats.Reverse();
      } else if (typeArg == typeof(double)) {
        return box.Doubles.Reverse();
      } else if (typeArg == typeof(byte)) {
        return box.Bytes.ToByteArray().Reverse();
      } else if (typeArg.IsPrimitive) {
        return box.UInts.Reverse();
      } else throw new ArgumentException($"invalid generic type argument {typeArg}");
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var type = obj.GetType();
      // for generic types with primitive type arguments we have already populated the collection in Extract() and there is nothing left to do
      if (type.IsGenericType && type.GetGenericArguments().Single().IsPrimitive) return;

      var elements = box.UInts.Select(mapper.GetObject);

      string methodName = string.Empty;
      if (type == typeof(Stack) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Stack<>))) {
        elements = elements.Reverse();
        methodName = "Push";
      } else if (type == typeof(Queue) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Queue<>))) {
        methodName = "Enqueue";
      } else {
        methodName = "Add";
      }

      MethodInfo addMethod = type.GetMethod(methodName);
      foreach (var e in elements)
        addMethod.Invoke(obj, new[] { e });
    }
  }

  [Transformer("C47A62F5-F113-4A43-A8EE-CF817EC799A2", 303)]
  [StorableType("BA53DF84-DC97-4D8D-A493-868972ED1002")]
  internal sealed class DictionaryTransformer : BoxTransformer<object> {
    public override bool CanTransformType(Type type) {
      return type.IsGenericType && typeof(Dictionary<,>) == type.GetGenericTypeDefinition();
    }

    protected override void Populate(Box box, object value, Mapper mapper) {
      var kvpBox = new KeyValuePairBox();
      box.KeyValuePairs.Add(kvpBox);
      foreach (DictionaryEntry item in (IDictionary)value) {
        if (mapper.CancellationToken.IsCancellationRequested) return;
        kvpBox.KeyValuePairs.Add(mapper.GetBoxId(item.Key), mapper.GetBoxId(item.Value));
      }

      var type = value.GetType();
      var propertyInfo = type.GetProperty("Comparer");
      var comparer = propertyInfo.GetValue(value);

      var comparerType = comparer.GetType();
      if (StorableTypeAttribute.IsStorableType(comparerType))
        box.ComparerId = mapper.GetBoxId(comparer);
      else if (comparerType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).Any())
        throw new NotSupportedException("Cannot serialize non-storable equality comparers with fields");
      else
        box.ComparerId = mapper.GetBoxId(comparerType);
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      var comparerObj = mapper.GetObject(box.ComparerId);
      var comparer = comparerObj is Type ? Activator.CreateInstance((Type)comparerObj) : comparerObj;

      return Activator.CreateInstance(type, box.KeyValuePairs[0].KeyValuePairs.Count, comparer);
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var type = obj.GetType();

      var addMethod = type.GetMethod("Add");
      var kvpBox = box.KeyValuePairs[0];
      foreach (var entry in kvpBox.KeyValuePairs) {
        var key = mapper.GetObject(entry.Key);
        var value = mapper.GetObject(entry.Value);
        addMethod.Invoke(obj, new[] { key, value });
      }
    }
  }

  [Transformer("93FF076B-BC4B-4C39-8C40-15E004468C98", 219)]
  [StorableType("F8A7DBB4-E1CE-4DB8-905E-F3A05F5CA000")]
  internal sealed class EnumTransformer : Transformer {
    public override bool CanTransformType(Type type) {
      return typeof(Enum).IsAssignableFrom(type);
    }

    public override Box ToBox(object o, Mapper mapper) {
      var box = new Box {
        TransformerId = mapper.GetTransformerId(this),
        TypeBoxId = mapper.GetBoxId(o.GetType()),
        UInt = mapper.GetStringId(Enum.Format(o.GetType(), o, "G")) // TODO: introduce old names for enum values to enable refactoring
      };
      return box;
    }

    public override object ToObject(Box box, Mapper mapper) {
      var type = (Type)mapper.GetObject(box.TypeBoxId);
      return type == null ? null : Enum.Parse(type, mapper.GetString(box.UInt));
    }
  }

  // transforms objects (e.g. [Storable] object o = new object())
  [Transformer("268617FE-3F0F-4029-8248-EDA420901FB6", 10000)]
  [StorableType("320CBA58-1AAF-492B-BE3B-F59EEA085EBE")]
  internal sealed class ObjectBoxTransformer : BoxTransformer<object> {
    protected override void Populate(Box box, object value, Mapper mapper) { }
    protected override object Extract(Box box, Type type, Mapper mapper) { return new object(); }
  }


  [Transformer("90F9F16D-9F94-491B-AC3B-E1C6F3432127", 400)]
  [StorableType("C3C23FEE-DB96-4CEA-A38B-6BB73811F877")]
  internal sealed class DecimalBoxTransformer : BoxTransformer<decimal> {
    protected override decimal Extract(Box box, Type type, Mapper mapper) {
      return ParseG30(mapper.GetString(box.UInt));
    }

    protected override void Populate(Box box, decimal value, Mapper mapper) {
      box.UInt = mapper.GetStringId(FormatG30(value)); // TODO: improve efficiency for decimals
    }

    private static decimal ParseG30(string s) {
      decimal d;
      if (decimal.TryParse(s,
        NumberStyles.AllowDecimalPoint |
        NumberStyles.AllowExponent |
        NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out d))
        return d;
      throw new FormatException(
        string.Format("Invalid decimal G30 number format \"{0}\" could not be parsed", s));
    }

    private static string FormatG30(decimal d) {
      return d.ToString("g30", CultureInfo.InvariantCulture);
    }
  }

  [Transformer("1C6C350F-B2C8-40F0-A964-54DDB0D087A3", 401)]
  [StorableType("9D99D155-E3BB-40CE-AF64-6E153D876148")]
  internal sealed class DateTimeBoxTransformer : BoxTransformer<DateTime> {
    protected override void Populate(Box box, DateTime value, Mapper mapper) { box.Long = value.Ticks; }
    protected override DateTime Extract(Box box, Type type, Mapper mapper) { return new DateTime(box.Long); }
  }

  [Transformer("964074C9-4B82-4725-97AF-612A193EA5C6", 402)]
  [StorableType("9CA7C1F7-784C-48D5-A6F4-E1FD7B3A2FEC")]
  internal sealed class TimeSpanBoxTransformer : BoxTransformer<TimeSpan> {
    protected override void Populate(Box box, TimeSpan value, Mapper mapper) { box.Long = value.Ticks; }
    protected override TimeSpan Extract(Box box, Type type, Mapper mapper) { return new TimeSpan(box.Long); }
  }

  [Transformer("B0C0165B-6279-4CC3-8DB7-D36898BFBC38", 403)]
  [StorableType("ED6B7AE1-CE2B-4891-9BF3-72F5D1D67D93")]
  internal sealed class PointTransformer : BoxTransformer<Point> {
    protected override void Populate(Box box, Point value, Mapper mapper) {
      box.Ints.AddRange(new[] { value.X, value.Y });
    }

    protected override Point Extract(Box box, Type type, Mapper mapper) {
      var ints = box.Ints;
      return new Point(ints[0], ints[1]);
    }
  }

  [Transformer("D912D573-CE41-40B8-8F95-646C183662F6", 406)]
  [StorableType("3ED2C12F-BE41-45F5-AA0A-C2F23EB99FBD")]
  internal sealed class KeyValuePairBoxTransformer : BoxTransformer<object> {
    public override bool CanTransformType(Type type) {
      return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>));
    }

    protected override void Populate(Box box, object value, Mapper mapper) {
      var uints = box.UInts;

      var type = value.GetType();
      var pair = new[] {
        mapper.GetBoxId(type.GetProperty("Key").GetValue(value)) ,
        mapper.GetBoxId(type.GetProperty("Value").GetValue(value))
      };

      uints.AddRange(pair);
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var uints = box.UInts;
      var key = mapper.GetObject(uints[0]);
      var val = mapper.GetObject(uints[1]);
      var type = obj.GetType();
      //DataMemberAccessor.GenerateFieldSetter(type.GetField("key", BindingFlags.NonPublic | BindingFlags.Instance))(obj, key);
      //DataMemberAccessor.GenerateFieldSetter(type.GetField("value", BindingFlags.NonPublic | BindingFlags.Instance))(obj, val);
      type.GetField("key", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, key);
      type.GetField("value", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, val);
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      return Activator.CreateInstance(type);
    }
  }

  [Transformer("BBA08D33-FDA4-4EB6-8D94-9BF3D30C3E11", 407)]
  [StorableType("B0333A12-7D91-47A5-ACD4-49E43E5A18DC")]
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
      var uints = box.UInts;
      for (int i = 1; i <= type.GetGenericArguments().Length; i++) {
        string name = string.Format("Item{0}", i);
        uints.Add(mapper.GetBoxId(type.GetProperty(name).GetValue(value)));
      }
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      var defaultValues = type.GetGenericArguments().Select(x =>
        x.IsValueType ? Activator.CreateInstance(x) : null
      ).ToArray();
      return Activator.CreateInstance(type, defaultValues);
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var uints = box.UInts;
      var elements = uints.Select(mapper.GetObject).ToArray();
      var type = obj.GetType();
      for (int i = 1; i <= elements.Length; i++) {
        string name = string.Format("m_Item{0}", i);
        var fieldInfo = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        fieldInfo.SetValue(obj, elements[i - 1]);
      }
    }
  }

  [Transformer("731F9A18-6BF4-43BE-95CF-8205552C9B70", 500)]
  [StorableType("73FCDE79-436D-476E-B2EA-A7A1A4A810B5")]
  internal sealed class StructBoxTransformer : BoxTransformer<object> {
    public override bool CanTransformType(Type type) {
      return type.IsValueType && !type.IsPrimitive && !type.IsEnum && type.IsSealed;
    }

    protected override void Populate(Box box, object value, Mapper mapper) {
      var type = value.GetType();

      var kvpBox = new KeyValuePairBox();
      box.KeyValuePairs.Add(kvpBox);

      foreach (var fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        kvpBox.KeyValuePairs.Add(mapper.GetStringId(fieldInfo.Name), mapper.GetBoxId(fieldInfo.GetValue(value)));
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      var obj = Activator.CreateInstance(type);

      var kvpBox = box.KeyValuePairs[0];
      foreach (var t in kvpBox.KeyValuePairs) {
        string name = mapper.GetString(t.Key);
        MemberInfo[] mis = type.GetMember(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (mis.Length != 1)
          throw new Exception("ambiguous struct member name " + name);
        MemberInfo mi = mis[0];
        if (StorableAttribute.IsStorable(mi))
          throw new PersistenceException("Don't use storable attributes for structs as all fields are serialized automatically.");
        if (mi.MemberType == MemberTypes.Field)
          ((FieldInfo)mi).SetValue(obj, mapper.GetObject(t.Value));
        else
          throw new Exception("invalid struct member type " + mi.MemberType.ToString());
      }

      return obj;
    }
  }

  [Transformer("78556C88-0FEE-4602-95C7-A469B2DDB468", 600)]
  [StorableType("3A578289-43CA-40F8-9F1E-2BDD255CB8FB")]
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
      do {
        var kvpBox = new KeyValuePairBox();
        box.KeyValuePairs.Add(kvpBox);

        var typeInfo = Mapper.StaticCache.GetTypeInfo(type);
        kvpBox.TypeId = mapper.GetStringId(typeInfo.StorableTypeAttribute.Guid.ToString().ToUpperInvariant());

        foreach (var componentInfo in typeInfo.Fields) {
          kvpBox.KeyValuePairs.Add(
            mapper.GetStringId(componentInfo.Name),
            mapper.GetBoxId(componentInfo.MemberInfo.GetValue(value))
          );
        }

        foreach (var componentInfo in typeInfo.Properties.Where(x => x.Readable)) {
          var declaringType = componentInfo.DeclaringType;

          if (!set.Add(Tuple.Create(declaringType, componentInfo.Name))) continue;

          kvpBox.KeyValuePairs.Add(
            mapper.GetStringId(componentInfo.Name),
            mapper.GetBoxId(componentInfo.MemberInfo.GetValue(value, null))
          );
        }

        type = type.BaseType;
      } while (StorableTypeAttribute.IsStorableType(type) && !mapper.CancellationToken.IsCancellationRequested);
    }

    protected override object Extract(Box box, Type type, Mapper mapper) {
      return mapper.CreateInstance(type);
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var dict = new Dictionary<string, object>();
      foreach (var kvpBox in box.KeyValuePairs) {
        foreach (var entry in kvpBox.KeyValuePairs) {
          string key = mapper.GetString(kvpBox.TypeId) + "." + mapper.GetString(entry.Key);
          object value = mapper.GetObject(entry.Value);
          dict.Add(key, value);
        }
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

        foreach (var componentInfo in typeInfo.Properties.Where(x => x.Writeable)) {
          var attrib = componentInfo.StorableAttribute;
          var propertyInfo = componentInfo.MemberInfo;

          if (dict.TryGetValue(componentInfo.FullName, out object value)) {
            propertyInfo.SetValue(obj, value, null);
          } else if (attrib != null && attrib.DefaultValue != null) {
            propertyInfo.SetValue(obj, attrib.DefaultValue, null);
          }
        }

        // set all members
        //foreach (var kvp in dict) {
        //  string key = kvp.Key;
        //  object val = kvp.Value;

        //  string[] keyParts = key.Split('.');
        //  var guid = Guid.Parse(keyParts[0]);
        //  string ident = keyParts[1];

        //  if (guid != typeInfo.StorableTypeAttribute.Guid) continue;

        //  var fieldInfo = typeInfo.Fields.FirstOrDefault(fi => fi.Name == ident);
        //  if (fieldInfo != null) {
        //    var field = (FieldInfo)fieldInfo.MemberInfo;
        //    field.SetValue(obj, val);
        //    //dict.Remove(guid.ToString().ToUpperInvariant() + "." + fieldInfo.Name); // only for consistency check
        //    continue;
        //  }

        //  var propInfo = typeInfo.Properties.Where(x => x.Writeable).FirstOrDefault(fi => fi.Name == ident);
        //  if (propInfo != null) {
        //    var prop = (PropertyInfo)propInfo.MemberInfo;
        //    prop.SetValue(obj, val, null);
        //    //dict.Remove(guid.ToString().ToUpperInvariant() + "." + propInfo.Name); // only for consistency check
        //    continue;
        //  }
        //}

        //var undefinedMembers = dict.Where(x => Guid.Parse(x.Key.Split('.')[0]) == typeInfo.StorableTypeAttribute.Guid);

        //if (undefinedMembers.Any())
        //  throw new PersistenceException(string.Format("Invalid conversion method. The following members are undefined in type {0} version {1}: {2}",
        //    typeInfo.StorableTypeAttribute.Guid,
        //    typeInfo.StorableTypeAttribute.Version,
        //    string.Join(", ", undefinedMembers.Select(x => x.Key))));
      }
    }
  }
}
