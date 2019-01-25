#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Collections.Generic;

namespace HEAL.Attic {
  public class SerializationInfo {
    public TimeSpan Duration { get; internal set; }
    public int NumberOfSerializedObjects { get; internal set; }
    public IEnumerable<Guid> UnknownTypeGuids { get; internal set; }
    public IEnumerable<Type> SerializedTypes { get; internal set; }
  }
}