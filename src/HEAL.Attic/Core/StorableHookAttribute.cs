#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HEAL.Attic {
  /// <summary>
  /// Indicates the time at which the hook should be invoked.
  /// </summary>
  public enum HookType {
    /// <summary>
    /// States that this hook should be called before the storable
    /// serializer starts decomposing the object.
    /// </summary>
    BeforeSerialization,

    /// <summary>
    /// States that this hook should be called after the storable
    /// serializer hast complete re-assembled the object.
    /// </summary>
    AfterDeserialization
  };

  /// <summary>
  /// Mark methods that should be called at certain times during
  /// serialization/deserialization by the <c>StorableSerializer</c>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
  [StorableType("983bf558-1458-4129-b018-7e121ac3840e")]
  public sealed class StorableHookAttribute : Attribute {
    private static IDictionary<MethodInfo, StorableHookAttribute[]> attributeCache = new Dictionary<MethodInfo, StorableHookAttribute[]>();

    #region Properties
    /// <summary>
    /// Gets the type of the hook.
    /// </summary>
    /// <value>The type of the hook.</value>
    public HookType HookType {
      get { return hookType; }
    }
    private readonly HookType hookType;
    #endregion

    /// <summary>
    /// Mark method as <c>StorableSerializer</c> hook to be run
    /// at the <c>HookType</c> time.
    /// </summary>
    /// <param name="hookType">MemberSelection of the hook.</param>
    public StorableHookAttribute(HookType hookType) {
      this.hookType = hookType;
    }

    public static bool IsStorableHook(MethodInfo methodInfo) {
      return GetStorableHookAttributes(methodInfo) != null;
    }

    public static StorableHookAttribute[] GetStorableHookAttributes(MethodInfo methodInfo) {
      StorableHookAttribute[] attribs;

      if (!attributeCache.TryGetValue(methodInfo, out attribs)) {
        attribs = (StorableHookAttribute[])GetCustomAttributes(methodInfo, typeof(StorableHookAttribute), false);
        if (attribs.Any()) attributeCache[methodInfo] = attribs;
      }

      return attribs;
    }
  }
}