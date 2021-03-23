#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion


using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HEAL.Attic.Benchmarks {
  [StorableType("4FBE3889-0944-469A-8B5A-82BAA2969A0D")]
  public class DictionaryTestData {
    public Dictionary<string, Tuple<double, double>> data = new Dictionary<string, Tuple<double, double>>();

    [StorableConstructor]
    public DictionaryTestData(StorableConstructorFlag _) { }
    public DictionaryTestData(int elements) {
      var rand = new System.Random();
      for (int i = 0; i < elements; i++) {
        data.Add(i.ToString(), Tuple.Create(i * 0.1, i * 2.0));
      }
    }

    public DictionaryTestData(Dictionary<string, Tuple<double, double>> data) {
      this.data = data;
    }
  }

  [StorableType("38D6B290-856C-4571-A399-3B973E4EDD8A")]
  public sealed class DirectSerialization : DictionaryTestData {
    [StorableConstructor]
    public DirectSerialization(StorableConstructorFlag _) : base(_) { }
    public DirectSerialization(Dictionary<string, Tuple<double, double>> data) : base(data) { }


    [Storable]
    private Dictionary<string, Tuple<double, double>> StorableInformation {
      get => base.data;
      set => base.data = value;
    }
  }


  [StorableType("FBD02568-49BC-4B72-A888-7E9B74FE93A3")]
  public sealed class KeyValuePairSerialization : DictionaryTestData {
    [StorableConstructor]
    public KeyValuePairSerialization(StorableConstructorFlag _) : base(_) { }
    public KeyValuePairSerialization(Dictionary<string, Tuple<double, double>> data) : base(data) { }

    [Storable]
    private KeyValuePair<string, double[]>[] StorableInformation {
      get {
        var l = new KeyValuePair<string, double[]>[data.Count];
        var counter = 0;
        foreach (var entry in data) {
          l[counter] = new KeyValuePair<string, double[]>(entry.Key, new double[] { entry.Value.Item1, entry.Value.Item1 });
          counter++;
        }
        return l;
      }
      set {
        foreach (var entry in value)
          data.Add(entry.Key, Tuple.Create(entry.Value[0], entry.Value[1]));
      }
    }
  }

  [StorableType("C0AA0AA9-B9CB-4E9C-BC63-78EDF8E2CEA8")]
  public sealed class EnumerableTuplesSerialization : DictionaryTestData {
    [StorableConstructor]
    private EnumerableTuplesSerialization(StorableConstructorFlag _) : base(_) { }
    public EnumerableTuplesSerialization(Dictionary<string, Tuple<double, double>> data) : base(data) { }

    [Storable]
    private IEnumerable<Tuple<string, double, double>> StorableInformation {
      get {
        var l = new List<Tuple<string, double, double>>();
        foreach (var entry in data) {
          l.Add(Tuple.Create(entry.Key, entry.Value.Item1, entry.Value.Item2));
        }
        return l;
      }
      set {
        foreach (var entry in value) {
          data.Add(entry.Item1, Tuple.Create(entry.Item2, entry.Item3));
        }
      }
    }
  }

  [StorableType("3023BFDA-0B3C-4C3B-8AAC-A06E60006445")]
  public sealed class ArraySerialization : DictionaryTestData {
    [StorableConstructor]
    public ArraySerialization(StorableConstructorFlag _) : base(_) { }
    public ArraySerialization(Dictionary<string, Tuple<double, double>> data) : base(data) { }

    [Storable]
    private Tuple<string[], double[], double[]> StorableInformation {
      get {
        var keys = data.Keys.ToArray();
        var values1 = data.Values.Select(t => t.Item1).ToArray();
        var values2 = data.Values.Select(t => t.Item2).ToArray();

        return Tuple.Create(keys, values1, values2);
      }
      set {
        var keys = value.Item1;
        var values1 = value.Item2;
        var values2 = value.Item3;

        for (int i = 0; i < keys.Length; i++) {
          data.Add(keys[i], Tuple.Create(values1[i], values2[i]));
        }
      }
    }
  }


  [SimpleJob(RuntimeMoniker.HostProcess)]
  public class ValueTypePerformance {
    private List<DictionaryTestData> data = new List<DictionaryTestData>();

    [Params(20, 100, 500)]
    public int duplicateObjects;

    [Params(200)]
    public int elements;

    [GlobalSetup]
    public void Setup() {
      for (int i = 0; i < duplicateObjects; i++)
        data.Add(new DictionaryTestData(elements));
    }

    public void Serialize(object obj) {
      var serializer = new ProtoBufSerializer();
      using (var memStream = new MemoryStream()) {
        serializer.Serialize(obj, memStream, false);
      }
    }

    [Benchmark]
    public void Serialize_KeyValuePair() {
      var obj = data.Select(d => new KeyValuePairSerialization(d.data)).ToList();
      Serialize(obj);
    }

    [Benchmark]
    public void Serialize_Direct() {
      var obj = data.Select(d => new DirectSerialization(d.data)).ToList();
      Serialize(obj);
    }

    [Benchmark]
    public void Serialize_EnumerableTuple() {
      var obj = data.Select(d => new EnumerableTuplesSerialization(d.data)).ToList();
      Serialize(obj);
    }


    [Benchmark]
    public void Serialize_Array() {
      var obj = data.Select(d => new ArraySerialization(d.data)).ToList();
      Serialize(obj);
    }
  }


}
