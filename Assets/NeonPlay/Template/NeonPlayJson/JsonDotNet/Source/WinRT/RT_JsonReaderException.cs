#if (UNITY_WINRT || UNITY_WP_8_1) && !UNITY_EDITOR && !UNITY_WP8
#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Globalization;
using System.Runtime.Serialization;
using NeonPlay.Newtonsoft.Json.Utilities;

namespace NeonPlay.Newtonsoft.Json
{
  /// <summary>
  /// The exception thrown when an error occurs while reading Json text.
  /// </summary>
  public class JsonReaderException : JsonException
  {
    /// <summary>
    /// Gets the line number indicating where the error occurred.
    /// </summary>
    /// <value>The line number indicating where the error occurred.</value>
    public int LineNumber { get; private set; }

    /// <summary>
    /// Gets the line position indicating where the error occurred.
    /// </summary>
    /// <value>The line position indicating where the error occurred.</value>
    public int LinePosition { get; private set; }

    /// <summary>
    /// Gets the path to the JSON where the error occurred.
    /// </summary>
    /// <value>The path to the JSON where the error occurred.</value>
    public string Path { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonReaderException"/> class.
    /// </summary>
    public JsonReaderException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonReaderException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public JsonReaderException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonReaderException"/> class
    /// with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
    public JsonReaderException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    internal JsonReaderException(string message, Exception innerException, string path, int lineNumber, int linePosition)
      : base(message, innerException)
    {
      Path = path;
      LineNumber = lineNumber;
      LinePosition = linePosition;
    }

    internal static JsonReaderException Create(JsonReader reader, string message)
    {
      return Create(reader, message, null);
    }

    internal static JsonReaderException Create(JsonReader reader, string message, Exception ex)
    {
      return Create(reader as IJsonLineInfo, reader.Path, message, ex);
    }

    internal static JsonReaderException Create(IJsonLineInfo lineInfo, string path, string message, Exception ex)
    {
      message = JsonPosition.FormatMessage(lineInfo, path, message);

      int lineNumber;
      int linePosition;
      if (lineInfo != null && lineInfo.HasLineInfo())
      {
        lineNumber = lineInfo.LineNumber;
        linePosition = lineInfo.LinePosition;
      }
      else
      {
        lineNumber = 0;
        linePosition = 0;
      }

      return new JsonReaderException(message, ex, path, lineNumber, linePosition);
    }
  }
}

#endif