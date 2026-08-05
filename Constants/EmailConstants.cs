namespace WebApplication2.Constants
{
    public static class EmailConstants
    {
        public const string OfflineReportSubject =
            "Offline People Report";

        public const string OfflineReportBodyTemplate =
            "The scheduled offline people report was generated successfully." +
            "\nOffline people count: {0}." +
            "\nThe CSV report is attached.";
    }
}
