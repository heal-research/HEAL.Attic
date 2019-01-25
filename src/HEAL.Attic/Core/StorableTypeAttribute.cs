#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace HEAL.Attic {
  /// <summary>
  /// Mark a class to be considered by the <c>StorableSerializer</c>.
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Delegate,
    Inherited = false,
    AllowMultiple = false
  )]
  public sealed class StorableTypeAttribute : Attribute {
    private static IDictionary<Type, StorableTypeAttribute> attributeCache = new Dictionary<Type, StorableTypeAttribute>();

    #region Properties
    /// <summary>
    /// Specify how members are selected for serialization.
    /// </summary>
    public StorableMemberSelection MemberSelection { get; private set; }

    /// <summary>
    /// The GUID that identifies the type.
    /// </summary>
    /// <value>The GUID.</value>
    public Guid Guid {
      get { return Guids[0]; }
      private set { Guids = new[] { value }; }
    }

    /// <summary>
    /// The GUIDs that identify the type.
    /// </summary>
    /// <value>The GUIDs.</value>
    public Guid[] Guids { get; private set; }
    #endregion

    /// <summary>
    /// Mark a class to be serialize by the <c>StorableSerizlier</c>
    /// </summary>
    /// <param name="memberSelection">The storable class memberSelection.</param>
    public StorableTypeAttribute(StorableMemberSelection memberSelection, string guid) {
      MemberSelection = memberSelection;
      Guid = new Guid(guid);
    }

    public StorableTypeAttribute(string guid) {
      Guid = new Guid(guid);
    }

    public StorableTypeAttribute(StorableMemberSelection memberSelection, params string[] guids) {
      MemberSelection = memberSelection;
      Guids = guids.Select(x => new Guid(x)).ToArray();
    }

    public StorableTypeAttribute(params string[] guids) {
      Guids = guids.Select(x => new Guid(x)).ToArray();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StorableTypeAttribute"/> class.
    /// The default value for <see cref="StorableMemberSelection"/> is
    /// <see cref="StorableMemberSelection.MarkedOnly"/>.
    /// </summary>
    private StorableTypeAttribute() { }

    /// <summary>
    ///  Checks if the <see cref="StorableTypeAttribute"/> is present on a memberSelection.
    /// </summary>
    /// <param name="type">The memberSelection which should be checked for the <see cref="StorableTypeAttribute"/></param>
    /// <returns></returns>
    public static bool IsStorableType(Type type) {
      return GetStorableTypeAttribute(type) != null;
    }

    public static StorableTypeAttribute GetStorableTypeAttribute(Type type) {
      StorableTypeAttribute attrib;

      if (!attributeCache.TryGetValue(type, out attrib)) {
        attrib = (StorableTypeAttribute)GetCustomAttribute(type, typeof(StorableTypeAttribute), false);
        if (attrib != null) attributeCache[type] = attrib;
      }

      return attrib;
    }
  }
}

