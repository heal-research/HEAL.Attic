#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Reflection;

namespace HEAL.Attic {
  public sealed class ComponentInfo<T> where T : MemberInfo {
    public string Name { get; private set; }
    public string FullName { get; private set; }
    public T MemberInfo { get; private set; }
    public Type DeclaringType { get; private set; }
    public StorableAttribute StorableAttribute { get; private set; }
    public bool Readable { get; private set; }
    public bool Writeable { get; private set; }

    public ComponentInfo(string name, string fullName, T memberInfo, Type declaringType, StorableAttribute storableAttribute, bool readable, bool writeable) {
      Name = name;
      FullName = fullName;
      MemberInfo = memberInfo;
      DeclaringType = declaringType;
      StorableAttribute = storableAttribute;
      Readable = readable;
      Writeable = writeable;
    }
  }
}
