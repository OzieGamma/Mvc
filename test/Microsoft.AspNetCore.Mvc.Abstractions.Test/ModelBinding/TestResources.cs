using System.Globalization;

namespace Microsoft.AspNetCore.Mvc.Abstractions.Test.ModelBinding
{
    public static class TestResources
    {
        public static string Type_Three_Name => "type three name " + CultureInfo.CurrentCulture;
        public static string Type_Three_Description => "type three description " + CultureInfo.CurrentCulture;
        public static string Type_Three_Prompt => "type three prompt " + CultureInfo.CurrentCulture;
    }
}
