// This code was autogenerated.
// Changes to this file will be lost if the file is regenerated.
using System;
namespace PowerDoc.PowerObjects {
	public class MaskDataType : HardwiredObject {
		static MaskDataType instance;

		static MaskDataType() {
			instance = new MaskDataType();
		}

		static public void Init() {
		}

		public static MaskDataType Instance {
			get { return instance; }
		}

		protected MaskDataType() : base(PowerObject.PowerObjectParent, "MaskDataType", EnumeratedType.Instance) {
		}
	}
}