#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System.IO;
using System.Threading;

namespace HEAL.Attic {
  [StorableType("81ac6e7f-1a3e-4a60-a146-c407104c02ca")]
  public interface ISerializer {
    void Serialize(object o, Stream stream,
                   bool disposeStream = true,
                   CancellationToken cancellationToken = default(CancellationToken));
    void Serialize(object o, Stream stream,
                   out SerializationInfo info,
                   bool disposeStream = true,
                   CancellationToken cancellationToken = default(CancellationToken));
    void Serialize(object o, string path,
                   CancellationToken cancellationToken = default(CancellationToken));
    void Serialize(object o, string path,
                   out SerializationInfo info,
                   CancellationToken cancellationToken = default(CancellationToken));
    byte[] Serialize(object o,
                     CancellationToken cancellationToken = default(CancellationToken));
    byte[] Serialize(object o,
                     out SerializationInfo info,
                     CancellationToken cancellationToken = default(CancellationToken));
    object Deserialize(Stream stream,
                       bool disposeStream = true,
                       CancellationToken cancellationToken = default(CancellationToken));
    object Deserialize(Stream stream,
                       out SerializationInfo info,
                       bool disposeStream = true,
                       CancellationToken cancellationToken = default(CancellationToken));
    object Deserialize(string path,
                       CancellationToken cancellationToken = default(CancellationToken));
    object Deserialize(string path,
                       out SerializationInfo info,
                       CancellationToken cancellationToken = default(CancellationToken));
    object Deserialize(byte[] data,
                       CancellationToken cancellationToken = default(CancellationToken));
    object Deserialize(byte[] data,
                       out SerializationInfo info,
                       CancellationToken cancellationToken = default(CancellationToken));
    bool CanDeserialize(Stream stream);
    bool CanDeserialize(string path);
    bool CanDeserialize(byte[] data);
  }
}
