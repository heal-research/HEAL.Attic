#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;

namespace HEAL.Attic {
  [StorableType("51aec2b4-6e18-4e6d-b00b-ee656747be78")]
  public interface ITransformer {
    Guid Guid { get; }
    uint Priority { get; }

    bool CanTransformType(Type type);
    Box CreateBox(object o, Mapper mapper);
    void FillBox(Box box, object o, Mapper mapper);
    object ToObject(Box box, Mapper mapper);
    void FillFromBox(object obj, Box box, Mapper mapper);
  }
}
