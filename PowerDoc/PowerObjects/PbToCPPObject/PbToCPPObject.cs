// This code was autogenerated.
// Changes to this file will be lost if the file is regenerated.
using System;
namespace PowerDoc.PowerObjects {
	public class PbToCPPObject : HardwiredObject {
		static PbToCPPObject instance;

		static PbToCPPObject() {
			instance = new PbToCPPObject();
		}

		static public void Init() {
		}

		public static PbToCPPObject Instance {
			get { return instance; }
		}

		protected PbToCPPObject() : base(PowerObject.PowerObjectParent, "PbToCPPObject", PowerObject.PowerObjectInstance) {
		}
	}
}