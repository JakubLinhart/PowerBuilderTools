// This code was autogenerated.
// Changes to this file will be lost if the file is regenerated.
using System;
namespace PowerDoc.PowerObjects {
	public class TreeView : HardwiredObject {
		static TreeView instance;

		static TreeView() {
			instance = new TreeView();
		}

		static public void Init() {
		}

		public static TreeView Instance {
			get { return instance; }
		}

		protected TreeView() : base(PowerObject.PowerObjectParent, "TreeView", DragObject.Instance) {
		}
	}
}
