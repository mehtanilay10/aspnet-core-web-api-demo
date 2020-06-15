using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DemoApp.DataAccess.ExtensionMethods;
using DemoApp.DataAccess.Models;

namespace DemoApp.DataAccess.Services
{
    #region Interface

    public interface ISpecification<T>
    {
        IList<Expression<Func<T, bool>>> Criteria { get; }
        IList<Expression<Func<T, object>>> Includes { get; }
        IList<string> IncludeStrings { get; }

        void AddInclude(Expression<Func<T, object>> includeExpression);
        void AddInclude(string includeString);
        void AddFilter(Expression<Func<T, bool>> criteria);
        void AddFilters(IList<Condition> conditions);
        void RemoveAllFilters();
    }

    #endregion

    public sealed class Specification<T> : ISpecification<T>
    {
        private IList<Expression<Func<T, bool>>> _Criteria;
        private IList<Expression<Func<T, object>>> _Includes;
        private IList<string> _IncludeStrings;
        public Specification()
        {
            this._Criteria = new List<Expression<Func<T, bool>>>();
            this._Includes = new List<Expression<Func<T, object>>>();
            this._IncludeStrings = new List<string>();
        }

        IList<Expression<Func<T, bool>>> ISpecification<T>.Criteria => this._Criteria;

        IList<Expression<Func<T, object>>> ISpecification<T>.Includes => this._Includes;

        IList<string> ISpecification<T>.IncludeStrings => this._IncludeStrings;

        public void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            this._Includes.Add(includeExpression);
        }
        public void AddInclude(string includeString)
        {
            this._IncludeStrings.Add(includeString);
        }
        public void AddFilter(Expression<Func<T, bool>> criteria)
        {
            this._Criteria.Add(criteria);
        }

        public void AddFilters(IList<Condition> conditions)
        {
            foreach (var condition in conditions)
            {
                var colPredicate = ExpressionBuilder.BuildPredicate<T>(condition.Value, condition.Operator, condition.FieldName);
                if (colPredicate != null)
                    this._Criteria.Add(colPredicate);
            }
        }

        public void RemoveAllFilters()
        {
            this._Criteria.Clear();
        }

        //public void AddFilters(object searchModel)
        //{
        //    foreach (var property in typeof(T).GetProperties())
        //    {
        //        string propertyValue = Convert.ToString(property.GetValue(searchModel)) ?? string.Empty;

        //        var isNumericField = new string[] { "Int64", "Int32", "Int16", "Byte" }.Contains(property.PropertyType.Name);
        //        dynamic dynamicValue = null;
        //        OperatorComparer @operator;

        //        //if it is numeric, ignore operator, always set to equals, other operator are not performance wise with numeric
        //        if (isNumericField)
        //        {
        //            if (property.PropertyType.Name.Equals("Int64"))
        //                dynamicValue = Convert.ToInt64(propertyValue);
        //            else if (property.PropertyType.Name.Equals("Int32"))
        //                dynamicValue = Convert.ToInt32(propertyValue);
        //            else if (property.PropertyType.Name.Equals("Int16") || property.PropertyType.Name.Equals("Byte"))
        //                dynamicValue = Convert.ToInt16(propertyValue);

        //            @operator = OperatorComparer.Equals;
        //        }

        //        else
        //        {
        //            // Sometimes the field is not detected as numeric but data is a numeric type
        //            dynamicValue = Convert.ToString(propertyValue);
        //            @operator = OperatorComparer.Contains;
        //        }

        //        if (!string.IsNullOrEmpty(propertyValue))
        //        {
        //            var colPredicate = ExpressionBuilder.BuildPredicate<T>(propertyValue, @operator, property.Name);
        //            if (colPredicate != null)
        //                this._Criteria.Add(colPredicate);
        //        }
        //    }
        //}
    }
}
