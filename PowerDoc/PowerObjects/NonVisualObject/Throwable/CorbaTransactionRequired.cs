// This code was autogenerated.
// Changes to this file will be lost if the file is regenerated.
using System;
namespace PowerDoc.PowerObjects {
	public class CorbaTransactionRequired : HardwiredObject {
		static CorbaTransactionRequired instance;

		static CorbaTransactionRequired() {
			instance = new CorbaTransactionRequired();
		}

		static public void Init() {
		}

		public static CorbaTransactionRequired Instance {
			get { return instance; }
		}

		protected CorbaTransactionRequired() : base(PowerObject.PowerObjectParent, "CorbaTransactionRequired", CorbaSystemException.Instance) {
		}
	}
}