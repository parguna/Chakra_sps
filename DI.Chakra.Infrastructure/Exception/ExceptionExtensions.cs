using System.Text;

namespace DI.Chakra.Infrastructure
{
    public static class ExceptionExtensions
    {
        public static string GetCompleteException(this System.Exception ex)
        {
            var innerException = ex;
            StringBuilder sb = new StringBuilder();
            do
            {
                if (innerException != null)
                {
                    sb.AppendLine($"Message: {innerException.Message}");

                    if (!string.IsNullOrWhiteSpace(innerException.StackTrace))
                        sb.AppendLine($"StackTrace: {innerException.StackTrace}");

                    if (!string.IsNullOrWhiteSpace(innerException.Source))
                        sb.AppendLine($"Source: {innerException.Source}");
                }
                innerException = innerException?.InnerException;
            }
            while (innerException != null);

            return sb.ToString();
        }
    }
}
