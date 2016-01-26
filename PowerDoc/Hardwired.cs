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

namespace PowerDoc {
	public class Hardwired : IPBContainer {
		private static Hardwired instance;
		private static Hashtable globalClasses = new Hashtable();

		static Hardwired() {
			instance = new Hardwired();
		}

		public static Hardwired Instance {
			get { return instance; }
		}

		public static void InitObjects() {
			PowerObjects.InitHardwired.Init();
			// zakladni datove typy
/*			PowerObjects.Any.Init();
			PowerObjects.Blob.Init();
			PowerObjects.Boolean.Init();
			PowerObjects.Date.Init();
			PowerObjects.DateTime.Init();
			PowerObjects.Decimal.Init();
			PowerObjects.Double.Init();
			PowerObjects.Integer.Init();
			PowerObjects.Long.Init();
			PowerObjects.Real.Init();
			PowerObjects.String.Init();
			PowerObjects.Time.Init();
			PowerObjects.UnsignedInteger.Init();
			PowerObjects.UnsignedLong.Init();

			// potomci PowerObject
			PowerObjects.Application.Init();
			PowerObjects.function_object.Init();
			PowerObjects.GraphicObject.Init();
			PowerObjects.GrAxis.Init();
			PowerObjects.GrDispAttr.Init();
			PowerObjects.NonVisualObject.Init();
			PowerObjects.PbToCPPObject.Init();
			PowerObjects.Structure.Init();
			PowerObjects.TraceActivityNode.Init();

			// potomci GraphicObject
			PowerObjects.Animation.Init();
			PowerObjects.CommandButton.Init();
			PowerObjects.DataWindow.Init();
			PowerObjects.DatePicker.Init();
			PowerObjects.DragObject.Init();
			PowerObjects.DrawObject.Init();
			PowerObjects.DropDownListbox.Init();
			PowerObjects.DropDownPictureListbox.Init();
			PowerObjects.EditMask.Init();
			PowerObjects.Graph.Init();
			PowerObjects.GroupBox.Init();
			PowerObjects.HProgressBar.Init();
			PowerObjects.HScrollBar.Init();
			PowerObjects.HTrackBar.Init();
			PowerObjects.Checkbox.Init();
			PowerObjects.InkEdit.Init();
			PowerObjects.InkPicture.Init();
			PowerObjects.Line.Init();
			PowerObjects.ListBox.Init();
			PowerObjects.ListView.Init();
			PowerObjects.MdiClient.Init();
			PowerObjects.Menu.Init();
			PowerObjects.MenuCascade.Init();
			PowerObjects.MonthCalendar.Init();
			PowerObjects.MultiLineEdit.Init();
			PowerObjects.OleControl.Init();
			PowerObjects.OleCustomControl.Init();
			PowerObjects.OMControl.Init();
			PowerObjects.OMCustomControl.Init();
			PowerObjects.OMEmbeddedControl.Init();
			PowerObjects.Oval.Init();
			PowerObjects.Picture.Init();
			PowerObjects.PictureButton.Init();
			PowerObjects.PictureHyperlink.Init();
			PowerObjects.PictureListBox.Init();
			PowerObjects.RadioButton.Init();
			PowerObjects.Rectangle.Init();
			PowerObjects.RichTextEdit.Init();
			PowerObjects.RoundRectangle.Init();
			PowerObjects.SingleLineEdit.Init();
			PowerObjects.StaticHyperlink.Init();
			PowerObjects.StaticText.Init();
			PowerObjects.Tab.Init();
			PowerObjects.TreeView.Init();
			PowerObjects.UserObject.Init();
			PowerObjects.VProgressBar.Init();
			PowerObjects.VScrollBar.Init();
			PowerObjects.VTrackBar.Init();
			PowerObjects.Window.Init();
			PowerObjects.WindowObject.Init();

			// potomci NonVisualObject
			PowerObjects.AdoResultSet.Init();
			PowerObjects.Connection.Init();
			PowerObjects.ConnectObject.Init();
			PowerObjects.ContextInformation.Init();
			PowerObjects.ContextKeyword.Init();
			PowerObjects.CPlusPlus.Init();
			PowerObjects.DataStore.Init();
			PowerObjects.DWObject.Init();
			PowerObjects.DynamicDescriptionArea.Init();
			PowerObjects.DynamicStagingArea.Init();
			PowerObjects.Error.Init();
			PowerObjects.ErrorLogging.Init();
			PowerObjects.ExtObject.Init();
			PowerObjects.INet.Init();
			PowerObjects.InternetResult.Init();
			PowerObjects.JaguarOrb.Init();
			PowerObjects.MailSession.Init();
			PowerObjects.Message.Init();
			PowerObjects.OleObject.Init();
			PowerObjects.OleStorage.Init();
			PowerObjects.OleStream.Init();
			PowerObjects.OleTXNoObject.Init();
			PowerObjects.OMObject.Init();
			PowerObjects.OmStorage.Init();
			PowerObjects.OmStream.Init();
			PowerObjects.Orb.Init();
			PowerObjects.Pipeline.Init();
			PowerObjects.RemoteObject.Init();
			PowerObjects.ResultSet.Init();
			PowerObjects.ResultSets.Init();
			PowerObjects.Service.Init();
			PowerObjects.SSLCallback.Init();
			PowerObjects.SSLServiceProvider.Init();
			PowerObjects.Timing.Init();
			PowerObjects.Transaction.Init();
			PowerObjects.TransactionServer.Init();

			// potomci Throwable
			PowerObjects.CorbaBadAdContext.Init();
			PowerObjects.CorbaBadDataConversion.Init();
			PowerObjects.CorbaBadInvOrder.Init();
			PowerObjects.CorbaBadOperation.Init();
			PowerObjects.CorbaBadParam.Init();
			PowerObjects.CorbaBadTypeCode.Init();
			PowerObjects.CorbaCommFailure.Init();
			PowerObjects.CorbaCurrent.Init();
			PowerObjects.CorbaFreeMem.Init();
			PowerObjects.CorbaImpLimit.Init();
			PowerObjects.CorbaInitialize.Init();
			PowerObjects.CorbaInternal.Init();
			PowerObjects.CorbaIntFrePos.Init();
			PowerObjects.CorbaInvalidTransaction.Init();
			PowerObjects.CorbaInvFlag.Init();
			PowerObjects.CorbaInvIdent.Init();
			PowerObjects.CorbaInvObjRef.Init();
			PowerObjects.CorbaMarshal.Init();
			PowerObjects.CorbaNoImplement.Init();
			PowerObjects.CorbaNoMemory.Init();
			PowerObjects.CorbaNoPermission.Init();
			PowerObjects.CorbaNoResources.Init();
			PowerObjects.CorbaNoResponse.Init();
			PowerObjects.CorbaObjAdapter.Init();
			PowerObjects.CorbaObject.Init();
			PowerObjects.CorbaObjectNotExists.Init();
			PowerObjects.CorbaPersistStore.Init();
			PowerObjects.CorbaSystemException.Init();
			PowerObjects.CorbaTransactionRequired.Init();
			PowerObjects.CorbaTransactionRolledBack.Init();
			PowerObjects.CorbaTransient.Init();
			PowerObjects.CorbaUnion.Init();
			PowerObjects.CorbaUnknown.Init();
			PowerObjects.CorbaUserException.Init();
			PowerObjects.DivideByZeroError.Init();
			PowerObjects.DWRuntimeError.Init();
			PowerObjects.Exception.Init();
			PowerObjects.NullObjectError.Init();
			PowerObjects.OleRuntimeError.Init();
			PowerObjects.PBXRuntimeError.Init();
			PowerObjects.RuntimeError.Init();

			// potomci PbToCPPObject
			PowerObjects.ArrayBounds.Init();
			PowerObjects.ClassDefinition.Init();
			PowerObjects.ClassDefinitionObject.Init();
			PowerObjects.EnumerationDefinition.Init();
			PowerObjects.ProfileCall.Init();
			PowerObjects.ProfileClass.Init();
			PowerObjects.ProfileLine.Init();
			PowerObjects.ProfileRoutine.Init();
			PowerObjects.Profiling.Init();
			PowerObjects.ScriptDefinition.Init();
			PowerObjects.SimpleTypeDefinition.Init();
			PowerObjects.TraceFile.Init();
			PowerObjects.TraceTree.Init();
			PowerObjects.TraceTreeError.Init();
			PowerObjects.TraceTreeGarbageCollect.Init();
			PowerObjects.TraceTreeLine.Init();
			PowerObjects.TraceTreeNode.Init();
			PowerObjects.TraceTreeObject.Init();
			PowerObjects.TraceTreeRoutine.Init();
			PowerObjects.TraceTreeSQL.Init();
			PowerObjects.TraceTreeUser.Init();
			PowerObjects.TypeDefinition.Init();
			PowerObjects.VariableCardinalityDefinition.Init();
			PowerObjects.VariableDefinition.Init();
			PowerObjects.EnumerationItemDefinition.Init();

			// potomci PbToCPPObject
			PowerObjects.ArrayBounds.Init();
			PowerObjects.ClassDefinition.Init();
			PowerObjects.ClassDefinitionObject.Init();
			PowerObjects.EnumerationDefinition.Init();
			PowerObjects.EnumerationItemDefinition.Init();
			PowerObjects.ProfileCall.Init();
			PowerObjects.ProfileClass.Init();
			PowerObjects.ProfileLine.Init();
			PowerObjects.ProfileRoutine.Init();
			PowerObjects.Profiling.Init();
			PowerObjects.ScriptDefinition.Init();
			PowerObjects.SimpleTypeDefinition.Init();
			PowerObjects.TraceFile.Init();
			PowerObjects.TraceTree.Init();
			PowerObjects.TraceTreeError.Init();
			PowerObjects.TraceTreeGarbageCollect.Init();
			PowerObjects.TraceTreeLine.Init();
			PowerObjects.TraceTreeNode.Init();
			PowerObjects.TraceTreeObject.Init();
			PowerObjects.TraceTreeRoutine.Init();
			PowerObjects.TraceTreeSQL.Init();
			PowerObjects.TraceTreeUser.Init();
			PowerObjects.TypeDefinition.Init();
			PowerObjects.VariableCardinalityDefinition.Init();
			PowerObjects.VariableDefinition.Init();

			// potomci Structure
			PowerObjects.ConnectionInfo.Init();
			PowerObjects.DataWindowChild.Init();
			PowerObjects.Environment.Init();
			PowerObjects.ListViewItem.Init();
			PowerObjects.MailFileDescription.Init();
			PowerObjects.MailMessage.Init();
			PowerObjects.MailRecipient.Init();
			PowerObjects.TreeViewItem.Init();
 
			// potomci TraceActivityNode
			PowerObjects.TraceBeginEnd.Init();
			PowerObjects.TraceError.Init();
			PowerObjects.TraceGarbageCollect.Init();
			PowerObjects.TraceLine.Init();
			PowerObjects.TraceObject.Init();
			PowerObjects.TraceRoutine.Init();
			PowerObjects.TraceSQL.Init();
			PowerObjects.TraceUser.Init();

			// Enumerated
			PowerObjects.AccessibleRole.Init();
			PowerObjects.Alignment.Init();
			PowerObjects.ArgCallingConvention.Init();
			PowerObjects.ArrangeOpen.Init();
			PowerObjects.ArrangeTypes.Init();
			PowerObjects.Band.Init();
			PowerObjects.Border.Init();
			PowerObjects.BorderStyle.Init();
			PowerObjects.Button.Init();
			PowerObjects.CharSet.Init();
			PowerObjects.ClipboardFormat.Init();
			PowerObjects.ConnectPrivilege.Init();
			PowerObjects.ConvertType.Init();
			PowerObjects.CpuTypes.Init();
			PowerObjects.DateTimeFormat.Init();
			PowerObjects.Direction.Init();
			PowerObjects.DisplaySizeMode.Init();
			PowerObjects.DragModes.Init();
			PowerObjects.DWBuffer.Init();
			PowerObjects.DWConflictResolution.Init();
			PowerObjects.DWItemStatus.Init();
			PowerObjects.Encoding.Init();
			PowerObjects.ErrorReturn.Init();
			PowerObjects.ExceptionAction.Init();
			PowerObjects.FileAccess.Init();
			PowerObjects.FileLock.Init();
			PowerObjects.FileMode.Init();
			PowerObjects.FileType.Init();
			PowerObjects.FillPattern.Init();
			PowerObjects.FontCharSet.Init();
			PowerObjects.FontFamily.Init();
			PowerObjects.FontPitch.Init();
			PowerObjects.GrAxisDataType.Init();
			PowerObjects.GrColorType.Init();
			PowerObjects.GrDataType.Init();
			PowerObjects.GrGraphType.Init();
			PowerObjects.GrLegendType.Init();
			PowerObjects.GrObjectType.Init();
			PowerObjects.GrResetType.Init();
			PowerObjects.GrRoundToType.Init();
			PowerObjects.GrScaleType.Init();
			PowerObjects.GrScaleValue.Init();
			PowerObjects.GrSortType.Init();
			PowerObjects.GrSymbolType.Init();
			PowerObjects.GrTicType.Init();
			PowerObjects.HelpCommand.Init();
			PowerObjects.HTickMarks.Init();
			PowerObjects.Icon.Init();
			PowerObjects.InkCollectionMode.Init();
			PowerObjects.InkCompressionMode.Init();
			PowerObjects.InkEditStatus.Init();
			PowerObjects.InkMode.Init();
			PowerObjects.InkPenTip.Init();
			PowerObjects.InkPersistenceFormat.Init();
			PowerObjects.InkPicEditMode.Init();
			PowerObjects.InkPicStatus.Init();
			PowerObjects.KeyCode.Init();
			PowerObjects.LanguageID.Init();
			PowerObjects.LanguageSortID.Init();
			PowerObjects.LibDirType.Init();
			PowerObjects.LibExportType.Init();
			PowerObjects.LibImportType.Init();
			PowerObjects.LineStyle.Init();
			PowerObjects.ListViewView.Init();
			PowerObjects.Location.Init();
			PowerObjects.MailFileType.Init();
			PowerObjects.MailLogonOption.Init();
			PowerObjects.MailReadOption.Init();
			PowerObjects.MailRecipientType.Init();
			PowerObjects.MailReturnCode.Init();
			PowerObjects.MaskDataType.Init();
			PowerObjects.MenuItemType.Init();
			PowerObjects.MenuMergeOption.Init();
			PowerObjects.MenuStyle.Init();
			PowerObjects.MetaDataType.Init();
			PowerObjects.MonCalDisplayState.Init();
			PowerObjects.MonCalRepeatType.Init();
			PowerObjects.Object.Init();
			PowerObjects.OleFunctioncallType.Init();
			PowerObjects.OmActivateType.Init();
			PowerObjects.OmActivation.Init();
			PowerObjects.OmContentsAllowed.Init();
			PowerObjects.OmDisplayType.Init();
			PowerObjects.OmLinkUpdateOptions.Init();
			PowerObjects.OsTypes.Init();
			PowerObjects.ParagraphSetting.Init();
			PowerObjects.ParmType.Init();
			PowerObjects.PbTypes.Init();
			PowerObjects.PdfMethod.Init();
			PowerObjects.Pointer.Init();
			PowerObjects.ProfilerOutIneKind.Init();
			PowerObjects.RegistryValueType.Init();
			PowerObjects.RowFocusInd.Init();
			PowerObjects.SaveAsType.Init();
			PowerObjects.SaveMetaData.Init();
			PowerObjects.ScriptKind.Init();
			PowerObjects.SeekType.Init();
			PowerObjects.SetPosType.Init();
			PowerObjects.SizeMode.Init();
			PowerObjects.Spacing.Init();
			PowerObjects.SqlPreviewFunction.Init();
			PowerObjects.SqlPreviewType.Init();
			PowerObjects.StgReadMode.Init();
			PowerObjects.StgShareMode.Init();
			PowerObjects.TabPosition.Init();
			PowerObjects.TextCase.Init();
			PowerObjects.TextStyle.Init();
			PowerObjects.TimerKind.Init();
			PowerObjects.ToolBarAlignment.Init();
			PowerObjects.ToolBarStyle.Init();
			PowerObjects.TraceActivity.Init();
			PowerObjects.TraceCategory.Init();
			PowerObjects.TreeNavigation.Init();
			PowerObjects.TrigEvent.Init();
			PowerObjects.TypeCategory.Init();
			PowerObjects.UserObjects.Init();
			PowerObjects.ValSchemeType.Init();
			PowerObjects.VarAccess.Init();
			PowerObjects.VariableCardinalityType.Init();
			PowerObjects.VariableKind.Init();
			PowerObjects.VTextAlign.Init();
			PowerObjects.VTickMarks.Init();
			PowerObjects.WebPagingMethod.Init();
			PowerObjects.WeekDay.Init();
			PowerObjects.WindowState.Init();
			PowerObjects.WindowType.Init();
			PowerObjects.WriteMode.Init();*/
		}

		public static void AddClass(PowerObjects.HardwiredObject obj) {
			globalClasses.Add(obj.Name, obj);
		}

		public static void AddClassAlias(string alias, PowerObjects.HardwiredObject obj) {
			globalClasses.Add(alias.ToLower(), obj);
		}

		protected Hardwired() {
		}

		public PBRoot GetParent() {
			return null;
		}
	}
}