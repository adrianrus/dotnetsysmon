``` ini

BenchmarkDotNet=v0.11.3, OS=Windows 10.0.14393.2248 (1607/AnniversaryUpdate/Redstone1)
Intel Core i7-4790 CPU 3.60GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3507519 Hz, Resolution=285.1018 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3062.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3062.0


```
| Method |     Mean |    Error |   StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|------- |---------:|---------:|---------:|------------:|------------:|------------:|--------------------:|
|    Get | 520.2 ms | 10.32 ms | 14.47 ms |           - |           - |           - |             1.14 MB |
