using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace ResourcesConvert
{
	public static class FileManager
	{
		#region Const Fields
		public const string URL = "ftp://ftp.deaddodogames.com/";
        #endregion
        #region Default Folder Locations
        public static string AppDataPath {  get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); } }
        #endregion
        #region Retrieve Local Files
        public static XmlDocument RetriveLocalIsolatedXml(string filepath)
		{
			XmlDocument xmlDocument = new XmlDocument ();
			string xml = RetrieveLocalIsolatedFile (filepath);
			if (xml != null)
			{
				try
				{
					xmlDocument.LoadXml(xml);
				}
				catch (XmlException e)
				{
				}
			}
			return xmlDocument;
		}
		public static string RetrieveLocalIsolatedFile(string filepath)
		{
			using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly()) 
			{
				if (storage.FileExists(filepath))
				{
					using (IsolatedStorageFileStream fileStream = storage.OpenFile (filepath, FileMode.Open, FileAccess.Read)) 
					{
						using (StreamReader streamReader = new StreamReader (fileStream))
						{
							StringBuilder result = new StringBuilder ();
							return streamReader.ReadToEnd ();
							while (streamReader.Read () >= 0) 
							{
								result.Append ((char)streamReader.Read ());
							}
							return result.ToString();
						}
					}
				}
			}
			return null;
		}
        public static XmlDocument RetrieveLocalXml(string filepath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            string xml = RetrieveLocalFile(filepath);
            if (xml != null)
            {
                try
                {
                    xmlDocument.LoadXml(xml);
                }
                catch (XmlException e)
                {
                }
            }
            return xmlDocument;
        }
        public static string RetrieveLocalFile(string filepath)
        {
            if (FileExists(filepath))
            {
                using (FileStream fileStream = new FileStream(filepath, FileMode.Open))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            return null;
        }
        #endregion
        #region Write Local Files
        public static void WriteLocalIsolatedXml(string filepath, XmlDocument xmlDocument)
		{
			WriteLocalIsolatedFile (filepath, xmlDocument.InnerXml);
		}
		public static void WriteLocalIsolatedFile(string filepath, string data)
		{
            //Helper.Windows.App.AsyncOperations++;
			using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly()) 
			{
				using (IsolatedStorageFileStream fileStream = storage.OpenFile (filepath, FileMode.Create, FileAccess.Write)) 
				{
					using (StreamWriter streamWriter = new StreamWriter (fileStream))
					{
						streamWriter.Write (data);
					}
				}
			}
		}
        public static void WriteLocalXml(string filepath, XmlDocument xmlDocument)
        {
            WriteLocalFile(filepath, xmlDocument.InnerXml);
        }
        public static void WriteLocalFile(string filepath, string data)
        {
            using (FileStream fileStream = new FileStream(filepath, FileMode.Create))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(data);
                }
            }
        }
		#endregion
        #region Delete Local Files
        public static void DeleteLocalFile(string filepath)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
        }
        public static void DeleteLocalIsolatedFile(string filepath)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (storage.FileExists(filepath))
                {
                    storage.DeleteFile(filepath);
                }
            }
        }
        #endregion
        #region Write Cloud Files
        public static void WriteRemoteFileFtp(string filepath, string url)
		{
			FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(url);
			request.Method = WebRequestMethods.Ftp.UploadFile;
			request.Credentials = new NetworkCredential ("edcombs_personal", "J@gu@rAss@s1n");
			request.UsePassive = true;
			request.UseBinary = true;
			request.KeepAlive = false;

			using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
			{
				if (storage.FileExists(filepath))
				{
					byte[] fileContents = Encoding.UTF8.GetBytes(RetrieveLocalIsolatedFile(filepath));//streamReader.ReadToEnd());
					request.ContentLength = fileContents.Length;

					Stream stream = request.GetRequestStream();
					stream.Write(fileContents, 0, fileContents.Length);
					stream.Close();
				}
			}
            //Helper.Windows.App.AsyncOperations--;
		}
		#endregion
		#region Read Cloud Files
		public static async System.Threading.Tasks.Task ReadRemoteFileFtp(string filepath, string url)
		{
			FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(url);
			request.Credentials = new NetworkCredential ("edcombs_personal", "J@gu@rAss@s1n");
			request.UsePassive = true;
			request.UseBinary = true;
			request.KeepAlive = true;
			request.Method = WebRequestMethods.Ftp.DownloadFile;

			WebResponse response =  request.GetResponse();
			Stream stream = response.GetResponseStream();
			using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly()) 
			{
				using (IsolatedStorageFileStream fileStream = storage.OpenFile (filepath, FileMode.Create, FileAccess.Write)) 
				{
					int bufferSize = 2048;
					byte[] buffer = new byte[bufferSize];

					int count = stream.Read(buffer, 0, bufferSize);
					while (count > 0)
					{
						fileStream.Write(buffer, 0, count);
						count = stream.Read(buffer, 0, bufferSize);
					}
				}
			}
		}
        public delegate void Complete(bool success);
        public delegate void UpdateProgress(long recieved, long total);
        public static async System.Threading.Tasks.Task ReadRemoteFileHttp(string filepath, string url, Complete callBack)
        {
            bool success = false;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.KeepAlive = true;
            request.Method = WebRequestMethods.Http.Get;

            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                using (IsolatedStorageFileStream fileStream = storage.OpenFile(filepath, FileMode.Create, FileAccess.Write))
                {
                    int bufferSize = 2048;
                    byte[] buffer = new byte[bufferSize];

                    int count = stream.Read(buffer, 0, bufferSize);
                    while (count > 0)
                    {
                        fileStream.Write(buffer, 0, count);
                        count = stream.Read(buffer, 0, bufferSize);
                        success = true;
                    }
                }
            }
            callBack(success);
        }
        public static void ReadRemoteMediaFileHttp(string filepath, string url, Complete callBack)
        {
            bool success = false;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.KeepAlive = true;
            request.Method = WebRequestMethods.Http.Get;

            WebResponse response = request.GetResponse();
            using (Stream output = File.OpenWrite(filepath))
            {
                using (Stream input = response.GetResponseStream())
                {
                    input.CopyTo(output);
                    success = true;
                }
            }
            callBack(success);
        }
        public static void ReadRemoteMediaFileHttpWithProgress(string filepath, string url, UpdateProgress progress, Complete complete)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFileCompleted += (sender, e) =>
                {
                    complete(!e.Cancelled);
                };
                webClient.DownloadProgressChanged += (sender, e) => 
                {
                    progress(e.BytesReceived, e.TotalBytesToReceive);
                };
                Uri uri = new Uri(url);
                try
                {
                    webClient.DownloadFileAsync(uri, filepath);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
        #endregion
        #region File and Directory Check
        public static bool FileExists(string filepath)
        {
            return File.Exists(filepath);
        }
        public static bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
        public static string GetFilePath(string folder, string filename)
        {
            return Path.Combine(folder, filename);
        }
        public static void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
        public static string OpenDirectoryLocator()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                return folderBrowserDialog.SelectedPath;
            }
            return null;
        }
        public static string[] OpenFileLocator(bool multiselect, string defaultExtension)
        {
            return OpenFileLocator(multiselect, defaultExtension, string.Empty);
        }
        public static string[] OpenFileLocator(bool multiselect, string defaultExtension, string filter)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = multiselect;
            openFileDialog.DefaultExt = defaultExtension;
            openFileDialog.Filter = filter;
            Nullable<bool> result = openFileDialog.ShowDialog();
            if (result == true)
            {
                return openFileDialog.FileNames;
            }
            return null;
        }
        public static string SaveFileLocator(string defaultExtension)
        {
            return SaveFileLocator(defaultExtension, string.Empty);
        }
        public static string SaveFileLocator(string defaultExtension, string filter)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.DefaultExt = defaultExtension;
            saveFileDialog.Filter = filter;
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                return saveFileDialog.FileNames[0];
            }
            return null;
        }
        #endregion
    }
}

