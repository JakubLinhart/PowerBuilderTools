// This code was autogenerated.
// Changes to this file will be lost if the file is regenerated.
using System;
namespace PowerDoc.PowerObjects {
	public class PbTypes : HardwiredObject {
		static PbTypes instance;

		static PbTypes() {
			instance = new PbTypes();
		}

		static public void Init() {
		}

		public static PbTypes Instance {
			get { return instance; }
		}

		protected PbTypes() : base(PowerObject.PowerObjectParent, "PbTypes", EnumeratedType.Instance) {
		}
	}
}
