using System;
using System.Linq.Expressions;
using System.Reflection;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Data_EF.Extensions
{
    public static class SoftDeleteQueryExtension
    {
        public static void AddSoftDeleteQueryFilter(
            this IMutableEntityType entityData)
        {
            var methodToCall = typeof(SoftDeleteQueryExtension)
                .GetMethod(nameof(GetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(entityData.ClrType);
            if (methodToCall != null)
            {
                var filter = methodToCall.Invoke(null, new object[] { });
                entityData.SetQueryFilter((LambdaExpression) filter);
            }

            entityData.AddIndex(entityData.FindProperty(nameof(SafeEntity.IsDeleted)));
        }

        private static LambdaExpression GetSoftDeleteFilter<TEntity>()
            where TEntity : SafeEntity
        {
            Expression<Func<TEntity, bool>> filter = x => x.IsDeleted == false;
            return filter;
        }
    }
}