// This code was autogenerated.
// Changes to this file will be lost if the file is regenerated.
using System;
namespace PowerDoc.PowerObjects {
	public class LibExportType : HardwiredObject {
		static LibExportType instance;

		static LibExportType() {
			instance = new LibExportType();
		}

		static public void Init() {
		}

		public static LibExportType Instance {
			get { return instance; }
		}

		protected LibExportType() : base(PowerObject.PowerObjectParent, "LibExportType", EnumeratedType.Instance) {
		}
	}
}