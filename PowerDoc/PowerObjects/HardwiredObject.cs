using System;

namespace PowerDoc.PowerObjects {
	public class HardwiredObject : PowerObject {
		protected HardwiredObject(IPBContainer parent, string name, PowerObject super_class) : base (parent, name, super_class) {
			Hardwired.AddClass(this);
		}

		public override bool IsHardwired {
			get { return true; }
		}
	}
}
