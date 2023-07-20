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


    public static int FullAmountKopecks(int rubles, int kopeks) => rubles * 100 + kopeks;
    
    /// <summary>
    /// Переопределение операторов
    /// </summary>
    public static Money operator +(Money money1, Money money2)
    {
        int fullAmountMoney1 = FullAmountKopecks(money1.Rubles, money1.Kopeks);
        int fullAmountMoney2 = FullAmountKopecks(money2.Rubles, money2.Kopeks);

        if (!money1.IsNegative && !money2.IsNegative)
            return new Money((fullAmountMoney1 + fullAmountMoney2) / 100, (fullAmountMoney1 + fullAmountMoney2) % 100);
        if (money1.IsNegative && !money2.IsNegative)
        {
            var rubles = ((fullAmountMoney1 * (-1)) + fullAmountMoney2) / 100;
            var kopeks = 0;
            if (rubles < 0)
            {
                kopeks = (fullAmountMoney1 * (-1) + fullAmountMoney2) * (-1) % 100;

                return new Money(true, rubles * (-1), kopeks);
            }

            kopeks = (fullAmountMoney1 + fullAmountMoney2) % 100;

            return new Money(false, rubles, kopeks);
        }
        if (!money1.IsNegative && money2.IsNegative)
        {
            var rubles = (fullAmountMoney1 + fullAmountMoney2 * (-1)) / 100;
            var kopeks = (fullAmountMoney1 + fullAmountMoney2) % 100;

            return rubles < 0 ? new Money(true, rubles, kopeks) : new Money(false, rubles, kopeks);
        }
        else
            return new Money(true, (fullAmountMoney1 + fullAmountMoney2) / 100, (fullAmountMoney1 + fullAmountMoney2) % 100);
    }

    public static Money operator -(Money money1, Money money2)
	{
        int fullAmountMoney1 = FullAmountKopecks(money1.Rubles, money1.Kopeks);
        int fullAmountMoney2 = FullAmountKopecks(money2.Rubles, money2.Kopeks);

        if (!money1.IsNegative && !money2.IsNegative)
        {
            var rubles = (fullAmountMoney1 - fullAmountMoney2) / 100;
            var kopeks = 0;
            if (rubles < 0)
            {
                kopeks = (fullAmountMoney1 - fullAmountMoney2) * (-1) % 100;

                return new Money(true, rubles * (-1), kopeks);
            }

            kopeks = (fullAmountMoney1 - fullAmountMoney2) % 100;

            return new Money(false, rubles, kopeks);
        }
        if (money1.IsNegative && !money2.IsNegative)
        {
            var rubles = ((fullAmountMoney1 * (-1)) - fullAmountMoney2) / 100;
            var kopeks = (fullAmountMoney1 * (-1) - fullAmountMoney2) * (-1) % 100;

            return new Money(true, rubles * (-1), kopeks);
        }
        if (!money1.IsNegative && money2.IsNegative)
        {
            var rubles = (fullAmountMoney1 + fullAmountMoney2) / 100;
            var kopeks = (fullAmountMoney1 + fullAmountMoney2) % 100;

            return new Money(false, rubles, kopeks);
        }
        else
        {
            var rubles = fullAmountMoney1 * (-1) - fullAmountMoney2 * (-1);
            var kopeks = 0;
            if (rubles < 0)
            {
                kopeks = (fullAmountMoney1 * (-1) - fullAmountMoney2 * (-1)) * (-1) % 100;

                return new Money(true, rubles * (-1) / 100, kopeks);
            }

            kopeks = (fullAmountMoney1 * (-1) - fullAmountMoney2 * (-1)) % 100;

            return new Money(false, rubles / 100, kopeks);
        }
    }

    public static bool operator >(Money money1, Money money2)
	{
        int fullAmountMoney1 = FullAmountKopecks(money1.Rubles, money1.Kopeks);
        int fullAmountMoney2 = FullAmountKopecks(money2.Rubles, money2.Kopeks);

        if (money1.IsNegative && money2.IsNegative)
            return fullAmountMoney1 * (-1) > fullAmountMoney2 * (-1);
        else
        {
            if(money1.IsNegative)
                return fullAmountMoney1 * (-1) > fullAmountMoney2;
            if (money2.IsNegative)
                return fullAmountMoney1 > fullAmountMoney2 * (-1);

            return fullAmountMoney1 > fullAmountMoney2;
        }
    }

    public static bool operator <(Money money1, Money money2)
	{
        int fullAmountMoney1 = FullAmountKopecks(money1.Rubles, money1.Kopeks);
        int fullAmountMoney2 = FullAmountKopecks(money2.Rubles, money2.Kopeks);

        if (money1.IsNegative && money2.IsNegative)
            return fullAmountMoney1 * (-1) < fullAmountMoney2 * (-1);
        else
        {
            if (money1.IsNegative)
                return fullAmountMoney1 * (-1) < fullAmountMoney2;
            if (money2.IsNegative)
                return fullAmountMoney1 < fullAmountMoney2 * (-1);

            return fullAmountMoney1 < fullAmountMoney2;
        }
    }

    public static bool operator >=(Money money1, Money money2)
    {
        int fullAmountMoney1 = FullAmountKopecks(money1.Rubles, money1.Kopeks);
        int fullAmountMoney2 = FullAmountKopecks(money2.Rubles, money2.Kopeks);

        if (money1.IsNegative && money2.IsNegative)
            return fullAmountMoney1 * (-1) >= fullAmountMoney2 * (-1);
        else
        {
            if (money1.IsNegative)
                return fullAmountMoney1 * (-1) >= fullAmountMoney2;
            if (money2.IsNegative)
                return fullAmountMoney1 >= fullAmountMoney2 * (-1);

            return fullAmountMoney1 >= fullAmountMoney2;
        }
    }

    public static bool operator <=(Money money1, Money money2)
    {
        int fullAmountMoney1 = FullAmountKopecks(money1.Rubles, money1.Kopeks);
        int fullAmountMoney2 = FullAmountKopecks(money2.Rubles, money2.Kopeks);

        if (money1.IsNegative && money2.IsNegative)
            return fullAmountMoney1 * (-1) <= fullAmountMoney2 * (-1);
        else
        {
            if (money1.IsNegative)
                return fullAmountMoney1 * (-1) <= fullAmountMoney2;
            if (money2.IsNegative)
                return fullAmountMoney1 <= fullAmountMoney2 * (-1);

            return fullAmountMoney1 <= fullAmountMoney2;
        }
    }

    /// <summary>
    /// Переопределение методов класса Object
    /// </summary>
    public override string ToString() => IsNegative ? $"-{Rubles}руб. {Kopeks}коп." : $"{Rubles}руб. {Kopeks}коп.";

    public override bool Equals(object? obj) => obj is Money money && this.Rubles == money.Rubles && this.Kopeks == money.Kopeks && this.IsNegative == money.IsNegative;

    public override int GetHashCode() => HashCode.Combine(Rubles, Kopeks, IsNegative);
}