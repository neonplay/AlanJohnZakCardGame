#if !UNITY_WINRT || UNITY_EDITOR || (UNITY_WP8 &&  !UNITY_WP_8_1)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonPlay.Newtonsoft.Json.Serialization
{
  /// <summary>
  /// Contract details for a <see cref="Type"/> used by the <see cref="JsonSerializer"/>.
  /// </summary>
  public class JsonLinqContract : JsonContract
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonLinqContract"/> class.
    /// </summary>
    /// <param name="underlyingType">The underlying type for the contract.</param>
    public JsonLinqContract(Type underlyingType)
      : base(underlyingType)
    {
    }
  }
}
#endif