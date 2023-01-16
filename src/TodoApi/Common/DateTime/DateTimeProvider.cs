namespace TodoApi.Common.DateTime;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public System.DateTime Now => System.DateTime.Now;
    public System.DateTime UtcNow => System.DateTime.UtcNow;
}