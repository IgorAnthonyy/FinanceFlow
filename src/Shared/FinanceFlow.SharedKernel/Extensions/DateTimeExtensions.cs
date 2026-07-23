namespace FinanceFlow.SharedKernel.Extensions;

/// <summary>
/// Classe responsável por estender o DateTime para converter para o fuso horário de Brasília.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Converte o DateTime para o fuso horário de Brasília.
    /// </summary>
    /// <param name="dateTime">O DateTime a ser convertido.</param>
    /// <returns>O DateTime convertido para o fuso horário de Brasília.</returns>
    public static DateTime ToBrasiliaTime(this DateTime dateTime)
    {
        try
        {
            var timezoneId = OperatingSystem.IsWindows()
                ? "E. South America Standard Time"

                : "America/Sao_Paulo";
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime.Kind == DateTimeKind.Utc ? dateTime : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc), tz);
        }
        catch
        {
            return dateTime.AddHours(-3);
        }
    }
}
