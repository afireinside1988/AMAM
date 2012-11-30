using System;
using System.Data.SqlClient;
using System.Data;

namespace Amam
{
	class Sqlhelper
	{
		public static bool TableExists(SqlConnection connection, string tableName)
		{
			var filter = new String[4];
			filter[2] = tableName;
			filter[3] = "BASE TABLE";
			DataTable dt = connection.GetSchema("Tables", filter);
			return dt.Rows.Count == 1;
		}

	}
}
