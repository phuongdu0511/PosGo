namespace PosGo.Contract.Extensions;

public static class SqlExtensions
{
    public static string AddWhereCondition(this string sql, string condition, ref List<object> parameters, object value)
    {
        var hasWhere = sql.ToUpper().Contains(" WHERE ");
        var connector = hasWhere ? " AND " : " WHERE ";

        var paramIndex = parameters.Count;
        parameters.Add(value);

        // Tìm vị trí chèn (trước ORDER BY nếu có)
        var orderByIndex = sql.ToUpper().IndexOf(" ORDER BY");
        var insertPosition = orderByIndex > 0 ? orderByIndex : sql.Length;

        return sql.Insert(insertPosition, $"{connector}{condition} = {{{paramIndex}}}");
    }
}
