using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DI.Chakra.Repository
{
    public class AttestationTaskSummaryRepository : RepositoryBase<Task>
    {
        public AttestationTaskSummaryRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public List<PresentationAttestationTaskSummary> GetByEndPointTypeName(string endPointTypeName)
        {
            return (from task in CurrentContext.Tasks
                    join attestation in CurrentContext.Attestations on task.AttestationId equals attestation.Id
                    join attestationEndPoint in CurrentContext.AttestationEndpoints on attestation.Id equals attestationEndPoint.AttestationId
                    join endPointType in CurrentContext.EndpointSystemTypes on attestationEndPoint.EndpointSystemType equals endPointType.EndpointSystemTypeName
                    where endPointType.DisplayName == endPointTypeName
                    select new
                    {
                        IsCompleted = task.IsCompleted,
                        IsDelegated = task.IsDelegated,
                        IsEscalated = task.IsEscalated
                    }).ToList().Select(leTaskSummary => new PresentationAttestationTaskSummary
                    {
                        StatusCount = 1,

                        Status = ((!leTaskSummary.IsCompleted && !leTaskSummary.IsCompleted) ? "Pending" :
                                                            (leTaskSummary.IsCompleted && !leTaskSummary.IsDelegated && !leTaskSummary.IsEscalated) ? "Completed" :
                                                            (leTaskSummary.IsCompleted && leTaskSummary.IsDelegated && !leTaskSummary.IsEscalated) ? "Delegated" :
                                                            (leTaskSummary.IsCompleted && !leTaskSummary.IsDelegated && leTaskSummary.IsEscalated) ? "Escalated" : ""),


                        IsExploded = ((!leTaskSummary.IsCompleted && !leTaskSummary.IsCompleted) ? true :
                                                            (leTaskSummary.IsCompleted && !leTaskSummary.IsDelegated && !leTaskSummary.IsEscalated) ? false :
                                                            (leTaskSummary.IsCompleted && leTaskSummary.IsDelegated && !leTaskSummary.IsEscalated) ? false :
                                                            (leTaskSummary.IsCompleted && !leTaskSummary.IsDelegated && leTaskSummary.IsEscalated) ? false : false),

                        BGColor = ((!leTaskSummary.IsCompleted && !leTaskSummary.IsCompleted) ? "#ffdd00" :
                                                            (leTaskSummary.IsCompleted && !leTaskSummary.IsDelegated && !leTaskSummary.IsEscalated) ? "#279691" :
                                                            (leTaskSummary.IsCompleted && leTaskSummary.IsDelegated && !leTaskSummary.IsEscalated) ? "#59F0BD" :
                                                            (leTaskSummary.IsCompleted && !leTaskSummary.IsDelegated && leTaskSummary.IsEscalated) ? "#F51B31" : ""),


                    }).ToList();

        }

    }
}
