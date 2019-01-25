#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;

namespace HEAL.Attic {
  /// <summary>
  /// Mark the member of a class to be considered by the <c>StorableSerializer</c>.
  /// The class must be marked as <c>[StorableClass("05FE6F11-87C6-435E-800A-166AFACCF5AC")]</c> and the
  /// <c>StorableMemberSelection</c> should be set to <c>MarkedOnly</c> for
  /// this attribute to kick in.
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = false
  )]
  public sealed class StorableAttribute : Attribute {
    private static IDictionary<MemberInfo, StorableAttribute> attributeCache = new Dictionary<MemberInfo, StorableAttribute>();

    #region Properties
    /// <summary>
    /// An optional name for this member that will be used during serialization.
    /// This allows to rename a field/property in code but still be able to read
    /// the old serialized format.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }


    /// <summary>
    /// A default value in case the field/property was not present or not serialized
    /// in a previous version of the class and could therefore be absent during
    /// deserialization.
    /// </summary>
    /// <value>The default value.</value>
    public object DefaultValue { get; set; }

    /// <summary>
    /// Allow storable attribute on properties with only a getter or a setter. These
    /// properties will then by either only serialized (getter only) but not deserialized or only
    /// deserialized (setter only) but not serialized again.
    /// </summary>
    [Obsolete("Use OldName instead.")]
    public bool AllowOneWay { get; set; }

    public string OldName { get; set; }
    #endregion

    public static bool IsStorable(MemberInfo memberInfo) {
      return GetStorableAttribute(memberInfo) != null;
    }
    public static StorableAttribute GetStorableAttribute(MemberInfo memberInfo) {
      StorableAttribute attrib;

      if (!attributeCache.TryGetValue(memberInfo, out attrib)) {
        attrib = (StorableAttribute)GetCustomAttribute(memberInfo, typeof(StorableAttribute), false);
        if (attrib != null) attributeCache[memberInfo] = attrib;
      }

      return attrib;
    }
  }
}
