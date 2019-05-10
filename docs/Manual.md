# HEAL.Attic Manual
HEAL.Attic is a .NET software framework for serialization and persistence of object graphs. It uses Google Protocol Buffers for compact storage of serialized data.

HEAL.Attic is implemented for the .NET Standard 2.0 interface and can be used from .NET Core as well as .NET Framework projects.

## Basic Usage
HEAL.Attic provides .NET attributes to mark classes, properties and fields for persistence. For example, if we have a class for a person then we simply mark the class as a `StorableType` and mark the properties `Name` and `Address` as `Storable`     
```csharp
  [StorableType(Guid = "38C30283-9FF1-4EDC-A842-5DE60E1BBD73")]
  public class Person {
      [Storable]
      public string Name { get; set; }
      [Storable]
      public string Address { get; set; }

      public Person(string name, string address) {
          this.Name = name;
          this.Address = address;
      }
  }
```

We can then save objects of type `Person` to a file using the `ProtoBufSerializer`.

```csharp
  public class Program {
      public static void Main(string[] args) {
          var p = new Person("Guy Incognito", "123 Fake Street");
          var serializer = new ProtoBufSerializer();
          serializer.Serialize(p, "person.bin");
      }
  }
```

Similarly, we can restore the person object using the  `Deserialize` method.
```csharp
   ...
   var person = (Person)serializer.Deserialize("person.bin");
   ... 
```

.NET collection types and references within object graphs are handled automatically as shown in the following example.
```csharp
  [StorableType(Guid = "38C30283-9FF1-4EDC-A842-5DE60E1BBD73")]
  public class Person {
      [Storable]
      public string Name { get; set; }

      [Storable]
      public List<Person> Children { get; }

      public Person(string name) : this(name, new Person[] {}) { }

      public Person(string name, IEnumerable<Person> children) {
          this.Name = name;
          this.Children = new List<Person>(children);
      }
  }

  public class Program {
      public static void Main(string[] args) {
          var a = new Person("Bart");
          var b = new Person("Lisa");
          var c = new Person("Maggie");
          var d = new Person("Homer", new Person[] {a, b, c});
          var e = new Person("Marge", new Person[] {a, b, c});
          var family = new List<Person>(new [] {a, b, c, d, e});

          var serializer = new ProtoBufSerializer();
          seriaizer.Serialize(family, "family.bin");
      }
  }
``` 

### Principles
HEAL.Attic transforms object graphs to a serialized format called `Bundle`. The `Bundle` is a ProtoBuf message type and basically holds an array of `Boxes`. `Box` is another ProtoBuf message type which represents each object from the serialized object graph. The `Box` contains the information to restore the object. For a scalar types (int, long, double, ...), the box contains only the value. 
For `StorableTypes`, all class members  with the `Storable` attribute (fields and properties) including inherited members are stored in the `Box`. For each `Storable` member, HEAL.Attic stores the member value and additionally the name of the member as a key. Keys are used on deserialization to assign values to the correct members. The member value is stored in a separate `Box` and only the index of the box is stored as a reference. 
HEAL.Attic correctly resolves multiple references to an object within the object graph. It generates a `Box` when the object is first visited and adds the box to a collection. When HEAL.Attic detects that a `Box` for an object has already been generated then it will only store the reference to the `Box`. 

