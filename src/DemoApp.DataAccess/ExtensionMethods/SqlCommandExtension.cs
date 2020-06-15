using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace DemoApp.DataAccess.ExtensionMethods
{
    public static class SqlCommandExtension
    {
        public static string ParameterValueForSQL(this SqlParameter sp)
        {
            if (sp == null)
            {
                throw new ArgumentNullException(nameof(sp));
            }

            if (sp.Value == DBNull.Value)
                return string.Empty;

            string retval = "";

            switch (sp.SqlDbType)
            {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.Time:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                    retval = $"'{sp.Value?.ToString().Replace("'", "''")}'";
                    break;

                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.DateTimeOffset:
                    if (sp.Value is DateTime || sp.Value is DateTime?)
                    {
                        DateTime dateTime = Convert.ToDateTime(sp.Value);
                        retval = $"'{dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
                    }
                    else
                    {
                        retval = $"'{sp.Value?.ToString().Replace("'", "''")}'";
                    }

                    break;

                case SqlDbType.Bit:
                    retval = (sp.Value.ToBooleanOrDefault(false)) ? "1" : "0";
                    break;

                default:
                    retval = sp.Value?.ToString().Replace("'", "''");
                    break;
            }

            return retval;
        }

        public static string CommandAsSql(this DbCommand sc)
        {
            StringBuilder sql = new StringBuilder();
            bool FirstParam = true;

            sql.AppendLine($"USE [{sc.Connection.Database}];");
            switch (sc.CommandType)
            {
                case CommandType.StoredProcedure:
                    sql.AppendLine("DECLARE @return_value INT;");

                    foreach (SqlParameter sp in sc.Parameters)
                    {
                        if ((sp.Direction == ParameterDirection.InputOutput) || (sp.Direction == ParameterDirection.Output))
                        {

                            sql.Append($"DECLARE {sp.ParameterName} \t {sp.SqlDbType.ToString()} \t= ");

                            sql.AppendLine(((sp.Direction == ParameterDirection.Output) ? "NULL" : sp.ParameterValueForSQL()) + ";");

                        }
                    }

                    sql.AppendLine($"EXEC [{sc.CommandText}]");

                    foreach (SqlParameter sp in sc.Parameters)
                    {
                        if (sp.Direction != ParameterDirection.ReturnValue)
                        {
                            string separater = (FirstParam) ? "\t" : "\t, ";

                            if (sp.Direction == ParameterDirection.Input)
                            {
                                // In Parameters
                                string parameterValue = sp.ParameterValueForSQL();
                                if (!string.IsNullOrEmpty(parameterValue))
                                {
                                    if (FirstParam) FirstParam = false;
                                    sql.AppendLine($"{separater}{sp.ParameterName} = {parameterValue}");
                                }
                            }
                            else
                            {
                                // Out Parameters
                                sql.AppendLine($"{separater}{sp.ParameterName} = {sp.ParameterName} OUTPUT");
                            }
                        }
                    }
                    sql.AppendLine(";");

                    sql.AppendLine("SELECT 'Return Value' = CONVERT(VARCHAR, @return_value);");

                    foreach (SqlParameter sp in sc.Parameters)
                    {
                        if ((sp.Direction == ParameterDirection.InputOutput) || (sp.Direction == ParameterDirection.Output))
                        {
                            sql.AppendLine($"SELECT '{sp.ParameterName}' = CONVERT(VARCHAR, {sp.ParameterName});");
                        }
                    }
                    break;
                case CommandType.Text:
                    sql.AppendLine(sc.CommandText);
                    break;
            }

            return sql.ToString();
        }

        public static bool ToBooleanOrDefault(this object o, bool Default)
        {
            bool ReturnVal = Default;
            try
            {
                if (o != null)
                {
                    switch (o.ToString().ToLower())
                    {
                        case "yes":
                        case "true":
                        case "ok":
                        case "y":
                            ReturnVal = true;
                            break;
                        case "no":
                        case "false":
                        case "n":
                            ReturnVal = false;
                            break;
                        default:
                            ReturnVal = bool.Parse(o.ToString());
                            break;
                    }
                }
            }
            catch
            {
            }
            return ReturnVal;
        }
    }
}
