#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HEAL.Attic;

namespace HEAL.Attic.Tests {

  class Util {
    public static bool AreElementsEqual<T>(T[,] a, T[,] b) {
      if (a.GetLength(0) != b.GetLength(0) ||
        a.GetLength(1) != b.GetLength(1)) return false;
      for (int i = 0; i < a.GetLength(0); i++) {
        for (int j = 0; j < a.GetLength(1); j++) {
          if (!a[i, j].Equals(b[i, j])) return false;
        }
      }
      return true;
    }
    public static bool AreElementsEqual<T>(IEnumerable<T> a, IEnumerable<T> b) {
      var aEnum = a.GetEnumerator();
      var bEnum = b.GetEnumerator();
      while (aEnum.MoveNext() & bEnum.MoveNext()) {
        if (!aEnum.Current.Equals(bEnum.Current)) return false;
      }
      // no more elements?
      if (aEnum.MoveNext() | bEnum.MoveNext()) return false;
      return true;
    }
  }
  public static class EnumerableTimeSpanExtensions {
    public static TimeSpan Average(this IEnumerable<TimeSpan> span) {
      var avg = (long)Math.Round(span.Select(x => x.Ticks).Average());
      return new TimeSpan(avg);
    }
  }


  [StorableType("22B5FC22-44FA-40B4-84E3-BB53540E812E")]
  public class NumberTest {
    [Storable]
    private bool _bool = true;
    [Storable]
    private byte _byte = 0xFF;
    [Storable]
    private sbyte _sbyte = 0xF;
    [Storable]
    private short _short = -123;
    [Storable]
    private ushort _ushort = 123;
    [Storable]
    private int _int = -123;
    [Storable]
    private uint _uint = 123;
    [Storable]
    private long _long = 123456;
    [Storable]
    private ulong _ulong = 123456;
    public override bool Equals(object obj) {
      NumberTest nt = obj as NumberTest;
      if (nt == null)
        throw new NotSupportedException();
      return
        nt._bool == _bool &&
        nt._byte == _byte &&
        nt._sbyte == _sbyte &&
        nt._short == _short &&
        nt._ushort == _ushort &&
        nt._int == _int &&
        nt._uint == _uint &&
        nt._long == _long &&
        nt._ulong == _ulong;
    }
    public override int GetHashCode() {
      return
        _bool.GetHashCode() ^
        _byte.GetHashCode() ^
        _sbyte.GetHashCode() ^
        _short.GetHashCode() ^
        _short.GetHashCode() ^
        _int.GetHashCode() ^
        _uint.GetHashCode() ^
        _long.GetHashCode() ^
        _ulong.GetHashCode();
    }

    [StorableConstructor]
    protected NumberTest(StorableConstructorFlag _) {
    }
    public NumberTest() {
    }
  }

  [StorableType("2D94AD3B-D411-403F-AC42-60824C78D802")]
  public class IntWrapper {

    [Storable]
    public int Value;

    [StorableConstructor]
    protected IntWrapper(StorableConstructorFlag _) {
    }

    private IntWrapper() { }

    public IntWrapper(int value) {
      this.Value = value;
    }

    public override bool Equals(object obj) {
      if (obj as IntWrapper == null)
        return false;
      return Value.Equals(((IntWrapper)obj).Value);
    }
    public override int GetHashCode() {
      return Value.GetHashCode();
    }

  }

  [StorableType("45337DD7-26D0-42D0-8CC4-92E184AE0218")]
  public class PrimitivesTest : NumberTest {
    [Storable]
    private char c = 'e';
    [Storable]
    private long[,] _long_array =
      new long[,] { { 123, 456, }, { 789, 123 } };
    [Storable]
    public List<int> list = new List<int> { 1, 2, 3, 4, 5 };
    [Storable]
    private object o = new object();
    public override bool Equals(object obj) {
      PrimitivesTest pt = obj as PrimitivesTest;
      if (pt == null)
        throw new NotSupportedException();
      return base.Equals(obj) &&
        c == pt.c &&
        Util.AreElementsEqual(_long_array, pt._long_array) &&
        Util.AreElementsEqual(list, pt.list);
    }

    public override int GetHashCode() {
      return base.GetHashCode() ^
        c.GetHashCode() ^
        _long_array.GetHashCode() ^
        list.GetHashCode();
    }

    [StorableConstructor]
    protected PrimitivesTest(StorableConstructorFlag _) : base(_) {
    }
    public PrimitivesTest() {
    }
  }

  [StorableType("2F63F603-CE7D-4262-99B4-A797F4D04907")]
  public enum TestEnum { va1, va2, va3, va8 };

  [StorableType("DC944CA9-5F6A-4EF3-AFBD-881FC63797DF")]
  public class RootBase {
    [Storable]
    private string baseString = "   Serial  ";
    [Storable]
    public TestEnum myEnum = TestEnum.va3;
    public override bool Equals(object obj) {
      RootBase rb = obj as RootBase;
      if (rb == null)
        throw new NotSupportedException();
      return baseString == rb.baseString &&
        myEnum == rb.myEnum;
    }
    public override int GetHashCode() {
      return baseString.GetHashCode() ^
        myEnum.GetHashCode();
    }

    [StorableConstructor]
    protected RootBase(StorableConstructorFlag _) {
    }
    public RootBase() {
    }
  }

  [StorableType("C478905A-5029-4F31-9D92-524F41272D46")]
  public class Root : RootBase {
    [Storable]
    public Stack<int> intStack = new Stack<int>();
    [Storable]
    public int[] i = new[] { 3, 4, 5, 6 };
    [Storable(Name = "Test String")]
    public string s;
    [Storable]
    public ArrayList intArray = new ArrayList(new[] { 1, 2, 3 });
    [Storable]
    public List<int> intList = new List<int>(new[] { 321, 312, 321 });
    [Storable]
    public Custom c;
    [Storable]
    public List<Root> selfReferences;
    [Storable]
    public double[,] multiDimArray = new double[,] { { 1, 2, 3 }, { 3, 4, 5 } };
    [Storable]
    public bool boolean = true;
    [Storable]
    public DateTime dateTime;
    [Storable]
    public KeyValuePair<string, int> kvp = new KeyValuePair<string, int>("Serial", 123);
    [Storable]
    public Dictionary<string, int> dict = new Dictionary<string, int>();
    [Storable(DefaultValue = "default")]
    public string uninitialized;
    [Storable]
    public Type dataType = typeof(Root);

    [StorableConstructor]
    protected Root(StorableConstructorFlag _) : base(_) {
    }
    public Root() {
    }
  }

  [StorableType("23DCF22C-EDAB-4C5A-9941-0F2D6030D467")]
  public enum SimpleEnum { one, two, three }
  [StorableType("1FA5C129-129E-485C-A8A7-59FCA10CBB20")]
  public enum ComplexEnum { one = 1, two = 2, three = 3 }
  [FlagsAttribute]
  [StorableType("D4A5D0CD-295C-4AC1-B5DA-D8DA2861E82C")]
  public enum TrickyEnum { zero = 0, one = 1, two = 2 }

  [StorableType("C6EC77AF-C565-4A83-8922-3C6E2370627B")]
  public class EnumTest {
    [Storable]
    public SimpleEnum simpleEnum = SimpleEnum.one;
    [Storable]
    public ComplexEnum complexEnum = (ComplexEnum)2;
    [Storable]
    public TrickyEnum trickyEnum = (TrickyEnum)15;

    [StorableConstructor]
    protected EnumTest(StorableConstructorFlag _) {
    }
    public EnumTest() {
    }
  }

  [StorableType("9E73E52B-9BF1-489D-9349-C490D518B7C4")]
  public class Custom {
    [Storable]
    public int i;
    [Storable]
    public Root r;
    [Storable]
    public string name = "<![CDATA[<![CDATA[Serial]]>]]>";

    [StorableConstructor]
    protected Custom(StorableConstructorFlag _) {
    }

    public Custom() {

    }
  }

  [StorableType("CEE5C689-948F-443A-A645-54868D913364")]
  public class Manager {

    public DateTime lastLoadTime;
    [Storable]
    private DateTime lastLoadTimePersistence {
      get { return lastLoadTime; }
      set { lastLoadTime = DateTime.Now; }
    }
    [Storable]
    public double? dbl;

    [StorableConstructor]
    protected Manager(StorableConstructorFlag _) {
    }
    public Manager() {
    }
  }

  [StorableType("14EB77CD-7061-4B2E-96EB-3E45CC265256")]
  public class C {
    [Storable]
    public C[][] allCs;
    [Storable]
    public KeyValuePair<List<C>, C> kvpList;

    [StorableConstructor]
    protected C(StorableConstructorFlag _) {
    }
    public C() {
    }
  }

  public class NonSerializable {
    int x = 0;
    public override bool Equals(object obj) {
      NonSerializable ns = obj as NonSerializable;
      if (ns == null)
        throw new NotSupportedException();
      return ns.x == x;
    }
    public override int GetHashCode() {
      return x.GetHashCode();
    }
  }

  [StorableType("FD953B0A-BDE6-41E6-91A8-CA3D90C91CDB")]
  public class SimpleClass {
    [Storable]
    public double x { get; set; }
    [Storable]
    public int y { get; set; }
  }

  [StorableType("33224778-C426-4D1D-93B3-FD42ED207C7C")]
  class IntValue {
    [Storable]
    public int Value { get; set; }
    [StorableConstructor]
    public IntValue(StorableConstructorFlag _) { }
    public IntValue(int value) {
      this.Value = value;
    }
  }

  [StorableType("EDB15EA3-FE86-40BF-9878-B8C878F1B415")]
  class StringValue {
    [Storable]
    public string Value { get; set; }
    [StorableConstructor]
    public StringValue(StorableConstructorFlag _) { }
    public StringValue(string value) {
      this.Value = value;
    }
  }



  [TestClass]
  public class UseCases {

    #region Helpers
    private string tempFile;

    // the following are necessary to test backwards compatibility test cases
    private static Dictionary<Guid, Type> guid2Type;
    private static Dictionary<Type, Guid> type2Guid;
    private static Dictionary<Type, TypeInfo> typeInfos;
    private static PropertyInfo guidPropertyInfo = typeof(StorableTypeAttribute).GetProperty("Guid");


    [ClassInitialize]
    public static void Initialize(TestContext testContext) {
      var guid2TypeFieldInfo = typeof(StaticCache).GetField("guid2Type", BindingFlags.NonPublic | BindingFlags.Instance);
      var type2GuidFieldInfo = typeof(StaticCache).GetField("type2Guid", BindingFlags.NonPublic | BindingFlags.Instance);
      var typeInfosFieldInfo = typeof(StaticCache).GetField("typeInfos", BindingFlags.NonPublic | BindingFlags.Instance);

      guid2Type = (Dictionary<Guid, Type>)guid2TypeFieldInfo.GetValue(Mapper.StaticCache);
      type2Guid = (Dictionary<Type, Guid>)type2GuidFieldInfo.GetValue(Mapper.StaticCache);
      typeInfos = (Dictionary<Type, TypeInfo>)typeInfosFieldInfo.GetValue(Mapper.StaticCache);
    }

    [TestInitialize()]
    public void CreateTempFile() {
      tempFile = Path.GetTempFileName();
    }

