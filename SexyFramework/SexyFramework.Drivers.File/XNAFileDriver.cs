using System;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Content;
using SexyFramework.Drivers.App;

namespace SexyFramework.Drivers.File
{
	public class XNAFileDriver : IFileDriver
	{
		protected SexyAppBase mApp;

		protected ContentManager mContentManager;

		public ContentManager GetContentManager()
		{
			return mContentManager;
		}

		public override bool InitFileDriver(SexyAppBase theApp)
		{
			mApp = theApp;
			mContentManager = ((WP7AppDriver)mApp.mAppDriver).mContentManager;
			return mContentManager != null;
		}

		public override void InitSaveDataFolder()
		{
		}

		public override string GetSaveDataPath()
		{
			return string.Empty;
		}

		public override string GetCurPath()
		{
			return string.Empty;
		}

		public override string GetLoadDataPath()
		{
			throw new NotImplementedException();
		}

		public override IFile CreateFile(string thePath)
		{
			return new XNAFile(thePath, this);
		}

		public override IFile CreateFileWithBuffer(string thePath, byte theBuffer, uint theBufferSize)
		{
			throw new NotImplementedException();
		}

		public override IFile CreateFileDirect(string thePath)
		{
			throw new NotImplementedException();
		}

		public override long GetFileSize(string thePath)
		{
			throw new NotImplementedException();
		}

		public override long GetFileTime(string thePath)
		{
			throw new NotImplementedException();
		}

		public override bool FileExists(string thePath, ref bool isFolder)
		{
			bool flag = false;
			IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
			if (userStoreForApplication != null && userStoreForApplication.FileExists(thePath))
			{
				isFolder = false;
				flag = true;
			}
			if (!flag && userStoreForApplication != null && userStoreForApplication.DirectoryExists(thePath))
			{
				isFolder = true;
				flag = true;
			}
			return flag;
		}

		public override bool MakeFolders(string theFolder)
		{
			IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
			string[] array = theFolder.Split('\\');
			string text = theFolder;
			if (array.Length > 0)
			{
				text = array[0];
			}
			for (int i = 1; i != array.Length; i++)
			{
				try
				{
					userStoreForApplication.CreateDirectory(text);
				}
				catch
				{
				}
				text += "\\";
				text += array[i];
			}
			try
			{
				userStoreForApplication.CreateDirectory(text);
			}
			catch
			{
			}
			return userStoreForApplication.DirectoryExists(theFolder);
		}

		public override bool DeleteTree(string thePath)
		{
			bool result = false;
			IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
			try
			{
				string[] directoryNames = userStoreForApplication.GetDirectoryNames(thePath + "\\*");
				string[] array = directoryNames;
				foreach (string text in array)
				{
					if (!DeleteTree(thePath + "\\" + text))
					{
						throw new Exception(string.Format("Can't delete sub directory '{0}'", thePath + "\\" + text));
					}
				}
				string[] fileNames = userStoreForApplication.GetFileNames();
				string[] array2 = fileNames;
				foreach (string text2 in array2)
				{
					userStoreForApplication.DeleteFile(thePath + "\\" + text2);
				}
				userStoreForApplication.DeleteDirectory(thePath);
				result = true;
			}
			catch (Exception)
			{
			}
			return result;
		}

		public override bool DeleteFile(string thePath)
		{
			bool result = false;
			try
			{
				IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
				userStoreForApplication.DeleteFile(thePath);
				result = true;
			}
			catch
			{
			}
			return result;
		}

		public override bool MoveFile(string thePathSrc, string thePathDest)
		{
			bool result = false;
			try
			{
				IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
				userStoreForApplication.MoveFile(thePathSrc, thePathDest);
				result = true;
			}
			catch
			{
			}
			return result;
		}

		public override IFileSearch FileSearchStart(string theCriteria, FileSearchInfo outInfo)
		{
			throw new NotImplementedException();
		}

		public override bool FileSearchNext(IFileSearch theSearch, FileSearchInfo theInfo)
		{
			throw new NotImplementedException();
		}

		public override bool FileSearchEnd(IFileSearch theInfo)
		{
			throw new NotImplementedException();
		}
	}
}
