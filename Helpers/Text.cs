namespace pyttogpanne_api.Helpers
{
    public static class Text
    {
        /// <summary>The trimmed value, or null when it is blank. Blank and absent mean the same here.</summary>
        public static string? Trimmed(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
