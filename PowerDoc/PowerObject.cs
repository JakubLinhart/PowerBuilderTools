// Copyright (C) 2007  Jakub Linhart
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections;
using System.Diagnostics;

namespace PowerDoc {
	public class PowerObject : PBRoot, IPBContainer {
		public enum Access {Public, Protected, Private, Global};

		private static PowerObject powerObject;

		private PowerObject superClass;                       // predek
		private PowerObject parentClass;                      // v kterem objektu je tento objekt umisten
		private Hashtable childClasses = new Hashtable();     // tridy, ktere jsou obsazene v tomto objektu
		private Hashtable inheritedClasses = new Hashtable(); // oddedene tridy z tohoto objektu
		private PowerObject[] sortedInheritedClasses = new PowerObject[0];
		private Hashtable scriptFunctions = new Hashtable();
		private ScriptFunctionsEnumerable functionsEnumerable;
		private ScriptFunction[] sortedFunctions = new ScriptFunction[0];
		private Hashtable scriptEvents = new Hashtable();
		private ScriptEvent[] sortedEvents = new ScriptEvent[0];
		private bool autoInstantiate = false;

		private string unresolvedSuperClass;

		static PowerObject() {
			powerObject = new PowerObject(Hardwired.Instance, "PowerObject");
			Hardwired.InitObjects();
		}

		public static PowerObject PowerObjectInstance {
			get { return powerObject; }
		}

		public static IPBContainer PowerObjectParent {
			get { return Hardwired.Instance; }
		}

		protected PowerObject(IPBContainer parent, string name) : base (parent.GetParent(), name) {
			this.parentClass = null;
			Namespace.AddObject(this);

			functionsEnumerable = new ScriptFunctionsEnumerable(scriptFunctions);
			this.documentation = new PowerObjectDoc(this);
		}

		public PowerObject(IPBContainer parent, string name, PowerObject super_class) : base (parent.GetParent(), name) {
			this.parentClass = null;
			if (super_class == null) {
				// TODO: super_class musi byt PowerObject
			} else {
				this.superClass = super_class;
			}
			super_class.AddInheritedClass(this);
			Namespace.AddObject(this);

			functionsEnumerable = new ScriptFunctionsEnumerable(scriptFunctions);

			this.documentation = new PowerObjectDoc(this);
		}

		public PowerObject(IPBContainer parent, string name, PowerObject super_class, PowerObject parent_class) : base (parent.GetParent(), name) {
			this.parentClass = parent_class;
			this.superClass = super_class;

			if (parent_class != null) parent_class.AddChildClass(this);
			if (super_class == null) {
				// TODO: super_class musi byt PowerObject
			}
			super_class.AddInheritedClass(this);

			functionsEnumerable = new ScriptFunctionsEnumerable(scriptFunctions);
			this.documentation = new PowerObjectDoc(this);
		}

		// konstruktory pro unresolved objekty, tzn. objekty, ktere znaji pouze jmena svych potomku a predku
		public PowerObject(IPBContainer parent, string name, string super_class) : base(parent.GetParent(), name) {
			unresolvedSuperClass = super_class.ToLower();
			Namespace.AddObject(this);

			functionsEnumerable = new ScriptFunctionsEnumerable(scriptFunctions);
			this.documentation = new PowerObjectDoc(this);
		}

		public PowerObject(IPBContainer parent, string name, string super_class, PowerObject parent_class) : base(parent.GetParent(), name) {
			unresolvedSuperClass = super_class.ToLower();
			parent_class.AddChildClass(this);
			this.parentClass = parent_class;

			functionsEnumerable = new ScriptFunctionsEnumerable(scriptFunctions);
			this.documentation = new PowerObjectDoc(this);
		}

		public override string FullName {
			get {
				string name = this.Name;
				PowerObject obj = this;

				while (obj.ParentClass != null) {
					name = obj.ParentClass.Name + "." + name;
					obj = obj.ParentClass;
				}

				return name;
			}
		}

		public Library ParentLibrary {
			get { return ( Library ) this.Parent; } 
		}

