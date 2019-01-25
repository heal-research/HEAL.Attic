#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Reflection;

namespace HEAL.Attic {


  /// <summary>
  /// Indicate that this constructor should be used instead of the default constructor
  /// when the <c>StorableSerializer</c> instantiates this class during
  /// deserialization.
  /// 
  /// The constructor must take exactly one <c>bool</c> argument that will be
  /// set to <c>true</c> during deserialization.
  /// </summary>
  [AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
  public sealed class StorableConstructorAttribute : Attribute {
    public static bool IsStorableConstructor(ConstructorInfo constructorInfo) {
      return Attribute.IsDefined(constructorInfo, typeof(StorableConstructorAttribute));
    }
  }
}
