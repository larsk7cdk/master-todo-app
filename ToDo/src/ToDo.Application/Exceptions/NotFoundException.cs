using System;

namespace ToDo.Shared.Application.Exceptions;

public sealed class NotFoundException(string name, object key) : Exception($"{name} with key {key} not found");
