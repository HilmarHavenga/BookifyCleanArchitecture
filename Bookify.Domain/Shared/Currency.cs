namespace Bookify.Domain.Shared;

public record Currency
{
    internal static Currency None = new("");
    public static Currency Usd { get; } = new("USD");
    public static Currency Eur { get; } = new("EUR");

    private Currency(string code) => Code = code;

    public string Code { get; init; }

    public static Currency FromCode(string code)
    {
        return AllCurrencies.FirstOrDefault(c => c.Code == code) ??
            throw new ApplicationException($"The currency code is invalid");
    }

    public static readonly IReadOnlyCollection<Currency> AllCurrencies = [ Usd, Eur ];
}