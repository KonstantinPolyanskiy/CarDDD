using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.ApplicationServices.Services.Helpers;

public static class ConditionParser
{
    public static List<Condition> Parse(IEnumerable<string> values)
    {
        var result = new List<Condition>();
        foreach (var s in values)
        {
            if (Enum.TryParse<Condition>(s, ignoreCase: true, out var cond))
            {
                result.Add(cond);
            }
        }
        return result;
    }
}
