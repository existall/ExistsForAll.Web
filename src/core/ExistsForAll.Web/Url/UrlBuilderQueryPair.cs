namespace ExistsForAll.Web.Url
{
    public class UrlBuilderQueryPair
    {
        public UrlBuilderQueryPair(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
