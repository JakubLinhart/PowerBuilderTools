// This code was autogenerated.
// Changes to this file will be lost if the file is regenerated.
using System;
namespace PowerDoc.PowerObjects {
	public class ScriptDefinition : HardwiredObject {
		static ScriptDefinition instance;

		static ScriptDefinition() {
			instance = new ScriptDefinition();
		}

		static public void Init() {
		}

		public static ScriptDefinition Instance {
			get { return instance; }
		}

		protected ScriptDefinition() : base(PowerObject.PowerObjectParent, "ScriptDefinition", ClassDefinitionObject.Instance) {
		}
	}
}
