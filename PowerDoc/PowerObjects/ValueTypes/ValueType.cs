using System;
using System.Diagnostics;

namespace PowerDoc.PowerObjects {
	public class ValueType : HardwiredObject {
		protected ValueType(IPBContainer parent, string name, PowerObject super_class) : base (parent, name, super_class) {
		}	
	}
}