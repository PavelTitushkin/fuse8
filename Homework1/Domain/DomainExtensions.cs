namespace Fuse8_ByteMinds.SummerSchool.Domain;

public static class DomainExtensions
{
    /// <summary>
    /// Метод IsNullOrEmpty от IEnumerable<T>.Возвращает true, если переданная коллекция равна null или не содержит элементов
    /// </summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection) => collection == null || !collection.Any();

    /// <summary>
    /// Метод JoinToString от IEnumerable<T>. Принимает строку разделитель и возвращает единую строку, состоящую из значений коллекции соединенных с помощью разделителя.
    /// </summary>
    public static string JoinToString<T>(this IEnumerable<T> collection, string separator)
    {
        if (collection != null && separator != null)
        {
            var singleLine = string.Join(separator, collection);

            return singleLine;
        }
        else
            throw new ArgumentNullException(nameof(collection));
    }

    /// <summary>
    /// Метод DaysCountBetween от DateTimeOffset.Принимает второй DateTimeOffset и возвращает количество дней между двумя датами
    /// </summary>
    public static int DaysCountBetween(this DateTimeOffset firstDate, DateTimeOffset secondDate)
    {
        var date = firstDate.Subtract(secondDate);
        double days = date.TotalDays;
        int countDays = (int)days;
        if (countDays < 0)
            countDays *= -1;

        return countDays;
    }
}