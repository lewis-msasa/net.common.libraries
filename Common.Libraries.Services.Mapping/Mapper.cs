using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Mapping
{
    public interface IMapFrom<TEntity>
    {
        void MapFrom(TEntity entity);
    }
    public static class Mapper
    {
        public static TDestination Map<TSource, TDestination>(TSource source)
            where TDestination : IMapFrom<TSource>, new()
        {
            var destination = new TDestination();
            destination.MapFrom(source);
            return destination;
        }
       

        public static List<TDestination> MapList<TSource, TDestination>(IEnumerable<TSource> sourceList)
            where TDestination : IMapFrom<TSource>, new()
        {
            return sourceList.Select(Map<TSource, TDestination>).ToList();
        }
    }
    public static class MapperExtensions
    {
        public static TDestination Map<TSource, TDestination>(this TSource source)
          where TDestination : IMapFrom<TSource>, new()
        {
            var destination = new TDestination();
            destination.MapFrom(source);
            return destination;
        }
        public static List<TDestination> MapList<TSource, TDestination>(this IEnumerable<TSource> sourceList)
           where TDestination : IMapFrom<TSource>, new()
        {
            return sourceList.Select(Map<TSource, TDestination>).ToList();
        }
    }

}
