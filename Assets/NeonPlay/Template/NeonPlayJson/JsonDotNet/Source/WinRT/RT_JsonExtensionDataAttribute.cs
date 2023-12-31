#if (UNITY_WINRT || UNITY_WP_8_1) && !UNITY_EDITOR && !UNITY_WP8
using System;

namespace NeonPlay.Newtonsoft.Json
{
  /// <summary>
  /// Instructs the <see cref="JsonSerializer"/> to deserialize properties with no matching class member into the specified collection
  /// and write values during serialization.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class JsonExtensionDataAttribute : System.Attribute
  {
    /// <summary>
    /// Gets or sets a value that indicates whether to write extension data when serializing the object.
    /// </summary>
    /// <value>
    /// 	<c>true</c> to write extension data when serializing the object; otherwise, <c>false</c>. The default is <c>true</c>.
    /// </value>
    public bool WriteData { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether to read extension data when deserializing the object.
    /// </summary>
    /// <value>
    /// 	<c>true</c> to read extension data when deserializing the object; otherwise, <c>false</c>. The default is <c>true</c>.
    /// </value>
    public bool ReadData { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonExtensionDataAttribute"/> class.
    /// </summary>
    public JsonExtensionDataAttribute()
    {
      WriteData = true;
      ReadData = true;
    }
  }
}
#endif