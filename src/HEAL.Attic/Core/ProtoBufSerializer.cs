#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System.IO;
using Google.Protobuf;

namespace HEAL.Attic {
  public sealed class ProtoBufSerializer : Serializer {
    protected override void SerializeBundle(Bundle bundle, Stream stream, bool disposeStream = true) {
      var outputStream = new CodedOutputStream(stream);
      try {
        bundle.WriteTo(outputStream);
      } finally {
        outputStream.Flush();
        if (disposeStream)
          outputStream.Dispose();
      }
    }

    protected override Bundle DeserializeBundle(Stream stream, bool disposeStream = true) {
      var inputStream = new CodedInputStream(stream);
      try {
        return Bundle.Parser.ParseFrom(inputStream);
      } finally {
        if (disposeStream)
          inputStream.Dispose();
      }
    }
  }
}
