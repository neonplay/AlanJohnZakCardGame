using System;

namespace NeonPlay {

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class SerialiseIncludeAttribute : Attribute {

	}
}
