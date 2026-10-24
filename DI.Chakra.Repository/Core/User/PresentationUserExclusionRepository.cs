using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;

namespace DI.Chakra.Repository
{
    public class PresentationUserExclusionRepository : RepositoryBase<PresentationUserExclusion>
    {
        public PresentationUserExclusionRepository(DataAccessTransaction transaction) : base(transaction) { }

        public IList<PresentationUserExclusion> GetAllUserExclusion()
        {
            var result = (from usrExclusion in CurrentContext.UserExclusions
                    join usr in CurrentContext.User
                    on usrExclusion.UserId equals usr.Id
                    //where usrExclusion.IsDeleted == false
                    select new PresentationUserExclusion
                    {
                        Id = usrExclusion.Id,
                        UserId = usrExclusion.UserId,
                        EventType = usrExclusion.EventType.ToString(),
                        //AttachmentFile = usrExclusion.AttachmentFile,
                        ContentType = usrExclusion.ContentType,
                        CreatedBy = usrExclusion.CreatedBy,
                        CreatedDate = usrExclusion.CreatedDate,
                        DisplayName = usr.DisplayName,
                        EffectiveDate = usrExclusion.EffectiveDate,
                        FileName = usrExclusion.FileName,
                       // HasAttachments = usrExclusion.AttachmentFile.Count() > 0 ? true : false,
                        LastModifiedBy = usrExclusion.LastModifiedBy,
                        LastModifiedDate = usrExclusion.LastModifiedDate,
                        Remarks = usrExclusion.Remarks,
                        UserName = usr.UserName,
                        IsDeleted = !usrExclusion.IsDeleted
                    }).ToList();
            result.ForEach(x => x.HasAttachments = string.IsNullOrEmpty(x.FileName) ? false : true);
            return result;
        }
    }
}
