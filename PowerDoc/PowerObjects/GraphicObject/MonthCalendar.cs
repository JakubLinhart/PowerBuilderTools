// This code was autogenerated.
// Changes to this file will be lost if the file is regenerated.
using System;
namespace PowerDoc.PowerObjects {
	public class MonthCalendar : HardwiredObject {
		static MonthCalendar instance;

		static MonthCalendar() {
			instance = new MonthCalendar();
		}

		static public void Init() {
		}

		public static MonthCalendar Instance {
			get { return instance; }
		}

		protected MonthCalendar() : base(PowerObject.PowerObjectParent, "MonthCalendar", DragObject.Instance) {
		}
	}
}