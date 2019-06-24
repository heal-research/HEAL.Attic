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
      using (var outputStream = new CodedOutputStream(stream, leaveOpen: !disposeStream)) {
        bundle.WriteTo(outputStream);
        outputStream.Flush();
      }
    }

    protected override Bundle DeserializeBundle(Stream stream, bool disposeStream = true) {
      using (var inputStream = new CodedInputStream(stream, leaveOpen: !disposeStream)) {
        try {
          return Bundle.Parser.ParseFrom(inputStream);
        } catch (InvalidProtocolBufferException e) {
          throw new PersistenceException("Invalid format.", e);
        }
      }
    }
  }
}
