[![Linux](https://github.com/heal-research/HEAL.Attic/actions/workflows/linux.yml/badge.svg)](https://github.com/heal-research/HEAL.Attic/actions/workflows/linux.yml) [![Windows](https://github.com/heal-research/HEAL.Attic/actions/workflows/windows.yml/badge.svg)](https://github.com/heal-research/HEAL.Attic/actions/workflows/windows.yml)

# Introduction

HEAL.Attic is a serialization and persistence framework for .NET. It allows you to save objects to a file and later restore these objects. HEAL.Attic serializes and deserializes complete object graphs. It uses Google Protocol Buffers for compact storage.

# Examples
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

We can then save objects of type `Person` using the `ProtoBufSerializer`.

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
          serializer.Serialize(family, "family.bin");
      }
  }
```
More examples can be found in the [manual](docs/Manual.md).

# Using HEAL.Attic
Simply add the [HEAL.Attic nuget package](https://www.nuget.org/packages/HEAL.Attic/).

HEAL.Attic uses the .NET Standard 2.0 interface. It can be included in .NET Core as well as in .NET Framework projects. 



# Background Information
HEAL.Attic is a spin-off project of [HeuristicLab](https://dev.heuristiclab.com) which is a software environment for heuristic optimization algorithms by the research group [HEAL](https://heal.heuristiclab.com) of the University of Applied Sciences Upper Austria. We have been working on HeuristicLab for more than 15 years and need a stable and efficient persistence framework. We use HEAL.Attic for example to save optimization results or to save the state of running algorithms. Our platform for distributed computing uses HEAL.Attic to serialize and transfer jobs and results to and from compute nodes. 

Our main objectives for HEAL.Attic are:
 - Support for large and complex object graphs
 - Long-term stable data format which allows deserialization of files even if:
   - fields or properties have been renamed or removed,
   - classes have been renamed or moved to a different namespace or assembly,
   - the class hierarchy has been changed (e.g. by introducing a new base type).
 - Small file size  

# Software using HEAL.Attic
* [HeuristicLab](https://dev.heuristiclab.com) is a software environment for heuristic optimization algorithms. 

# License
HEAL.Attic is [licensed](LICENSE.txt) under the MIT License.

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