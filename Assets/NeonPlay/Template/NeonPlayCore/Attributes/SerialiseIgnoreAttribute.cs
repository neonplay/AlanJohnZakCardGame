using System;

namespace NeonPlay {

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class SerialiseIgnoreAttribute : Attribute {

	}
}
