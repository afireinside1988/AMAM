using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;

namespace Amam
{
	class sqlhelper
	{
		public static bool TableExists(SqlConnection connection, string tableName)
		{
			String[] filter = new String[4];
			filter[2] = tableName;
			filter[3] = "BASE TABLE";
			DataTable dt = connection.GetSchema("Tables", filter);
			return dt.Rows.Count == 1;
		}
	}
}
