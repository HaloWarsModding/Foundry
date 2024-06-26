﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using KSoft;
using KSoft.Collections;

namespace PhxGui
{
	public enum MiscFlags
	{
		[Display(	Name="Don't overwrite existing files",
					Description="Files that already exist will not be overwritten (only supported for EXPAND right now!)")]
		DontOverwriteExistingFiles,
		[Display(	Name="Decompress Scaleform files",
					Description="During ERA expansion, Scaleform files will be decompressed to a matching .BIN file")]
		DecompressUIFiles,
		[Display(	Name="Transform GFX files",
					Description="During ERA expansion, .GFX files will be transformed to matching .SWF file")]
		TransformGfxFiles,
		[Display(	Name="Ignore non-data files",
					Description="During ERA expansion, only text and .XMB files will be extracted")]
		IgnoreNonDataFiles,

		[Browsable(false)] // no longer letting the user toggle this, they can just use the tool to convert the desired XMBs
		[Display(	Name="Don't automatically translate XMB to XML",
					Description="When expanding, XMB files encountered will not be automatically translated into XML")]
		DontTranslateXmbFiles,
		[Browsable(false)]
		[Display(	Name="Don't automatically remove XMB or XML files",
					Description="When expanding, don't ignore files when both their XMB or XML exists in the ERA")]
		DontRemoveXmlOrXmbFiles,

		[Display(	Name="Always build with XML instead of XMB",
					Description="During ERA generation, if an XMB is referenced but an XML version exists, the XML file will be picked instead")]
		AlwaysUseXmlOverXmb,

		[Display(	Name="Verbose Output",
					Description="When performing operations, include any verbose details")]
		UseVerboseOutput,

		[Display(	Name="Skip Verification",
					Description= "During ERA or ECF expansion, ignore checksums that appear to be wrong and would halt progress")]
		SkipVerification,

		// #HACK ECFs
		[Display(	Name="Assume Drag n Drop Files are ECF",
					Description= "ALL files that you drag and drop into this app we will try to treat as an ECF-based file")]
		AssumeDragAndDropFilesAreEcf,

		kNumberOf,
	};

