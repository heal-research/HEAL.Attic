#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System;
using System.Drawing;

namespace HEAL.Attic {

  [Transformer("AFF27987-3301-4D70-9601-EFCA31BDA0DB", 405)]
  
  internal sealed class FontTransformer : BoxTransformer<Font> {
    protected override void Populate(Box box, Font value, Mapper mapper) {
      box.Values = new RepeatedValueBox();
      box.Values.UInts = new RepeatedUIntBox();
      var uints = box.Values.UInts;
      uints.Values.Add(mapper.GetStringId(GetFontFamilyName(value.FontFamily)));
      uints.Values.Add(mapper.GetBoxId(value.Size));
      uints.Values.Add(mapper.GetBoxId(value.Style));
      uints.Values.Add(mapper.GetBoxId(value.Unit));
      uints.Values.Add(mapper.GetBoxId(value.GdiCharSet));
      uints.Values.Add(mapper.GetBoxId(value.GdiVerticalFont));
    }

    protected override Font Extract(Box box, Type type, Mapper mapper) {
      var fontData = box.Values.UInts.Values;
      return new Font(
        GetFontFamily(mapper.GetString((uint)fontData[0])),
        (float)mapper.GetObject((uint)fontData[1]),
        (FontStyle)mapper.GetObject((uint)fontData[2]),
        (GraphicsUnit)mapper.GetObject((uint)fontData[3]),
        (byte)mapper.GetObject((uint)fontData[4]),
        (bool)mapper.GetObject((uint)fontData[5])
      );
    }

    public const string GENERIC_MONOSPACE_NAME = "_GenericMonospace";
    public const string GENERIC_SANS_SERIF_NAME = "_GenericSansSerif";
    public const string GENERIC_SERIF_NAME = "_GenericSerif";

    public static FontFamily GetFontFamily(string name) {
      if (name == GENERIC_MONOSPACE_NAME) return FontFamily.GenericMonospace;
      if (name == GENERIC_SANS_SERIF_NAME) return FontFamily.GenericSansSerif;
      if (name == GENERIC_SERIF_NAME) return FontFamily.GenericSerif;
      return new FontFamily(name);
    }

    public static string GetFontFamilyName(FontFamily ff) {
      if (ff.Equals(FontFamily.GenericMonospace)) return GENERIC_MONOSPACE_NAME;
      if (ff.Equals(FontFamily.GenericSansSerif)) return GENERIC_SANS_SERIF_NAME;
      if (ff.Equals(FontFamily.GenericSerif)) return GENERIC_SERIF_NAME;
      return ff.Name;
    }
  }
}
