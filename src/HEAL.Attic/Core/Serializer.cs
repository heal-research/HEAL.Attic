#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System.IO;
using System.IO.Compression;
using System.Threading;

namespace HEAL.Attic {
  public abstract class Serializer : ISerializer {
    public virtual void Serialize(object o, Stream stream,
                                  bool disposeStream = true,
                                  CancellationToken cancellationToken = default(CancellationToken)) {
      Serialize(o, stream, out SerializationInfo _, disposeStream, cancellationToken);
    }
    public virtual void Serialize(object o, Stream stream,
                                  out SerializationInfo info,
                                  bool disposeStream = true,
                                  CancellationToken cancellationToken = default(CancellationToken)) {
      using (var deflateStream = new DeflateStream(stream, CompressionMode.Compress)) {
        SerializeBundle(Mapper.ToBundle(o, out info, cancellationToken), deflateStream, disposeStream);
      }
    }
    public virtual void Serialize(object o, string path,
                                  CancellationToken cancellationToken = default(CancellationToken)) {
      Serialize(o, path, out SerializationInfo _, cancellationToken);
    }
    public virtual void Serialize(object o, string path,
                                  out SerializationInfo info,
                                  CancellationToken cancellationToken = default(CancellationToken)) {
      string tempfile = Path.GetTempFileName();

      using (FileStream fileStream = File.Create(tempfile)) {
        Serialize(o, fileStream, out info, false, cancellationToken);
      }
      if (!cancellationToken.IsCancellationRequested) {
        File.Copy(tempfile, path, true);
      }
      File.Delete(tempfile);
    }
    public virtual byte[] Serialize(object o,
                                    CancellationToken cancellationToken = default(CancellationToken)) {
      return Serialize(o, out SerializationInfo _, cancellationToken);
    }
    public virtual byte[] Serialize(object o,
                                    out SerializationInfo info,
                                    CancellationToken cancellationToken = default(CancellationToken)) {
      using (var memoryStream = new MemoryStream()) {
        Serialize(o, memoryStream, out info, false, cancellationToken);
        return memoryStream.ToArray();
      }
    }
    protected abstract void SerializeBundle(Bundle bundle, Stream stream, bool disposeStream = true);

    public virtual object Deserialize(Stream stream,
                                      bool disposeStream = true,
                                      CancellationToken cancellationToken = default(CancellationToken)) {
      return Deserialize(stream, out SerializationInfo _, disposeStream, cancellationToken);
    }
    public virtual object Deserialize(Stream stream,
                                      out SerializationInfo info,
                                      bool disposeStream = true,
                                      CancellationToken cancellationToken = default(CancellationToken)) {
      try {
        using (var deflateStream = new DeflateStream(stream, CompressionMode.Decompress)) {
          return Mapper.ToObject(DeserializeBundle(deflateStream, disposeStream), out info, cancellationToken);
        }
      } catch (InvalidDataException ide) {
        throw new PersistenceException("Invalid data in stream. Maybe the data was serialized without using a DeflateStream.", ide);
      }
    }
    public virtual object Deserialize(string path,
                                      CancellationToken cancellationToken = default(CancellationToken)) {
      return Deserialize(path, out SerializationInfo _, cancellationToken);
    }
    public virtual object Deserialize(string path,
                                      out SerializationInfo info,
                                      CancellationToken cancellationToken = default(CancellationToken)) {
      using (var fileStream = File.Open(path, FileMode.Open)) {
        return Deserialize(fileStream, out info, false, cancellationToken);
      }
    }
    public virtual object Deserialize(byte[] data,
                                      CancellationToken cancellationToken = default(CancellationToken)) {
      return Deserialize(data, out SerializationInfo _, cancellationToken);
    }
    public virtual object Deserialize(byte[] data,
                                      out SerializationInfo info,
                                      CancellationToken cancellationToken = default(CancellationToken)) {
      using (var memoryStream = new MemoryStream(data)) {
        return Deserialize(memoryStream, out info, false, cancellationToken);
      }
    }
    protected abstract Bundle DeserializeBundle(Stream stream, bool disposeStream = true);
  }
}
