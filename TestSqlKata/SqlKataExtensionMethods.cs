using SqlKata;
using SqlKata.Execution;

public static class SqlKataExtensionMethods
{
    public static Query Query<T>(this QueryFactory qf)
    {
        return qf.Query(typeof(T).Name);
    }

    public static Query Join<R, T>(this Query q)
    {
        var rname = typeof(R).Name;
        var tname = typeof(T).Name;

        return q.Join($"{tname}", $"{rname}.Id", $"{tname}.{rname}Id");
    }

    public static Query JoinChild<R, T>(this Query q)
    {
        var rname = typeof(R).Name;
        var tname = typeof(T).Name;

        return q.Join($"{tname}", $"{tname}.Id", $"{rname}.{tname}Id");
    }

    public static Query Where<T>(this Query q, string field, object val)
    {
        return q.Where($"{typeof(T).Name}.{field}", val);
    }
}