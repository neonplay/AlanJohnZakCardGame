#if !UNITY_WINRT || UNITY_EDITOR || (UNITY_WP8 &&  !UNITY_WP_8_1)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonPlay.Newtonsoft.Json.Linq
{
  /// <summary>
  /// Represents a collection of <see cref="JToken"/> objects.
  /// </summary>
  /// <typeparam name="T">The type of token</typeparam>
  public interface IJEnumerable<T> : IEnumerable<T> where T : JToken
  {
    /// <summary>
    /// Gets the <see cref="IJEnumerable{JToken}"/> with the specified key.
    /// </summary>
    /// <value></value>
    IJEnumerable<JToken> this[object key] { get; }
  }
}
#endif