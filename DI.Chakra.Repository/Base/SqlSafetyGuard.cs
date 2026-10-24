using System.Text.RegularExpressions;

namespace DI.Chakra.Repository
{
    /// <summary>
    /// Shared guard for endpoints that let a caller supply a raw, ad-hoc SQL query
    /// (e.g. custom report builders). Restricts execution to single, read-only
    /// SELECT statements so a supplied query cannot be used to run DML/DDL or
    /// chain additional statements.
    /// </summary>
    public static class SqlSafetyGuard
    {
        private static readonly Regex DisallowedKeywords = new Regex(
            @"\b(delete |insert |update |create |with |drop |alter |--|cursor |exec |execute |commit|waitfor |kill )\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static bool IsSelectOnly(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return false;

            string trimmed = query.Trim();
            return trimmed.ToLowerInvariant().StartsWith("select") && !DisallowedKeywords.IsMatch(trimmed);
        }
    }
}
