
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore
{
    public static class PrestoPropertyBuilderExtensions
    {
        public static PropertyBuilder IsRowType(this PropertyBuilder propertyBuilder)
        {
            propertyBuilder.HasColumnType("row");

            return propertyBuilder;
        }

        public static PropertyBuilder IsArrayType(this PropertyBuilder propertyBuilder)
        {
            propertyBuilder.HasColumnType("array");

            return propertyBuilder;
        }
    }
}
