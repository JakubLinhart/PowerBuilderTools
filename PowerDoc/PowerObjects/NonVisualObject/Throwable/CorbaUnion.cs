// This code was autogenerated.
// Changes to this file will be lost if the file is regenerated.
using System;
namespace PowerDoc.PowerObjects {
	public class CorbaUnion : HardwiredObject {
		static CorbaUnion instance;

		static CorbaUnion() {
			instance = new CorbaUnion();
		}

		static public void Init() {
		}

		public static CorbaUnion Instance {
			get { return instance; }
		}

		protected CorbaUnion() : base(PowerObject.PowerObjectParent, "CorbaUnion", NonVisualObject.Instance) {
		}
	}
}
