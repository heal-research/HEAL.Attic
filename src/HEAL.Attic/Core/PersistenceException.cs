#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace HEAL.Attic {

  /// <summary>
  /// Exception thrown by components inside the persistence framework.
  /// </summary>
  [Serializable]
  public class PersistenceException : Exception {

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistenceException"/> class.
    /// </summary>
    public PersistenceException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistenceException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public PersistenceException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistenceException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PersistenceException(string message, Exception innerException) :
      base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistenceException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerExceptions">The inner exceptions.</param>
    public PersistenceException(string message, IEnumerable<Exception> innerExceptions)
      : base(message) {
      int i = 0;
      foreach (var x in innerExceptions) {
        i += 1;
        this.Data.Add("Inner Exception " + i, x);
      }
    }

    public PersistenceException(string message, Type type) : this($"{nameof(PersistenceException)} in type {type.FullName}: {message}") { }

    protected PersistenceException(System.Runtime.Serialization.SerializationInfo info, StreamingContext context) : base(info, context) { }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    /// <PermissionSet>
    /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/>
    /// </PermissionSet>
    public override string ToString() {
      var sb = new StringBuilder()
        .Append(base.ToString())
        .Append('\n');
      foreach (Exception x in Data.Values) {
        sb.Append(x.ToString()).Append('\n');
      }
      return sb.ToString();
    }
  }

}