// This code was autogenerated.
// Changes to this file will be lost if the file is regenerated.
using System;
namespace PowerDoc.PowerObjects {
	public class DataWindowChild : HardwiredObject {
		static DataWindowChild instance;

		static DataWindowChild() {
			instance = new DataWindowChild();
		}

		static public void Init() {
		}

		public static DataWindowChild Instance {
			get { return instance; }
		}

		protected DataWindowChild() : base(PowerObject.PowerObjectParent, "DataWindowChild", Structure.Instance) {
		}
	}
}