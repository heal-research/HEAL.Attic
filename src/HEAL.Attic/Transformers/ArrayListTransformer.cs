#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Collections;

namespace HEAL.Attic {
  [Transformer("A8269C73-30A6-45AD-901E-F4986192765F", 400)]
  internal sealed class ArrayListTransformer : BoxTransformer<ArrayList> {
    protected override ArrayList Extract(Box box, Type type, Mapper mapper) {
      return new ArrayList(box.Values.UInts.Values.Count);
    }

    public override void FillFromBox(object obj, Box box, Mapper mapper) {
      var list = (ArrayList)obj;
      foreach (var id in box.Values.UInts.Values) {
        list.Add(mapper.GetObject(id));
      }
    }

    protected override void Populate(Box box, ArrayList value, Mapper mapper) {
      box.Values = new RepeatedValueBox();
      box.Values.UInts = new RepeatedUIntBox();
      foreach (var o in value) {
        box.Values.UInts.Values.Add(mapper.GetBoxId(o));
      }
    }
  }
}