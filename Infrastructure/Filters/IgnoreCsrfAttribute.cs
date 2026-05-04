using System;

namespace AvalWebBackend.Infrastructure.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class IgnoreCsrfAttribute : Attribute
{
}