	internal partial class MainWindowViewModel
		: KSoft.ObjectModel.BasicViewModel
	{
		#region Flags
		private static KSoft.WPF.BitVectorUserInterfaceData gFlagsUserInterfaceSource;
		public static KSoft.WPF.BitVectorUserInterfaceData FlagsUserInterfaceSource { get {
			if (gFlagsUserInterfaceSource == null)
				gFlagsUserInterfaceSource = KSoft.WPF.BitVectorUserInterfaceData.ForEnum(typeof(MiscFlags));
			return gFlagsUserInterfaceSource;
		} }

		KSoft.Collections.BitVector32 mFlags;
		public KSoft.Collections.BitVector32 Flags
		{
			get { return mFlags; }
			set { this.SetFieldVal(ref mFlags, value); }
		}
		#endregion

		#region StatusText
		string mStatusText;
		public string StatusText
		{
			get { return mStatusText; }
			set { this.SetFieldObj(ref mStatusText, value); }
		}
		#endregion

		#region ProcessFilesHelpText
		string mProcessFilesHelpText;
		public string ProcessFilesHelpText
		{
			get { return mProcessFilesHelpText; }
			set { this.SetFieldObj(ref mProcessFilesHelpText, value); }
		}
		#endregion

		#region MessagesText
		string mMessagesText;
		public string MessagesText
		{
			get { return mMessagesText; }
			set { this.SetFieldObj(ref mMessagesText, value); }
		}
		#endregion

		#region IsProcessing
		bool mIsProcessing;
		public bool IsProcessing
		{
			get { return mIsProcessing; }
			set { this.SetFieldVal(ref mIsProcessing, value); }
		}
		#endregion

		public ICommand DataLoadTest { get; private set; }

		public MainWindowViewModel()
		{
			mFlags.Set(MiscFlags.DontTranslateXmbFiles);
			mFlags.Set(MiscFlags.DontRemoveXmlOrXmbFiles);
			mFlags.Set(MiscFlags.UseVerboseOutput);

			ClearStatus();
			ClearProcessFilesHelpText();
			ClearMessages();

			DataLoadTest = new KSoft.WPF.RelayCommand(
				CanExecuteDataLoadTest,
				ExecuteDataLoadTest);
		}

		#region DataLoadTest
		static bool CanExecuteDataLoadTest(object unused)
		{
			var settings = Properties.Settings.Default;

			return System.IO.Directory.Exists(settings.EraExpandOutputPath);
		}

		private void ExecuteDataLoadTest(object unused)
		{
			ClearMessages();
			IsProcessing = true;

			var task = Task.Run((Func<bool>)RunDataLoadTest);

			var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
			task.ContinueWith(t =>
			{
				if (t.IsFaulted || !t.Result)
				{
					bool verbose = Flags.Test(MiscFlags.UseVerboseOutput);

					AggregateException ae = t.IsFaulted ? t.Exception : null;
					string error = "";
					if (ae != null)
					{
						var e = ae.GetOnlyExceptionOrAll();
						error = verbose
							? e.ToVerboseString()
							: e.ToBasicString();
					}
					MessagesText += string.Format("Test data load finished with errors: {0}{1}{2}",
						"See PhxGui.log for any additional details",
						Environment.NewLine,
						error);
				}
				else
				{
					MessagesText = "Test data load success!";
				}

				FinishProcessing();
			}, scheduler);
		}

		static bool RunDataLoadTest()
		{
			var settings = Properties.Settings.Default;
			var path = settings.EraExpandOutputPath;
			var targets_360 = settings.GameVersion == GameVersionType.Xbox360;

			var engine = KSoft.Phoenix.Engine.PhxEngine.CreateForHaloWars(path, path, targets_360);
			if (!engine.Preload())
				return false;

			if (!engine.Load())
				return false;

			return true;
		}
		#endregion

		private void ClearStatus()
		{
			StatusText = "Ready...";
		}

		public void ClearProcessFilesHelpText()
		{
			ProcessFilesHelpText = "Drag-n-drop files";
		}

		private void ClearMessages()
		{
			MessagesText = "";
		}

		public enum AcceptedFileType
		{
			Unaccepted,
			Directory,
			Era,
			EraDef,
			Ecf,
			EcfDef,
			Pkg,
			PkgDef,
			Exe,
			Xex,
			Xml,
			Xmb,
			BinaryDataTree,
			BinaryDataTreeXml,

			kNumberOf
		};
		public struct AcceptedFilesResults
		{
			public BitVector32 AcceptedFileTypes;
			public int FilesCount;
		};
		public static AcceptedFilesResults DetermineAcceptedFiles(string[] files, BitVector32 miscFlags)
		{
			var results = new AcceptedFilesResults();

			if (files == null || files.Length == 0)
				return results;

			results.FilesCount = files.Length;

			foreach (string path in files)
			{
				if (System.IO.Directory.Exists(path))
				{
					results.AcceptedFileTypes.Set(AcceptedFileType.Directory);
					continue;
				}

				string ext = System.IO.Path.GetExtension(path);
				if (string.IsNullOrEmpty(ext)) // extension-less file
				{
					results.AcceptedFileTypes.Set(AcceptedFileType.Unaccepted);
					continue;
				}

				// #HACK ECFs
				// Needs to come before all other file extension checks as it is a forced override
				if (miscFlags.Test(MiscFlags.AssumeDragAndDropFilesAreEcf))
				{
					results.AcceptedFileTypes.Set(AcceptedFileType.Ecf);
					continue;
				}

				switch (ext)
				{
					case KSoft.Phoenix.Resource.EraFileUtil.kExtensionEncrypted:
						results.AcceptedFileTypes.Set(AcceptedFileType.Era);
						break;
					case KSoft.Phoenix.Resource.EraFileBuilder.kNameExtension:
						results.AcceptedFileTypes.Set(AcceptedFileType.EraDef);
						break;
					case KSoft.Phoenix.Resource.ECF.EcfFileDefinition.kFileExtension:
						results.AcceptedFileTypes.Set(AcceptedFileType.EcfDef);
						break;
					case KSoft.Phoenix.Resource.PKG.CaPackageFile.kFileExtension:
						results.AcceptedFileTypes.Set(AcceptedFileType.Pkg);
						break;
					case KSoft.Phoenix.Resource.PKG.CaPackageFileDefinition.kFileExtension:
						results.AcceptedFileTypes.Set(AcceptedFileType.PkgDef);
						break;
					case ".exe":
						results.AcceptedFileTypes.Set(AcceptedFileType.Exe);
						break;
					case ".xex":
						results.AcceptedFileTypes.Set(AcceptedFileType.Exe);
						break;
					case ".xmb":
						results.AcceptedFileTypes.Set(AcceptedFileType.Xmb);
						break;
					case KSoft.Phoenix.Xmb.BinaryDataTree.kBinaryFileExtension:
						results.AcceptedFileTypes.Set(AcceptedFileType.BinaryDataTree);
						break;
					case KSoft.Phoenix.Xmb.BinaryDataTree.kTextFileExtension:
						results.AcceptedFileTypes.Set(AcceptedFileType.BinaryDataTreeXml);
						break;

					default:
						if (KSoft.Phoenix.Resource.ResourceUtils.IsXmlBasedFile(path))
						{
							results.AcceptedFileTypes.Set(AcceptedFileType.Xml);
						}
						break;
				}
			}

			return results;
		}

		public bool AcceptsFiles(string[] files)
		{
			var results = DetermineAcceptedFiles(files, this.Flags);
			if (results.FilesCount == 0)
				return false;

			if (results.AcceptedFileTypes.Cardinality != 0 && !results.AcceptedFileTypes.Test(AcceptedFileType.Unaccepted))
			{
				if (AcceptsFilesInternal(results, files))
					return true;
			}

			ProcessFilesHelpText = "Unacceptable file or group of files";
			return false;
		}
		public void ProcessFiles(string[] files)
		{
			var results = DetermineAcceptedFiles(files, this.Flags);
			if (results.FilesCount == 0)
				return;

			if (results.AcceptedFileTypes.Cardinality != 0 && !results.AcceptedFileTypes.Test(AcceptedFileType.Unaccepted))
			{
				if (ProcessFilesInternal(results, files))
					ProcessFilesHelpText = "";
			}
		}

		private bool AcceptsFilesInternal(AcceptedFilesResults results, string[] files)
		{
			foreach (int bitIndex in results.AcceptedFileTypes.SetBitIndices)
			{
				var type = (AcceptedFileType)bitIndex;
				switch (type)
				{
					case AcceptedFileType.EraDef:
					{
						if (results.FilesCount == 1)
						{
							ProcessFilesHelpText = "Build ERA";
							return true;
						}
						break;
					}

					case AcceptedFileType.EcfDef:
					{
						if (results.FilesCount == 1)
						{
							ProcessFilesHelpText = "Build ECF";
							return true;
						}
						break;
					}

					case AcceptedFileType.PkgDef:
					{
						if (results.FilesCount == 1)
						{
							ProcessFilesHelpText = "Build PKG";
							return true;
						}
						break;
					}

					case AcceptedFileType.Exe:
					case AcceptedFileType.Xex:
					{
						if (results.FilesCount == 1)
						{
							ProcessFilesHelpText = "Try to patch game EXE for modding";
							return true;
						}
						break;
					}

					case AcceptedFileType.Era:
					{
						if (results.AcceptedFileTypes.Cardinality == 1)
						{
							ProcessFilesHelpText = "Expand ERA(s)";
							return true;
						}
						break;
					}

					case AcceptedFileType.Ecf:
					{
						if (results.AcceptedFileTypes.Cardinality == 1)
						{
							ProcessFilesHelpText = "Expand ECF(s)";
							return true;
						}
						break;
					}

					case AcceptedFileType.Pkg:
					{
						if (results.AcceptedFileTypes.Cardinality == 1)
						{
							ProcessFilesHelpText = "Expand PKG(s)";
							return true;
						}
						break;
					}

					case AcceptedFileType.Xmb:
					{
						if (results.AcceptedFileTypes.Cardinality == 1)
						{
							ProcessFilesHelpText = "XMB->XML";
							return true;
						}
						break;
					}

					case AcceptedFileType.Directory:
					{
						if (results.AcceptedFileTypes.Cardinality == 1)
						{
							ProcessFilesHelpText = "XMB->XML (in directories)";
							return true;
						}
						break;
					}

					case AcceptedFileType.BinaryDataTree:
					{
						if (results.AcceptedFileTypes.Cardinality == 1)
						{
							ProcessFilesHelpText = "BinaryDataTree BIN->XML";
							return true;
						}
						break;
					}
					case AcceptedFileType.BinaryDataTreeXml:
					{
						if (results.AcceptedFileTypes.Cardinality == 1)
						{
							ProcessFilesHelpText = "BinaryDataTree XML->BIN";
							return true;
						}
						break;
					}
				}
			}

			return false;
		}
		private bool ProcessFilesInternal(AcceptedFilesResults results, string[] files)
		{
			foreach (int bitIndex in results.AcceptedFileTypes.SetBitIndices)
			{
				var type = (AcceptedFileType)bitIndex;
				switch (type)
				{
					case AcceptedFileType.EraDef:
					{
						if (results.FilesCount == 1)
						{
							ProcessEraListing(files[0]);
							return true;
						}
						break;
					}

					case AcceptedFileType.EcfDef:
					{
						if (results.FilesCount == 1)
						{
							ProcessEcfListing(files[0]);
							return true;
						}
						break;
					}

					case AcceptedFileType.PkgDef:
					{
						if (results.FilesCount == 1)
						{
							// TODO
							//ProcessPkgListing(files[0]);
							return true;
						}
						break;
					}

					case AcceptedFileType.Exe:
					case AcceptedFileType.Xex:
					{
						if (results.FilesCount == 1)
						{
							PatchGameExe(files[0], type);
							return true;
						}
						break;
					}

					case AcceptedFileType.Era:
					{
						if (results.AcceptedFileTypes.Cardinality == 1)
						{
							ProcessEraFiles(files);
							return true;
						}
						break;
					}

					case AcceptedFileType.Ecf:
					{
						if (results.AcceptedFileTypes.Cardinality == 1)
						{
							ProcessEcfFiles(files);
							return true;
						}
						break;
					}

					case AcceptedFileType.Pkg:
					{
						if (results.AcceptedFileTypes.Cardinality == 1)
						{
							// TODO
							//ProcessPkgFiles(files);
							return true;
						}
						break;
					}

					case AcceptedFileType.Xmb:
					{
						if (results.AcceptedFileTypes.Cardinality == 1)
						{
							XmbToXml(files);
							return true;
						}
						break;
					}

					case AcceptedFileType.Directory:
					{
						if (results.AcceptedFileTypes.Cardinality == 1)
						{
							XmbToXmlInDirectories(files);
							return true;
						}
						break;
					}

					case AcceptedFileType.BinaryDataTree:
					{
						if (results.AcceptedFileTypes.Cardinality == 1)
						{
							BinaryDataTreeBinToXml(files);
							return true;
						}
						break;
					}
					case AcceptedFileType.BinaryDataTreeXml:
					{
						if (results.AcceptedFileTypes.Cardinality == 1)
						{
							// #TODO
							throw new NotImplementedException(type.ToString());
							//return true;
						}
						break;
					}
				}
			}

			return false;
		}

		private void FinishProcessing()
		{
			ClearStatus();
			ClearProcessFilesHelpText();
			IsProcessing = false;
		}
	};
}