using System.Xml.Linq;
using System;

namespace Fuse8_ByteMinds.SummerSchool.Domain;

/// <summary>
/// Модель для хранения денег
/// </summary>
public class Money
{
	public Money(int rubles, int kopeks)
		: this(false, rubles, kopeks)
	{
	}

	public Money(bool isNegative, int rubles, int kopeks) 
	{
        if (kopeks > 99 || kopeks < 0 || rubles < 0 || isNegative && rubles == 0 && kopeks == 0)
        {
            throw new ArgumentException("Money нельзя создать с кол - вом коп. > 99 и < 0 и с кол - вом руб. < 0");
        }
        IsNegative = isNegative;
        Rubles = rubles;
        Kopeks = kopeks;
    }

	/// <summary>
	/// Отрицательное значение
	/// </summary>
	public bool IsNegative { get; }

	/// <summary>
	/// Число рублей
	/// </summary>
	public int Rubles { get; }

	/// <summary>
	/// Количество копеек
	/// </summary>
	public int Kopeks { get; }


    //public static int FullAmountKopecks(int rubles, int kopeks) => rubles * 100 + kopeks;
    private int FullAmountKopecks()
    {
        if (IsNegative)
            return (Rubles * 100 + Kopeks) * (-1);
        else
            return Rubles * 100 + Kopeks;
    }


    /// <summary>
    /// Переопределение операторов
    /// </summary>
    public static Money operator +(Money money1, Money money2)
    {
        int fullAmountMoney1 = money1.FullAmountKopecks();
        int fullAmountMoney2 = money2.FullAmountKopecks();
        var rubles = fullAmountMoney1 + fullAmountMoney2;
        var kopeks = 0;
        if (rubles < 0)
        {
            kopeks = (fullAmountMoney1 + fullAmountMoney2) * (-1) % 100;

            return new Money(true, rubles * (-1) / 100, kopeks);
        }

        kopeks = (fullAmountMoney1 + fullAmountMoney2) % 100;

        return new Money(false, rubles / 100, kopeks);
    }

    public static Money operator -(Money money1, Money money2)
	{
        int fullAmountMoney1 = money1.FullAmountKopecks();
        int fullAmountMoney2 = money2.FullAmountKopecks();
        var rubles = fullAmountMoney1 - fullAmountMoney2;
        var kopeks = 0;
        if (rubles < 0)
        {
            kopeks = (fullAmountMoney1 - fullAmountMoney2) * (-1) % 100;

            return new Money(true, rubles * (-1) / 100, kopeks);
        }

        kopeks = (fullAmountMoney1 - fullAmountMoney2) % 100;

        return new Money(false, rubles / 100, kopeks);
    }

    public static bool operator >(Money money1, Money money2) => money1.FullAmountKopecks() > money2.FullAmountKopecks();

    public static bool operator <(Money money1, Money money2) => money1.FullAmountKopecks() < money2.FullAmountKopecks();


    public static bool operator >=(Money money1, Money money2) => money1.FullAmountKopecks() >= money2.FullAmountKopecks();


    public static bool operator <=(Money money1, Money money2) => money1.FullAmountKopecks() <= money2.FullAmountKopecks();


    /// <summary>
    /// Переопределение методов класса Object
    /// </summary>
    public override string ToString() => IsNegative ? $"-{Rubles}руб. {Kopeks}коп." : $"{Rubles}руб. {Kopeks}коп.";

    public override bool Equals(object? obj) => obj is Money money && this.Rubles == money.Rubles && this.Kopeks == money.Kopeks && this.IsNegative == money.IsNegative;

    public override int GetHashCode() => HashCode.Combine(Rubles, Kopeks, IsNegative);
}