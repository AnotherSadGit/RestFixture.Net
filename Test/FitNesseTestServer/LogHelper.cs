namespace RestFixture.Net.FitNesseTestServer
{
    public class LogHelper
    {
        public static string GetDisplayText(string rawText)
        {
            if (rawText == null)
            {
                return "[NULL]";
            }

            if (rawText == "")
            {
                return "[EMPTY STRING]";
            }

            if (rawText.TrimEnd() == "")
            {
                return "[BLANK STRING]";
            }

            return rawText.Trim();
        } 
    }
}