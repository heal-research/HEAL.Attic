#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System.IO;
using System.Text;
using Google.Protobuf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HEAL.Attic.Tests {

  [TestClass]
  public class ExceptionTests {
    [TestMethod]
    public void TestMissingDeflateStreamException() {
      var tmpFile = Path.GetTempFileName();
      using (var outStream = File.OpenWrite(tmpFile)) {
        new ProtoBufSerializer().Serialize(new TestStorable(), outStream);
      }
      // This must fail, because Serialize did not use a DeflateStream
      // However, Deserialize(string path) uses a DeflateStream
      try {
        var instance = (TestStorable)new ProtoBufSerializer().Deserialize(tmpFile);
        Assert.Fail("This method should not succeed.");
      } catch (PersistenceException e) {
        Assert.AreEqual("Invalid data in stream. Maybe the data was serialized with or without using a DeflateStream.", e.Message);
        Assert.IsInstanceOfType(e.InnerException, typeof(InvalidDataException));
      }
      File.Delete(tmpFile);
    }

    [TestMethod]
    public void TestInvalidFormatException() {
      var tmpFile = Path.GetTempFileName();
      File.WriteAllBytes(tmpFile, Encoding.UTF8.GetBytes("Hello Attic"));
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
