#region License Information
/*
 * This file is part of HEAL.Attic which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

namespace HEAL.Attic.Tests {

  [StorableType("E0862640-FD57-4F1B-929D-03E38D8EA5D3")]
  class DemoClass {

    [Storable(Name = "TestProperty", DefaultValue = 12)]
    public object o = null;

    [Storable]
    public int x = 2;

    public int y = 0;

    [StorableConstructor]
    protected DemoClass(StorableConstructorFlag _) {
    }
    public DemoClass() {
    }
  }

  [StorableType("7C5431ED-9754-44B9-870D-F62E822A74CE")]
  class Base {
    public string baseName;
    [Storable]
    public virtual string Name {
      get { return "Base"; }
      set { baseName = value; }
    }

    [StorableConstructor]
    protected Base(StorableConstructorFlag _) {
    }
    public Base() {
    }
  }

  [StorableType("39BC0BDD-C5F4-4FAA-B227-558909E7AE6C")]
  class Override : Base {
    [Storable]
    public override string Name {
      get { return "Override"; }
      set { base.Name = value; }
    }

    [StorableConstructor]
    protected Override(StorableConstructorFlag _) : base(_) {
    }
    public Override() {
    }
  }

  [StorableType("50E13067-A343-4059-B44B-54511ED813AE")]
  class Intermediate : Override {
    [StorableConstructor]
    protected Intermediate(StorableConstructorFlag _) : base(_) {
    }
    public Intermediate() {
    }
  }

  [StorableType("DA7868B2-81CC-48AC-AEC3-2E3AD2E0A34E")]
  class New : Intermediate {
    public string newName;
    [Storable]
    public new string Name {
      get { return "New"; }
      set { newName = value; }
    }

    [StorableConstructor]
    protected New(StorableConstructorFlag _) : base(_) {
    }
    public New() {
    }
  }
}
