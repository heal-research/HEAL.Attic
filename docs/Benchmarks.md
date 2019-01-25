# HEAL.Attic Benchmarks
Benchmarking results for `HEAL.Attic.Benchmarks` are shown in the tables below.

Hardware Specs:  
Intel(R) Core(TM) i7-6600U CPU @ 2.60GHz, 2 Core(s), 4 Logical Processor(s)  
Physical Memory (RAM)	16.0 GB
Microsoft Windows 10 Education  
.NET Standard 2.0

### Array of random ints:

```csharp
      var arr = new int[size];
      for (int i = 0; i < arr.Length; i++) {
        arr[i] = rand.Next();
      }
```
| Elements | Serialization time (ms) | Deserialization time (ms) | File size (kB) | avg. bytes per element |
|---------:|------------------------:|--------------------------:|---------------:|-----------------------:|
|     2048 |                     3.9 |                       0.8 |            9.8 |                    4.9 |
|     4096 |                     3.9 |                       0.8 |           19.6 |                    4.9 |
|     8192 |                     3.9 |                       0.8 |           39.1 |                    4.9 |
|    16384 |                     4.0 |                       0.8 |           78.2 |                    4.9 |
|    32768 |                     4.4 |                       0.8 |          156.1 |                    4.9 |
|    65536 |                     5.7 |                       1.5 |          312.0 |                    4.9 |
|   131072 |                     8.9 |                       3.2 |          624.0 |                    4.9 |
|   262144 |                    16.1 |                       7.2 |        1 247.9 |                    4.9 |
|   524288 |                    32.0 |                      16.6 |        2 495.4 |                    4.9 |
|  1048576 |                    67.2 |                      36.3 |        4 991.0 |                    4.9 |
|  2097152 |                   135.0 |                      75.2 |        9 982.4 |                    4.9 |

The ProtoBuf encoding has an overhead of one byte per int when the range of values covers almost the whole int range. However, if only a limited range of values is possible then the ProtoBuf encoding will  have significant benefits (as shown below). 

### Array of ints from limited range:
```csharp
      int maxInt = 20;
      var arr = new int[size];
      for (int i = 0; i < arr.Length; i++) {
        arr[i] = rand.Next(maxInt);
      }
```

| Elements | Serialization time (ms) | Deserialization time (ms) | File size (kB) | avg. bytes per element |
|---------:|------------------------:|--------------------------:|---------------:|-----------------------:|
|     2048 |                     0.0 |                       0.0 |            2.1 |                    1.1 |
|     4096 |                     0.1 |                       0.0 |            4.1 |                    1.0 |
|     8192 |                     0.1 |                       0.0 |            8.1 |                    1.0 |
|    16384 |                     0.1 |                       0.0 |           16.1 |                    1.0 |
|    32768 |                     0.1 |                       0.0 |           32.1 |                    1.0 |
|    65536 |                     0.3 |                       0.3 |           64.1 |                    1.0 |
|   131072 |                     1.5 |                       1.4 |          128.1 |                    1.0 |
|   262144 |                     4.9 |                       4.9 |          256.1 |                    1.0 |
|   524288 |                    11.6 |                      11.2 |          512.1 |                    1.0 |
|  1048576 |                    27.2 |                      28.6 |        1 024.1 |                    1.0 |
|  2097152 |                    57.8 |                      59.1 |        2 048.1 |                    1.0 |

### Array of doubles:
```csharp
      var arr = new double[size];
      for (int i = 0; i < arr.Length; i++) {
        arr[i] = rand.NextDouble();
      }
```

| Elements | Serialization time (ms) | Deserialization time (ms) | File size (kB) | avg. bytes per element |
|---------:|------------------------:|--------------------------:|---------------:|-----------------------:|
|     2048 |                     0.1 |                       0.1 |           16.1 |                    8.1 |
|     4096 |                     0.1 |                       0.1 |           32.1 |                    8.0 |
|     8192 |                     0.1 |                       0.1 |           64.1 |                    8.0 |
|    16384 |                     0.1 |                       0.1 |          128.1 |                    8.0 |
|    32768 |                     0.4 |                       0.9 |          256.1 |                    8.0 |
|    65536 |                     1.5 |                       2.5 |          512.1 |                    8.0 |
|   131072 |                     4.7 |                       6.5 |        1 024.1 |                    8.0 |
|   262144 |                    12.0 |                      16.0 |        2 048.1 |                    8.0 |
|   524288 |                    30.0 |                      36.5 |        4 096.1 |                    8.0 |
|  1048576 |                    65.6 |                      77.8 |        8 192.1 |                    8.0 |
|  2097152 |                   143.8 |                     159.7 |       16 384.1 |                    8.0 |


### Random graph of nodes:  
```csharp
      int maxChildren = 20;
      var allNodes = new List<Node>();
      allNodes.Add(new Node()); // start with at least one node
      for (int i = 0; i < size; i++) {
        var node = new Node();
        var numChildren = rand.Next(maxChildren) + 1;
        for (int j = 0; j < numChildren; j++) {
          node.children.Add(allNodes[rand.Next(allNodes.Count)]);
        }
        allNodes.Add(node);
      }
```

| Elements | Serialization time (ms) | Deserialization time (ms) | File size (kB) | avg. bytes per element |
|---------:|------------------------:|--------------------------:|---------------:|-----------------------:|
|     2048 |                    20.8 |                      43.0 |           93.1 |                   46.6 |
|     4096 |                    62.8 |                     132.7 |          189.5 |                   47.4 |
|     8192 |                   165.4 |                     317.2 |          378.6 |                   47.3 |
|    16384 |                   423.5 |                     727.3 |          807.7 |                   50.5 |
|    32768 |                   994.2 |                   1 553.0 |        1 717.5 |                   53.7 |
|    65536 |                 2 203.7 |                   3 284.4 |        3 593.6 |                   56.2 |
|   131072 |                 4 539.8 |                   6 569.4 |        7 404.0 |                   57.8 |
|   262144 |                 9 078.2 |                  12 655.8 |       15 098.1 |                   59.0 |
|   524288 |                18 307.6 |                  24 562.7 |       30 504.1 |                   59.6 |
|  1048576 |                37 107.5 |                  48 034.5 |       61 451.6 |                   60.0 |