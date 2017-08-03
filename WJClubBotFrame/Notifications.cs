using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace WJClubBotFrame {
	public class Notifications {
		public static string botkey { get; } = "112582573:AAH5NwSyZtmclh9tQRf6XgBsY2Qjy2Kn3_8";
		public static string logChatId { get; } = "-1001108267237";


		public static void MakeWebRequest(string apikey, string json) {
			string url = "https://api.telegram.org/bot" + apikey + "/";
			string response;
			WebRequest request = WebRequest.Create(url);
			request.Method = "POST";
			request.Timeout = 10000;
			request.ContentType = "application/json";
			try {
				using (var dataStream = new StreamWriter(request.GetRequestStream())) {
					dataStream.Write(json);
					dataStream.Close();
				}
			} catch (WebException e) {
				Console.WriteLine(e.Message);
			}

			try {
				using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream())) {
					response = reader.ReadToEnd();
					reader.Close();
				}
			} catch (WebException e) {
				Console.WriteLine(e.Message);
			}
		}

		public static string ExecutePostRequest(string apikey, Dictionary<string, string> postData,FileInfo fileToUpload,string fileMimeType,string fileFormKey) {
			string url = "https://api.telegram.org/bot" + apikey + "/";
			WebRequest request = WebRequest.Create(url);
			request.Method = "POST";
			string boundary = CreateFormDataBoundary();
			request.ContentType = "multipart/form-data; boundary=" + boundary;
			Stream requestStream = request.GetRequestStream();
			postData.WriteMultipartFormData(requestStream, boundary);
			if (fileToUpload != null) {
				fileToUpload.WriteMultipartFormData(requestStream, boundary, fileMimeType, fileFormKey);
			}
			byte[] endBytes = System.Text.Encoding.UTF8.GetBytes("--" + boundary + "--");
			requestStream.Write(endBytes, 0, endBytes.Length);
			requestStream.Close();
			using (WebResponse response = request.GetResponse())
			using (StreamReader reader = new StreamReader(response.GetResponseStream())) {
				return reader.ReadToEnd();
			};
		}

		private static string CreateFormDataBoundary() {
			return "---------------------------" + DateTime.Now.Ticks.ToString("x");
		}

		public static void log(String messageText) {
			log(messageText, logChatId, botkey);
		}

		public static void log(string messageText, string chatId) {
			log(messageText, chatId, botkey);
		}

		public static void log(string messageText, string chatId, string botkey) {
			#if DEBUG
			string origin = "DEBUG";
			#else
			string origin = "PROD";
			#endif
			if (messageText.Length <= 2500) {
				messageText = string.Format("<b>[{0}]</b> {1}\n\n<code>{2}</code>", origin, AppDomain.CurrentDomain.FriendlyName, HtmlSpecialChars.Encode(messageText));
				string json = JsonConvert.SerializeObject(new {
					method = "sendMessage",
					chat_id = chatId,
					text = messageText,
					parse_mode = "HTML",
					disable_web_page_preview = true,
					disable_notification = true
				}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
				MakeWebRequest(botkey, json);
			} else {
				using (StreamWriter writer = new StreamWriter("last_error.txt", false)) {
					writer.Write(messageText);
				}
				ExecutePostRequest(botkey, new Dictionary<string, string> {
					{"method","sendDocument"},
					{"chat_id",chatId},
					{"caption",string.Format("[{0}] {1}",origin,AppDomain.CurrentDomain.FriendlyName)}
 				}, new FileInfo("last_error.txt"),"text/plain","document");
				File.Delete("last_error.txt");
			}
		}
	}
}

static class DictionaryExtensions {
	/// <summary>
	/// Template for a multipart/form-data item.
	/// </summary>
	public const string FormDataTemplate = "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n";

