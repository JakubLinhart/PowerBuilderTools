// This code was autogenerated.
// Changes to this file will be lost if the file is regenerated.
using System;
namespace PowerDoc.PowerObjects {
	public class OmDisplayType : HardwiredObject {
		static OmDisplayType instance;

		static OmDisplayType() {
			instance = new OmDisplayType();
		}

		static public void Init() {
		}

		public static OmDisplayType Instance {
			get { return instance; }
		}

		protected OmDisplayType() : base(PowerObject.PowerObjectParent, "OmDisplayType", EnumeratedType.Instance) {
		}
	}
}