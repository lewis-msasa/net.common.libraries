namespace Common.Libraries.USSD.Settings
{
    public interface IUSSDSettings
    {
        string IPAddress { get; set; }
        int Port { get; set; }
        string Network { get; set; }

        int SessionMinutesToRefresh { get; set; }

        string ShortCode { get; set; }
    }
}