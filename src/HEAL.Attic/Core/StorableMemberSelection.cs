#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion


namespace HEAL.Attic {

  /// <summary>
  /// Specifies which memebrs are selected for serialization by the StorableSerializer
  /// </summary>
  public enum StorableMemberSelection {

    /// <summary>
    /// Serialize only fields and properties that have been marked
    /// with the [Storable] attribute. This is the default value.
    /// </summary>
    MarkedOnly,

    /// <summary>
    /// Serialize all fields but ignore the 
    /// [Storable] attribute on properties.
    /// 
    /// This does not include generated backing fields
    /// for automatic properties.
    /// </summary>    
    AllFields,

    /// <summary>
    /// Serialize all properties but ignore the
    /// [Storable] attribute on fields.
    /// </summary>    
    AllProperties,

    /// <summary>
    /// Serialize all fields and all properties
    /// regardless of the [Storable] attribute.
    /// 
    /// This does not include generated backing fields
    /// for automatic properties, but uses the property
    /// accessors instead.
    /// </summary>    
    AllFieldsAndAllProperties
  };
}

