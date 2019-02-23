#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System.Collections.Generic;

namespace HEAL.Attic {
  internal class StorableTypeLayout {
    public string TypeGuid { get; set; }
    public List<string> MemberNames { get; set; } = new List<string>();
    public uint ParentLayoutId { get; set; }
    public bool IsPopulated { get; set; } = false;

    public StorableTypeLayout() { }
  }
}
