using System;

namespace NeonPlay {

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
	public class SerialiseOptInAttribute : Attribute {

	}
}
