using System;

namespace Fuse8_ByteMinds.SummerSchool.Domain;

/// <summary>
/// Значения ресурсов для календаря
/// </summary>
public class CalendarResource
{
	public static CalendarResource Instance;
	public static readonly string January;
	public static readonly string February;
	private static readonly string[] MonthNames;

	static CalendarResource()
	{
		MonthNames = new[]
		{
			"Январь",
			"Февраль",
			"Март",
			"Апрель",
			"Май",
			"Июнь",
			"Июль",
			"Август",
			"Сентябрь",
			"Октябрь",
			"Ноябрь",
			"Декабрь",
		};
		Instance = new();
		January = GetMonthByNumber(0);
		February = GetMonthByNumber(1);
	}

	private static string GetMonthByNumber(int number)
			=> MonthNames[number];

    /// <summary>
    /// Индексатор для получения названия месяца по енаму Month
    /// </summary>
    public string this[Month index]
	{
		get
		{
			switch (index)
			{
				case Month.January: return GetMonthByNumber(0);
				case Month.February: return GetMonthByNumber(1);
				case Month.March: return GetMonthByNumber(2);
				case Month.April: return GetMonthByNumber(3);
				case Month.May: return GetMonthByNumber(4);
				case Month.June: return GetMonthByNumber(5);
				case Month.July: return GetMonthByNumber(6);
				case Month.August: return GetMonthByNumber(7);
				case Month.September: return GetMonthByNumber(8);
				case Month.October: return GetMonthByNumber(9);
				case Month.November: return GetMonthByNumber(10);
				case Month.December: return GetMonthByNumber(11);

				default: throw new ArgumentOutOfRangeException();
			}
		}
	}
}
	public enum Month
	{
		January,
		February,
		March,
		April,
		May,
		June,
		July,
		August,
		September,
		October,
		November,
		December,
	}
