using System.Linq;
using System.Reflection;
using System.Security.AccessControl;

namespace Fuse8_ByteMinds.SummerSchool.Domain;

public static class AssemblyHelpers
{
	/// <summary>
	/// Получает информацию о базовых типах классов из namespace "Fuse8_ByteMinds.SummerSchool.Domain", у которых есть наследники.
	/// </summary>
	/// <remarks>
	///	Информация возвращается только по самым базовым классам.
	/// Информация о промежуточных базовых классах не возвращается
	/// </remarks>
	/// <returns>Список типов с количеством наследников</returns>
	public static (string BaseTypeName, int InheritorCount)[] GetTypesWithInheritors()
	{
		// Получаем все классы из текущей Assembly
		var assemblyClassTypes = Assembly.GetAssembly(typeof(AssemblyHelpers))
			!.DefinedTypes
			.Where(p => p.IsClass);

		var dictionary = new Dictionary<string, int>();
		foreach (var assembly in assemblyClassTypes)
		{
			if (!assembly.IsAbstract)
			{
                var type = GetBaseType(assembly.AsType());
                if (type != null && type.Namespace == "Fuse8_ByteMinds.SummerSchool.Domain")
                {
                    if (dictionary.ContainsKey(type.Name))
                        dictionary[type.Name]++;
                    else
                        dictionary.Add(type.Name, 1);
                }
            }
        }

		var tuplesList = new (string BaseTypeName, int InheritorCount)[dictionary.Count];
		int index = 0;
        foreach (var item in dictionary)
        {
			tuplesList[index] = (item.Key, item.Value);
			index++;
        }

        return tuplesList;
	}

	/// <summary>
	/// Получает базовый тип для класса
	/// </summary>
	/// <param name="type">Тип, для которого необходимо получить базовый тип</param>
	/// <returns>
	/// Первый тип в цепочке наследований. Если наследования нет, возвращает null
	/// </returns>
	/// <example>
	/// Класс A, наследуется от B, B наследуется от C
	/// При вызове GetBaseType(typeof(A)) вернется C
	/// При вызове GetBaseType(typeof(B)) вернется C
	/// При вызове GetBaseType(typeof(C)) вернется C
	/// </example>
	private static Type? GetBaseType(Type type)
	{
		var baseType = type;

		while (baseType.BaseType is not null && baseType.BaseType != typeof(object))
		{
			baseType = baseType.BaseType;
		}

		return baseType == type
			? null
			: baseType;
	}
}