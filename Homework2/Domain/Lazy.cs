namespace Fuse8_ByteMinds.SummerSchool.Domain;

/// <summary>
/// Контейнер для значения, с отложенным получением
/// </summary>
public class Lazy<TValue>
{
    private bool isValueCreated;
    private TValue? value;
    private Func<TValue> valueFactory;

    public Lazy(Func<TValue> valueFactory)
    {
        this.valueFactory = valueFactory;
    }

    public TValue? Value
    {
        get
        {
            if (!isValueCreated)
            {
                value = valueFactory();
                isValueCreated = true;
            }
            return value;
        }
    }
}