#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;

namespace HEAL.Attic {

  /// <summary>
  /// Mark a class to be considered by the <c>StorableSerializer</c>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  [Obsolete("Use StorableTypeAttribute instead")]
  public sealed class StorableClassAttribute : Attribute {


    /// <summary>
    /// Specify how members are selected for serialization.
    /// </summary>
    public StorableMemberSelection Type { get; private set; }

    /// <summary>
    /// Mark a class to be serialize by the <c>StorableSerizlier</c>
    /// </summary>
    /// <param name="type">The storable class type.</param>
    public StorableClassAttribute(StorableMemberSelection type) {
      Type = type;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StorableClassAttribute"/> class.
    /// The default value for <see cref="StorableClassType"/> is
    /// <see cref="StorableClassType.MarkedOnly"/>.
    /// </summary>
    public StorableClassAttribute() { }

    /// <summary>
    ///  Checks if the <see cref="StorableClassAttribute"/> is present on a type.
    /// </summary>
    /// <param name="type">The type which should be checked for the <see cref="StorableClassAttribute"/></param>
    /// <returns></returns>
    public static bool IsStorableClass(Type type) {
      object[] attribs = type.GetCustomAttributes(typeof(StorableClassAttribute), false);
      return attribs.Length > 0;
    }

  }
}

