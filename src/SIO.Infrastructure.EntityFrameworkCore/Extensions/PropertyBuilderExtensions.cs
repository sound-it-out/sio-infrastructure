using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIO.Infrastructure.EntityFrameworkCore.ChangeTracking;
using SIO.Infrastructure.EntityFrameworkCore.ValueConversion;

namespace SIO.Infrastructure.EntityFrameworkCore.Extensions
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<T> HasJsonValueConversion<T>(this PropertyBuilder<T> builder)
            where T : class
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new JsonValueConverter<T>())
                   .Metadata
                   .SetValueComparer(new JsonValueComparer<T>());

            return builder;
        }
    }
}
