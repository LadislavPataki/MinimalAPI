namespace TodoApi.Common.DateTime;

public interface IDateTimeProvider
{
    System.DateTime Now { get; }
    System.DateTime UtcNow { get; }
}