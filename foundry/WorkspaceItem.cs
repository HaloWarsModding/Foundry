﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry
{
	public enum WorkspaceItemChangedType
	{
		FileAdded,
		FileRemoved,
		FileChanged,
	}
	public class WorkspaceItemChangedArgs
	{
		public WorkspaceItem Item { get; set; }
		public WorkspaceItemChangedType Type { get; set; }
	}

	/// <summary>
	/// This class is just a decorated string -- it only holds data in the FullPath property.
	/// Everything else is just getters to extract information from the full path.
	/// </summary>
	public class WorkspaceItem
	{
		public WorkspaceItem(string path)
		{
			FullPath = Path.GetFullPath(path);
			FullPath = FullPath.Replace('\\', '/');
			if (IsDirectory)
			{
				if (!FullPath.EndsWith("/"))
				{
					FullPath += "/";
				}
			}
		}

		public string FullPath { get; private set; }
		public bool IsValid
		{
			get
			{
				if (File.Exists(FullPath) ||
					Directory.Exists(FullPath))
				{
					return true;
				}
				return false;
			}
		}
		public bool IsDirectory
		{
			get
			{
				if (!IsValid) return false;
				return Directory.Exists(FullPath);
			}
		}


		public string Extension
		{
			get
			{
				if (!IsValid) return "";

				FileAttributes attrs = File.GetAttributes(FullPath);
				if (attrs == FileAttributes.Directory)
				{
					return "";
				}
				else
				{
					return Path.GetExtension(FullPath);
				}
			}
		}
		public string Name
		{
			get
			{
				if (!IsValid) return "";

				return new DirectoryInfo(FullPath).Name;
			}
		}
		public string NameNoExt
		{
			get
			{
				if
					(IsDirectory) return Name;
				else
					return Name.Substring(0, Name.LastIndexOf('.'));
			}
		}
		public WorkspaceItem ParentDirectory
		{
			get
			{
				if (!IsValid) return null;

				// C# does it again!
				string fullPathNoTrailingSlash = FullPath.EndsWith("/") ? FullPath.Substring(0, FullPath.Length - 1) : FullPath;

				DirectoryInfo dirInfo = Directory.GetParent(fullPathNoTrailingSlash);
				if (dirInfo == null)
				{
					return null;
				}

				return new WorkspaceItem(dirInfo.FullName);
			}
		}
		public IEnumerable<WorkspaceItem> ChildDirectories
		{
			get
			{
				if (!IsValid || !IsDirectory)
				{
					return new List<WorkspaceItem>();
				}

				string[] dirs = Directory.GetDirectories(FullPath, "*", SearchOption.TopDirectoryOnly);

				List<WorkspaceItem> items = new List<WorkspaceItem>();
				foreach (string file in dirs)
				{
					items.Add(new WorkspaceItem(file));
				}
				return items;
			}
		}
		public IEnumerable<WorkspaceItem> ChildDirectoriesRecursive
        {
			get
            {
				List<WorkspaceItem> items = new List<WorkspaceItem>();

				items.AddRange(ChildDirectories);
				foreach(WorkspaceItem child in ChildDirectories)
                {
					items.AddRange(child.ChildDirectoriesRecursive);
                }

				return items;
            }
        }
		public IEnumerable<WorkspaceItem> ChildFiles
		{
			get
			{
				if (!IsValid || !IsDirectory)
				{
					return new List<WorkspaceItem>();
				}

				string[] files = Directory.GetFiles(FullPath, "*", SearchOption.TopDirectoryOnly);

				List<WorkspaceItem> items = new List<WorkspaceItem>();
				foreach (string file in files)
				{
					items.Add(new WorkspaceItem(file));
				}
				return items;
			}
		}
		public IEnumerable<WorkspaceItem> ChildFilesRecursive
		{
			get
			{
				List<WorkspaceItem> items = new List<WorkspaceItem>();

				items.AddRange(ChildFiles);
				foreach (WorkspaceItem child in ChildDirectories)
				{
					items.AddRange(child.ChildFilesRecursive);
				}

				return items;
			}
		}
		public IEnumerable<WorkspaceItem> ChildItems
		{
			get
			{
				List<WorkspaceItem> items = new List<WorkspaceItem>();
				items.AddRange(ChildDirectories);
				items.AddRange(ChildFiles);
				return items;
			}
		}
		public IEnumerable<WorkspaceItem> ChildItemsRecursive
		{
			get
			{
				List<WorkspaceItem> items = new List<WorkspaceItem>();

				items.AddRange(ChildItems);
				foreach (WorkspaceItem child in ChildItems)
				{
					items.AddRange(child.ChildItemsRecursive);
				}

				return items;
			}
		}

		public bool IsRelativeTo(WorkspaceItem item)
		{
			if (item is null)
            {
				return false;
            }
			if (!item.IsValid)
			{
				return false;
			}

			return FullPath.Contains(item.FullPath);
		}

		public static bool operator ==(WorkspaceItem a, WorkspaceItem b)
		{
			if (a is null)
			{
				return b is null;
			}
			return a.Equals(b);
		}
		public static bool operator !=(WorkspaceItem a, WorkspaceItem b)
		{
			return !(a == b);
		}
		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (obj is not WorkspaceItem) return false;
			return FullPath.Equals((obj as WorkspaceItem).FullPath);
		}
		public override int GetHashCode()
		{
			return FullPath.GetHashCode();
		}
	}

}
