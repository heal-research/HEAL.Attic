#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HEAL.Attic {
  public sealed class StaticCache {
    private static readonly object locker = new object();

    private readonly Dictionary<Guid, ITransformer> guid2Transformer;
    private readonly Dictionary<ITransformer, Guid> transformer2Guid;
    private readonly Dictionary<Guid, Type> guid2Type;
    private readonly Dictionary<Type, Guid> type2Guid;
    private readonly Dictionary<Type, TypeInfo> typeInfos;

    internal StaticCache() {
      guid2Transformer = new Dictionary<Guid, ITransformer>();
      transformer2Guid = new Dictionary<ITransformer, Guid>();
      guid2Type = new Dictionary<Guid, Type>();
      type2Guid = new Dictionary<Type, Guid>();
      typeInfos = new Dictionary<Type, TypeInfo>();

      foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
        foreach (var t in asm.GetTypes()) {
          if (typeof(ITransformer).IsAssignableFrom(t) && !t.IsAbstract) {
            var transformer = (ITransformer)Activator.CreateInstance(t);
            RegisterTransformer(transformer);
          }
        }
      }

      RegisterType(new Guid("ECAEA154-6BFF-419F-8BE6-2565E9314825"), typeof(object));
      RegisterType(new Guid("94AD8522-3F55-4580-A6F8-2D2AAEDD4B8C"), typeof(bool));
      RegisterType(new Guid("4A1C0FD5-423D-4F96-AB22-A496578C25AC"), typeof(byte));
      RegisterType(new Guid("C4B00F0B-FED7-439F-B1B2-8A0048B64882"), typeof(sbyte));
      RegisterType(new Guid("9F451811-3DE1-43AB-8B74-D7E03851857B"), typeof(short));
      RegisterType(new Guid("46244D54-0145-49F7-9CF3-9CDB7FB5F240"), typeof(ushort));
      RegisterType(new Guid("1FDDE40C-09E3-491F-8FBB-32BB3C885E9E"), typeof(char));
      RegisterType(new Guid("EE3E0F9C-A5C2-4461-AF36-28BD8F26E6FB"), typeof(int));
      RegisterType(new Guid("69476D18-D285-43E9-BC7C-6CC9E9F2321E"), typeof(uint));
      RegisterType(new Guid("7C7BC5EC-F001-4BA0-9F85-50DCBAA9AE81"), typeof(long));
      RegisterType(new Guid("AC808D5A-63BB-457C-9B92-C9B83DA2B139"), typeof(ulong));
      RegisterType(new Guid("BF22653A-026C-4367-BBBA-2125AECF6C08"), typeof(float));
      RegisterType(new Guid("8B49821A-3ADC-4715-9DB1-08E2F3CFDF15"), typeof(double));
      RegisterType(new Guid("5C6CB86C-81DA-4334-9196-6571C6706240"), typeof(decimal));

      RegisterType(new Guid("7BB386BF-6FD4-443D-A6C1-387096798C67"), typeof(DateTime));
      RegisterType(new Guid("724A2D49-7E7B-455B-BBA9-4214C64E8A21"), typeof(TimeSpan));
      RegisterType(new Guid("05551382-E894-4218-B860-FEE1D92CA07D"), typeof(Nullable<>));
      RegisterType(new Guid("4CC0D44E-65B2-4DF1-A333-30F058CB78CA"), typeof(Tuple<>));
      RegisterType(new Guid("5D451A64-EBD9-411F-A07E-232250B33784"), typeof(Tuple<,>));
      RegisterType(new Guid("EEBD2D26-56CC-45EA-900A-DD29B9F598CE"), typeof(Tuple<,,>));
      RegisterType(new Guid("B8EEEFA7-8576-42DD-937C-AFBB66297B56"), typeof(Tuple<,,,>));
      RegisterType(new Guid("4C3C8E0A-7DCF-4814-B798-AECB86E1E4E3"), typeof(Tuple<,,,,>));
      RegisterType(new Guid("EAA4FF91-B90C-47CB-AC0F-66AB66D41186"), typeof(Tuple<,,,,,>));
      RegisterType(new Guid("EB51B342-826F-4004-B2C6-F2A03A8C35C9"), typeof(Tuple<,,,,,,>));
      RegisterType(new Guid("14138614-45DB-4A0D-9E59-AE9373241D6B"), typeof(Tuple<,,,,,,,>));
      RegisterType(new Guid("4DB2ED2B-D9F2-4695-B555-A2CF42740740"), typeof(Color));
      RegisterType(new Guid("9BC74087-D5C0-4C39-99B4-D7465F478492"), typeof(Point));
      RegisterType(new Guid("E84C326A-7E14-4F28-AEFF-BC16CC671655"), typeof(KeyValuePair<,>));
      RegisterType(new Guid("F0280B55-25E8-4981-B309-D675D081402A"), typeof(string));

      RegisterType(new Guid("9CF55419-439B-4A90-B2ED-8C7F7768EB61"), typeof(Array));
      RegisterType(new Guid("B7313EE2-045F-41C9-AEC9-7B1A8CDBF90F"), typeof(IEnumerable));
      RegisterType(new Guid("D5265599-7380-4A28-AB0D-9F336EF290C5"), typeof(IEnumerable<>));
      RegisterType(new Guid("E451A99B-0CFD-48E6-9BEF-0AE9D1E73628"), typeof(IList));
      RegisterType(new Guid("DB2A785C-5803-42C7-A01F-E1FD845A83A1"), typeof(List<>));
      RegisterType(new Guid("EB98A564-BEDC-458A-9E9B-4BF3CDAFB9BE"), typeof(Stack<>));
      RegisterType(new Guid("D5E3118B-957A-43B5-A740-1BBAA0EAB666"), typeof(Stack));
      RegisterType(new Guid("8CBC5F9F-5671-4805-A690-DD034AEB7932"), typeof(ArrayList));
      RegisterType(new Guid("80CAB5BF-06C2-4DB2-9CF0-EE6E93D40B30"), typeof(Queue));
      RegisterType(new Guid("C528E123-F3DD-4711-83E2-A26E2A9A85E9"), typeof(Queue<>));
      RegisterType(new Guid("7E2F3277-7216-4295-A498-9ACA43527D5B"), typeof(HashSet<>));
      RegisterType(new Guid("E92C35AD-32B1-4F37-B8D2-BE2F5FEB465B"), typeof(Dictionary<,>));
      RegisterType(new Guid("18504ADF-2332-4497-8CA0-B2F00CA2EBA2"), typeof(Type));

      RegisterType(new Guid("67C35B3B-DC23-4929-9BA9-0A0CD5FE17F5"), Type.GetType("System.StringComparer"));
      RegisterType(new Guid("C7FDE8F0-EF3F-4A4D-8D87-5559C32A8B90"), Type.GetType("System.CultureAwareComparer"));
      RegisterType(new Guid("6A80741F-066D-460B-B333-4F766A4D4AEF"), Type.GetType("System.OrdinalComparer"));
      RegisterType(new Guid("94BB9853-8D25-4DD8-A915-FEF2EFF538BC"), Type.GetType("System.Resources.FastResourceComparer"));
      RegisterType(new Guid("65D2DE74-8BDF-4C74-9005-81A2C3991DC5"), Type.GetType("System.Collections.CompatibleComparer"));
      RegisterType(new Guid("075827F4-07D9-4E50-9D00-C9FF3E7DCF9A"), Type.GetType("System.Collections.IEqualityComparer"));
      RegisterType(new Guid("C8BF5C03-1614-46AC-8730-2F231CAD4232"), Type.GetType("System.Collections.Generic.EqualityComparer`1"));
      RegisterType(new Guid("FB247FE0-9203-4196-B4D3-71AB18CD12C5"), Type.GetType("System.Collections.Generic.GenericEqualityComparer`1"));
      RegisterType(new Guid("06443E04-FB1D-41E1-BAE2-1AB3728BDCC9"), Type.GetType("System.Collections.Generic.NullableEqualityComparer`1"));
      RegisterType(new Guid("D9029E74-C511-4D28-B8C5-D0E299EC45A5"), Type.GetType("System.Collections.Generic.ObjectEqualityComparer`1"));
      RegisterType(new Guid("C6D867E2-3CD2-4C59-8BA6-F18F00DDB997"), Type.GetType("System.Collections.Generic.ByteEqualityComparer"));
      RegisterType(new Guid("02F8D180-BABE-4073-82E0-7FE77814F53D"), Type.GetType("System.Collections.Generic.EnumEqualityComparer`1"));
      RegisterType(new Guid("D3D279FE-169E-46B6-93AE-589BB4F24A4D"), Type.GetType("System.Collections.Generic.LongEnumEqualityComparer`1"));
      RegisterType(new Guid("DAF22757-7FCC-49AC-B148-F3DD7E9E0A3B"), Type.GetType("System.Collections.Generic.IEqualityComparer`1"));

      // types form System.Drawing.Common
      RegisterType(new Guid("4151C39E-93BD-4B33-A0A9-84581C66235A"), typeof(System.Drawing.Bitmap));
      RegisterType(new Guid("9C9AB672-463E-4372-9950-2440C998BAAD"), typeof(System.Drawing.Font));
      RegisterType(new Guid("56B8ECAE-E1F5-4A3D-8FC3-FAE9A0EB3806"), typeof(System.Drawing.FontStyle));
      RegisterType(new Guid("464FB443-A034-4328-8D2D-94B3D33DED71"), typeof(System.Drawing.GraphicsUnit));

      foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
        foreach (var t in asm.GetTypes()) {
          if (StorableTypeAttribute.IsStorableType(t)) {
            type2Guid.Add(t, StorableTypeAttribute.GetStorableTypeAttribute(t).Guid);
            foreach (var guid in StorableTypeAttribute.GetStorableTypeAttribute(t).Guids) {
              guid2Type.Add(guid, t);
            }
          } else if (typeof(IStorableTypeMap).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract) {
            var knownTypeMap = (IStorableTypeMap)Activator.CreateInstance(t);
            foreach (var tup in knownTypeMap.KnownStorableTypes) {
              RegisterType(tup.Item1, tup.Item2);
            }
          }
        }
      }
    }

    public void RegisterTransformer(ITransformer transformer) {
      if (transformer == null) throw new ArgumentNullException(nameof(transformer));
      lock (locker) {
        guid2Transformer.Add(transformer.Guid, transformer);
        transformer2Guid.Add(transformer, transformer.Guid);
      }
    }

    public void RegisterType(Guid guid, Type type) {
      RegisterType(type, guid);
    }
    public void RegisterType(Type type, params Guid[] guids) {
      if (type == null) throw new ArgumentNullException(nameof(type));
      lock (locker) {
        foreach (var guid in guids) {
          guid2Type.Add(guid, type);
        }
        type2Guid.Add(type, guids[0]);
      }
    }

    public ITransformer GetTransformer(Guid guid) {
      return guid2Transformer[guid];
    }

    public Guid GetGuid(ITransformer transformer) {
      return transformer2Guid[transformer];
    }

    public Type GetType(Guid guid) {
      if (!guid2Type.TryGetValue(guid, out Type value)) {
        throw new PersistenceException($"Unknown StorableType with GUID {guid}");
      }
      return value;
    }

    public bool TryGetType(Guid guid, out Type type) {
      return guid2Type.TryGetValue(guid, out type);
    }

    public Guid GetGuid(Type type) {
      if (!type2Guid.TryGetValue(type, out Guid guid)) {
        throw new PersistenceException($"Type {type.FullName} is not registered as StorableType.");
      }
      return guid;
    }

    public TypeInfo GetTypeInfo(Type type) {
      lock (locker) {
        TypeInfo typeInfo;
        if (!typeInfos.TryGetValue(type, out typeInfo)) {
          var transformers = guid2Transformer.Values.OrderBy(x => x.Priority);
          ITransformer transformer = null;
          foreach (var t in transformers) {
            if (t.CanTransformType(type)) {
              transformer = t;
              break;
            }
          }
          if (transformer == null) throw new PersistenceException("No transformer found for type " + type.AssemblyQualifiedName);
          typeInfo = new TypeInfo(type, transformer);
          typeInfos.Add(type, typeInfo);
        }
        return typeInfo;
      }
    }
  }
}
