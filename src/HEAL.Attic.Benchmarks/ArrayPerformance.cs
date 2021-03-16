#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion


using System;
using System.Collections.Generic;

namespace HEAL.Attic.Benchmarks {
  public class UShortArrayPerformance : DefaultPerformance {
    protected override object CreateObject(int arrayLength) {
      var arr = new ushort[arrayLength];
      for (int i = 0; i < arr.Length; i++) {
        arr[i] = (ushort)rand.Next(ushort.MinValue, ushort.MaxValue);
      }
      return arr;
    }
  }

  public class IntArrayPerformance : DefaultPerformance {
    protected override object CreateObject(int arrayLength) {
      var arr = new int[arrayLength];
      for (int i = 0; i < arr.Length; i++) {
        arr[i] = rand.Next();
      }
      return arr;
    }
  }

  public class Int2ArrayPerformance : DefaultPerformance {
    protected override object CreateObject(int arrayLength) {
      var arr = new int[(int)Math.Sqrt(arrayLength), (int)Math.Sqrt(arrayLength)];
      for (int i = 0; i < arr.GetLength(0); i++) {
        for (int j = 0; j < arr.GetLength(1); j++) {
          arr[i, j] = rand.Next();
        }
      }
      return arr;
    }
  }


  public class Int3ArrayPerformance : DefaultPerformance {
    protected override object CreateObject(int arrayLength) {
      var arr = new int[(int)Math.Pow(arrayLength, 1.0 / 3), (int)Math.Pow(arrayLength, 1.0 / 3), (int)Math.Pow(arrayLength, 1.0 / 3)];
      for (int i = 0; i < arr.GetLength(0); i++) {
        for (int j = 0; j < arr.GetLength(1); j++) {
          for (int k = 0; k < arr.GetLength(2); k++) {
            arr[i, j, k] = rand.Next();
          }
        }
      }
      return arr;
    }
  }

  public class Int4ArrayPerformance : DefaultPerformance {
    protected override object CreateObject(int arrayLength) {
      var arr = new int[(int)Math.Pow(arrayLength, 1 / 4.0), (int)Math.Pow(arrayLength, 1 / 4.0), (int)Math.Pow(arrayLength, 1 / 4.0), (int)Math.Pow(arrayLength, 1 / 4.0)];
      for (int i = 0; i < arr.GetLength(0); i++) {
        for (int j = 0; j < arr.GetLength(1); j++) {
          for (int k = 0; k < arr.GetLength(2); k++) {
            for (int l = 0; l < arr.GetLength(3); l++) {
              arr[i, j, k, l] = rand.Next();
            }
          }
        }
      }
      return arr;
    }
  }

  public class IntLimitedArrayPerformance : DefaultPerformance {
    protected override object CreateObject(int arrayLength) {
      int maxInt = 20;
      var arr = new int[arrayLength];
      for (int i = 0; i < arr.Length; i++) {
        arr[i] = rand.Next(maxInt);
      }
      return arr;
    }
  }

  public class DoubleArrayPerformance : DefaultPerformance {
    protected override object CreateObject(int arrayLength) {
      var arr = new double[arrayLength];
      for (int i = 0; i < arr.Length; i++) {
        arr[i] = rand.NextDouble();
      }
      return arr;
    }
  }

  public class IntListPerformance : DefaultPerformance {
    protected override object CreateObject(int arrayLength) {
      var l = new List<int>(arrayLength);
      for (int i = 0; i < arrayLength; i++) {
        l.Add(rand.Next());
      }
      return l;

    }
  }

  public class IntLimitedListPerformance : DefaultPerformance {
    protected override object CreateObject(int arrayLength) {
      int maxInt = 20;
      var l = new List<int>(arrayLength);
      for (int i = 0; i < arrayLength; i++) {
        l.Add(rand.Next(maxInt));
      }
      return l;

    }
  }

  public class GraphPerformance : DefaultPerformance {
    public class Node {
      [Storable]
      public List<Node> children;
      private Node(StorableConstructorFlag _) { }
      public Node() {
        children = new List<Node>();
      }
    }
    protected override object CreateObject(int arrayLength) {
      int maxChildren = 20;
      var allNodes = new List<Node>();
      allNodes.Add(new Node()); // start with at least one node
      for (int i = 0; i < arrayLength; i++) {
        var node = new Node();
        var numChildren = rand.Next(maxChildren) + 1;
        for (int j = 0; j < numChildren; j++) {
          node.children.Add(allNodes[rand.Next(allNodes.Count)]);
        }
        allNodes.Add(node);
      }
      return allNodes;
    }
  }


  public class LinkedListPerformance : DefaultPerformance {

    [StorableType("B9C2AA20-A18C-4124-90B8-B181BF7691B4")]
    private class ListNode {
      [Storable]
      public ListNode Next;
    }
    protected override object CreateObject(int arrayLength) {
      var first = new ListNode();
      var cur = first;
      for (int i = 0; i < arrayLength; i++) {
        cur.Next = new ListNode();
        cur = cur.Next;
      }
      return first;
    }
  }
}
