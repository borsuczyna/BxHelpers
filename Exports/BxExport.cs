using System;

namespace BxHelpers.Exports;

/// <summary>
/// Attribute to mark methods as exportable functions.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class BxExport : Attribute
{
    public string ExportName { get; private set; }
    public Type Type { get; private set; }

    /// <summary>
    /// Exports a function.
    /// </summary>
    /// <param name="type">Data type associated with the export.</param>
    /// <returns></returns>
    public BxExport(Type type)
    {
        Type = type;
        ExportName = type.Name;
    }
}