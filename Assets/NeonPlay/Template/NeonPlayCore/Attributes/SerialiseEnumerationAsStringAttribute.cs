using System;

namespace NeonPlay {

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class SerialiseEnumerationAsStringAttribute : SerialiseIncludeAttribute {

		public readonly string Name;
		public readonly bool LowerCase;

		public SerialiseEnumerationAsStringAttribute(string propertyName = null, bool forceLowerCase = false) {

			Name = propertyName;
			LowerCase = forceLowerCase;
		}
	}
}