		public Target ParentTarget {
			get { return this.ParentLibrary.ParentTarget; }
		}

		public PowerObject SuperClass {
			get { return this.superClass; }
		}

		public PowerObject ParentClass {
			get { return this.parentClass; }
		}

		public PowerObject TopParentClass {
			get {
				PowerObject obj = this;
				while (obj.ParentClass != null) {
					obj = obj.ParentClass;
				}

				return obj;
			}
		}

		public IEnumerable InheritedObjects {
			get { return inheritedClasses.Values; }
		}

		public PowerObject[] SortedInheritedObjects {
			get { return sortedInheritedClasses; }
		}

		public IEnumerable ChildObjects {
			get { return this.childClasses.Values; }
		}

		public int ChildObjectsCount {
			get { return this.childClasses.Values.Count; }
		}

		public IEnumerable Events {
			get { return this.scriptEvents.Values; }
		}

		public ScriptEvent[] SortedEvents {
			get { return sortedEvents; }
		}

		public int EventsCount {
			get { return this.scriptEvents.Count; }
		}

		public IEnumerable Functions {
			get { return this.functionsEnumerable; }
		}

		public ScriptFunction[] SortedFunctions {
			get { return sortedFunctions; }
		}

		public int FunctionsCount {
			get { return this.scriptFunctions.Count; }
		}

		public bool IsNested {
			get { return (parentClass != null); }
		}

		public bool IsUnresolved {
			get { return (superClass == null && unresolvedSuperClass != null); }
		}

		public bool IsAutoInstantiate {
			get { return autoInstantiate; }
		}

		public virtual bool IsHardwired {
			get { 
				if (this.Name != "powerobject")
					return false;
				else
					return true;
			}
		}

		public new PowerObjectDoc Documentation {
			get { return ( PowerObjectDoc ) documentation; }
		}

		public void SortInheritedClasses() {
			if (inheritedClasses.Values.Count == 0) return;

			sortedInheritedClasses = new PowerObject[inheritedClasses.Values.Count];
			int i = 0;
			foreach (PowerObject obj in inheritedClasses.Values)
				sortedInheritedClasses[i++] = ( PowerObject ) obj;
			Array.Sort(sortedInheritedClasses);
		}

		public void SetAutoInstantiate(bool auto) {
			this.autoInstantiate = auto;
		}

		public int GetEventsCount(bool count_super) {
			if (count_super) {
				int count = this.EventsCount;
				PowerObject super = this.superClass;
				while (super != null) {
					count += super.EventsCount;
					super = super.superClass;
				}

				return count;
			} else
				return this.EventsCount;
		}

		public ScriptEvent GetEvent(string name) {
			ScriptEvent ev = this.scriptEvents[name.ToLower()] as ScriptEvent;

			if (ev == null) {
				if (this.superClass != null)
					return this.superClass.GetEvent(name);
			}

			return ev;
		}

		public ScriptFunction GetFunction(string name) {
			ArrayList fun_list = this.scriptFunctions[name.ToLower()] as ArrayList;

			if (fun_list == null) return null;

			return fun_list[0] as ScriptFunction;
		}

		public int GetFunctionsCount(bool count_super) {
			if (count_super) {
				int count = this.FunctionsCount;
				PowerObject super = this.superClass;
				while (super != null) {
					count += super.FunctionsCount;
					super = super.superClass;
				}

				return count;
			} else
				return this.FunctionsCount;
		}

		public int GetFunctionsCount(bool count_super, PowerObject.Access access) {
			int count = 0;
			PowerObject obj = this;
			while (obj != null) {
				foreach (ScriptFunction fun in obj.Functions) {
					if (fun.Access == access)
						count++;
				}
				if (count_super)
					obj = obj.superClass;
				else
					obj = null;
				if (obj != null)
					count += obj.GetFunctionsCount(count_super, access);
			}

			return count;
		}

		public void AddChildClass(PowerObject child) {
			child.parentClass = this;
			childClasses.Add(child.Name, child);
		}

		public void AddInheritedClass(PowerObject inherited) {
			inheritedClasses.Add(inherited.FullName, inherited);
		}

