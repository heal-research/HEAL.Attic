#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Google.Protobuf;

namespace HEAL.Attic {
  [Transformer("D0ADB806-2DFD-459D-B5DA-14B5F1152534", 404)]
  
  internal sealed class BitmapTransformer : BoxTransformer<Bitmap> {
    protected override void Populate(Box box, Bitmap value, Mapper mapper) {
      lock (value)
        using (var ms = new MemoryStream()) {
          value.Save(ms, ImageFormat.Png);
          box.Value = new ScalarValueBox();
          box.Value.Bytes = ByteString.CopyFrom(ms.ToArray());
        }
    }

    protected override Bitmap Extract(Box box, Type type, Mapper mapper) {
      using (var ms = new MemoryStream()) {
        ms.Write(box.Value.Bytes.ToArray(), 0, box.Value.Bytes.Length);
        ms.Seek(0, SeekOrigin.Begin);
        return new Bitmap(ms);
      }
    }
  }

}
