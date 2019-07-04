#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System.IO;
using System.IO.Compression;
using System.Text;
using Google.Protobuf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HEAL.Attic.Tests {

  [TestClass]
  public class ExceptionTests {
    [TestMethod]
    public void TestMissingDeflateStreamException() {
      var bytes = new ProtoBufSerializer().Serialize(new TestStorable()); // compresses bytes using DeflateStream
      var tmpFile = Path.GetTempFileName();

      using (var memoryStream = new MemoryStream(bytes))
      using (var deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress)) // decompress bytes
      using (var fileStream = File.OpenWrite(tmpFile)) {
        deflateStream.CopyTo(fileStream); // save decompressed bytes to file
      }

      // This must fail, because the file contents have not been serialized with a DeflateStream
      // However, Deserialize(string path) uses a DeflateStream
      try {
        var instance = (TestStorable)new ProtoBufSerializer().Deserialize(tmpFile);
        Assert.Fail("This method should not succeed.");
      } catch (PersistenceException e) {
        Assert.AreEqual("Invalid data in stream. Maybe the data was serialized without using a DeflateStream.", e.Message);
        Assert.IsInstanceOfType(e.InnerException, typeof(InvalidDataException));
      }
      File.Delete(tmpFile);
    }

    [TestMethod]
    public void TestInvalidFormatException() {
      var bytes = Encoding.UTF8.GetBytes("Hello Attic");
      var tmpFile = Path.GetTempFileName();

      using (var fileStream = File.OpenWrite(tmpFile))
      using (var deflateStream = new DeflateStream(fileStream, CompressionMode.Compress)) { // compress bytes
        deflateStream.Write(bytes, 0, bytes.Length); // save compressed bytes to file
      }

      // This must fail, because this file is not a valid format of Attic
      using (var stream = File.OpenRead(tmpFile)) {
        try {
          var instance = (TestStorable)new ProtoBufSerializer().Deserialize(stream, disposeStream: false);
          Assert.Fail("This method should not succeed.");
        } catch (PersistenceException e) {
          Assert.AreEqual("Invalid format.", e.Message);
          Assert.IsInstanceOfType(e.InnerException, typeof(InvalidProtocolBufferException));
        }
      }
      File.Delete(tmpFile);
    }

    [StorableType("5d89096d-24f1-4e9b-8bb1-b65ebd771d71")]
    private class TestStorable { }
  }
}
