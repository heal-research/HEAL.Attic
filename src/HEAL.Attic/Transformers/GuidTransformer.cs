using System;
using Google.Protobuf;

namespace HEAL.Attic.Transformers {
  [Transformer("8BE3B15D-F84F-4C57-86E3-D1DBD03988CE", 408)]
  internal sealed class GuidTransformer : BoxTransformer<Guid> {
    protected override void Populate(Box box, Guid value, Mapper mapper) { var b = new ScalarValueBox(); b.Bytes = ByteString.CopyFrom(value.ToByteArray()); box.Value = b; }
    protected override Guid Extract(Box box, Type type, Mapper mapper) { return new Guid(box.Value.Bytes.ToByteArray()); }
  }
}
