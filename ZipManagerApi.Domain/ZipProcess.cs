using System;

namespace ZipManagerApi.Domain;

public record ZipProcess(Guid ProcessId, Task<string> ZipTask) { }
