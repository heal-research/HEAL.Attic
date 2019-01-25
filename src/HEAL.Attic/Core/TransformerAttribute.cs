#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;

namespace HEAL.Attic {
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class TransformerAttribute : Attribute {
    public Guid Guid { get; private set; }
    public uint Priority { get; private set; }

    public TransformerAttribute(string guid, uint priority) {
      Guid = new Guid(guid);
      Priority = priority;
    }

    public static bool IsTransformer(Type type) {
      return Attribute.IsDefined(type, typeof(TransformerAttribute), false);
    }
    public static TransformerAttribute GetTransformerAttribute(Type type) {
      return (TransformerAttribute)Attribute.GetCustomAttribute(type, typeof(TransformerAttribute), false);
    }
    public static Guid GetGuid(Type type) {
      return GetTransformerAttribute(type).Guid;
    }
    public static uint GetPriority(Type type) {
      return GetTransformerAttribute(type).Priority;
    }
  }
}
