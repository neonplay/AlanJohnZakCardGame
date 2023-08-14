using System;

namespace NeonPlay {

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class SerialiseNameAttribute : SerialiseIncludeAttribute {

		public readonly string Name;

		public SerialiseNameAttribute(string propertyName) {

			Name = propertyName;
		}
	}
}
