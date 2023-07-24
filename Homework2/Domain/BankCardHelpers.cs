using System.Reflection;

namespace Fuse8_ByteMinds.SummerSchool.Domain;

public static class BankCardHelpers
{
	/// <summary>
	/// Получает номер карты без маски
	/// </summary>
	/// <param name="card">Банковская карта</param>
	/// <returns>Номер карты без маски</returns>
	public static string? GetUnmaskedCardNumber(BankCard card)
	{
		var privateField = card.GetType().GetField("_number", BindingFlags.NonPublic | BindingFlags.Instance);
		if (privateField != null && card != null)
			return (string?) privateField.GetValue(card);
		else
			return string.Empty;
    }
}