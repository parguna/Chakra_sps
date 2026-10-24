using AutoMapper;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using System.Web.Http.Filters;

namespace DI.Chakra.DomainApi.Controllers
{
    [BasicAuthentication]
    [Authorize]
    public abstract class BaseController<TModel, TEntity, TDto> : ApiController
        where TModel : DomainModelBase<TModel, TEntity>
        where TEntity : EntityModel.EntityBase
        where TDto : Dto.DtoBase
    {

        [HttpGet]
        [Route("")]
        public List<TDto> GetAll()
        {
            return DtoMapList(DomainModelBase<TModel, TEntity>.GetAll());
        }

        [HttpGet]
        [Route("{pageSize:int}/{pageNumber:int}")]
        public PagedResults<TDto> GetAllPaging(int pageSize, int pageNumber)
        {
            return DtoMapListPaging(DomainModelBase<TModel, TEntity>.GetAllPaging(pageSize,pageNumber));
        }

        [HttpGet]
        [Route("{eagerLoadAllProperties:bool}/{pageSize:int}/{pageNumber:int}")]
        public List<TDto> GetAll(bool eagerLoadAllProperties, int pageSize, int pageNumber)
        {
            return DtoMapList(DomainModelBase<TModel, TEntity>.GetAll(eagerLoadAllProperties, pageSize, pageNumber));
        }

        [HttpGet]
        [Route("{eagerLoadAllProperties:bool}/{pageSize:int}/{pageNumber:int}/{args:int}")]
        public PagedResults<TDto> GetAllPaging(bool eagerLoadAllProperties, int pageSize, int pageNumber, int args)
        {
            return DtoMapListPaging(DomainModelBase<TModel, TEntity>.GetAllPaging(eagerLoadAllProperties, pageSize, pageNumber));
        }

        [HttpGet]
        [Route("{filter:Expression<Func<TDto, bool>>}/{eagerLoadAllProperties:bool}/{pageSize:int}/{pageNumber:int}")]
        public static List<TDto> Get(Expression<Func<TDto, bool>> filter, bool eagerLoadAllProperties, int pageSize, int pageNumber)
        {
            return DtoMapList(DomainModelBase<TModel, TEntity>.Get(Mapper.Map<Expression<Func<TModel, bool>>>(filter), eagerLoadAllProperties, pageSize, pageNumber));
        }

        [HttpGet]
        [Route("{id:long}")]
        public TDto GetById(long id)
        {
            return DtoMap(DomainModelBase<TModel, TEntity>.GetById(id));
        }

        [HttpGet]
        [Route("{id:long}/{eagerLoad:bool}")]
        public TDto GetById(long id, bool eagerLoad)
        {
            return DtoMap(DomainModelBase<TModel, TEntity>.GetById(id, eagerLoad));

        }


        [HttpPost]
        [Route("")]
        public long Save(TDto dto)
        {
            return DtoUnMap(dto).Save(User.Identity.Name);
        }


        [HttpPost]
        [Route("SaveList")]
        public void SaveList(List<TDto> dtoList)
        {
            DomainModelBase<TModel, TEntity>.SaveList(DtoUnMapList(dtoList),User.Identity.Name);
        }

        //[HttpPost]
        //[Route("")]
        //public void Delete(TDto dto)
        //{
        //    DomainUnMap(dto).Delete();
        //}

        [HttpDelete]
        [Route("{id:long}")]
        public void Delete(long id)
        {
            DomainModelBase<TModel, TEntity>.Delete(id);
        }

        //[HttpPost]
        //[Route("{id:long}")]
        //public void DeleteList(List<TDto> dtoList)
        //{
        //    DomainModelBase<TModel, TEntity>.DeleteList(DomainUnMapList(dtoList));
        //}

        public static TDto DtoMap(TModel tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<TModel, TDto>(tModel);
        }

        public static List<TDto> DtoMapList(IList<TModel> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        public PagedResults<TDto> DtoMapListPaging(PagedResults<TModel> tModelList)
        {
            var results = tModelList.Results;
            var mappedResults = results.Select(DtoMap).ToList();

            return new PagedResults<TDto>()
            {
                PageNumber = tModelList.PageNumber,
                PageSize = tModelList.PageSize,
                TotalNumberOfPages = tModelList.TotalNumberOfPages,
                TotalNumberOfRecords = tModelList.TotalNumberOfRecords,
                Results = mappedResults
            };
        }

        public static TModel DtoUnMap(TDto dto)
        {
            if (dto == null)
                return default(TModel);

            return Mapper.Map<TDto, TModel>(dto);
        }

        public static List<TModel> DtoUnMapList(List<TDto> dtoList)
        {
            return dtoList.Select(DtoUnMap).ToList();
        }

        public static PagedResults<TDto> DtoMapListPager(PagedResults<TModel> tModelList)
        {
            var results = tModelList.Results;
            var mappedResults = DtoMapList(results);

            return new PagedResults<TDto>()
            {
                PageNumber = tModelList.PageNumber,
                PageSize = tModelList.PageSize,
                TotalNumberOfPages = tModelList.TotalNumberOfPages,
                TotalNumberOfRecords = tModelList.TotalNumberOfRecords,
                Results = mappedResults
            };


        }
    }
}