		public void AddEvent(ScriptEvent ev) {
			scriptEvents.Add(ev.Name, ev);
		}

		public void SortEvents() {
			if (scriptEvents.Values.Count == 0) return;

			sortedEvents = new ScriptEvent[scriptEvents.Values.Count];
			scriptEvents.Values.CopyTo(sortedEvents, 0);
			Array.Sort(sortedEvents);
		}

		public void AddFunction(ScriptFunction fun) {
			ArrayList list;

			if (scriptFunctions.Contains(fun.Name)) {
				list = ( ArrayList ) scriptFunctions[fun.Name];
				list.Add(fun);
			} else {
				list = new ArrayList();
				list.Add(fun);
				scriptFunctions.Add(fun.Name, list);
			}
		}

		public void SortFunctions() {
			if (scriptFunctions.Values.Count == 0) return;

			sortedFunctions = new ScriptFunction[scriptFunctions.Values.Count];
			int i = 0;
			foreach (ArrayList fun_list in scriptFunctions.Values)
				sortedFunctions[i++] = ( ScriptFunction ) fun_list[0];
			Array.Sort(sortedFunctions);
		}

		public PBRoot GetParent() {
			return this.parent;
		}

		public PowerObject GetLocalObject(string name) {
			PowerObject top = this.TopParentClass;

			return top.GetChildClass(name);
		}

		public PowerObject GetChildClass(string name) {
			// vychazi se z predpokladu, ze jmena vnorenych trid musi byt unikatni
			PowerObject child = childClasses[name] as PowerObject;

			if (child == null) {
				foreach (PowerObject obj in this.ChildObjects) {
					child = obj.GetChildClass(name);
					if (child != null)
						return child;
				}
			} else {
				return child;
			}

			if (this.SuperClass != null)
				return this.SuperClass.GetChildClass(name);

			return null;
		}

		public ArrayList GetAllSuperClasses() {
			ArrayList super_list = null;
			PowerObject super = this.superClass;
			while (super != null) {
				if (super_list == null) super_list = new ArrayList();
				super_list.Add(super);
				super = super.superClass;
			}

			return super_list;
		}

		public int GetFunctionOverloadCount(string name) {
			PowerObject obj = this;
			int count = 0;

			while (obj != null) {
				if (obj.scriptFunctions.ContainsKey(name)) {
					ArrayList list = obj.scriptFunctions[name] as ArrayList;
					count += list.Count;
				}
				obj = obj.superClass;
			}

			return count;
		}

		public override void Compile() {
		}

		public void Resolve() {
			if (!this.IsUnresolved) return;

			this.superClass = Namespace.GetObject(this.unresolvedSuperClass);
			if (this.superClass == null) {
				// TODO: jmeno predka bude v kompozitni forme (tzn. predek.vnorena trida)
				this.superClass = null;
				int pos;

				pos = this.unresolvedSuperClass.IndexOf('.');
				if (pos > 0) {
					string unresolved = this.unresolvedSuperClass.Substring(pos + 1, unresolvedSuperClass.Length - pos - 1);
					this.parentClass.superClass.Resolve(); // zde se muze stat, ze this.parentClass.superClass jeste neni resolved
					this.superClass = this.parentClass.superClass.GetChildClass(unresolved);
				}
			}

			if (this.superClass == null) {
				Trace.WriteLine("Predek " + this.unresolvedSuperClass + " objektu " + this.FullName + " nebyl nalezen. Bude formalne nahrazen tridou PowerObject.");
				this.superClass = PowerObject.PowerObjectInstance;
			}

			this.superClass.AddInheritedClass(this);

			ResolveChildObjects();
			ResolveFunctions();
			ResolveEvents();
		}

		protected void ResolveChildObjects() {
			foreach (PowerObject obj in this.ChildObjects) {
				obj.Resolve();
			}
		}

		protected void ResolveFunctions() {
			foreach (ScriptFunction fun in this.Functions) {
				fun.Resolve();
			}
		}

		protected void ResolveEvents() {
			foreach (ScriptEvent ev in this.Events) {
				ev.Resolve();
			}
		}
	}
}
