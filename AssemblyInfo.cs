using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(1)] // Use the number of logical CPU cores or desired parallelism level