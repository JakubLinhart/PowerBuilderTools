// This code was autogenerated.
// Changes to this file will be lost if the file is regenerated.
using System;
namespace PowerDoc.PowerObjects {
	public class OmLinkUpdateOptions : HardwiredObject {
		static OmLinkUpdateOptions instance;

		static OmLinkUpdateOptions() {
			instance = new OmLinkUpdateOptions();
		}

		static public void Init() {
		}

		public static OmLinkUpdateOptions Instance {
			get { return instance; }
		}

		protected OmLinkUpdateOptions() : base(PowerObject.PowerObjectParent, "OmLinkUpdateOptions", EnumeratedType.Instance) {
		}
	}
}
