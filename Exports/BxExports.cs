using System;
using System.Reflection;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Models.Exports;

namespace BxHelpers.Exports;

public class BxExports : BaseScript
{
    private static string _thisResourceName = null!;
    private static ExportDictionary _exports = null!;

    public BxExports()
    {
        // Get the current resource name and exports dictionary
        _thisResourceName = API.GetCurrentResourceName();
        _exports = Exports;

        // Register an export function
        Exports.Add("ExecuteExport", new Func<string, string, string, object?>(ExecuteExport));
    }

    private static object? CallExportInner(string resourceName, BaseExport data)
    {
        var dataType = data.GetType().Name;
        var serializedData = BxSerializer.Serialize(data);

        try
        {
            var returnVal = _exports[resourceName].ExecuteExport(_thisResourceName, dataType, serializedData);
            return returnVal;
        }
        catch
        {
            Debug.WriteLine($"Resource '{resourceName}' *probably* does not support BxExports, visit https://github.com/borsuczyna/BxHelpers?tab=readme-ov-file#failed-to-call-export for more information.");
        }

        return null;
    }

    /// <summary>
    /// Calls an export from another resource and returns the result.
    /// </summary>
    public static T? CallExport<T>(string resourceName, BaseExport data) where T : class
    {
        try
        {
            var returnVal = CallExportInner(resourceName, data);
            if (returnVal == null)
            {
                return null;
            }

            if (typeof(T).IsClass)
            {
                return BxSerializer.Deserialize<T>(returnVal.ToString());
            }

            return (T)Convert.ChangeType(returnVal, typeof(T));
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Calls an export from another resource and returns the result.
    /// </summary>
    public static T? CallExportSimple<T>(string resourceName, BaseExport data)
    {
        try
        {
            var returnVal = CallExportInner(resourceName, data);
            if (returnVal == null)
            {
                return default;
            }

            return (T)Convert.ChangeType(returnVal, typeof(T));
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Calls an export from another resource.
    /// </summary>
    public static void CallExport(string resourceName, BaseExport data)
    {
        CallExport<object>(resourceName, data);
    }

    /// <summary>
    /// Executes an export function when called by another resource.
    /// </summary>
    public string? ExecuteExport(string callerResourceName, string dataType, string serializedData)
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types)
        {
            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                var attr = (BxExport)Attribute.GetCustomAttribute(method, typeof(BxExport));

                if (attr != null && attr.Type.Name == dataType)
                {
                    var deserializedData = BxSerializer.Deserialize(serializedData, attr.Type);
                    if (!method.IsStatic)
                    {
                        Debug.WriteLine("Method must be static!!");
                        return null;
                    }

                    var result = method.Invoke(null, [deserializedData]);
                    return BxSerializer.Serialize(result);
                }
            }
        }

        return null;
    }
}