``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1992/22H2/2022Update/SunValley2)
11th Gen Intel Core i7-11390H 3.40GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.203
  [Host]     : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2
  Job-DRNPBU : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2

InvocationCount=10000  IterationCount=10  LaunchCount=1  
WarmupCount=1  

```
|           Method |     Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 |   Gen1 | Allocated | Alloc Ratio |
|----------------- |---------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
|    FastEndpoints | 76.18 μs | 40.535 μs | 26.812 μs |  1.00 |    0.00 | 2.7000 | 2.7000 |  16.65 KB |        1.00 |
|       MinimalApi | 61.27 μs |  6.838 μs |  4.069 μs |  0.93 |    0.24 | 2.7000 | 2.7000 |  16.95 KB |        1.02 |
| MinimalApiReprV2 | 79.60 μs |  6.995 μs |  4.162 μs |  1.22 |    0.38 | 3.8000 | 3.8000 |  23.64 KB |        1.42 |
|     MinimalApiV2 | 80.90 μs | 12.055 μs |  7.974 μs |  1.20 |    0.45 | 3.6000 | 3.6000 |  22.35 KB |        1.34 |
|    AspNetCoreMVC | 72.33 μs |  1.278 μs |  0.761 μs |  1.11 |    0.33 | 3.8000 | 3.8000 |  23.58 KB |        1.42 |
