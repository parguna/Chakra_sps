using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Helpers
{
    /// <summary>
    /// Helpers for reading files uploaded through Telerik RadAsyncUpload.
    /// </summary>
    public static class UploadHelper
    {
        /// <summary>
        /// Reads the complete content of an uploaded file.
        /// A single Stream.Read call may return fewer bytes than requested (RadAsyncUpload streams the
        /// file from a temp file), which silently stored truncated / zero-padded attachments.
        /// </summary>
        public static byte[] ReadAllBytes(UploadedFile file)
        {
            return ReadAllBytes(file.InputStream, file.ContentLength);
        }

        /// <summary>
        /// Reads a stream to the end, however many Read calls that takes.
        /// </summary>
        public static byte[] ReadAllBytes(Stream input, long expectedLength = 0)
        {
            if (input.CanSeek)
                input.Position = 0;

            using (MemoryStream buffer = new MemoryStream(expectedLength > 0 && expectedLength < int.MaxValue ? (int)expectedLength : 0))
            {
                input.CopyTo(buffer);
                return buffer.ToArray();
            }
        }

        /// <summary>
        /// Document types accepted for attachments (same list as the AsyncUploadAttach controls).
        /// </summary>
        public static readonly string[] DocumentExtensions =
            { ".jpeg", ".jpg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".csv", ".pdf", ".txt", ".zip" };

        /// <summary>
        /// True when the file name has one of the allowed extensions (case-insensitive).
        /// </summary>
        public static bool HasAllowedExtension(string fileName, params string[] allowedExtensions)
        {
            string name = StripPath(fileName);
            int dot = name.LastIndexOf('.');
            string extension = dot >= 0 ? name.Substring(dot) : string.Empty;
            return extension.Length > 1
                && allowedExtensions.Any(a => string.Equals(a, extension, StringComparison.OrdinalIgnoreCase));
        }

        // Keeps only the part after the last '/' or '\'. Done by hand because System.IO.Path
        // throws on characters such as '<', '>' or '|', which a client can put in a file name.
        private static string StripPath(string clientFileName)
        {
            string name = clientFileName ?? string.Empty;
            int slash = name.LastIndexOfAny(new[] { '\\', '/' });
            return slash >= 0 ? name.Substring(slash + 1) : name;
        }

        /// <summary>
        /// Builds a server-side file name from a client-supplied one: drops any path, keeps only
        /// letters, digits, '.', '-' and '_', and adds a unique prefix so uploads can't overwrite
        /// each other or escape the target folder.
        /// </summary>
        public static string SafeFileName(string clientFileName)
        {
            string name = StripPath(clientFileName);
            name = Regex.Replace(name, @"[^A-Za-z0-9._-]", "_").Trim('.');
            if (name.Length > 100)
                name = name.Substring(name.Length - 100);
            if (string.IsNullOrEmpty(name))
                name = "file";
            // Only [A-Za-z0-9._-] remain, so Path.GetFileName can't throw here; it also marks the value
            // as a plain file name for static analysis (CodeQL path-injection sanitizer).
            return Path.GetFileName(Guid.NewGuid().ToString("N") + "_" + name);
        }
    }
}
