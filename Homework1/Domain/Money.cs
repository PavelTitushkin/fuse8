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

    /// <summary>
    /// Переопределение операторов
    /// </summary>
    public static Money operator +(Money money1, Money money2)
    {
        int rublesSum = 0;
        int kopeksSum = 0;
        if (money1.IsNegative && money2.IsNegative || !money1.IsNegative && !money2.IsNegative)
        {
            rublesSum = money1.Rubles + money2.Rubles;
            kopeksSum = money1.Kopeks + money2.Kopeks;
            if (kopeksSum > 99)
            {
                kopeksSum -= 100;
                rublesSum++;
            }
            if (money1.IsNegative && money2.IsNegative)
                return new Money(true, rublesSum, kopeksSum);
            else
                return new Money(false, rublesSum, kopeksSum);
        }
        else
        {
            if (money1.IsNegative)
            {
                if (money1 > money2)
                {
                    rublesSum = money1.Rubles - money2.Rubles;
                    kopeksSum = money1.Kopeks - money2.Kopeks;
                    if (kopeksSum < 0)
                    {
                        kopeksSum += 100;
                        rublesSum--;
                    }

                    return new Money(true, rublesSum, kopeksSum);
                }
                else
                {
                    rublesSum = money2.Rubles - money1.Rubles;
                    kopeksSum = money2.Kopeks - money1.Kopeks;
                    if (kopeksSum < 0)
                    {
                        kopeksSum += 100;
                        rublesSum--;
                    }

                    return new Money(false, rublesSum, kopeksSum);
                }
            }
            else
            {
                if (money1 > money2)
                {
                    rublesSum = money1.Rubles - money2.Rubles;
                    kopeksSum = money1.Kopeks - money2.Kopeks;
                    if (kopeksSum < 0)
                    {
                        kopeksSum += 100;
                        rublesSum--;
                    }

                    return new Money(false, rublesSum, kopeksSum);
                }
                else
                {
                    rublesSum = money2.Rubles - money1.Rubles;
                    kopeksSum = money2.Kopeks - money1.Kopeks;
                    if (kopeksSum < 0)
                    {
                        kopeksSum += 100;
                        rublesSum--;
                    }

                    return new Money(true, rublesSum, kopeksSum);
                }
            }
        }
    }

	public static Money operator -(Money money1, Money money2)
	{
        int rublesSum = 0;
        int kopeksSum = 0;
        if (money1.IsNegative && money2.IsNegative || !money1.IsNegative && !money2.IsNegative)
        {
            if(money1.IsNegative && money2.IsNegative)
            {
                if (money1 < money2)
                {
                    rublesSum = money1.Rubles - money2.Rubles;
                    kopeksSum = money1.Kopeks - money2.Kopeks;
                    if (kopeksSum < 0)
                    {
                        kopeksSum += 100;
                        rublesSum--;
                    }

                    return new Money(false, rublesSum, kopeksSum);
                }
                else
                {
                    rublesSum = money2.Rubles - money1.Rubles;
                    kopeksSum = money2.Kopeks - money1.Kopeks;
                    if (kopeksSum < 0)
                    {
                        kopeksSum += 100;
                        rublesSum--;
                    }

                    return new Money(true, rublesSum, kopeksSum);
                }
            }
            else
            {
                if (money1 > money2)
                {
                    rublesSum = money1.Rubles - money2.Rubles;
                    kopeksSum = money1.Kopeks - money2.Kopeks;
                    if (kopeksSum < 0)
                    {
                        kopeksSum += 100;
                        rublesSum--;
                    }

                    return new Money(false, rublesSum, kopeksSum);
                }
                else
                {
                    rublesSum = money2.Rubles - money1.Rubles;
                    kopeksSum = money2.Kopeks - money1.Kopeks;
                    if (kopeksSum < 0)
                    {
                        kopeksSum += 100;
                        rublesSum--;
                    }
                    if(rublesSum==0||kopeksSum==0)
                        return new Money(false, rublesSum, kopeksSum);

                    return new Money(true, rublesSum, kopeksSum);
                }
            }
        }
        else
        {
            rublesSum = money1.Rubles + money2.Rubles;
            kopeksSum = money1.Kopeks + money2.Kopeks;
            if (kopeksSum > 99)
            {
                kopeksSum -= 100;
                rublesSum++;
            }
            if (money1.IsNegative)
                return new Money(true, rublesSum, kopeksSum);
            else
                return new Money(false, rublesSum, kopeksSum);
        }
    }

    public static bool operator >(Money money1, Money money2)
	{
        if (money1.IsNegative)
        {
            if(money2.IsNegative)
            {
                int sum1 = money1.Rubles * 100;
                sum1 += money1.Kopeks;
                int sum2 = money2.Rubles * 100;
                sum2 += money2.Kopeks;

                return sum1 < sum2;
            }
            else
                return false;
        }
        else
        {
            if (money2.IsNegative)
                return true;
            else
            {
                int sum1 = money1.Rubles * 100;
                sum1 += money1.Kopeks;
                int sum2 = money2.Rubles * 100;
                sum2 += money2.Kopeks;

                return sum1 > sum2;
            }
        }
	}

    public static bool operator <(Money money1, Money money2)
	{
        if (money1.IsNegative)
        {
            if (money2.IsNegative)
            {
                int sum1 = money1.Rubles * 100;
                sum1 += money1.Kopeks;
                int sum2 = money2.Rubles * 100;
                sum2 += money2.Kopeks;

                return sum1 > sum2;
            }
            else
                return true;
        }
        else
        {
            if (money2.IsNegative)
                return false;
            else
            {
                int sum1 = money1.Rubles * 100;
                sum1 += money1.Kopeks;
                int sum2 = money2.Rubles * 100;
                sum2 += money2.Kopeks;

                return sum1 < sum2;
            }
        }
    }

    public static bool operator >=(Money money1, Money money2)
    {
        if (money1.IsNegative)
        {
            if (money2.IsNegative)
            {
                int sum1 = money1.Rubles * 100;
                sum1 += money1.Kopeks;
                int sum2 = money2.Rubles * 100;
                sum2 += money2.Kopeks;

                return sum1 <= sum2;
            }
            else
                return false;
        }
        else
        {
            if (money2.IsNegative)
                return true;
            else
            {
                int sum1 = money1.Rubles * 100;
                sum1 += money1.Kopeks;
                int sum2 = money2.Rubles * 100;
                sum2 += money2.Kopeks;

                return sum1 >= sum2;
            }
        }
    }

    public static bool operator <=(Money money1, Money money2)
    {
        if (money1.IsNegative)
        {
            if (money2.IsNegative)
            {
                int sum1 = money1.Rubles * 100;
                sum1 += money1.Kopeks;
                int sum2 = money2.Rubles * 100;
                sum2 += money2.Kopeks;

                return sum1 >= sum2;
            }
            else
                return true;
        }
        else
        {
            if (money2.IsNegative)
                return false;
            else
            {
                int sum1 = money1.Rubles * 100;
                sum1 += money1.Kopeks;
                int sum2 = money2.Rubles * 100;
                sum2 += money2.Kopeks;

                return sum1 <= sum2;
            }
        }
    }

    /// <summary>
    /// Переопределение методов класса Object
    /// </summary>
    public override string ToString() => $"{Rubles}руб. {Kopeks}коп.";

    public override bool Equals(object? obj) => obj is Money money && this.Rubles == money.Rubles && this.Kopeks == money.Kopeks;

    public override int GetHashCode() => (Rubles, Kopeks).GetHashCode();
}