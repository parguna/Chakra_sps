using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DI.Chakra.MockedTests
{
    /// <summary>
    /// Small ADO.NET helper used to create and remove the tests' own rows. Every value is passed as a
    /// SQL parameter; table names only ever come from the system catalog.
    /// </summary>
    public static class TestDb
    {
        public static string ConnectionString
        {
            get
            {
                string name = ConfigurationManager.AppSettings["Connection"];
                return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
        }

        /// <summary>The product's provisioning log lines for a request (for explaining test failures).</summary>
        public static List<string> ProvisioningLog(long requestId)
        {
            var lines = new List<string>();
            using (var cn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("SELECT TOP 60 LogLevel, Message, ExceptionMessage FROM DI_Log_Provisioning WHERE AccessRequestId = @r ORDER BY Id", cn))
            {
                cmd.Parameters.AddWithValue("@r", requestId);
                cn.Open();
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) lines.Add($"[{r[0]}] {r[1]} {r[2]}");
            }
            return lines;
        }

        public static object Scalar(string sql, params (string Name, object Value)[] args)
        {
            using (var cn = new SqlConnection(ConnectionString))
            using (var cmd = Command(cn, sql, args))
            {
                cn.Open();
                object v = cmd.ExecuteScalar();
                return v == DBNull.Value ? null : v;
            }
        }

        public static long ScalarLong(string sql, params (string Name, object Value)[] args) => Convert.ToInt64(Scalar(sql, args) ?? 0L);

        public static int Execute(string sql, params (string Name, object Value)[] args)
        {
            using (var cn = new SqlConnection(ConnectionString))
            using (var cmd = Command(cn, sql, args))
            {
                cn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static DataRow Row(string sql, params (string Name, object Value)[] args)
        {
            using (var cn = new SqlConnection(ConnectionString))
            using (var cmd = Command(cn, sql, args))
            using (var da = new SqlDataAdapter(cmd))
            {
                var t = new DataTable();
                da.Fill(t);
                return t.Rows.Count > 0 ? t.Rows[0] : null;
            }
        }

        private static SqlCommand Command(SqlConnection cn, string sql, (string Name, object Value)[] args)
        {
            var cmd = new SqlCommand(sql, cn) { CommandTimeout = 300 };
            foreach (var (name, value) in args)
                cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
            return cmd;
        }

        /// <summary>
        /// Copies one row (all columns except identity/computed ones) and returns the new row's Id.
        /// Used to start from a real, valid row and then change only what the test needs, so the
        /// test doesn't depend on knowing every NOT NULL column of a 40-column table.
        /// </summary>
        /// <summary>
        /// Clones a row and applies <paramref name="update"/> (a statement using @id) to the copy.
        /// If the update fails the copy is deleted, so a half-finished setup never leaves a stray row.
        /// </summary>
        public static long CloneAndUpdate(string table, long sourceId, string update, params (string Name, object Value)[] args)
        {
            long id = CloneRow(table, sourceId);
            try
            {
                var all = new List<(string, object)>(args) { ("@id", id) };
                Execute(update, all.ToArray());
                return id;
            }
            catch
            {
                Execute("DELETE FROM " + Quote(table) + " WHERE Id = @id", ("@id", id));
                throw;
            }
        }

        // Table names here are compile-time constants in the tests; still, only allow plain identifiers.
        public static string Quote(string table)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(table, "^[A-Za-z0-9_]+$"))
                throw new ArgumentException("Invalid table name", nameof(table));
            return "[" + table + "]";
        }

        public static long CloneRow(string table, long sourceId)
        {
            const string sql = @"
DECLARE @cols nvarchar(max) = (
  SELECT STRING_AGG(CAST(QUOTENAME(c.name) AS nvarchar(max)), ',') WITHIN GROUP (ORDER BY c.column_id)
  FROM sys.columns c
  WHERE c.object_id = OBJECT_ID(@table) AND c.is_identity = 0 AND c.is_computed = 0);
IF @cols IS NULL THROW 50000, 'Unknown table', 1;
DECLARE @sql nvarchar(max) = N'INSERT INTO ' + QUOTENAME(@table) + N' (' + @cols + N') OUTPUT INSERTED.Id SELECT ' + @cols
  + N' FROM ' + QUOTENAME(@table) + N' WHERE Id = @id';
EXEC sp_executesql @sql, N'@id bigint', @id = @id;";
            long newId = ScalarLong(sql, ("@table", table), ("@id", sourceId));
            if (newId == 0) throw new InvalidOperationException($"Template row {table}#{sourceId} not found.");
            return newId;
        }

        /// <summary>
        /// Deletes every row in any table whose column <paramref name="columnNames"/> equals <paramref name="id"/>,
        /// repeating until nothing more can be deleted (so foreign-key order sorts itself out).
        /// Only for ids the tests created themselves (a fresh endpoint or user).
        /// </summary>
        public static int DeleteEverywhere(long id, IEnumerable<string> columnNames, IEnumerable<string> excludeTables)
        {
            const string sql = @"
DECLARE @total int = 0, @pass int = 0, @deleted int = 1, @sql nvarchar(max), @n int;
DECLARE @targets TABLE (tbl sysname, col sysname);
INSERT @targets
SELECT t.name, c.name
FROM sys.columns c JOIN sys.tables t ON t.object_id = c.object_id
WHERE c.name IN (SELECT value FROM STRING_SPLIT(@cols, ','))
  AND t.name NOT IN (SELECT value FROM STRING_SPLIT(@exclude, ','))
  AND TYPE_NAME(c.user_type_id) IN ('bigint','int');
WHILE @deleted > 0 AND @pass < 8
BEGIN
  SET @deleted = 0; SET @pass += 1;
  DECLARE cur CURSOR LOCAL FAST_FORWARD FOR SELECT tbl, col FROM @targets;
  DECLARE @tbl sysname, @col sysname;
  OPEN cur; FETCH NEXT FROM cur INTO @tbl, @col;
  WHILE @@FETCH_STATUS = 0
  BEGIN
    SET @sql = N'DELETE FROM ' + QUOTENAME(@tbl) + N' WHERE ' + QUOTENAME(@col) + N' = @id; SET @n = @@ROWCOUNT;';
    BEGIN TRY
      EXEC sp_executesql @sql, N'@id bigint, @n int OUTPUT', @id = @id, @n = @n OUTPUT;
      SET @deleted += @n;
    END TRY
    BEGIN CATCH
      -- probably a foreign key from a table not cleaned yet; retried on the next pass
    END CATCH
    FETCH NEXT FROM cur INTO @tbl, @col;
  END
  CLOSE cur; DEALLOCATE cur;
  SET @total += @deleted;
END
SELECT @total;";
            return Convert.ToInt32(Scalar(sql, ("@id", id), ("@cols", string.Join(",", columnNames)), ("@exclude", string.Join(",", excludeTables))));
        }
    }
}
