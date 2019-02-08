#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;

namespace HEAL.Attic {
  public abstract class Transformer : ITransformer {
    public Guid Guid { get; private set; }
    public uint Priority { get; private set; }

    protected Transformer() {
      Guid = TransformerAttribute.GetGuid(this.GetType());
      Priority = TransformerAttribute.GetPriority(this.GetType());
    }

    public abstract bool CanTransformType(Type type);
    public abstract Box CreateBox(object o, Mapper mapper);
    public abstract void FillBox(Box box, object o, Mapper mapper);
    public abstract object ToObject(Box box, Mapper mapper);

    public virtual void FillFromBox(object obj, Box box, Mapper mapper) { }
  }
}
