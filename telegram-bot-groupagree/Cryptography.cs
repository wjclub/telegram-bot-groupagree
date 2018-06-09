using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
namespace telegrambotgroupagree {
	public static class Cryptography {
		#region Settings 
		private static int _iterations = 2;
		private static int _keySize = 256;
		private static string _hash = "SHA1";
		private static string _salt = "sdfgh67876asdkjh"; // Random
		private static string _vector = "tkx72öl8d9098s56"; // Random
		private static string _stuffing = "lkr3lokjaf98dsflkj";
		#endregion

		public static string Encrypt(string value, string password) {
			return Encrypt<AesManaged>(value, password);
		}

		public static string Encrypt<T>(string value, string password) where T : SymmetricAlgorithm, new() {
			byte[] vectorBytes = Encoding.ASCII.GetBytes(_vector);
			byte[] saltBytes = Encoding.ASCII.GetBytes(_salt);
			byte[] valueBytes = Encoding.UTF8.GetBytes(value);
			byte[] encrypted;
			using (T cipher = new T()) {
				PasswordDeriveBytes _passwordBytes = new PasswordDeriveBytes(_stuffing, saltBytes, _hash, _iterations);
				byte[] keyBytes = _passwordBytes.GetBytes(_keySize / 8);
				cipher.Mode = CipherMode.CBC;
				using (ICryptoTransform encryptor = cipher.CreateEncryptor(keyBytes, vectorBytes)) {
					using (MemoryStream to = new MemoryStream()) {
						using (CryptoStream writer = new CryptoStream(to, encryptor, CryptoStreamMode.Write)) {
							writer.Write(valueBytes, 0, valueBytes.Length);
							writer.FlushFinalBlock();
							encrypted = to.ToArray();
						}
					}
				}
				cipher.Clear();
			}
			return HttpServerUtility.UrlTokenEncode(encrypted);
		}

		public static string Decrypt(string value, string password) {
			try {
				return DecryptFromPw(value, _stuffing);
			} catch (Exception) {
				return DecryptFromPw(value, password);
			}
		}	

		public static string DecryptFromPw(string value, string password) {
			return DecryptFromPw<AesManaged>(value, password);
		}

		public static string DecryptFromPw<T>(string value, string password) where T : SymmetricAlgorithm, new() {
			byte[] vectorBytes = Encoding.ASCII.GetBytes(_vector);
			byte[] saltBytes = Encoding.ASCII.GetBytes(_salt);
			byte[] valueBytes = null;
			try { //TODO Review this
				valueBytes = Convert.FromBase64String(value);
			} catch(Exception) {
				valueBytes = HttpServerUtility.UrlTokenDecode(value);
			}
			byte[] decrypted;
			int decryptedByteCount = 0;
			using (T cipher = new T()) {
				PasswordDeriveBytes _passwordBytes = new PasswordDeriveBytes(password, saltBytes, _hash, _iterations);
				byte[] keyBytes = _passwordBytes.GetBytes(_keySize / 8);
				cipher.Mode = CipherMode.CBC;
				try {
					using (ICryptoTransform decryptor = cipher.CreateDecryptor(keyBytes, vectorBytes)) {
						using (MemoryStream from = new MemoryStream(valueBytes)) {
							using (CryptoStream reader = new CryptoStream(from, decryptor, CryptoStreamMode.Read)) {
								decrypted = new byte[valueBytes.Length];
								decryptedByteCount = reader.Read(decrypted, 0, decrypted.Length);
							}
						}
					}
				} catch (Exception) {
					try {
						valueBytes = HttpServerUtility.UrlTokenDecode(value);
						using (ICryptoTransform decryptor = cipher.CreateDecryptor(keyBytes, vectorBytes)) {
							using (MemoryStream from = new MemoryStream(valueBytes)) {
								using (CryptoStream reader = new CryptoStream(from, decryptor, CryptoStreamMode.Read)) {
									decrypted = new byte[valueBytes.Length];
									decryptedByteCount = reader.Read(decrypted, 0, decrypted.Length);
								}
							}
						}
					} catch (Exception) {
						return String.Empty;
					}
				}
				cipher.Clear();
			}
			return Encoding.UTF8.GetString(decrypted, 0, decryptedByteCount);
		}
	}
}