As a user of HEAL.Attic you will never work directly with `Bundles` or `Boxes`. If you extend HEAL.Attic, for example by [adding transformers](#Adding%20Transformers), then you need to work with `Boxes` directly.

All strings referenced in the object graph as well as `Storable` member names and GUIDs are added to a separate set of strings for serialization. HEAL.Attic detects duplicate strings and stores them only once. A `Box` only contains the index of the referenced string. 

HEAL.Attic never stores names of types. Instead each known type is identified by a GUID. The mapping of GUIDs to types is defined in the code and includes all (see [Supported Types](#Supported%Types)) as well as all loaded `StorableTypes`. This allows more flexibility of changing type names and class hierarchies while still keeping backwards compatibility. When HEAL.Attic deserializes an object graph it must restore all `Storable` members. For example, when we have stored a `List<IComparable>` then we instantiate this object on deserialization. For this purpose we store GUIDs of serialized types in boxes. Generic types are supported (including) complex or deeply nested generic types). 

#### Structs
Structs that should be included in serialization must be marked with the `StorableType` attribute. `HEAL.Attic` automatically (de-)serializes all fields within structs. It is therefore not necessary to mark fields with the `Storable` attribute. `HEAL.Attic` even throws an exception on serialization when it finds fields with `Storable` attributes within structs.


### Storable Constructor
Each storable class should have an empty constructor marked with the `StorableConstructor` attribute. This constructor can be used by HEAL.Attic to create empty objects which are subsequently filled with serialized data. This can be useful to save effort and to speed up deserialization because it is not necessary to initialize members of an object using a default constructor when data is later loaded from serialized format. 

HEAL.Attic provides the type `StorableConstructorFlag` which must be used as formal parameter for the storable constructor to prevent collisions with existing constructors. The storable constructor should be protected (or private in sealed classes). Don't forget to call the `StorableConstructor` of the base class if there is one.
```csharp
  [StorableType(Guid = "...")]
  public class A : AnotherStorableType {
      // ...

      [StorableConstructor]
      private A(StorableConstructstructorFlag flag) 
         : base(flag) { } // Empty ctor for deserialization.
                          // The base() call is only necessary
                          // if the base type also has a 
                          // storable constructor.

      // default constructor for A
      public A()  {
          // initialize object ...
      }
  }
```     

### BeforeSerialization and AfterDeserialization Hooks
Sometimes it is necessary to run code after deserialization has restored all objects. For instance to register event handlers after deserialization. We can accomplish this using an `AfterDeserialization` hook. In each storable type a single method can be marked as an `AfterDeserialization` hook. The method must return void and must not have parameters. We recommend to set the visibility of the method to `private`. HEAL.Attic calls the hook using reflection. 

Example: 
```csharp
  [StorableType(...)]
  class A {
    [Storable]
    int f;

    // ...
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }
  }
```
The method marked as `StorableHook(HookType.AfterDeserialization)` is called after all objects have been restored from the file. The `AfterDeserialization` hook from the base class is called before the hook from the derived class (recursively). Other than that no order of execution can be assumed.

In the same way it is possible to execute code before serialization. For instance to clear or preprocess data structures which should not be serialized completely.
```csharp
  [StorableType(...)]
  class A {
    [Storable]
    int f;

    // ...
    [StorableHook(HookType.BeforeSerialization)]
    private void BeforeSerialization() {
      CompactCaches();
    }
  }
```
 The `BeforeSerialization` hook from the derived class is called before the hook from the base class (recursively). `BeforeSerialization` are guaranteed to be called before the serializaer accesses any of the members marked as `Storable`. Other than that no order of execution of `BeforeSerialization` hooks can be assumed.

### Member Selection
It is possible to change how HEAL.Attic handles storable members by setting the MemberSelection property of the `StorableType` attribute. The `StorableMemberSelection` enum has four possible values `MarkedOnly`, `AllFields`, `AllProperties`, `AllFieldsAndProperties`. Whereby `MarkedOnly` is the default and means that each member which should be included in serialization / deserialization must be marked with the `Storable` attribute. If member selection is set to `AllFields` then HEAL.Attic will automatically serialize and deserialize all fields in the type. 

## Internals

### Strings
Strings are handled as immutable values. On serialization, HEAL.Attic adds each string in the object graph to a dictionary - keeping only unique strings. Each unique string is mapped to an integer id. The dictionary of strings and their ids is also stored to the serialized format. On deserialization, strings are restored from the serialized format via the id and the restored dictionary. 

### Serialization Workflow
HEAL.Attic executes the following steps when you call `ProtoBufSerializer.Serialize(obj, filename)`:
1. Execute the `BeforeSerialization` hooks for `obj`. `BeforeSerialization` hooks from the derived class are called before  the hooks from the base class (recursively).
1. Find all `Storable` members for `obj` including inherited members.   
1. **Serialize** each member recursively (starting at step 1). `Storable` members from the derived class are serialized before storable members from the base class. No other order of serialization of  `Storable` members can be assumed. 

### Deserialization Workflow
HEAL.Attic executes the following steps when you call `ProtoBufSerializer.Deserialize(filename)`:
1. Iterate over all boxes in the serialized file and **create objects using the `StorableConstructor`** or alternatively the default constructor if no `StorableConstructor` is available for the type. 
1. Start from the root box and **Restore all values of `Storable` members** using data from the serialized format.  Members of base classes are set before members of derived classes (recursively). No order can be assumed about value initialization of `Storable` members. 
1. After the whole object graph has been restored **call `AfterDeserialization` hooks.** The hook from the base class is called before the hook from the derived class (recursively).


### StorableType Attribute

#### Why is it necessary to generate GUIDs for storable types?
HEAL.Attic does not store type names in the serialized format. This makes it easy to rename of type and move types between namespaces and assemblies. Instead, HEAL.Attic uses GUIDs to identify types. When an object is deserialized HEAL.Attic first looks up the type that is currently registered for the serialized type GUID and instantiates an object of this type. This means that the actual type for a serialized object can change over time as long as the new type is backwards compatible.

As a consequence, every type must be marked with the `StorableType` attribute with a GUID. We provide tools (Visual Studio Code Fixes) to automatically add a StorableType attribute with  an autogenerated GUID (see [tools section](#tools)).

 It is even possible to merge two different types into a single type by adding two StorableType GUIDs to a class.
```csharp
  [StorableType("251601b7-3406-4038-9cfc-6332966240a3", "3fcaca9c-b474-4313-b00b-3db8082cb9e1")]
  public class TypeAggregator {
    ...
  }
``` 
   


#### Why is it necessary to mark interfaces and enums with the StorableType attribute?
HEAL.Attic does not store names of types in the serialized format. Instead, every non-generic type is represented by a single GUID. For generic types HEAL.Attic stores the GUID of the generic type definition as well as the GUIDs for all generic type parameters (recursively). When HEAL.Attic deserializes an object graph it must generate each object from the type GUID. 

In the following example we serialize a `Course` object with a generic `List<IPerson>` of attendees. The class `Person` implements `IPerson` and has the `Storable` property `Name`. Consequently, the classes `Course` and  `Person` must be marked with the `StorableType` attribute. 

Even though the list of attendees only contains `Person` objects, HEAL.Attic must generate a `List<IPerson>` on deserialization which holds the persons objects. Therefore, we need also need to generate a GUID for the interface `IPerson`. The framework type `List` is already defined as a known type with an associated GUID within HEAL.Attic.   
Note that the property `Name` of `IPerson` must not be marked as storable.
```csharp
  [StorableType(Guid = "424AB5BD-DC4E-472E-AF53-4A46D581C89A")]
  public interface IPerson {
      string Name { get; }
      // ...  
  }
  
  [StorableType(Guid = "17D8FBBD-E982-4490-9BC0-2ADE7B4358CD")]
  public class Person : IPerson {
      [Storable]
      string Name { get; private set; }
      // ...
      public Person(string name) {
          this.Name = name;
      }
  }

  [StorableType(Guid = "A99D46DA-5A68-483C-A69F-2FB309A77AA0")]
  public class Course {
      [Storable]
      public string Name { get; private set; }
      [Storable]
      public List<IPerson> { get; private set; }
      // ...
  }
```


## Adding Transformers
HEAL.Attic uses so-called `Transformers` to create `ProtoBuf` boxes for objects. We have implemented transformers for all primitive types and many collections of primitive types. For special edge cases it might however be necessary to add custom transformers. All transformers must implement the interface `ITransformer` to create ProtoBuf boxes for objects and to fill objects from ProtoBuf boxes. HEAL.Attic provides an abstract base class for `Tranformer`s.
```csharp
  public abstract class Transformer : ITransformer {
    // ...
    
    public abstract bool CanTransformType(Type type);
    public abstract Box ToBox(object o, Mapper mapper);
    public abstract object ToObject(Box box, Mapper mapper);

    // ...
  }
```

Customer transformers must be marked with the `Transformer` attribute with a GUID and a priority.  
For each type that must be serialized, HEAL.Attic calls `CanTransformType` to find all compatible transformers and uses the transformer with the highest priority.  
`ToBox(object o, ...)` must produce a ProtoBuf box with the data for the object.  
`ToObject(Box box, ...)` is the inverse operation and must restore the object from the box.

It is possible to have multiple different transformers for the same type. On serialization, HEAL.Attic will use the transformer with the highest priority. The GUID of the transformer which has been used to serialize an object is stored in the serialized format. Therefore, HEAL.Attic will be able to re-create the object with the same transformer on deserialization. As a consequence, you should never delete transformers because it breaks backwards compatibility.

Additionally to the transformer you need to register GUIDs of all types that are supported by the transformer and which you want to (de-)serialize. This is described in the next section. 

### Supported Types
The following types are supported by HEAL.Attic:
```
- object
- bool
- byte
- sbyte
- short
- ushort
- char
- int
- uint
- long
- ulong
- float
- double
- decimal
- System.DateTime
- System.TimeSpan
- System.Nullable<>
- string
- System.Array
- System.Tuple<>
- System.Tuple<,>
- System.Tuple<,,>
- System.Tuple<,,,>
- System.Tuple<,,,,>
- System.Tuple<,,,,,>
- System.Tuple<,,,,,,>
- System.Tuple<,,,,,,,>
- System.Drawing.Color
- System.Drawing.Point
- System.Guid
- System.Collections.Generic.KeyValuePair<,>
- System.Collections.IEnumerable
- System.Collections.Generic.IEnumerable<>
- System.Collections.IList
- System.Collections.Generic.List<>
- System.Collections.Generic.Stack<>
- System.Collections.Stack
- System.Collections.ArrayList
- System.Collections.Queue
- System.Collections.Generic.Queue<>
- System.Collections.Generic.HashSet<>
- System.Collections.Generic.Dictionary<,>
- System.Type
- System.StringComparer
- System.CultureAwareComparer
- System.OrdinalComparer
- System.Resources.FastResourceComparer
- System.Collections.CompatibleComparer
- System.Collections.IEqualityComparer
- System.Collections.Generic.EqualityComparer`1
- System.Collections.Generic.GenericEqualityComparer`1
- System.Collections.Generic.NullableEqualityComparer`1
- System.Collections.Generic.ObjectEqualityComparer`1
- System.Collections.Generic.ByteEqualityComparer
- System.Collections.Generic.EnumEqualityComparer`1
- System.Collections.Generic.LongEnumEqualityComparer`1
- System.Collections.Generic.IEqualityComparer`1
- System.Drawing.Bitmap
- System.Drawing.Font
- System.Drawing.FontStyle
- System.Drawing.GraphicsUnit
```



## Backwards Compatibility Examples
One of the design goals of HEAL.Attic is to allow loading of persisted files even after the code has been refactored. However, developers using HEAL.Attic must be careful when changing code to guarantee backwards compatibility. In the following, we describe several examples of commonly occurring cases of changes to the code and highlight the required code for backwards compatibility.

### Changing a class name
Changing the name or namespace of a type is easily possible. As long as the GUID for the type is not changed it will be possible to open existing files. 

Old code:
```csharp
  [StorableType("d57e1695-16b8-4de0-baba-4055154a5abe")]
  class MyClass {
      [Storable]
      public int value;
  }
```

New code:
```csharp
  [StorableType("d57e1695-16b8-4de0-baba-4055154a5abe")]
  class MyRenamedClass {
      [Storable]
      public int value;
  }
```


### Changing a property or field name
When we renamed a field or a property we can use a persistence property for backwards compatibility. Persistence properties only have a setter but no getter and allow us to forward and convert values from the old format to the new format. HEAL.Attic will not include properties without a getter when serializing the new format.

Old code:
```csharp
[StorableType("28A5F6B8-49AF-4C6A-AF0E-F92EB4511722")]
class A {
  [Storable]
  public int v; // this field will be renamed to value
}
```

New code (using a persistence property):
```csharp
[StorableType("28A5F6B8-49AF-4C6A-AF0E-F92EB4511722")]
class A {
  #region persistence property
  [Storable]
  private int v { 
      set { this.value = value; } 
  }
  #endregion

  [Storable]
  public int value;
}
```

Alternatively, we can use the `OldName` property of the `Storable` attribute instead of the persistence property. However, please note that this only works for the first rename. For multiple renames we must use persistence properties (see next section below).

New code (alternative version using `OldName`):
```csharp
[StorableType("28A5F6B8-49AF-4C6A-AF0E-F92EB4511722")]
class A {
  [Storable(OldName = "v")]
  public int value;
}
```

On deserialization, HEAL.Attic will set `value` using the persisted value for `v`. If the same object is serialized again then HEAL.Attic will use the new name `value` to store the field.


### Multiple renames of fields or properties
When we change the name of the field or property again it is necessary to introduce a private property for backwards compatibility. We recommend to move these properties into a region.

New code:
```csharp
[StorableType("28A5F6B8-49AF-4C6A-AF0E-F92EB4511722")]
public class A {
  #region backwards compatibility
  [Storable(OldName = "v")]
  private int value { 
    set { this.val = value; } // set value of the currently used field when v is deserialized
  }
  #endregion

  [Storable(OldName = "value")]
  public int val; // renamed from value (which was previously v)
}
```

### Changing the type of a property or field
We can use the `OldName` property for type conversion as well.

Old code:
```csharp
[StorableType("d57e1695-16b8-4de0-baba-4055154a5abe")]
class A {
  int value; // this is changed to a string
}
```

New code:
```csharp
[StorableType("d57e1695-16b8-4de0-baba-4055154a5abe")]
class A {
  #region backwards compatibility
  [Storable(OldName = "value")]
  private int intValue {
    set { this.value = value.ToString(); }
  }
  #endregion

  [Storable(Name = "stringValue")]
  string value; // used to be an int
}
```
In this example we want to keep the name `value` for the field in the code but change the data type from `int` to `string`. Because we now have two different versions of `value` we must distinguish these values in the serialized format with different names. Files stored in the old format contain an `int` for `A.value` while files stored with the new format contain a `string` for `A.value`. To work around this issue and support backwards compatibility, we use a persistence property with type `int` and `OldName = "value"` which is used to deserialize old files. Correspondingly, we tell HEAL.Attic to use `Name = "stringValue"` for `A.value` when using the new format.

### Non-trivial changes of field or property types
It can also be useful to merge multiple separate fields into a a single field using a structured type or splitting values of a structured type into separate fields. With HEAL.Attic this is possible using persistence properties and/or an `AfterDeserialization` hook (see above). 

In the following example we split a `Point` field into two `int` fields.
First version of `B`:
```csharp
[StorableType("7DCED655-D724-492E-9C6A-A22B376BADB2")]
class B {
  [Storable]
  public Point p; // will be split into x and y
}
```

Second version of `B` (split Point into x and y):
```csharp
[StorableType("7DCED655-D724-492E-9C6A-A22B376BADB2")]
class B {
  #region backwards compatibility
  // storable properties without a getter are not included in serialization
  [Storable]
  private Point p {
    set {
      x = value.X;
      y = value.Y;
    }
  }
  #endregion

  [Storable]
  public int x;

  [Storable]
  public int y;
}
```
In the example above we use a persistence property to set the values of x and y when a file with the old format is deserialized. `B.p` only has a setter. Therefore, HEAL.Attic will not include the property when serializing the new format. Instead, the fields `B.x`, and `B.y` will be included in the new format. Note, that we do not need the `OldName` property here because the names of old and new fields are different and we can therefore keep the name `p` for the persistence property. 

In the following example we combine two `int` fields `x` and `y` into a single `Point` field. 
Third version of `B` (combine x and y into a Point):
```csharp
[StorableType("7DCED655-D724-492E-9C6A-A22B376BADB2")]
class B {
  #region backwards compatibility
  [Storable]
  private int x { set { p = new Point(value, p.Y); } }

  [Storable]
  private int y { set { p = new Point(p.X, value); } }
  #endregion

  [Storable]
  public Point p { get; private set; }
} 
```
Again we do not need to use `OldName` because the names of fields are different.
We can consider the following cases:
 - Serialize V1 -> Deserialize V2: store `Point p` and load `int x, y` via persistence property
 - Serialize V1 -> Deserialize V3: store and load `Point p`
 - Serialize V2 -> Deserialize V3: store `int x, y` and load `Point p` via persistence property (p is set twice)

### Changing the class hierarchy
#### Adding new base classes
When you introduce a a new class within the class hierarchy of a storable class then it is necessary to specify how the storable members of the new class are set when deserializing the old format.

In the following example we assume that a new class is introduced whereby storable members of the parent class are moved to the new child class.

Old code:
```csharp
[StorableType(Guid = "7D5EF5BE-71ED-4DE7-BB5E-0AD4E94844DB")]
class A {
  [Storable]
  string Name;

  [Storable]
  string Description;
}

[StorableType(Guid = "B95F7976-9313-4473-B238-3EE44F1576A7")]
class C : A {
  [Storable]
  int p;
}
```

New code (`Description` is moved from existing class`A` to new class `B`):
```csharp
[StorableType(Guid = "7D5EF5BE-71ED-4DE7-BB5E-0AD4E94844DB")]
class A {
  [Storable]
  string Name;
}

[StorableType(Guid = "3DCA742D-C294-45A9-8E72-D21A9382DAE4")]
class B : A {
  [Storable(OldName = "base.Description")] // we can access storable members of the parent class with base. 
  string Description;
}

[StorableType(Guid = "B95F7976-9313-4473-B238-3EE44F1576A7")]
class C : B {
  [Storable]
  int p;
}
```

Here we can use the `OldName` property and the contextual keyword `base` to refer to a storable member which the old serialized format is stored as member of `A`. It is possible to traverse the parent hierarchy with multiple `base` references (`base.base.<...>`).
Since `B.Description` is a storable field it is included by HEAL.Attic the new serialization format.


#### Removing a base class
HEAL.Attic also supports removal of a base class in a hierarchy whereby two separate classes are merged into a single class. 

Old code: 
```csharp
[StorableType(Guid = "7D5EF5BE-71ED-4DE7-BB5E-0AD4E94844DB")]
class A {
  [Storable]
  string Name;
}

[StorableType(Guid = "3DCA742D-C294-45A9-8E72-D21A9382DAE4")]
class B : A {
  [Storable]
  string Description;
}

[StorableType(Guid = "B95F7976-9313-4473-B238-3EE44F1576A7")]
class C : B {
  [Storable]
  int p;
}
```

New code (class `AB` takes over the role of `A` and `B`, and `B.Description` is moved to `AB.Description`):
```csharp
[StorableType(Guid = "0F15C129-12D8-4319-8FF4-8F8FECB30A16")] // new GUID different from A and B
class AB {
  [Storable(OldName = "7D5EF5BE-71ED-4DE7-BB5E-0AD4E94844DB.Name", DefaultValue = "No name")] // retrieve value using the old GUID of A
  string Name;

  [Storable(OldName = "3DCA742D-C294-45A9-8E72-D21A9382DAE4.Description", DefaultValue = "No description")] 
  string Description; // retrieve value using the old GUID of B
}

[StorableType(Guid = "B95F7976-9313-4473-B238-3EE44F1576A7")]
class C : AB {
  [Storable]
  int p;
}
```
Carefully note the GUIDs used for A, B and AB. Here we do not reuse the type GUID for A or B and instead introduce a new type. We use the GUIDs of A and B to explicitly specify which fields we want to read from the serialized format.
If HEAL.Attic does not find the specified values in the serialized format the fields will be set to the specified `DefaultValue`s.


However, we need to be careful to handle the case correctly when a class X directly inherits from A instead of B.
Assume we have class `NewType` inheriting directly from `A` which also has a `Description` field. 
```csharp
[StorableType(Guid = "67C909B1-68C0-40A8-8B6F-771940267E17")]
class NewType : A {
  [Storable]
  string Description;
}
```
Here we assume that we now want to use `AB` and `AB.Description` instead of `NewType.Description`. Therefore, we need to set `AB.Description` when deserializing `NewType` in the old format. We accomplish this again with a persistence property.

```csharp
[StorableType(Guid = "67C909B1-68C0-40A8-8B6F-771940267E17")]
class NewType : AB {
  [Storable(OldName = "Description")]
  string Description_Persistence_Setter {
    private set {
      base.Description = value;
    }
  }
}
```


### Deleting a type
When you want to remove a type which has been marked as a `StorableType` then you have several options to make sure that old files can still be opened with the new version of your code.
1. Keep the (empty) class definition including the attached `StorableType` attribute. HEAL.Attic will create an object of the class on deserialization.
1. Delete the type but use a different (compatible) type as a replacement. Add the GUID from the deleted class to the list of GUIDs in the `StorableType` attribute of replacement type
1. Delete the type and remove the GUID. HEAL.Attic will detect that there is no registered type for a GUID it finds in the serialized format and will therefore set the corresponding `Storable` member to null.    


## Registering Types 
There are two use cases where you must register types explicitly:
 - you have implemented a custom transformer (for a new type) and
 - you load or generate types marked with `StorableType` at runtime (after HEAL.Attic has been initialized) 

### External types (without `StorableType` attribute)
With custom transformers you are able to implement (de-)serialization for any type including types which are defined within a library or nuget package (see [Adding Transformers](#Adding%20Transformers)). However, HEAL.Attic also requires that a GUID is registered for each serialized type so that later it is able to deserialize a file (see section [Principles](#Principles)). 

For the declaration of types with GUIDs you can simply add a class that implements the `IStorableTypeMap` interface. 
```csharp
  public interface IStorableTypeMap {
    IEnumerable<Tuple<Guid, Type>> KnownStorableTypes { get; }
  }
```

All GUID and type pairs returned in the `KnownStorableTypes` are registered by HEAL.Attic when it is initialized. 

### Dynamically loaded types (with `StorableType` attribute)
HEAL.Attic automatically discovers all loaded types marked with the `StorableType` attribute when it is first used (e.g. using `Serialize()` or `Deserialize()` or accessing `Mapper.StaticCache`). HEAL.Attic is therefore not aware of types that are loaded or generated at a later time. These types can be registered explicitly. For example if the new type `MyDynamicType` is loaded at runtime you can register the type using:
```csharp
  Mapper.StaticCache.RegisterType(
    new Guid("9CF55419-439B-4A90-B2ED-8C7F7768EB61"), 
    Type.GetType("MyDynamicType"));
``` 

This method of registering types can be used for any type including types marked with the `StorableType` attribute. Therefore, you can also use it instead of the `IStorableTypeMap` interface as explained above. However, be aware that on the first call of `Mapper.StaticCache.RegisterType` you will trigger the initialization of HEAL.Attic, whereby it discovers all currently loaded `StorableType`s. 

## Benchmarks
The source folder contains the `HEAL.Attic.Benchmarks` console application which runs a set of benchmark tests. The benchmarking results can be found in [Benchmarks](Benchmarks.md)

## License
HEAL.Attic is [licensed](../LICENSE.txt) under the MIT License.

```
MIT License

Copyright (c) 2018-present Heuristic and Evolutionary Algorithms Laboratory

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```