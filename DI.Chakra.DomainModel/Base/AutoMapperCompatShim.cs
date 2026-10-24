using System;
using Microsoft.Extensions.Logging.Abstractions;

namespace AutoMapper
{
    /// <summary>
    /// Drop-in replacement for the static Mapper API that AutoMapper 4.x shipped (removed
    /// upstream starting with AutoMapper 10). Backed internally by the current
    /// MapperConfiguration/IMapper engine, so the existing call sites across this solution
    /// (Mapper.CreateMap&lt;T,U&gt;().ReverseMap(), Mapper.Map&lt;T&gt;(source), etc.) did not
    /// need to change when AutoMapper was upgraded to fix CVE-2026-32933 (GHSA-rvv3-g6hj-g44x).
    ///
    /// CreateMap intentionally returns MappingExpressionHandle/UntypedMappingExpressionHandle
    /// (defined below) rather than AutoMapper's own IMappingExpression(&lt;,&gt;): those real
    /// AutoMapper types are only usable by projects that directly reference the AutoMapper
    /// assembly, which would collide with this shim's own Mapper/TypeConverter types (both
    /// named identically to the pre-v10 API they replace) wherever a project references both
    /// this assembly and AutoMapper directly. Routing everything through this shim's own
    /// wrapper types means only this assembly needs the direct AutoMapper reference.
    /// </summary>
    public static class Mapper
    {
        private static readonly object Gate = new object();
        private static readonly MapperConfigurationExpression ConfigExpression = new MapperConfigurationExpression();
        private static IMapper _mapper;

        public static MappingExpressionHandle<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            lock (Gate)
            {
                _mapper = null;
                return new MappingExpressionHandle<TSource, TDestination>(ConfigExpression.CreateMap<TSource, TDestination>());
            }
        }

        public static UntypedMappingExpressionHandle CreateMap(Type sourceType, Type destinationType)
        {
            lock (Gate)
            {
                _mapper = null;
                return new UntypedMappingExpressionHandle(ConfigExpression.CreateMap(sourceType, destinationType));
            }
        }

        public static TDestination Map<TDestination>(object source)
        {
            return GetMapper().Map<TDestination>(source);
        }

        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            return GetMapper().Map<TSource, TDestination>(source);
        }

        private static IMapper GetMapper()
        {
            if (_mapper != null)
                return _mapper;

            lock (Gate)
            {
                if (_mapper == null)
                {
                    var configuration = new MapperConfiguration(ConfigExpression, NullLoggerFactory.Instance);
                    _mapper = configuration.CreateMapper();
                }
                return _mapper;
            }
        }
    }

    /// <summary>
    /// Drop-in replacement for AutoMapper 4.x's abstract TypeConverter&lt;TSource,TDestination&gt;
    /// base class (removed upstream), used by this solution's .ConvertUsing&lt;T&gt;() custom
    /// converters. Adapts the legacy ConvertCore(source) shape onto the current
    /// ITypeConverter&lt;TSource,TDestination&gt; interface.
    /// </summary>
    public abstract class TypeConverter<TSource, TDestination> : ITypeConverter<TSource, TDestination>
    {
        protected abstract TDestination ConvertCore(TSource source);

        TDestination ITypeConverter<TSource, TDestination>.Convert(TSource source, TDestination destination, ResolutionContext context)
        {
            return ConvertCore(source);
        }
    }

    /// <summary>
    /// Wraps AutoMapper's IMappingExpression&lt;TSource,TDestination&gt; so callers that only
    /// reference this assembly (not AutoMapper directly) can still chain .ReverseMap() /
    /// .ConvertUsing&lt;T&gt;() exactly as the old Mapper.CreateMap&lt;T,U&gt;() API allowed.
    /// </summary>
    public struct MappingExpressionHandle<TSource, TDestination>
    {
        private readonly IMappingExpression<TSource, TDestination> _inner;

        internal MappingExpressionHandle(IMappingExpression<TSource, TDestination> inner)
        {
            _inner = inner;
        }

        public MappingExpressionHandle<TDestination, TSource> ReverseMap()
        {
            return new MappingExpressionHandle<TDestination, TSource>(_inner.ReverseMap());
        }

        public MappingExpressionHandle<TSource, TDestination> ConvertUsing<TConverter>()
            where TConverter : TypeConverter<TSource, TDestination>
        {
            _inner.ConvertUsing<TConverter>();
            return this;
        }
    }

    /// <summary>
    /// Non-generic counterpart of <see cref="MappingExpressionHandle{TSource,TDestination}"/>,
    /// for the open-generic Mapper.CreateMap(Type, Type) overload.
    /// </summary>
    public struct UntypedMappingExpressionHandle
    {
        private readonly IMappingExpression _inner;

        internal UntypedMappingExpressionHandle(IMappingExpression inner)
        {
            _inner = inner;
        }

        public UntypedMappingExpressionHandle ReverseMap()
        {
            return new UntypedMappingExpressionHandle(_inner.ReverseMap());
        }
    }
}
