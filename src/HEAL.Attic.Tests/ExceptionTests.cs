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


    [StorableType("2F344E0B-29CB-4910-8CC1-6ADE2A1F0DB8")]
    private class Type_2F344E0B { }

    // registration of this type with guid of type Type_2F344E0B should fail
    private class TypeWithDuplicateGuid { }

    [TestMethod]
    public void DuplicateGuidTest() {
      var guid = StorableTypeAttribute.GetStorableTypeAttribute(typeof(Type_2F344E0B)).Guid;

      try {
        TypeManipulation.RegisterType(guid, typeof(TypeWithDuplicateGuid));
        Assert.Fail("This method should not succeed.");
      } catch (PersistenceException e) {
        Assert.AreEqual($"PersistenceException in type HEAL.Attic.Tests.ExceptionTests+{nameof(TypeWithDuplicateGuid)}:" +
          $" The GUID {guid} is already used by type HEAL.Attic.Tests.ExceptionTests+{nameof(Type_2F344E0B)}.", e.Message);
        Assert.AreEqual(TypeManipulation.GetType(guid), typeof(Type_2F344E0B));
      }
    }

    [StorableType("CDAE4A2A-1A7D-44A6-A401-D74120949EE8")]
    private class TypeWithoutValidCtor {
      // no default ctor
      // no storable ctor
      public TypeWithoutValidCtor(object o) { }
    }

    [TestMethod]
    public void MissingConstructorTest() {
      var x = new TypeWithoutValidCtor(null);
      var ser = new ProtoBufSerializer();
      try {
        ser.Serialize(x);
        Assert.Fail("This method should not succeed.");
      } catch (PersistenceException e) {
        Assert.AreEqual($"PersistenceException in type HEAL.Attic.Tests.ExceptionTests+{nameof(TypeWithoutValidCtor)}:" +
          " No storable constructor or parameterless constructor found.", e.Message);
      }
    }

    [StorableType("DF90167A-2616-412C-97DA-8CBAAEFA9EC2")]
    private class TypeWithInvalidStorableField {
      // cannot use OldName and Name at the same time
      [Storable(OldName = "a", Name = "b")]
      public object o;
    }

    [TestMethod]
    public void UseOfNameAndOldNameFieldTest() {
      var x = new TypeWithInvalidStorableField();
      var ser = new ProtoBufSerializer();
      try {
        ser.Serialize(x);
        Assert.Fail("This method should not succeed.");
      } catch (PersistenceException e) {
        Assert.AreEqual($"PersistenceException in type HEAL.Attic.Tests.ExceptionTests+{nameof(TypeWithInvalidStorableField)}:" +
          $" Field {nameof(x.o)} cannot use Name and OldName at the same time.", e.Message);
      }
    }

    [StorableType("826ED91B-636C-40AE-8B4E-EA04C3BC57CB")]
    private class TypeWithInvalidStorableFieldPath {
      // path is invalid
      [Storable(Name = "826ED91B-636C-40AE-8B4E-EA04C3BC57CB..o")]
      public object o;
    }

    [TestMethod]
    public void InvalidFieldPathTest() {
      var x = new TypeWithInvalidStorableFieldPath();
      var ser = new ProtoBufSerializer();
      try {
        ser.Serialize(x);
        Assert.Fail("This method should not succeed.");
      } catch (PersistenceException e) {
        Assert.AreEqual($"PersistenceException in type HEAL.Attic.Tests.ExceptionTests+{nameof(TypeWithInvalidStorableFieldPath)}:" +
          $" Field {nameof(x.o)} has an invalid path.", e.Message);
      }
    }

    [StorableType("949392EA-6D80-42EB-94F2-89488F0ED6B0")]
    private class TypeWithPropertyUsingOldNameAndName {
      // cannot use OldName and Name at the same time
      [Storable(OldName = "a", Name = "b")]
      public object O { get; set; }
    }

    [TestMethod]
    public void UseOfNameAndOldNamePropertyTest() {
      var x = new TypeWithPropertyUsingOldNameAndName();
      var ser = new ProtoBufSerializer();
      try {
        ser.Serialize(x);
        Assert.Fail("This method should not succeed.");
      } catch (PersistenceException e) {
        Assert.AreEqual($"PersistenceException in type HEAL.Attic.Tests.ExceptionTests+{nameof(TypeWithPropertyUsingOldNameAndName)}:" +
          $" Property {nameof(x.O)} cannot use Name and OldName at the same time.", e.Message);
      }
    }

    [StorableType("1A146AE3-6D6F-4E5E-A79A-80AC22C8A75F")]
    private class TypeWithPropertyUsingAllowOneWayAndName {
      // cannot use OldName and AllowOneWay at the same time
#pragma warning disable CS0618 // Type or member is obsolete
      [Storable(OldName = "a", AllowOneWay = true)]
#pragma warning restore CS0618 // Type or member is obsolete
      public object O { get; set; }
    }

    [TestMethod]
    public void UseOfAllowOneWayAndOldNamePropertyTest() {
      var x = new TypeWithPropertyUsingAllowOneWayAndName();
      var ser = new ProtoBufSerializer();
      try {
        ser.Serialize(x);
        Assert.Fail("This method should not succeed.");
      } catch (PersistenceException e) {
        Assert.AreEqual($"PersistenceException in type HEAL.Attic.Tests.ExceptionTests+{nameof(TypeWithPropertyUsingAllowOneWayAndName)}:" +
          $" Property {nameof(x.O)} cannot use AllowOneWay and OldName at the same time.", e.Message);
      }
    }

    [StorableType("F1FB543A-EC1C-411E-AB5E-532C39E9985C")]
    private class ReadableOnlyType {
      // no setter
      // no AllowOneWay
      // no OldName
      [Storable]
      public object O { get; }
    }

    [TestMethod]
    public void OmittingAllowOneWayOrOldNameTest() {
      var x = new ReadableOnlyType();
      var ser = new ProtoBufSerializer();
      try {
        ser.Serialize(x);
        Assert.Fail("This method should not succeed.");
      } catch (PersistenceException e) {
        Assert.AreEqual($"PersistenceException in type HEAL.Attic.Tests.ExceptionTests+{nameof(ReadableOnlyType)}:" +
          $" Property {nameof(x.O)} must be readable and writable or have one way serialization explicitly enabled or use OldName.", e.Message);
      }
    }

    [StorableType("36920F6B-1212-408F-8CF9-0C7231546CE2")]
    private class TypeWithInvalidStorablePropertyPath {
      // path is invalid
      [Storable(Name = "36920F6B-1212-408F-8CF9-0C7231546CE2..o")]
      public object O { get; set; }
    }

    [TestMethod]
    public void InvalidPropertyPathTest() {
      var x = new TypeWithInvalidStorablePropertyPath();
      var ser = new ProtoBufSerializer();
      try {
        ser.Serialize(x);
        Assert.Fail("This method should not succeed.");
      } catch (PersistenceException e) {
        Assert.AreEqual($"PersistenceException in type HEAL.Attic.Tests.ExceptionTests+{nameof(TypeWithInvalidStorablePropertyPath)}:" +
          $" Property {nameof(x.O)} has an invalid path.", e.Message);
      }
    }
  }
}
