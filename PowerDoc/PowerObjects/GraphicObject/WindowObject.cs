// This code was autogenerated.
// Changes to this file will be lost if the file is regenerated.
using System;
namespace PowerDoc.PowerObjects {
	public class WindowObject : HardwiredObject {
		static WindowObject instance;

		static WindowObject() {
			instance = new WindowObject();
		}

		static public void Init() {
		}

		public static WindowObject Instance {
			get { return instance; }
		}

		protected WindowObject() : base(PowerObject.PowerObjectParent, "WindowObject", GraphicObject.Instance) {
		}
	}
}