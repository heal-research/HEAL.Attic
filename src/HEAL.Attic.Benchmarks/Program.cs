#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion


using BenchmarkDotNet.Reports;

namespace HEAL.Attic.Benchmarks {
  public class Program {
    public static void Main(string[] args) {
      Summary summary;

      summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<UShortArrayPerformance>();
      summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<IntArrayPerformance>();
      summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<Int2ArrayPerformance>();
      summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<Int3ArrayPerformance>();
      summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<Int4ArrayPerformance>();
      summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<IntLimitedArrayPerformance>();
      summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<DoubleArrayPerformance>();
      summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<IntListPerformance>();
      summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<IntLimitedListPerformance>();
      summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<GraphPerformance>();
      summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<LinkedListPerformance>();

      //summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<ValueTypePerformance>();

      //    var x = new PerformanceTests();
      //    x.duplicateObjects = 500;
      //    x.elements = 200;
      //    x.Setup();
      //    x.Serialize_KeyValuePair();
    }
  }
}
