using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DI.Chakra.DomainApi
{
    public class WorkflowCriteriaDtoConverter : TypeConverter<DomainModel.Presentation.PresentationWorkflowCriteriaBase, Dto.Presentation.PresentationWorkflowCriteriaBase>
    {
        protected override Dto.Presentation.PresentationWorkflowCriteriaBase ConvertCore(DomainModel.Presentation.PresentationWorkflowCriteriaBase source)
        {
            if (source is DomainModel.Presentation.SubmitActionCriteria) return Mapper.Map<Dto.Presentation.SubmitActionCriteria>(source);
            else if (source is DomainModel.Presentation.ApprovalActionCriteria) return Mapper.Map<Dto.Presentation.ApprovalActionCriteria>(source);
            else if (source is DomainModel.Presentation.WithdrawActionCriteria) return Mapper.Map<Dto.Presentation.WithdrawActionCriteria>(source);
            return null;
        }
    }

    public class WorkflowCriteriaDomainConverter : TypeConverter<Dto.Presentation.PresentationWorkflowCriteriaBase, DomainModel.Presentation.PresentationWorkflowCriteriaBase>
    {
        protected override DomainModel.Presentation.PresentationWorkflowCriteriaBase ConvertCore(Dto.Presentation.PresentationWorkflowCriteriaBase source)
        {
            if (source is Dto.Presentation.SubmitActionCriteria) return Mapper.Map<DomainModel.Presentation.SubmitActionCriteria>(source);
            else if (source is Dto.Presentation.ApprovalActionCriteria) return Mapper.Map<DomainModel.Presentation.ApprovalActionCriteria>(source);
            else if (source is Dto.Presentation.WithdrawActionCriteria) return Mapper.Map<DomainModel.Presentation.WithdrawActionCriteria>(source);
            return null;
        }

    }
}