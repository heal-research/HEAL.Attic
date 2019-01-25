#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Collections.Generic;

namespace HEAL.Attic {
  /// <summary>
  /// This interface allows users to add known storable types where only a mapping between GUID and type is necessary.
  /// On initialization of the StaticCache, all IStorableTypeMap implementations will be discovered and the known storable
  /// types will be registered with their GUIDs.
  /// </summary>
  public interface IStorableTypeMap {
    IEnumerable<Tuple<Guid, Type>> KnownStorableTypes { get; }
  }
}
