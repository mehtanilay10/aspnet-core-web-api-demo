using DemoApp.DataAccess.Models.SearchTable;
using DemoApp.Models.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoApp.DataAccess.ExtensionMethods
{
    public static class SearchTableExtensionMethods
    {
        public static string GetWhereClause(this SearchTableRequest request, string joinWith = AppConstants.SQL_AND)
        {
            StringBuilder whereClause = new StringBuilder();
            if (joinWith.ToUpper() != AppConstants.SQL_AND && joinWith.ToUpper() != AppConstants.SQL_OR)
                throw new Exception("Invalid JoinWith value for generating where clause.");

            if (request.FilterParas != null)
            {
                foreach (SearchTableFilter filter in request.FilterParas)
                {
                    if (filter.FieldType == FieldType.Text)
                    {
                        switch (filter.Operator)
                        {
                            case FieldOperator.Contains:
                                whereClause.Append($"{filter.FieldName} LIKE '%{filter.Value}%'");
                                break;
                            case FieldOperator.StartsWith:
                                whereClause.Append($"{filter.FieldName} LIKE '{filter.Value}%'");
                                break;
                            case FieldOperator.EndsWith:
                                whereClause.Append($"{filter.FieldName} LIKE '%{filter.Value}'");
                                break;
                            case FieldOperator.Equals:
                                whereClause.Append($"{filter.FieldName} = '{filter.Value}'");
                                break;
                            case FieldOperator.NotEqual:
                                whereClause.Append($"{filter.FieldName} <> '{filter.Value}'");
                                break;
                        }
                    }
                    else if (filter.FieldType == FieldType.Numeric)
                    {
                        switch (filter.Operator)
                        {
                            case FieldOperator.GreaterThan:
                                whereClause.Append($"{filter.FieldName} > {filter.Value}");
                                break;
                            case FieldOperator.GreaterThanOrEqual:
                                whereClause.Append($"{filter.FieldName} >= {filter.Value}");
                                break;
                            case FieldOperator.LessThan:
                                whereClause.Append($"{filter.FieldName} < {filter.Value}");
                                break;
                            case FieldOperator.LessThanOrEqual:
                                whereClause.Append($"{filter.FieldName} <= {filter.Value}");
                                break;
                            case FieldOperator.Equals:
                                whereClause.Append($"{filter.FieldName} = {filter.Value}");
                                break;
                            case FieldOperator.NotEqual:
                                whereClause.Append($"{filter.FieldName} <> {filter.Value}");
                                break;
                        }
                    }
                    else if (filter.FieldType == FieldType.Date)
                    {
                        string dateInSqlFormat = DateTime.ParseExact(filter.Value, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"); ;
                        switch (filter.Operator)
                        {
                            case FieldOperator.GreaterThan:
                                whereClause.Append($"{filter.FieldName} > '{dateInSqlFormat}'");
                                break;
                            case FieldOperator.GreaterThanOrEqual:
                                whereClause.Append($"{filter.FieldName} >= '{dateInSqlFormat}'");
                                break;
                            case FieldOperator.LessThan:
                                whereClause.Append($"{filter.FieldName} < '{dateInSqlFormat}'");
                                break;
                            case FieldOperator.LessThanOrEqual:
                                whereClause.Append($"{filter.FieldName} <= '{dateInSqlFormat}'");
                                break;
                            case FieldOperator.Equals:
                                whereClause.Append($"{filter.FieldName} = '{dateInSqlFormat}'");
                                break;
                            case FieldOperator.NotEqual:
                                whereClause.Append($"{filter.FieldName} <> '{dateInSqlFormat}'");
                                break;
                        }
                    }
                    else if (filter.FieldType == FieldType.MultiSelectText)
                    {
                        switch (filter.Operator)
                        {
                            case FieldOperator.In:
                                whereClause.Append($"{filter.FieldName} IN ('{string.Join("','", filter.Value.Split(','))}')");
                                break;
                            case FieldOperator.NotIn:
                                whereClause.Append($"{filter.FieldName} NOT IN ('{string.Join("','", filter.Value.Split(','))}')");
                                break;
                        }
                    }
                    else if (filter.FieldType == FieldType.MultiSelectLookup)
                    {
                        System.Reflection.Assembly modelAssembly = typeof(AppConstants).Assembly;
                        Type lookupType = modelAssembly.GetType(filter.LookupName);
                        List<int> lookupValues = new List<int>();

                        foreach (string val in filter.Value.Split(','))
                        {
                            lookupValues.Add((int)Enum.Parse(lookupType, val));
                        }

                        switch (filter.Operator)
                        {
                            case FieldOperator.In:
                                whereClause.Append($"{filter.FieldName} IN ({string.Join(",", lookupValues)})");
                                break;
                            case FieldOperator.NotIn:
                                whereClause.Append($"{filter.FieldName} NOT IN ({string.Join(",", lookupValues)})");
                                break;
                        }
                    }

                    whereClause.Append(joinWith);
                }
            }

            string returnData = whereClause.ToString().TrimEnd(joinWith.ToCharArray());
            if (string.IsNullOrEmpty(returnData))
                returnData = "1 = 1";

            return returnData;
        }
    }
}
