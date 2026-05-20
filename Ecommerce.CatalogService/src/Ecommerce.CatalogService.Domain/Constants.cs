namespace Ecommerce.CatalogService.Domain
{
    public static class Constants
    {
        public const int MaxCategoryNameLength = 50;

        public const decimal MinProductPrice = default;

        public const int MinProductAmount = default;

        public const int DefaultMessageRetryCount = 0;

        public class CatalogEventTypes
        {
            public const string ProductUpdated = "ProductUpdated";
        }
    }
}