    [TestCleanup()]
    public void ClearTempFile() {
      StreamReader reader = new StreamReader(tempFile);
      string s = reader.ReadToEnd();
      reader.Close();
      File.Delete(tempFile);
    }
    #endregion

    [TestMethod]
    public void TestBool() {
      var test = new Func<object>(() => { return true; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      bool result = (bool)o;
      Assert.AreEqual(test(), result);
    }

    [TestMethod]
    public void TestInt() {
      var test = new Func<object>(() => { return (int)42; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      int result = (int)o;
      Assert.AreEqual(test(), result);
    }

    [TestMethod]
    public void TestDouble() {
      var test = new Func<object>(() => { return 42.5d; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      double result = (double)o;
      Assert.AreEqual(test(), result);
      Assert.IsTrue(o is double);
    }

    [TestMethod]
    public void TestFloat() {
      var test = new Func<object>(() => { return 42.5f; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      float result = (float)o;
      Assert.AreEqual(test(), result);
      Assert.IsTrue(o is float);
    }

    [TestMethod]
    public void TestDecimal() {
      var test = new Func<object>(() => { return 42.5m; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      decimal result = (decimal)o;
      Assert.AreEqual(test(), result);
      Assert.IsTrue(o is decimal);
    }


    [TestMethod]
    public void TestDecimalArray() {
      var test = new decimal[2, 2];
      test[0, 0] = decimal.MaxValue;
      test[1, 1] = decimal.MinValue;
      test[0, 1] = decimal.MinusOne;
      test[1, 0] = decimal.One;
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test, tempFile);
      object o = serializer.Deserialize(tempFile);
      var result = (decimal[,])o;
      CollectionAssert.AreEqual(test, result);
    }

    [TestMethod]
    public void TestLong() {
      var test = new Func<object>(() => { return 42l; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      long result = (long)o;
      Assert.AreEqual(test(), result);
      Assert.IsTrue(o is long);
    }

    [TestMethod]
    public void TestUInt() {
      var test = new Func<object>(() => { return 42u; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      uint result = (uint)o;
      Assert.AreEqual(test(), result);
      Assert.IsTrue(o is uint);
    }

    [TestMethod]
    public void TestShort() {
      var test = new Func<object>(() => { short s = 42; return s; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      short result = (short)o;
      Assert.IsTrue(o is short);
      Assert.AreEqual(test(), result);
    }

    [TestMethod]
    public void TestByte() {
      var test = new Func<object>(() => { byte b = 42; return b; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      byte result = (byte)o;
      Assert.IsTrue(o is byte);
      Assert.AreEqual(test(), result);
    }

    [TestMethod]
    public void TestEnumSimple() {
      var test = new Func<object>(() => { return SimpleEnum.two; });

      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      SimpleEnum result = (SimpleEnum)o;
      Assert.AreEqual(test(), result);
    }

    [TestMethod]
    public void TestEnumComplex() {
      var test = new Func<object>(() => { return ComplexEnum.three; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      ComplexEnum result = (ComplexEnum)o;
      Assert.AreEqual(test(), result);
    }

    [TestMethod]
    public void TestBytes() {
      var test = new Func<byte[]>(() => { return new byte[] { 3, 1 }; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      byte[] result = (byte[])o;
      Assert.AreEqual(test()[0], result[0]);
      Assert.AreEqual(test()[1], result[1]);
    }

    [TestMethod]
    public void TestSBytes() {
      var test = new Func<sbyte[]>(() => { return new sbyte[] { 3, 1 }; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      sbyte[] result = (sbyte[])o;
      Assert.AreEqual(test()[0], result[0]);
      Assert.AreEqual(test()[1], result[1]);
    }

    [TestMethod]
    public void TestChars() {
      var test = new Func<char[]>(() => { return new char[] { 'a', 'b' }; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      char[] result = (char[])o;
      Assert.AreEqual(test()[0], result[0]);
      Assert.AreEqual(test()[1], result[1]);
    }

    [TestMethod]
    public void TestShorts() {
      var test = new Func<short[]>(() => { return new short[] { 3, 1 }; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      short[] result = (short[])o;
      Assert.AreEqual(test()[0], result[0]);
      Assert.AreEqual(test()[1], result[1]);
    }

    [TestMethod]
    public void TestUShorts() {
      var test = new Func<ushort[]>(() => { return new ushort[] { 3, 1 }; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      ushort[] result = (ushort[])o;
      Assert.AreEqual(test()[0], result[0]);
      Assert.AreEqual(test()[1], result[1]);
    }

    [TestMethod]
    public void TestString() {
      var test = new Func<object>(() => { return "Hello World!"; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      string result = (string)o;
      Assert.AreEqual(test(), result);
    }

    [TestMethod]
    public void TestColor() {
      var test = new Func<object>(() => { return Color.FromArgb(12, 34, 56, 78); });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      Color result = (Color)o;
      Assert.AreEqual(test(), result);
    }

    [TestMethod]
    public void TestPoint() {
      var test = new Func<object>(() => { return new Point(3, 4); });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      Point result = (Point)o;
      Assert.AreEqual(((Point)test()).X, result.X);
      Assert.AreEqual(((Point)test()).Y, result.Y);
    }

    [TestMethod]
    public void TestBoolArray() {
      var test = new Func<bool[]>(() => { return new[] { true, false, true }; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      bool[] result = (bool[])o;
      Assert.AreEqual(test()[0], result[0]);
      Assert.AreEqual(test()[1], result[1]);
      Assert.AreEqual(test()[2], result[2]);
    }

    [TestMethod]
    public void TestIntArray() {
      var test = new Func<int[]>(() => { return new[] { 41, 22, 13 }; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      int[] result = (int[])o;
      Assert.AreEqual(test()[0], result[0]);
      Assert.AreEqual(test()[1], result[1]);
      Assert.AreEqual(test()[2], result[2]);
    }

    [TestMethod]
    public void TestLongArray() {
      var test = new Func<long[]>(() => { return new[] { 414481188112191633l, 245488586662l, 13546881335845865l }; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      long[] result = (long[])o;
      Assert.AreEqual(test()[0], result[0]);
      Assert.AreEqual(test()[1], result[1]);
      Assert.AreEqual(test()[2], result[2]);
    }

    [TestMethod]
    public void TestDoubleArray() {
      var test = new Func<double[]>(() => { return new[] { 41.5, 22.7, 13.8 }; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      double[] result = (double[])o;
      Assert.AreEqual(test()[0], result[0]);
      Assert.AreEqual(test()[1], result[1]);
      Assert.AreEqual(test()[2], result[2]);
    }

    [TestMethod]
    public void TestObjectArray() {
      var test = new Func<SimpleClass[]>(() => {
        return new[] { new SimpleClass() { x = 42, y = 43 },
                       new SimpleClass() { x = 44.44, y = 5677 },
                       new SimpleClass() { x = 533.33, y = 2345 } };
      });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      SimpleClass[] result = (SimpleClass[])o;
      Assert.AreEqual(test()[0].x, result[0].x);
      Assert.AreEqual(test()[0].y, result[0].y);
      Assert.AreEqual(test()[1].x, result[1].x);
      Assert.AreEqual(test()[1].y, result[1].y);
      Assert.AreEqual(test()[2].x, result[2].x);
      Assert.AreEqual(test()[2].y, result[2].y);
    }

    [TestMethod]
    public void TestStack() {
      var test = new Func<Stack>(() => {
        return new Stack(new int[] { 1, 2, 3 });
      });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      Stack result = (Stack)o;
      var actualStack = test();
      Assert.AreEqual(actualStack.Pop(), result.Pop());
      Assert.AreEqual(actualStack.Pop(), result.Pop());
      Assert.AreEqual(actualStack.Pop(), result.Pop());
    }

    [TestMethod]
    public void TestArrayOfStack() {
      var test = new Func<object[]>(() => {
        return new object[] {
          new Stack(new int[] { 1, 2, 3 }),
          new Stack<int>(new int[] { 1, 2, 3 }),
      };
      });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      var result = (object[])o;
      var firstStack = (Stack)result[0];
      var secondStack = (Stack<int>)result[1];
      var actual = test();
      var actualFirst = (Stack)actual[0];
      var actualSecond = (Stack<int>)actual[1];

      Assert.AreEqual(actualFirst.Pop(), firstStack.Pop());
      Assert.AreEqual(actualFirst.Pop(), firstStack.Pop());
      Assert.AreEqual(actualFirst.Pop(), firstStack.Pop());
      Assert.AreEqual(actualSecond.Pop(), secondStack.Pop());
      Assert.AreEqual(actualSecond.Pop(), secondStack.Pop());
      Assert.AreEqual(actualSecond.Pop(), secondStack.Pop());
    }

    [TestMethod]
    public void TestIntValueArray() {
      var test = new Func<IntValue[]>(() => { return new[] { new IntValue(41), new IntValue(22), new IntValue(13) }; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      IntValue[] result = (IntValue[])o;
      Assert.AreEqual(test()[0].Value, result[0].Value);
      Assert.AreEqual(test()[1].Value, result[1].Value);
      Assert.AreEqual(test()[2].Value, result[2].Value);
    }

    [TestMethod]
    public void TestIntValueArrayArray() {
      var test = new Func<IntValue[][]>(() => { return new IntValue[][] { new IntValue[] { new IntValue(41), new IntValue(22), new IntValue(13) } }; });
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      IntValue[][] result = (IntValue[][])o;
      Assert.AreEqual(test()[0][0].Value, result[0][0].Value);
      Assert.AreEqual(test()[0][1].Value, result[0][1].Value);
      Assert.AreEqual(test()[0][2].Value, result[0][2].Value);
    }

    [TestMethod]
    public void ComplexStorable() {
      Root r = InitializeComplexStorable();
      var ser = new ProtoBufSerializer();
      ser.Serialize(r, tempFile);
      Root newR = (Root)ser.Deserialize(tempFile);
      CompareComplexStorables(r, newR);
    }

    private static void CompareComplexStorables(Root r, Root newR) {
      Assert.AreSame(newR, newR.selfReferences[0]);
      Assert.AreNotSame(r, newR);
      Assert.AreEqual(r.myEnum, TestEnum.va1);
      Assert.AreEqual(r.i[0], 7);
      Assert.AreEqual(r.i[1], 5);
      Assert.AreEqual(r.i[2], 6);
      Assert.AreEqual(r.s, "new value");
      Assert.AreEqual(r.intArray[0], 3);
      Assert.AreEqual(r.intArray[1], 2);
      Assert.AreEqual(r.intArray[2], 1);
      Assert.AreEqual(r.intList[0], 9);
      Assert.AreEqual(r.intList[1], 8);
      Assert.AreEqual(r.intList[2], 7);
      Assert.AreEqual(r.multiDimArray[0, 0], 5);
      Assert.AreEqual(r.multiDimArray[0, 1], 4);
      Assert.AreEqual(r.multiDimArray[0, 2], 3);
      Assert.AreEqual(r.multiDimArray[1, 0], 1);
      Assert.AreEqual(r.multiDimArray[1, 1], 4);
      Assert.AreEqual(r.multiDimArray[1, 2], 6);
      Assert.IsFalse(r.boolean);
      Assert.IsTrue((DateTime.Now - r.dateTime).TotalSeconds < 10);
      Assert.AreEqual(r.kvp.Key, "string key");
      Assert.AreEqual(r.kvp.Value, 321);
      Assert.IsNull(r.uninitialized);
      Assert.AreEqual(newR.myEnum, TestEnum.va1);
      Assert.AreEqual(newR.i[0], 7);
      Assert.AreEqual(newR.i[1], 5);
      Assert.AreEqual(newR.i[2], 6);
      Assert.AreEqual(newR.s, "new value");
      Assert.AreEqual(newR.intArray[0], 3);
      Assert.AreEqual(newR.intArray[1], 2);
      Assert.AreEqual(newR.intArray[2], 1);
      Assert.AreEqual(newR.intList[0], 9);
      Assert.AreEqual(newR.intList[1], 8);
      Assert.AreEqual(newR.intList[2], 7);
      Assert.AreEqual(newR.multiDimArray[0, 0], 5);
      Assert.AreEqual(newR.multiDimArray[0, 1], 4);
      Assert.AreEqual(newR.multiDimArray[0, 2], 3);
      Assert.AreEqual(newR.multiDimArray[1, 0], 1);
      Assert.AreEqual(newR.multiDimArray[1, 1], 4);
      Assert.AreEqual(newR.multiDimArray[1, 2], 6);
      Assert.AreEqual(newR.intStack.Pop(), 3);
      Assert.AreEqual(newR.intStack.Pop(), 2);
      Assert.AreEqual(newR.intStack.Pop(), 1);
      Assert.IsFalse(newR.boolean);
      Assert.IsTrue((DateTime.Now - newR.dateTime).TotalSeconds < 10);
      Assert.AreEqual(newR.kvp.Key, "string key");
      Assert.AreEqual(newR.kvp.Value, 321);
      Assert.IsNull(newR.uninitialized);
    }

    private static Root InitializeComplexStorable() {
      Root r = new Root();
      r.intStack.Push(1);
      r.intStack.Push(2);
      r.intStack.Push(3);
      r.selfReferences = new List<Root> { r, r };
      r.c = new Custom { r = r };
      r.dict.Add("one", 1);
      r.dict.Add("two", 2);
      r.dict.Add("three", 3);
      r.myEnum = TestEnum.va1;
      r.i = new[] { 7, 5, 6 };
      r.s = "new value";
      r.intArray = new ArrayList { 3, 2, 1 };
      r.intList = new List<int> { 9, 8, 7 };
      r.multiDimArray = new double[,] { { 5, 4, 3 }, { 1, 4, 6 } };
      r.boolean = false;
      r.dateTime = DateTime.Now;
      r.kvp = new KeyValuePair<string, int>("string key", 321);
      r.uninitialized = null;

      return r;
    }

    [TestMethod]
    public void SelfReferences() {
      C c = new C();
      C[][] cs = new C[2][];
      cs[0] = new C[] { c };
      cs[1] = new C[] { c };
      c.allCs = cs;
      c.kvpList = new KeyValuePair<List<C>, C>(new List<C> { c }, c);
      new ProtoBufSerializer().Serialize(cs, tempFile);
      object o = new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreSame(c, c.allCs[0][0]);
      Assert.AreSame(c, c.allCs[1][0]);
      Assert.AreSame(c, c.kvpList.Key[0]);
      Assert.AreSame(c, c.kvpList.Value);
      C[][] newCs = (C[][])o;
      C newC = newCs[0][0];
      Assert.AreSame(newC, newC.allCs[0][0]);
      Assert.AreSame(newC, newC.allCs[1][0]);
      Assert.AreSame(newC, newC.kvpList.Key[0]);
      Assert.AreSame(newC, newC.kvpList.Value);
    }

    [TestMethod]
    public void ArrayCreation() {
      ArrayList[] arrayListArray = new ArrayList[4];
      arrayListArray[0] = new ArrayList();
      arrayListArray[0].Add(arrayListArray);
      arrayListArray[0].Add(arrayListArray);
      arrayListArray[1] = new ArrayList();
      arrayListArray[1].Add(arrayListArray);
      arrayListArray[2] = new ArrayList();
      arrayListArray[2].Add(arrayListArray);
      arrayListArray[2].Add(arrayListArray);
      Array a = Array.CreateInstance(
                              typeof(object),
                              new[] { 1, 2 }, new[] { 3, 4 });
      arrayListArray[2].Add(a);
      new ProtoBufSerializer().Serialize(arrayListArray, tempFile);
      object o = new ProtoBufSerializer().Deserialize(tempFile);
      ArrayList[] newArray = (ArrayList[])o;
      Assert.AreSame(arrayListArray, arrayListArray[0][0]);
      Assert.AreSame(arrayListArray, arrayListArray[2][1]);
      Assert.AreSame(newArray, newArray[0][0]);
      Assert.AreSame(newArray, newArray[2][1]);
    }

    [TestMethod]
    public void CustomSerializationProperty() {
      Manager m = new Manager();
      new ProtoBufSerializer().Serialize(m, tempFile);
      Manager newM = (Manager)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(m.dbl, newM.dbl);
      Assert.AreEqual(m.lastLoadTime, new DateTime());
      Assert.AreNotEqual(newM.lastLoadTime, new DateTime());
      Assert.IsTrue((DateTime.Now - newM.lastLoadTime).TotalSeconds < 10);
    }

    [TestMethod]
    public void Primitives() {
      PrimitivesTest sdt = new PrimitivesTest();
      new ProtoBufSerializer().Serialize(sdt, tempFile);
      var sdt2 = (PrimitivesTest)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(sdt, sdt2);
    }

    [TestMethod]
    public void MultiDimensionalArray() {
      string[,] mDimString = new string[,] {
        {"one", "two", "three"},
        {"four", "five", "six"}
      };
      new ProtoBufSerializer().Serialize(mDimString, tempFile);
      var mDimString2 = (string[,])new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(string.Join(' ', mDimString), string.Join(' ', mDimString2));
    }

    [StorableType("59E73F41-B9D4-489B-AA9C-3A72173498CC")]
    public class NestedType {
      [Storable]
      private string value = "value";
      public override bool Equals(object obj) {
        NestedType nt = obj as NestedType;
        if (nt == null)
          throw new NotSupportedException();
        return nt.value == value;
      }
      public override int GetHashCode() {
        return value.GetHashCode();
      }

      [StorableConstructor]
      protected NestedType(StorableConstructorFlag _) {
      }
      public NestedType() {
      }
    }

    [TestMethod]
    public void NestedTypeTest() {
      NestedType t = new NestedType();
      new ProtoBufSerializer().Serialize(t, tempFile);
      var t2 = (NestedType)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(t, t2);
    }


    [TestMethod]
    public void SimpleArray() {
      string[] strings = { "one", "two", "three" };
      new ProtoBufSerializer().Serialize(strings, tempFile);
      var strings2 = (string[])new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(string.Join(' ', strings), string.Join(' ', strings2));
    }

    [TestMethod]
    public void PrimitiveRoot() {
      var f = 12.3f;
      new ProtoBufSerializer().Serialize(f, tempFile);
      var f2 = (float)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(f, f2);
    }

    [TestMethod]
    public void TestEnums() {
      EnumTest et = new EnumTest();
      et.simpleEnum = SimpleEnum.two;
      et.complexEnum = ComplexEnum.three;
      et.trickyEnum = TrickyEnum.two | TrickyEnum.one;
      new ProtoBufSerializer().Serialize(et, tempFile);
      EnumTest newEt = (EnumTest)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(et.simpleEnum, SimpleEnum.two);
      Assert.AreEqual(et.complexEnum, ComplexEnum.three);
      Assert.AreEqual(et.trickyEnum, (TrickyEnum)3);
    }

    [TestMethod]
    public void TestAliasingWithOverriddenEquals() {
      List<IntWrapper> ints = new List<IntWrapper>();
      ints.Add(new IntWrapper(1));
      ints.Add(new IntWrapper(1));
      Assert.AreEqual(ints[0], ints[1]);
      Assert.AreNotSame(ints[0], ints[1]);
      new ProtoBufSerializer().Serialize(ints, tempFile);
      List<IntWrapper> newInts = (List<IntWrapper>)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(newInts[0].Value, 1);
      Assert.AreEqual(newInts[1].Value, 1);
      Assert.AreEqual(newInts[0], newInts[1]);
      Assert.AreNotSame(newInts[0], newInts[1]);
    }

    [TestMethod]
    public void TestSavingException() {
      List<int> list = new List<int> { 1, 2, 3 };
      new ProtoBufSerializer().Serialize(list, tempFile);
      NonSerializable s = new NonSerializable();
      try {
        new ProtoBufSerializer().Serialize(s, tempFile);
        Assert.Fail("Exception expected");
      } catch (PersistenceException) { }
      List<int> newList = (List<int>)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(list[0], newList[0]);
      Assert.AreEqual(list[1], newList[1]);
    }

    [TestMethod]
    public void TestMultipleFailure() {
      List<NonSerializable> l = new List<NonSerializable>();
      l.Add(new NonSerializable());
      l.Add(new NonSerializable());
      l.Add(new NonSerializable());
      try {
        var s = new ProtoBufSerializer();
        s.Serialize(l);
        Assert.Fail("Exception expected");
      } catch (PersistenceException px) {
      }
    }

    [TestMethod]
    public void InheritanceTest() {
      New n = new New();
      new ProtoBufSerializer().Serialize(n, tempFile);
      New nn = (New)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(n.Name, nn.Name);
      Assert.AreEqual(((Override)n).Name, ((Override)nn).Name);
    }

    [StorableType("78636BDB-03B9-4BA1-979D-358997AA8063")]
    class Child {
      [Storable]
      public GrandParent grandParent;

      [StorableConstructor]
      protected Child(StorableConstructorFlag _) {
      }
      public Child() {
      }
    }

    [StorableType("B90F2371-DE30-48ED-BDAA-671B175C5698")]
    class Parent {
      [Storable]
      public Child child;

      [StorableConstructor]
      protected Parent(StorableConstructorFlag _) {
      }
      public Parent() {
      }
    }

    [StorableType("C48C28A9-F197-4B75-A21D-F21EF6AC0602")]
    class GrandParent {
      [Storable]
      public Parent parent;

      [StorableConstructor]
      protected GrandParent(StorableConstructorFlag _) {
      }
      public GrandParent() {
      }
    }

    [TestMethod]
    public void InstantiateParentChainReference() {
      GrandParent gp = new GrandParent();
      gp.parent = new Parent();
      gp.parent.child = new Child();
      gp.parent.child.grandParent = gp;
      Assert.AreSame(gp, gp.parent.child.grandParent);
      new ProtoBufSerializer().Serialize(gp, tempFile);
      GrandParent newGp = (GrandParent)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreSame(newGp, newGp.parent.child.grandParent);
    }

    [StorableType("FB4F08BB-6B65-4FBE-BA72-531DB2194F1F")]
    struct TestStruct {
      int value;
      int PropertyValue { get; set; }
      public TestStruct(int value)
        : this() {
        this.value = value;
        PropertyValue = value;
      }
    }

    [TestMethod]
    public void StructTest() {
      TestStruct s = new TestStruct(10);
      new ProtoBufSerializer().Serialize(s, tempFile);
      TestStruct newS = (TestStruct)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(s, newS);
    }

    [TestMethod]
    public void PointTest() {
      Point p = new Point(12, 34);
      new ProtoBufSerializer().Serialize(p, tempFile);
      Point newP = (Point)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(p, newP);
    }

    [TestMethod]
    public void NullableValueTypes() {
      double?[] d = new double?[] { null, 1, 2, 3 };
      new ProtoBufSerializer().Serialize(d, tempFile);
      double?[] newD = (double?[])new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(d[0], newD[0]);
      Assert.AreEqual(d[1], newD[1]);
      Assert.AreEqual(d[2], newD[2]);
      Assert.AreEqual(d[3], newD[3]);
    }

    [StorableType("5924A5A2-24C7-4588-951E-61212B041B0A")]
    private class PersistenceHooks {
      [Storable]
      public int a;
      [Storable]
      public int b;
      public int sum;
      public bool WasSerialized { get; private set; }
      [StorableHook(HookType.BeforeSerialization)]
      void PreSerializationHook() {
        WasSerialized = true;
      }
      [StorableHook(HookType.AfterDeserialization)]
      void PostDeserializationHook() {
        sum = a + b;
      }

      [StorableConstructor]
      protected PersistenceHooks(StorableConstructorFlag _) {
      }
      public PersistenceHooks() {
      }
    }

    [TestMethod]
    public void HookTest() {
      PersistenceHooks hookTest = new PersistenceHooks();
      hookTest.a = 2;
      hookTest.b = 5;
      Assert.IsFalse(hookTest.WasSerialized);
      Assert.AreEqual(hookTest.sum, 0);
      new ProtoBufSerializer().Serialize(hookTest, tempFile);
      Assert.IsTrue(hookTest.WasSerialized);
      Assert.AreEqual(hookTest.sum, 0);
      PersistenceHooks newHookTest = (PersistenceHooks)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(newHookTest.a, hookTest.a);
      Assert.AreEqual(newHookTest.b, hookTest.b);
      Assert.AreEqual(newHookTest.sum, newHookTest.a + newHookTest.b);
      Assert.IsFalse(newHookTest.WasSerialized);
    }

    [StorableType("35824217-F1BC-450F-BB40-9B0A4F7C7582")]
    private class CustomConstructor {
      public string Value = "none";
      public CustomConstructor() {
        Value = "default";
      }
      [StorableConstructor]
      private CustomConstructor(StorableConstructorFlag _) {
        Value = "persistence";
      }
    }

    [TestMethod]
    public void TestCustomConstructor() {
      CustomConstructor cc = new CustomConstructor();
      Assert.AreEqual(cc.Value, "default");
      new ProtoBufSerializer().Serialize(cc, tempFile);
      CustomConstructor newCC = (CustomConstructor)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(newCC.Value, "persistence");
    }

    [TestMethod]
    public void TestStreaming() {
      using (MemoryStream stream = new MemoryStream()) {
        Root r = InitializeComplexStorable();
        var ser = new ProtoBufSerializer();
        ser.Serialize(r, stream);
        using (MemoryStream stream2 = new MemoryStream(stream.ToArray())) {
          Root newR = (Root)ser.Deserialize(stream2);
          CompareComplexStorables(r, newR);
        }
      }
    }

    [StorableType("7CD5F148-397E-4539-88E0-EE19907E8BA6")]
    public class HookInheritanceTestBase {
      [Storable]
      public object a;
      public object link;
      [StorableHook(HookType.AfterDeserialization)]
      private void relink() {
        link = a;
      }

      [StorableConstructor]
      protected HookInheritanceTestBase(StorableConstructorFlag _) {
      }
      public HookInheritanceTestBase() {
      }
    }

    [StorableType("79E3EF89-A19A-408B-A18C-BFEB345159F0")]
    public class HookInheritanceTestDerivedClass : HookInheritanceTestBase {
      [Storable]
      public object b;
      [StorableHook(HookType.AfterDeserialization)]
      private void relink() {
        Assert.AreSame(a, link);
        link = b;
      }

      [StorableConstructor]
      protected HookInheritanceTestDerivedClass(StorableConstructorFlag _) : base(_) {
      }
      public HookInheritanceTestDerivedClass() {
      }
    }

    [TestMethod]
    public void TestLinkInheritance() {
      HookInheritanceTestDerivedClass c = new HookInheritanceTestDerivedClass();
      c.a = new object();
      new ProtoBufSerializer().Serialize(c, tempFile);
      HookInheritanceTestDerivedClass newC = (HookInheritanceTestDerivedClass)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreSame(c.b, c.link);
    }

    [StorableType(StorableMemberSelection.AllFields, "B32F5C7A-F1C5-4B96-8A61-01E0DB1C526B")]
    public class AllFieldsStorable {
      public int Value1 = 1;
      [Storable]
      public int Value2 = 2;
      public int Value3 { get; private set; }
      public int Value4 { get; private set; }
      [StorableConstructor]
      protected AllFieldsStorable(StorableConstructorFlag _) { }
      public AllFieldsStorable() {
        Value1 = 12;
        Value2 = 23;
        Value3 = 34;
        Value4 = 56;
      }
    }

    [TestMethod]
    public void TestStorableClassDiscoveryAllFields() {
      AllFieldsStorable afs = new AllFieldsStorable();
      new ProtoBufSerializer().Serialize(afs, tempFile);
      AllFieldsStorable newAfs = (AllFieldsStorable)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(afs.Value1, newAfs.Value1);
      Assert.AreEqual(afs.Value2, newAfs.Value2);
      Assert.AreEqual(0, newAfs.Value3);
      Assert.AreEqual(0, newAfs.Value4);
    }

    [StorableType(StorableMemberSelection.AllProperties, "60EE99CA-B391-4211-9FFB-2677490B33B6")]
    public class AllPropertiesStorable {
      public int Value1 = 1;
      [Storable]
      public int Value2 = 2;
      public int Value3 { get; private set; }
      public int Value4 { get; private set; }
      [StorableConstructor]
      protected AllPropertiesStorable(StorableConstructorFlag _) { }
      public AllPropertiesStorable() {
        Value1 = 12;
        Value2 = 23;
        Value3 = 34;
        Value4 = 56;
      }
    }

    [TestMethod]
    public void TestStorableClassDiscoveryAllProperties() {
      AllPropertiesStorable afs = new AllPropertiesStorable();
      new ProtoBufSerializer().Serialize(afs, tempFile);
      AllPropertiesStorable newAfs = (AllPropertiesStorable)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(1, newAfs.Value1);
      Assert.AreEqual(2, newAfs.Value2);
      Assert.AreEqual(afs.Value3, newAfs.Value3);
      Assert.AreEqual(afs.Value4, newAfs.Value4);

    }

    [StorableType(StorableMemberSelection.AllFieldsAndAllProperties, "97FAFC16-EC58-44CC-A833-CB951C0DD23B")]
    public class AllFieldsAndAllPropertiesStorable {
      public int Value1 = 1;
      [Storable]
      public int Value2 = 2;
      public int Value3 { get; private set; }
      public int Value4 { get; private set; }
      [StorableConstructor]
      protected AllFieldsAndAllPropertiesStorable(StorableConstructorFlag _) { }
      public AllFieldsAndAllPropertiesStorable() {
        Value1 = 12;
        Value2 = 23;
        Value3 = 34;
        Value4 = 56;
      }
    }

    [TestMethod]
    public void TestStorableClassDiscoveryAllFieldsAndAllProperties() {
      AllFieldsAndAllPropertiesStorable afs = new AllFieldsAndAllPropertiesStorable();
      new ProtoBufSerializer().Serialize(afs, tempFile);
      AllFieldsAndAllPropertiesStorable newAfs = (AllFieldsAndAllPropertiesStorable)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(afs.Value1, newAfs.Value1);
      Assert.AreEqual(afs.Value2, newAfs.Value2);
      Assert.AreEqual(afs.Value3, newAfs.Value3);
      Assert.AreEqual(afs.Value4, newAfs.Value4);
    }

    [StorableType("74BDE240-59D5-48C9-9A2A-5D44750DAF78")]
    public class MarkedOnlyStorable {
      public int Value1 = 1;
      [Storable]
      public int Value2 = 2;
      public int Value3 { get; private set; }
      public int Value4 { get; private set; }
      [StorableConstructor]
      protected MarkedOnlyStorable(StorableConstructorFlag _) { }
      public MarkedOnlyStorable() {
        Value1 = 12;
        Value2 = 23;
        Value3 = 34;
        Value4 = 56;
      }
    }

    [TestMethod]
    public void TestStorableClassDiscoveryMarkedOnly() {
      MarkedOnlyStorable afs = new MarkedOnlyStorable();
      new ProtoBufSerializer().Serialize(afs, tempFile);
      MarkedOnlyStorable newAfs = (MarkedOnlyStorable)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(1, newAfs.Value1);
      Assert.AreEqual(afs.Value2, newAfs.Value2);
      Assert.AreEqual(0, newAfs.Value3);
      Assert.AreEqual(0, newAfs.Value4);
    }

    [TestMethod]
    public void TestLineEndings() {
      List<string> lineBreaks = new List<string> { "\r\n", "\n", "\r", "\n\r", Environment.NewLine };
      List<string> lines = new List<string>();
      foreach (var br in lineBreaks)
        lines.Add("line1" + br + "line2");
      new ProtoBufSerializer().Serialize(lines, tempFile);
      List<string> newLines = (List<string>)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(lines.Count, newLines.Count);
      for (int i = 0; i < lineBreaks.Count; i++) {
        Assert.AreEqual(lines[i], newLines[i]);
      }
    }

    [TestMethod]
    public void TestSpecialNumbers() {
      List<double> specials = new List<double>() { 1.0 / 0, -1.0 / 0, 0.0 / 0 };
      Assert.IsTrue(double.IsPositiveInfinity(specials[0]));
      Assert.IsTrue(double.IsNegativeInfinity(specials[1]));
      Assert.IsTrue(double.IsNaN(specials[2]));
      new ProtoBufSerializer().Serialize(specials, tempFile);
      List<double> newSpecials = (List<double>)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.IsTrue(double.IsPositiveInfinity(newSpecials[0]));
      Assert.IsTrue(double.IsNegativeInfinity(newSpecials[1]));
      Assert.IsTrue(double.IsNaN(newSpecials[2]));
    }

    [TestMethod]
    public void TestCompactNumberArraySerializer() {
      System.Random r = new System.Random();
      double[] a = new double[20000];
      for (int i = 0; i < a.Length; i++)
        a[i] = r.Next(10);
      new ProtoBufSerializer().Serialize(a, tempFile);
      double[] newA = (double[])new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(a.Length, newA.Length);
      for (int i = 0; i < a.Rank; i++) {
        Assert.AreEqual(a.GetLength(i), newA.GetLength(i));
        Assert.AreEqual(a.GetLowerBound(i), newA.GetLowerBound(i));
      }
      for (int i = 0; i < a.Length; i++) {
        Assert.AreEqual(a[i], newA[i]);
      }
    }
    [StorableType("A174C85C-3B7C-477D-9E6C-121301470DDE")]
    private class IdentityComparer<T> : IEqualityComparer<T> {

      public bool Equals(T x, T y) {
        return x.Equals(y);
      }

      public int GetHashCode(T obj) {
        return obj.GetHashCode();
      }

      [StorableConstructor]
      protected IdentityComparer(StorableConstructorFlag _) {
      }
      public IdentityComparer() {
      }
    }

    [TestMethod]
    public void TestHashSetSerializer() {
      var hashSets = new List<HashSet<int>>() {
        new HashSet<int>(new[] { 1, 2, 3 }),
        new HashSet<int>(new[] { 4, 5, 6 }, new IdentityComparer<int>()),
      };
      new ProtoBufSerializer().Serialize(hashSets, tempFile);
      var newHashSets = (List<HashSet<int>>)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.IsTrue(newHashSets[0].Contains(1));
      Assert.IsTrue(newHashSets[0].Contains(2));
      Assert.IsTrue(newHashSets[0].Contains(3));
      Assert.IsTrue(newHashSets[1].Contains(4));
      Assert.IsTrue(newHashSets[1].Contains(5));
      Assert.IsTrue(newHashSets[1].Contains(6));
      Assert.AreEqual(newHashSets[0].Comparer.GetType(), new HashSet<int>().Comparer.GetType());
      Assert.AreEqual(newHashSets[1].Comparer.GetType(), typeof(IdentityComparer<int>));
    }

    [TestMethod]
    public void TestConcreteDictionarySerializer() {
      var dictionaries = new List<Dictionary<int, int>>() {
        new Dictionary<int, int>(),
        new Dictionary<int, int>(new IdentityComparer<int>()),
      };
      dictionaries[0].Add(1, 1);
      dictionaries[0].Add(2, 2);
      dictionaries[0].Add(3, 3);
      dictionaries[1].Add(4, 4);
      dictionaries[1].Add(5, 5);
      dictionaries[1].Add(6, 6);
      new ProtoBufSerializer().Serialize(dictionaries, tempFile);
      var newDictionaries = (List<Dictionary<int, int>>)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.IsTrue(newDictionaries[0].ContainsKey(1));
      Assert.IsTrue(newDictionaries[0].ContainsKey(2));
      Assert.IsTrue(newDictionaries[0].ContainsKey(3));
      Assert.IsTrue(newDictionaries[1].ContainsKey(4));
      Assert.IsTrue(newDictionaries[1].ContainsKey(5));
      Assert.IsTrue(newDictionaries[1].ContainsKey(6));
      Assert.IsTrue(newDictionaries[0].ContainsValue(1));
      Assert.IsTrue(newDictionaries[0].ContainsValue(2));
      Assert.IsTrue(newDictionaries[0].ContainsValue(3));
      Assert.IsTrue(newDictionaries[1].ContainsValue(4));
      Assert.IsTrue(newDictionaries[1].ContainsValue(5));
      Assert.IsTrue(newDictionaries[1].ContainsValue(6));
      Assert.AreEqual(new Dictionary<int, int>().Comparer.GetType(), newDictionaries[0].Comparer.GetType());
      Assert.AreEqual(typeof(IdentityComparer<int>), newDictionaries[1].Comparer.GetType());
    }

    [StorableType("A5DAC970-4E03-4B69-A95A-9DAC683D051F")]
    public class ReadOnlyFail {
      [Storable]
      public string ReadOnly {
        get { return "fail"; }
      }

      [StorableConstructor]
      protected ReadOnlyFail(StorableConstructorFlag _) {
      }
      public ReadOnlyFail() {
      }
    }

    [TestMethod]
    public void TestReadOnlyFail() {
      try {
        new ProtoBufSerializer().Serialize(new ReadOnlyFail(), tempFile);
        Assert.Fail("Exception expected");
      } catch (PersistenceException) {
      } catch {
        Assert.Fail("PersistenceException expected");
      }
    }


    [StorableType("653EBC18-E461-4F5C-8FD6-9F588AAC70D9")]
    public class WriteOnlyFail {
      [Storable]
      public string WriteOnly {
        set { throw new InvalidOperationException("this property should never be set."); }
      }

      [StorableConstructor]
      protected WriteOnlyFail(StorableConstructorFlag _) {
      }
      public WriteOnlyFail() {
      }
    }

    [TestMethod]
    public void TestWriteOnlyFail() {
      try {
        new ProtoBufSerializer().Serialize(new WriteOnlyFail(), tempFile);
        Assert.Fail("Exception expected");
      } catch (PersistenceException) {
      } catch {
        Assert.Fail("PersistenceException expected.");
      }
    }

    [StorableType("67BEAF29-9D7C-4C82-BD9F-9957798D6A2D")]
    public class OneWayTest {
      [StorableConstructor]
      protected OneWayTest(StorableConstructorFlag _) {
      }

      public OneWayTest() { this.value = "default"; }
      public string value;
      [Storable(AllowOneWay = true)]
      public string ReadOnly {
        get { return "ReadOnly"; }
      }
      [Storable(AllowOneWay = true)]
      public string WriteOnly {
        set { this.value = value; }
      }
    }

    [TestMethod]
    public void TupleTest() {
      var t1 = Tuple.Create(1);
      var t2 = Tuple.Create('1', "2");
      var t3 = Tuple.Create(3.0, 3f, 5);
      var t4 = Tuple.Create(Tuple.Create(1, 2, 3), Tuple.Create(4, 5, 6), Tuple.Create(8, 9, 10));
      var tuple = Tuple.Create(t1, t2, t3, t4);
      new ProtoBufSerializer().Serialize(tuple, tempFile);
      var newTuple = (Tuple<Tuple<int>, Tuple<char, string>, Tuple<double, float, int>, Tuple<Tuple<int, int, int>, Tuple<int, int, int>, Tuple<int, int, int>>>)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(tuple, newTuple);
    }
    [StorableType("6923FC3A-AC33-4CA9-919F-9707C00A663B")]
    public class G<T, T2> {
      [StorableType("16B88964-ECB3-4B41-95BC-EE3BE908CE4A")]
      public class S { }
      [StorableType("23CC1C7C-031E-4CBD-A87A-8F2235803BB4")]
      public class S2<T3, T4> { }
    }

    [TestMethod]
    public void TestInternalClassOfGeneric() {
      var s = new G<int, char>.S();
      new ProtoBufSerializer().Serialize(s, tempFile);
      var s1 = new ProtoBufSerializer().Deserialize(tempFile);
    }

    [TestMethod]
    public void TestInternalClassOfGeneric2() {
      var s = new G<int, float>.S2<int, char>();
      new ProtoBufSerializer().Serialize(s, tempFile);
      var s1 = new ProtoBufSerializer().Deserialize(tempFile);
    }

    [TestMethod]
    public void TestSpecialCharacters() {
      var s = "abc" + "\x15" + "def";
      new ProtoBufSerializer().Serialize(s, tempFile);
      var newS = new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(s, newS);
    }

    [TestMethod]
    public void TestByteArray() {
      var b = new byte[3];
      b[0] = 0;
      b[1] = 200;
      b[2] = byte.MaxValue;
      new ProtoBufSerializer().Serialize(b, tempFile);
      var newB = (byte[])new ProtoBufSerializer().Deserialize(tempFile);
      CollectionAssert.AreEqual(b, newB);
    }

    [TestMethod]
    public void TestOptionalNumberEnumerable() {
      var values = new List<double?> { 0, null, double.NaN, double.PositiveInfinity, double.MaxValue, 1 };
      new ProtoBufSerializer().Serialize(values, tempFile);
      var newValues = (List<double?>)new ProtoBufSerializer().Deserialize(tempFile);
      CollectionAssert.AreEqual(values, newValues);
    }

    [TestMethod]
    public void TestOptionalDateTimeEnumerable() {
      var values = new List<DateTime?> { DateTime.MinValue, null, DateTime.Now, DateTime.Now.Add(TimeSpan.FromDays(1)),
        DateTime.ParseExact("10.09.2014 12:21", "dd.MM.yyyy hh:mm", CultureInfo.InvariantCulture), DateTime.MaxValue};
      new ProtoBufSerializer().Serialize(values, tempFile);
      var newValues = (List<DateTime?>)new ProtoBufSerializer().Deserialize(tempFile);
      CollectionAssert.AreEqual(values, newValues);
    }

    [TestMethod]
    public void TestStringEnumerable() {
      var values = new List<string> { "", null, "s", "string", string.Empty, "123", "<![CDATA[nice]]>", "<![CDATA[nasty unterminated" };
      new ProtoBufSerializer().Serialize(values, tempFile);
      var newValues = (List<String>)new ProtoBufSerializer().Deserialize(tempFile);
      CollectionAssert.AreEqual(values, newValues);
    }

    [TestMethod]
    public void TestUnicodeCharArray() {
      var s = Encoding.UTF8.GetChars(new byte[] { 0, 1, 2, 03, 04, 05, 06, 07, 08, 09, 0xa, 0xb });
      new ProtoBufSerializer().Serialize(s, tempFile);
      var newS = (char[])new ProtoBufSerializer().Deserialize(tempFile);
      CollectionAssert.AreEqual(s, newS);
    }

    [TestMethod]
    public void TestUnicode() {
      var s = Encoding.UTF8.GetString(new byte[] { 0, 1, 2, 03, 04, 05, 06, 07, 08, 09, 0xa, 0xb });
      new ProtoBufSerializer().Serialize(s, tempFile);
      var newS = new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(s, newS);
    }

    [TestMethod]
    public void TestGenericQueue() {
      var q = new Queue<int>(new[] { 1, 2, 3, 4, 0 });
      new ProtoBufSerializer().Serialize(q, tempFile);
      var newQ = (Queue<int>)new ProtoBufSerializer().Deserialize(tempFile);
      CollectionAssert.AreEqual(q, newQ);
    }

    [TestMethod]
    public void TestQueue() {
      var q = new Queue(new object[] { 1, 2, 3, 4, 0, true, 1d, 1l, 1f });
      new ProtoBufSerializer().Serialize(q, tempFile);
      var newQ = (Queue)new ProtoBufSerializer().Deserialize(tempFile);
      CollectionAssert.AreEqual(q, newQ);
    }

    [TestMethod]
    public void TestGenericStackOfBools() {
      var s = new Stack<bool>(new[] { true, false, false });
      new ProtoBufSerializer().Serialize(s, tempFile);
      var newS = (Stack<bool>)new ProtoBufSerializer().Deserialize(tempFile);
      CollectionAssert.AreEqual(s, newS);
    }

    [TestMethod]
    public void TestStackOfBools() {
      var s = new Stack(new[] { true, false, false });
      new ProtoBufSerializer().Serialize(s, tempFile);
      var newS = (Stack)new ProtoBufSerializer().Deserialize(tempFile);
      CollectionAssert.AreEqual(s, newS);
    }


    [StorableType("6075F1E8-948A-4AD8-8F5A-942B777852EC")]
    public class A {
      [Storable]
      public B B { get; set; }

      [Storable]
      public int i;
    }

    [StorableType("287BFEA0-6E27-4839-BCEF-D134FE738AC8")]
    public class B {
      [Storable]
      public A A { get; set; }
    }

    [TestMethod]
    public void TestIndirectCyclicReferences() {
      var test = new Func<A>(() => {
        var a = new A { i = 4 };
        var b = new B { A = a };
        a.B = b;
        return a;
      });

      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(test(), tempFile);
      object o = serializer.Deserialize(tempFile);
      var orig = test();
      A result = (A)o;
      Assert.AreEqual(orig.i, result.i);
      Assert.AreSame(orig.B.A, orig);
      Assert.AreSame(result.B.A, result);
    }


    [StorableType("B5128F37-E992-48AA-9E1A-05696886AC1E")]
    public class Cyclic {
      [Storable]
      public Cyclic C { get; set; }
    }

    [TestMethod]
    public void TestDirectCyclicReference() {
      var test = new Func<Cyclic>(() => {
        var c = new Cyclic();
        c.C = c;
        return c;
      });

      var orig = test();
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(orig, tempFile);
      object o = serializer.Deserialize(tempFile);
      Cyclic result = (Cyclic)o;
      Assert.AreSame(orig.C, orig);
      Assert.AreSame(result.C, result);
    }

    [StorableType("89cc7ca9-260b-4427-9722-c8529e441561")]
    internal class WorkflowTestBaseClass {
      public static readonly List<string> WorkflowLog = new List<string>();

      public int value;
      [Storable]
      public int Value {
        get { WorkflowLog.Add("get Base.Value"); return value; }
        set { WorkflowLog.Add("set Base.Value"); this.value = value; }
      }

      public int value2;
      [Storable]
      public virtual int VirtualValue {
        get { WorkflowLog.Add("get Base.VirtualValue"); return value2; }
        set { WorkflowLog.Add("set Base.VirtualValue"); this.value2 = value; }
      }

      [StorableConstructor]
      protected WorkflowTestBaseClass(StorableConstructorFlag _) {
        WorkflowLog.Add("Base.StorableCtor");
      }

      public WorkflowTestBaseClass() {
        WorkflowLog.Add("Base.Ctor");
        value = 12;
        value2 = 34;
      }

      [StorableHook(HookType.BeforeSerialization)]
      private void BeforeSerialization() {
        WorkflowLog.Add("Base.BeforeSerialization");
      }

      [StorableHook(HookType.AfterDeserialization)]
      private void AfterDeserialization() {
        WorkflowLog.Add("Base.AfterDeserialization");
      }
    }

    [StorableType("6f9d1b2d-b20f-47b1-a9c2-93fc39ce328b")]
    internal class WorkflowTestClass : WorkflowTestBaseClass {
      public int value;
      [Storable]
      public new int Value {
        get { WorkflowLog.Add("get Derived.Value"); return value; }
        set { WorkflowLog.Add("set Derived.Value"); this.value = value; }
      }

      public int value2;
      [Storable]
      public override int VirtualValue {
        get { WorkflowLog.Add("get Derived.VirtualValue"); return value2; }
        set { WorkflowLog.Add("set Derived.VirtualValue"); this.value2 = value; }
      }

      [StorableConstructor]
      protected WorkflowTestClass(StorableConstructorFlag _) : base(_) {
        WorkflowLog.Add("Derived.StorableCtor");
      }

      public WorkflowTestClass() : base() {
        WorkflowLog.Add("Derived.Ctor");
        value = 12;
        value2 = 34;
      }

      [StorableHook(HookType.BeforeSerialization)]
      private void BeforeSerialization() {
        WorkflowLog.Add("Derived.BeforeSerialization");
      }

      [StorableHook(HookType.AfterDeserialization)]
      private void AfterDeserialization() {
        WorkflowLog.Add("Derived.AfterDeserialization");
      }
    }

    [TestMethod]
    public void TestWorkflow() {
      // 1. beforeserialization hook for all objects
      // 1a. order of BeforeSerialization hook calls from derived to base
      // 2. serialize all objects (no order on fields can be assumed)
      // 2a. order of serialization from derived to base 
      // 3. deserialize all objects using StorableCtors 
      // 3a. order of StorableCtors from base to derived
      // 4. call AfterDeserialization hook for all objects
      // 4a. order of AfterDeserialization hook calls form base to derived

      WorkflowTestClass.WorkflowLog.Clear();
      var derived = new WorkflowTestClass();
      var serializer = new ProtoBufSerializer();
      serializer.Serialize(derived, tempFile);
      var o = (WorkflowTestClass)serializer.Deserialize(tempFile);
      var workflow = string.Join(Environment.NewLine, WorkflowTestClass.WorkflowLog.Select((str, i) => $"{1 + i}. {str}"));

      // fails because:
      // - Base.BeforeSerialization is not called on 4. but instead later after 5. and 6.
      // - in 8. we actually call Derived.VirtualValue (again!)
      // - Similarly, in 12. we also call Derived.VirtualValue instead of Base.VirtualValue
      Assert.AreEqual(@"1. Base.Ctor
2. Derived.Ctor
3. Derived.BeforeSerialization
4. Base.BeforeSerialization
5. get Derived.Value
6. get Derived.VirtualValue
7. get Base.Value
8. Base.StorableCtor
9. Derived.StorableCtor
10. set Base.Value
11. set Derived.Value
12. set Derived.VirtualValue
13. Base.AfterDeserialization
14. Derived.AfterDeserialization", workflow);

      Assert.AreEqual(derived.value, o.value);
      Assert.AreEqual(derived.value2, o.value2);
      Assert.AreNotEqual(((WorkflowTestBaseClass)derived).value2, ((WorkflowTestBaseClass)o).value2); // not equal because the field value2 is not marked storable and VirtualValue is overridden. 
    }

    [StorableType("A6922DE4-A344-42D2-BC9C-ECD1DB9001EE")]
    internal class ReadonlyStorableFieldClass {
      [Storable]
      public readonly StringValue s;
      [StorableConstructor]
      protected ReadonlyStorableFieldClass(StorableConstructorFlag _) { }
      public ReadonlyStorableFieldClass() { s = new StringValue("abc"); }
    }
    [TestMethod]
    public void TestReadonlyStorableFields() {
      var o = new ReadonlyStorableFieldClass();
      var ser = new ProtoBufSerializer();
      ser.Serialize(o, tempFile);
      var o1 = (ReadonlyStorableFieldClass)ser.Deserialize(tempFile);
      Assert.AreEqual(o.s.Value, o1.s.Value);
    }

    [TestMethod]
    public void BitmapTest() {
      Icon icon = System.Drawing.SystemIcons.Hand;
      Bitmap bitmap = icon.ToBitmap();
      new ProtoBufSerializer().Serialize(bitmap, tempFile);
      Bitmap newBitmap = (Bitmap)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(bitmap.Size, newBitmap.Size);
      for (int i = 0; i < bitmap.Size.Width; i++)
        for (int j = 0; j < bitmap.Size.Height; j++)
          Assert.AreEqual(bitmap.GetPixel(i, j), newBitmap.GetPixel(i, j));
    }


    [TestMethod]
    public void FontTest() {
      List<Font> fonts = new List<Font>() {
       new Font(FontFamily.GenericSansSerif, 12),
       new Font("Times New Roman", 21, FontStyle.Bold, GraphicsUnit.Pixel),
       new Font("Courier New", 10, FontStyle.Underline, GraphicsUnit.Document),
       new Font("Helvetica", 21, FontStyle.Strikeout, GraphicsUnit.Inch, 0, true),
     };
      new ProtoBufSerializer().Serialize(fonts, tempFile);
      var newFonts = (List<Font>)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(fonts[0], newFonts[0]);
      Assert.AreEqual(fonts[1], newFonts[1]);
      Assert.AreEqual(fonts[2], newFonts[2]);
      Assert.AreEqual(fonts[3], newFonts[3]);
    }

    [TestMethod]
    public void TestSerializedTypes() {
      var c = InitializeComplexStorable();
      var ser = new ProtoBufSerializer();
      ser.Serialize(c, tempFile, out SerializationInfo info);
      Assert.IsNotNull(info);
      Assert.IsTrue(info.SerializedTypes.Any());
      Console.WriteLine(string.Join(Environment.NewLine, info.SerializedTypes.Select(t => t.FullName)));
    }

    [TestMethod]
    public void TestBigMultiDimArray() {
      int k = 100;
      var bigArray = new float[k, k, k];
      for (int i = 0; i < k; i++)
        for (int j = 0; j < k; j++)
          for (int l = 0; l < k; l++)
            bigArray[i, j, l] = i * k * k + j * k + l;
      var ser = new ProtoBufSerializer();

      ser.Serialize(bigArray, tempFile);
      var bigArray2 = (float[,,])ser.Deserialize(tempFile);
      CollectionAssert.AreEqual(bigArray, bigArray2);
    }

    [TestMethod]
    public void TestCancellationOfArray() {
      int k = 300 * 300 * 300;
      var bigArray = new float[k];
      for (int i = 0; i < k; i++)
        bigArray[i] = i;
      var ser = new ProtoBufSerializer();
      var cancellationToken = new CancellationTokenSource(2000).Token;
      var existingFileInfo = new FileInfo(tempFile);
      var prevTime = existingFileInfo.CreationTime;
      var sw = new Stopwatch();
      sw.Start();
      ser.Serialize(bigArray, tempFile, cancellationToken);
      sw.Stop();

      var newFileInfo = new FileInfo(tempFile);
      var newTime = existingFileInfo.CreationTime;
      // file must be unchanged
      Assert.AreEqual(prevTime, newTime);
      Assert.AreEqual(existingFileInfo.Length, newFileInfo.Length);
    }

    [TestMethod]
    public void TestCancellationOfEnumerable() {
      int k = 300 * 300 * 10;
      var bigArray = new SimpleClass[k];
      for (int i = 0; i < k; i++)
        bigArray[i] = new SimpleClass();
      var ser = new ProtoBufSerializer();
      var cancellationToken = new CancellationTokenSource(2000).Token;
      var existingFileInfo = new FileInfo(tempFile);
      var prevTime = existingFileInfo.CreationTime;
      var sw = new Stopwatch();
      sw.Start();
      ser.Serialize(bigArray, tempFile, cancellationToken);
      sw.Stop();

      var newFileInfo = new FileInfo(tempFile);
      var newTime = existingFileInfo.CreationTime;
      // file must be unchanged
      Assert.AreEqual(prevTime, newTime);
      Assert.AreEqual(existingFileInfo.Length, newFileInfo.Length);
      // time 'til cancellation should be around 2sec
      Assert.IsTrue(sw.Elapsed.TotalSeconds > 2 && sw.Elapsed.TotalSeconds < 6);
    }

    [StorableType("B9C2AA20-A18C-4124-90B8-B181BF7691B4")]
    private class ListNode {
      [Storable]
      public ListNode Next;
      [Storable]
      public int Value;
    }
    private static ListNode MakeLinkedList(int size, Random rand) {
      var first = new ListNode();
      var cur = first;
      for (int i = 0; i < size; i++) {
        cur.Next = new ListNode() { Value = rand.Next() };
        cur = cur.Next;
      }
      return first;
    }
    [TestMethod]
    public void LinkedList() {
      var l = MakeLinkedList(5000, new Random());
      var ser = new ProtoBufSerializer();
      ser.Serialize(l, tempFile);

      var l2 = (ListNode)ser.Deserialize(tempFile);
      while (l != null && l2 != null) {
        Assert.AreEqual(l.Value, l.Value);
        l = l.Next;
        l2 = l2.Next;
      }
      Assert.AreEqual(l, l2);
    }

    [StorableType("A65B7AD9-12EB-4767-958B-86193AA3DA79")]
    public class MyStorable {
      [StorableHook(HookType.AfterDeserialization)]
      private void AfterDeserialization() {
        throw new NotImplementedException("Afterdeserialization is called"); // this should be thrown
      }

      [StorableConstructor]
      protected MyStorable(StorableConstructorFlag _) { }
      public MyStorable() { }
    }

    [StorableType("341D1976-A9B2-428B-8292-B1AA3EA4BAE7")]
    public class MyRoot {
      private List<MyStorable> storables;
      [Storable]
      public IEnumerable<MyStorable> Storables {
        get { return storables; }
        set { storables = new List<MyStorable>(value); }
      }

      [StorableConstructor]
      protected MyRoot(StorableConstructorFlag _) { }
      public MyRoot() {

        storables = new List<MyStorable>(new[] { new MyStorable() });
      }
    }
    [TestMethod]
    public void TestHooksWithinNonStorable() {
      var ser = new ProtoBufSerializer();

      var r = new MyRoot();
      ser.Serialize(r, tempFile);
      try {
        var r2 = (MyRoot)ser.Deserialize(tempFile);
        Assert.Fail("Expected exception in AfterDeserializationHook");
      } catch (NotImplementedException) {
        // this is ok
      }
    }

    [TestMethod]
    public void TestNullableArray() {
      {
        double?[,,,] a = new double?[2, 2, 1, 1];
        a[0, 0, 0, 0] = 1.0;
        a[0, 1, 0, 0] = null;
        a[1, 0, 0, 0] = 2.0;
        a[1, 1, 0, 0] = 3.0;
        var ser = new ProtoBufSerializer();
        ser.Serialize(a, tempFile);
        var arr2 = (double?[,,,])ser.Deserialize(tempFile);
        CollectionAssert.AreEqual(a, arr2);
      }
      {
        double?[,] a = new double?[2, 2];
        a[0, 0] = 1.0;
        a[0, 1] = null;
        a[1, 0] = 2.0;
        a[1, 1] = 3.0;
        var ser = new ProtoBufSerializer();
        ser.Serialize(a, tempFile);
        var arr2 = (double?[,])ser.Deserialize(tempFile);
        CollectionAssert.AreEqual(a, arr2);
      }
    }

    [TestMethod]
    public void TestSpecialMultiDimArray() {
      {
        object[,,,] arr = new object[0, 0, 0, 0];
        var ser = new ProtoBufSerializer();
        ser.Serialize(arr, tempFile);
        var arr2 = (object[,,,])ser.Deserialize(tempFile);
        Assert.AreEqual(arr.Length, arr2.Length);
      }
      {
        float[,] arr = (float[,])Array.CreateInstance(typeof(float), lengths: new int[] { 100, 100 }, lowerBounds: new int[] { 1, 1 });
        for (int i = 1; i < 101; i++)
          for (int j = 1; j < 101; j++)
            arr[i, j] = 1.0f;
        var ser = new ProtoBufSerializer();
        ser.Serialize(arr, tempFile);
        var arr2 = (float[,])ser.Deserialize(tempFile);
        Assert.AreEqual(arr.GetLength(0), arr2.GetLength(0));
        Assert.AreEqual(arr.GetLength(1), arr2.GetLength(1));
        Assert.AreEqual(arr.GetLowerBound(0), arr2.GetLowerBound(0));
        Assert.AreEqual(arr.GetLowerBound(1), arr2.GetLowerBound(1));

        for (int i = 1; i < 101; i++)
          for (int j = 1; j < 101; j++)
            Assert.AreEqual(1.0f, arr2[i, j]);
      }
    }

    [TestMethod]
    public void TestSpecialMultiDimStringArray() {
      string[,,,] arr = (string[,,,])Array.CreateInstance(typeof(string), lengths: new int[] { 2, 3, 4, 5 }, lowerBounds: new int[] { -1, 0, 1, 2 });
      for (int i = 0; i < 2; i++)
        for (int j = 0; j < 3; j++)
          for (int k = 0; k < 4; k++)
            for (int l = 0; l < 4; l++)
              arr[i - 1, j + 0, k + 1, l + 2] = "abc";

      var ser = new ProtoBufSerializer();
      ser.Serialize(arr, tempFile);
      var arr2 = (string[,,,])ser.Deserialize(tempFile);
      Assert.AreEqual(arr.GetLength(0), arr2.GetLength(0));
      Assert.AreEqual(arr.GetLength(1), arr2.GetLength(1));
      Assert.AreEqual(arr.GetLowerBound(0), arr2.GetLowerBound(0));
      Assert.AreEqual(arr.GetLowerBound(1), arr2.GetLowerBound(1));

      for (int i = 0; i < 2; i++)
        for (int j = 0; j < 3; j++)
          for (int k = 0; k < 4; k++)
            for (int l = 0; l < 4; l++)
              Assert.AreEqual("abc", arr2[i - 1, j + 0, k + 1, l + 2]);
    }


    [StorableType("4624BEBE-C795-4A80-B5FC-87B99BFD214E")]
    public class ObjectFilledOnAssignment_A {
      private ObjectFilledOnAssignment_B b;
      [Storable]
      public ObjectFilledOnAssignment_B Value {
        get { return b; }
        set {
          b = value;
          //  a should be completely restored at this point. Check whether the a.o property is set
          Assert.IsNotNull(b.Value);
        }
      }

      [StorableConstructor]
      public ObjectFilledOnAssignment_A(StorableConstructorFlag _) { }
      public ObjectFilledOnAssignment_A() {
      }
    }


    [StorableType("E21F1305-8C8E-4E9F-9717-9586EE3A560E")]
    public class ObjectFilledOnAssignment_B {
      private ObjectFilledOnAssignment_A b;
      [Storable]
      public ObjectFilledOnAssignment_A Value {
        get { return b; }
        set {
          b = value;
          //  b should be completely restored at this point. Check whether the b.A property is set
          Assert.IsNotNull(b.Value);
        }
      }

      [StorableConstructor]
      public ObjectFilledOnAssignment_B(StorableConstructorFlag _) { }
      public ObjectFilledOnAssignment_B() {
      }
    }

    [TestMethod]
    public void TestObjectFilledOnAssignment() {
      var test_a = new ObjectFilledOnAssignment_A();
      var test_b = new ObjectFilledOnAssignment_B();
      test_a.Value = test_b;
      test_b.Value = test_a;
      var ser = new ProtoBufSerializer();
      ser.Serialize(test_a, tempFile);
      var test2 = (ObjectFilledOnAssignment_A)ser.Deserialize(tempFile);
      Assert.IsNotNull(test2.Value);
      Assert.IsNotNull(test2.Value.Value);
    }

    #region backwards compatibility tests
    #region helpers

    public static void RegisterType(Guid guid, Type type) {
      Mapper.StaticCache.RegisterType(guid, type);
    }

    public static void DeregisterType(Guid guid) {
      if (!guid2Type.ContainsKey(guid)) return;
      type2Guid.Remove(guid2Type[guid]);
      guid2Type.Remove(guid);
    }

    public static TypeInfo GetTypeInfo(Type type) {
      return Mapper.StaticCache.GetTypeInfo(type);
    }

    private static void ReplaceTypeImplementation(Type old, Guid oldGuid, Type @new) {
      DeregisterType(oldGuid);
      DeregisterType(StorableTypeAttribute.GetStorableTypeAttribute(@new).Guid);

      RegisterType(oldGuid, @new);
      SetTypeGuid(@new, oldGuid);
      typeInfos.Remove(old);
    }

    public static void SetTypeGuid(Type type, Guid guid) {
      var typeInfo = GetTypeInfo(type);
      guidPropertyInfo.SetValue(typeInfo.StorableTypeAttribute, guid);
      var reflectMethod = typeInfo.GetType().GetMethod("Reflect", BindingFlags.NonPublic | BindingFlags.Instance);
      reflectMethod.Invoke(typeInfo, new object[0]);
    }
    #endregion

    [StorableType("28A5F6B8-49AF-4C6A-AF0E-F92EB4511722")]
    private class PersistenceTestA0 {
      [Storable]
      public IntValue v;
    }

    [StorableType("00000000-0000-0000-0000-0000000000A1")]
    private class PersistenceTestA1 {
      [Storable(OldName = "v")]
      private IntValue v1 {
        set { v = value.Value; }
      }

      [Storable(Name = "v2")]
      public int v;
    }

    [StorableType("00000000-0000-0000-0000-0000000000A2")]
    private class PersistenceTestA2 {
      [Storable(OldName = "v")]
      private IntValue v1 {
        set { v2 = value.Value; }
      }

      [Storable(OldName = "v2")]
      private int v2 {
        set { v = (double)value; }
      }

      [Storable(Name = "v3")]
      public double v;
    }

    [TestMethod]
    public void TestConversionSample1() {
      var v0Type = typeof(PersistenceTestA0);
      var v0Guid = StorableTypeAttribute.GetStorableTypeAttribute(v0Type).Guid;

      var test = new Func<PersistenceTestA0>(() => {
        return new PersistenceTestA0() { v = new IntValue(1337) };
      });

      ProtoBufSerializer serializer = new ProtoBufSerializer();
      var v0 = test();
      serializer.Serialize(v0, tempFile);

      ReplaceTypeImplementation(v0Type, v0Guid, typeof(PersistenceTestA1));

      var v1 = (PersistenceTestA1)serializer.Deserialize(tempFile);
      Assert.AreEqual(v0.v.Value, v1.v);

      ReplaceTypeImplementation(typeof(PersistenceTestA1), v0Guid, typeof(PersistenceTestA2));

      var v2 = (PersistenceTestA2)serializer.Deserialize(tempFile);
      Assert.AreEqual(v2.v, v1.v);
    }
    public void TestGenericList() {
      var l = new List<bool>(new[] { true, false, true });
      new ProtoBufSerializer().Serialize(l, tempFile);
      var newL = (List<bool>)new ProtoBufSerializer().Deserialize(tempFile);
      CollectionAssert.AreEqual(l, newL);
    }

    [StorableType("7DCED655-D724-492E-9C6A-A22B376BADB2")]
    private class PersistenceTestB0 {
      [Storable]
      public Point p;
    }

    [StorableType("00000000-0000-0000-0000-0000000000B1")]
    private class PersistenceTestB1 {
      [Storable(AllowOneWay = true)]
      private Point p {
        set {
          x = value.X;
          y = value.Y;
        }
      }

      [Storable]
      public int x;

      [Storable]
      public int y;
    }

    [StorableType("00000000-0000-0000-0000-0000000000B2")]
    private class PersistenceTestB2 {
      [Storable(AllowOneWay = true)]
      private int x { set { p = new Point(value, p.Y); } }

      [Storable(AllowOneWay = true)]
      private int y { set { p = new Point(p.X, value); } }

      [Storable]
      public Point p { get; private set; }
    }

    [TestMethod]
    public void TestConversionSample2() {
      var v0Type = typeof(PersistenceTestB0);
      var v0Guid = StorableTypeAttribute.GetStorableTypeAttribute(v0Type).Guid;

      var test = new Func<PersistenceTestB0>(() => {
        return new PersistenceTestB0() { p = new Point(1, 2) };
      });

      ProtoBufSerializer serializer = new ProtoBufSerializer();
      var v0 = test();

      // seriealize B0, deserialize B0
      serializer.Serialize(v0, tempFile);
      var newV0 = (PersistenceTestB0)serializer.Deserialize(tempFile);
      Assert.AreEqual(v0.p.X, newV0.p.X);
      Assert.AreEqual(v0.p.Y, newV0.p.Y);

      // serialize B0, deserialize B1
      ReplaceTypeImplementation(v0Type, v0Guid, typeof(PersistenceTestB1));
      var v1 = (PersistenceTestB1)serializer.Deserialize(tempFile);
      Assert.AreEqual(v0.p.X, v1.x);
      Assert.AreEqual(v0.p.Y, v1.y);

      // serialize B0, deserialize B2
      ReplaceTypeImplementation(v0Type, v0Guid, typeof(PersistenceTestB2));
      var v2 = (PersistenceTestB2)serializer.Deserialize(tempFile);
      Assert.AreEqual(v0.p.X, v2.p.X);
      Assert.AreEqual(v0.p.Y, v2.p.Y);

      // serialize B1, deserialize B1
      ReplaceTypeImplementation(v0Type, v0Guid, typeof(PersistenceTestB1));
      serializer.Serialize(v1, tempFile);
      var newV1 = (PersistenceTestB1)serializer.Deserialize(tempFile);
      Assert.AreEqual(v1.x, newV1.x);
      Assert.AreEqual(v1.y, newV1.y);

      // serialize B1, deserialize B2
      ReplaceTypeImplementation(v0Type, v0Guid, typeof(PersistenceTestB2));
      v2 = (PersistenceTestB2)serializer.Deserialize(tempFile);
      Assert.AreEqual(v1.x, v2.p.X);
      Assert.AreEqual(v1.y, v2.p.Y);

      // serialize B2, deserialize B2
      serializer.Serialize(v2, tempFile);
      var newV2 = (PersistenceTestB2)serializer.Deserialize(tempFile);
      Assert.AreEqual(v2.p.X, newV2.p.X);
      Assert.AreEqual(v2.p.Y, newV2.p.Y);
    }
    [TestMethod]
    public void TestList() {
      var l = new ArrayList(new object[] { 1, 2, 3, 4, 0, true, 1d, 1l, 1f });
      new ProtoBufSerializer().Serialize(l, tempFile);
      var newL = (ArrayList)new ProtoBufSerializer().Deserialize(tempFile);
      CollectionAssert.AreEqual(l, newL);
    }

    [StorableType("FB649DF5-5B99-45DE-807A-27E86CB22F4B")]
    private class PersistenceTestC0 {
      [Storable]
      public PersistenceTestC0 neighbor;

      [Storable]
      public int value;
    }

    [StorableType("00000000-0000-0000-0000-0000000000C1")]
    private class PersistenceTestC1 {
      [Storable]
      private PersistenceTestC1 neighbor;

      [Storable]
      public int neighborValue;

      [Storable]
      public int value;

      [StorableHook(HookType.AfterDeserialization)]
      private void AfterDeserialization() {
        if (neighbor != null)
          neighborValue = neighbor.value;
      }
    }

    [TestMethod]
    public void TestConversionSample3() {
      var v0Type = typeof(PersistenceTestC0);
      var v0Guid = StorableTypeAttribute.GetStorableTypeAttribute(v0Type).Guid;

      var test = new Func<PersistenceTestC0>(() => {
        var c0 = new PersistenceTestC0() {
          value = 90
        };
        c0.neighbor = c0;
        return c0;
      });

      ProtoBufSerializer serializer = new ProtoBufSerializer();
      var v0 = test();

      // seriealize C0, deserialize C0
      serializer.Serialize(v0, tempFile);
      var newV0 = (PersistenceTestC0)serializer.Deserialize(tempFile);
      Assert.AreEqual(v0.value, newV0.value);
      Assert.ReferenceEquals(newV0.neighbor, newV0);

      // serialize C0, deserialize C1
      ReplaceTypeImplementation(v0Type, v0Guid, typeof(PersistenceTestC1));
      var v1 = (PersistenceTestC1)serializer.Deserialize(tempFile);
      Assert.AreEqual(v0.value, v1.value);
      Assert.AreEqual(v1.neighborValue, v0.neighbor.value);
    }


    [StorableType("5AF91FB9-DB53-40BD-B961-A968D2F7CDE3")]
    private class PersistenceTestSample4A0 {
      [Storable]
      public string Name;

      [Storable]
      public string Description;
    }

    [StorableType("5EDF9D5D-3B60-42E9-BA8D-7A007916D6AE")]
    private class PersistenceTestSample4C0 : PersistenceTestSample4A0 {
      [Storable]
      public int p;
    }

    [StorableType("00000000-0000-0000-0000-0000000000D1")]
    private class PersistenceTestSample4A1 {
      [Storable]
      public string Name;
    }

    [StorableType("A55EE4D0-EE2F-4DBD-834E-51200BFDBB9C")]
    private class PersistenceTestSample4B : PersistenceTestSample4A1 {
      [Storable(OldName = "base.Description")]
      private string Description1 { set { Description = value; } }

      [Storable(Name = "Description2")]
      public string Description;
    }

    [StorableType("00000000-0000-0000-0000-0000000000E1")]
    private class PersistenceTestSample4C1 : PersistenceTestSample4B {
      [Storable]
      public int p;
    }

    [StorableType("8CB577B8-C36B-4749-8CF4-33DE4D5AC4BF")]
    class PersistenceTestSample4NewType0 : PersistenceTestSample4A1 {
      [Storable]
      public StringValue Description;
    }

    [StorableType("BCD9D34C-901C-44E6-B1FD-8F5E9E09D686")]
    class AB {
      [Storable(OldName = "5AF91FB9-DB53-40BD-B961-A968D2F7CDE3.Name", DefaultValue = "No name")]
      public string Name;

      [Storable(OldName = "A55EE4D0-EE2F-4DBD-834E-51200BFDBB9C.Description2", DefaultValue = "No description")]
      public string Description;
    }

    [StorableType("00000000-0000-0000-0000-0000000000F1")]
    class PersistenceTestSample4C2 : AB {
      [Storable]
      public int p;
    }

    [StorableType("00000000-0000-0000-0000-0000000001A1")]
    class PersistenceTestSample4NewType1 : AB {
      [Storable(OldName = "Description")]
      private StringValue Description_Persistence_Setter {
        set {
          base.Description = value.Value;
        }
      }
    }

    [TestMethod]
    public void TestConversionSample4() {
      var a0Type = typeof(PersistenceTestSample4A0);
      var a0Guid = StorableTypeAttribute.GetStorableTypeAttribute(a0Type).Guid;
      var c0Type = typeof(PersistenceTestSample4C0);
      var c0Guid = StorableTypeAttribute.GetStorableTypeAttribute(c0Type).Guid;

      var test = new Func<PersistenceTestSample4C0>(() => {
        return new PersistenceTestSample4C0 {
          Name = "test",
          Description = "description",
          p = 99
        };
      });

      ProtoBufSerializer serializer = new ProtoBufSerializer();
      var c0 = test();

      // seriealize C0, deserialize C0
      serializer.Serialize(c0, tempFile);
      var newC0 = (PersistenceTestSample4C0)serializer.Deserialize(tempFile);
      Assert.AreEqual(c0.Name, newC0.Name);
      Assert.AreEqual(c0.Description, newC0.Description);
      Assert.AreEqual(c0.p, newC0.p);



      // serialize C0, deserialize C1
      ReplaceTypeImplementation(c0Type, c0Guid, typeof(PersistenceTestSample4C1));
      ReplaceTypeImplementation(a0Type, a0Guid, typeof(PersistenceTestSample4A1));
      serializer = new ProtoBufSerializer();
      var c1 = (PersistenceTestSample4C1)serializer.Deserialize(tempFile);
      Assert.AreEqual(c0.Name, c1.Name);
      Assert.AreEqual(c0.Description, c1.Description);
      Assert.AreEqual(c0.p, c1.p);

      // serialize C1, deserialize C1
      serializer.Serialize(c1, tempFile);
      var newC1 = (PersistenceTestSample4C1)serializer.Deserialize(tempFile);
      Assert.AreEqual(c1.Name, newC1.Name);
      Assert.AreEqual(c1.Description, newC1.Description);
      Assert.AreEqual(c1.p, newC1.p);

      // serialize C1, deserialize C2
      serializer.Serialize(c1, tempFile);
      ReplaceTypeImplementation(c0Type, c0Guid, typeof(PersistenceTestSample4C2));
      var c2 = (PersistenceTestSample4C2)serializer.Deserialize(tempFile);
      Assert.AreEqual(c1.Name, c2.Name);
      Assert.AreEqual(c1.Description, c2.Description);
      Assert.AreEqual(c1.p, c2.p);

      // serialize NewType0, deserialize NewType1
      var newType0Type = typeof(PersistenceTestSample4NewType0);
      var newType0Guid = StorableTypeAttribute.GetStorableTypeAttribute(newType0Type).Guid;

      var newType0 = new PersistenceTestSample4NewType0 {
        Name = "yeah",
        Description = new StringValue("blubb")
      };

      serializer.Serialize(newType0, tempFile);
      ReplaceTypeImplementation(newType0Type, newType0Guid, typeof(PersistenceTestSample4NewType1));
      var newType1 = (PersistenceTestSample4NewType1)serializer.Deserialize(tempFile);
      Assert.AreEqual(newType0.Name, newType1.Name);
      Assert.AreEqual(newType0.Description.Value, newType1.Description);
    }

    // Attach StorableTypeAttribute twice (e.g. when a class is deleted).
    // This doesn't work yet because HEAL.Attic only allows a 1:1 relationship between type and GUID
    [StorableType("BA23926C-BAC8-4CD3-B7F5-65AE2B532857")]
    private interface IItem {
      string Value { get; }
    }

    [StorableType("C0B91371-31F8-4767-8BC1-CD25E2BCB203")]
    private class DeletedTypeSurrogate : IItem {
      public string Value => "deleted";
    }

    [StorableType("AD479BE0-CCF2-4380-BC98-FCE9C2404C39")]
    private class TypeToDelete : IItem {
      [Storable]
      public string Value { get; set; }
    }

    [TestMethod]
    public void DuplicateStorableTypeAttributeTest() {
      var d = new TypeToDelete();
      d.Value = "will be lost";
      var s = new DeletedTypeSurrogate();
      var l = new List<IItem>(new IItem[] { d, s }); // after deletion of TypeToDelete and deserialization both should be DeletedTypeSurrogate
      var ser = new ProtoBufSerializer();
      ser.Serialize(l, tempFile);


      var deletedType = typeof(TypeToDelete);
      var deletedTypeGuid = StorableTypeAttribute.GetStorableTypeAttribute(deletedType).Guid;

      var deletedTypeSurrogate = typeof(DeletedTypeSurrogate);
      var deletedTypeSurrogateGuid = StorableTypeAttribute.GetStorableTypeAttribute(deletedTypeSurrogate).Guid;

      DeregisterType(deletedTypeGuid);
      DeregisterType(deletedTypeSurrogateGuid);

      var l2 = (IList<IItem>)ser.Deserialize(tempFile);
      Assert.IsNull(l2[0]);

      Mapper.StaticCache.RegisterType(deletedTypeSurrogate, deletedTypeGuid, deletedTypeSurrogateGuid);

      var l3 = (IList<IItem>)ser.Deserialize(tempFile);
      Assert.AreEqual(l3.Count, 2);
      Assert.IsTrue(l3[0] is DeletedTypeSurrogate);
      Assert.IsTrue(l3[1] is DeletedTypeSurrogate);
      Assert.AreEqual(l3[0].Value, "deleted");
    }
    #endregion

  }
}
