// This code was autogenerated.
// Changes to this file will be lost if the file is regenerated.
using System;
namespace PowerDoc.PowerObjects {
	public class RoundRectangle : HardwiredObject {
		static RoundRectangle instance;

		static RoundRectangle() {
			instance = new RoundRectangle();
		}

		static public void Init() {
		}

		public static RoundRectangle Instance {
			get { return instance; }
		}

		protected RoundRectangle() : base(PowerObject.PowerObjectParent, "RoundRectangle", DrawObject.Instance) {
		}
	}
}
