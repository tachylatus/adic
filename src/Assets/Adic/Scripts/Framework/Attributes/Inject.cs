using System;

namespace Adic {
	/// <summary>
	/// Marks a setter injection point.
	/// 
	/// If an identifier is provided, the injector looks the binder for a key with the given name.
	/// 
	/// If no identifier is provided, the injector looks the binder for a key of the type of the field/property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,
		AllowMultiple = false,
		Inherited = true)]
	public class Inject : Attribute {
		/// <summary>The identifier of the binding to inject.</summary>
		public string identifier;

		/// <summary>
		/// Initializes a new instance of the <see cref="Inject"/> class.
		/// </summary>
		public Inject() {
			this.identifier = null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Inject"/> class.
		/// </summary>
		/// <param name="identifier">The identifier of the binding to inject.</param>
		public Inject(string identifier) {
			this.identifier = identifier;
		}
	}
}