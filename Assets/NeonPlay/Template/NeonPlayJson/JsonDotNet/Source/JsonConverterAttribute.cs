#if !UNITY_WINRT || UNITY_EDITOR || (UNITY_WP8 &&  !UNITY_WP_8_1)
using System;
using NeonPlay.Newtonsoft.Json.Utilities;
using System.Globalization;

namespace NeonPlay.Newtonsoft.Json
{
  /// <summary>
  /// Instructs the <see cref="JsonSerializer"/> to use the specified <see cref="JsonConverter"/> when serializing the member or class.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Parameter, AllowMultiple = false)]
  public sealed class JsonConverterAttribute : System.Attribute
  {
    private readonly Type _converterType;

    /// <summary>
    /// Gets the type of the converter.
    /// </summary>
    /// <value>The type of the converter.</value>
    public Type ConverterType
    {
      get { return _converterType; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonConverterAttribute"/> class.
    /// </summary>
    /// <param name="converterType">Type of the converter.</param>
    public JsonConverterAttribute(Type converterType)
    {
      if (converterType == null)
        throw new ArgumentNullException("converterType");

      _converterType = converterType;
    }

    internal static JsonConverter CreateJsonConverterInstance(Type converterType)
    {
      try
      {
        return (JsonConverter)Activator.CreateInstance(converterType);
      }
      catch (Exception ex)
      {
        throw new Exception("Error creating {0}".FormatWith(CultureInfo.InvariantCulture, converterType), ex);
      }
    }
  }
}
#endif