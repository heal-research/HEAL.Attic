#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System.Collections.Generic;

namespace HEAL.Attic {
  internal sealed class Index<T> where T : class {
    private readonly Dictionary<T, uint> indexes;
    private readonly Dictionary<uint, T> values;
    private uint nextIndex;

    public Index() {
      this.indexes = new Dictionary<T, uint>();
      this.values = new Dictionary<uint, T>();
      nextIndex = 1;
    }

    public Index(IEnumerable<T> values) : this() {
      foreach (var value in values) {
        if (value != null) {
          if (!this.indexes.ContainsKey(value))
            this.indexes.Add(value, nextIndex);
          this.values.Add(nextIndex, value);
        }
        nextIndex++;
      }
    }

    public uint GetIndex(T value) {
      uint index = 0;
      if (value != null && !indexes.TryGetValue(value, out index)) {
        index = nextIndex;
        nextIndex++;
        indexes.Add(value, index);
        values.Add(index, value);
      }
      return index;
    }

    public T GetValue(uint index) {
      return index == 0 ? null : values[index];
    }

    public bool TryGetValue(uint index, out T value) {
      return values.TryGetValue(index, out value);
    }

    public IEnumerable<T> GetValues() {
      return values.Values;
    }
  }
}