	/// <summary>
	/// Writes a dictionary to a stream as a multipart/form-data set.
	/// </summary>
	/// <param name="dictionary">The dictionary of form values to write to the stream.</param>
	/// <param name="stream">The stream to which the form data should be written.</param>
	/// <param name="mimeBoundary">The MIME multipart form boundary string.</param>
	/// <exception cref="System.ArgumentNullException">
	/// Thrown if <paramref name="stream" /> or <paramref name="mimeBoundary" /> is <see langword="null" />.
	/// </exception>
	/// <exception cref="System.ArgumentException">
	/// Thrown if <paramref name="mimeBoundary" /> is empty.
	/// </exception>
	/// <remarks>
	/// If <paramref name="dictionary" /> is <see langword="null" /> or empty,
	/// nothing wil be written to the stream.
	/// </remarks>
	public static void WriteMultipartFormData(
	  this Dictionary<string, string> dictionary,
	  Stream stream,
	  string mimeBoundary) {
		if (dictionary == null || dictionary.Count == 0) {
			return;
		}
		if (stream == null) {
			throw new ArgumentNullException("stream");
		}
		if (mimeBoundary == null) {
			throw new ArgumentNullException("mimeBoundary");
		}
		if (mimeBoundary.Length == 0) {
			throw new ArgumentException("MIME boundary may not be empty.", "mimeBoundary");
		}
		foreach (string key in dictionary.Keys) {
			string item = String.Format(FormDataTemplate, mimeBoundary, key, dictionary[key]);
			byte[] itemBytes = System.Text.Encoding.UTF8.GetBytes(item);
			stream.Write(itemBytes, 0, itemBytes.Length);
		}
	}
}

static class FileInfoExtensions {
	/// <summary>
	/// Template for a file item in multipart/form-data format.
	/// </summary>
	public const string HeaderTemplate = "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n";

	/// <summary>
	/// Writes a file to a stream in multipart/form-data format.
	/// </summary>
	/// <param name="file">The file that should be written.</param>
	/// <param name="stream">The stream to which the file should be written.</param>
	/// <param name="mimeBoundary">The MIME multipart form boundary string.</param>
	/// <param name="mimeType">The MIME type of the file.</param>
	/// <param name="formKey">The name of the form parameter corresponding to the file upload.</param>
	/// <exception cref="System.ArgumentNullException">
	/// Thrown if any parameter is <see langword="null" />.
	/// </exception>
	/// <exception cref="System.ArgumentException">
	/// Thrown if <paramref name="mimeBoundary" />, <paramref name="mimeType" />,
	/// or <paramref name="formKey" /> is empty.
	/// </exception>
	/// <exception cref="System.IO.FileNotFoundException">
	/// Thrown if <paramref name="file" /> does not exist.
	/// </exception>
	public static void WriteMultipartFormData(
	  this FileInfo file,
	  Stream stream,
	  string mimeBoundary,
	  string mimeType,
	  string formKey) {
		if (file == null) {
			throw new ArgumentNullException("file");
		}
		if (!file.Exists) {
			throw new FileNotFoundException("Unable to find file to write to stream.", file.FullName);
		}
		if (stream == null) {
			throw new ArgumentNullException("stream");
		}
		if (mimeBoundary == null) {
			throw new ArgumentNullException("mimeBoundary");
		}
		if (mimeBoundary.Length == 0) {
			throw new ArgumentException("MIME boundary may not be empty.", "mimeBoundary");
		}
		if (mimeType == null) {
			throw new ArgumentNullException("mimeType");
		}
		if (mimeType.Length == 0) {
			throw new ArgumentException("MIME type may not be empty.", "mimeType");
		}
		if (formKey == null) {
			throw new ArgumentNullException("formKey");
		}
		if (formKey.Length == 0) {
			throw new ArgumentException("Form key may not be empty.", "formKey");
		}
		string header = String.Format(HeaderTemplate, mimeBoundary, formKey, file.Name, mimeType);
		byte[] headerbytes = Encoding.UTF8.GetBytes(header);
		stream.Write(headerbytes, 0, headerbytes.Length);
		using (FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read)) {
			byte[] buffer = new byte[1024];
			int bytesRead = 0;
			while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0) {
				stream.Write(buffer, 0, bytesRead);
			}
			fileStream.Close();
		}
		byte[] newlineBytes = Encoding.UTF8.GetBytes("\r\n");
		stream.Write(newlineBytes, 0, newlineBytes.Length);
	}
}