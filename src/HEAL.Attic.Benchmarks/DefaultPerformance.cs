#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HEAL.Attic.Benchmarks {
  [SimpleJob(RuntimeMoniker.HostProcess)]
  [Orderer(SummaryOrderPolicy.Declared, MethodOrderPolicy.Declared)]
  [RPlotExporter]
  public class DefaultPerformance {
    protected Random rand = new Random(1234);

    protected virtual object CreateObject(int elements) { return null; }

    [ParamsSource(nameof(ElementsSource))]
    public int elements;
    public IEnumerable<int> ElementsSource => Enumerable.Range(13, 5).Select(e => 1 << e);


    private object obj;
    [GlobalSetup(Target = nameof(Serialize))]
    public void SetupSerialize() {
      obj = CreateObject(elements);
    }

    [Benchmark]
    public byte[] Serialize() {
      var serializer = new ProtoBufSerializer();
      long fileSize = 0;
      byte[] buf;

      using (var memStream = new MemoryStream()) {
        serializer.Serialize(obj, memStream, disposeStream: false);
        buf = memStream.GetBuffer();
        fileSize = memStream.Length;
      }

      return buf;
    }

    private byte[] buffer;
    [GlobalSetup(Target = nameof(Deserialize))]
    public void SetupDeserialize() {
      obj = CreateObject(elements);
      buffer = Serialize();

    }
    [Benchmark]
    public void Deserialize() {
      var serializer = new ProtoBufSerializer();
      using (var memStream = new MemoryStream(buffer)) {
        var obj = serializer.Deserialize(memStream);
      }
    }
  }
}
