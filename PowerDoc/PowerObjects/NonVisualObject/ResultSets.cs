// This code was autogenerated.
// Changes to this file will be lost if the file is regenerated.
using System;
namespace PowerDoc.PowerObjects {
	public class ResultSets : HardwiredObject {
		static ResultSets instance;

		static ResultSets() {
			instance = new ResultSets();
		}

		static public void Init() {
		}

		public static ResultSets Instance {
			get { return instance; }
		}

		protected ResultSets() : base(PowerObject.PowerObjectParent, "ResultSets", NonVisualObject.Instance) {
		}
	}
}
