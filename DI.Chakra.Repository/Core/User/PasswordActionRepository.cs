using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System.Data;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class PasswordActionRepository : RepositoryBase<PasswordActionInfo>
    {
        public PasswordActionRepository(DataAccessTransaction transaction) : base(transaction) { }

        public PasswordActionInfo GetLastActionByUserId(long userId, PasswordActionType actionType)
        {
            try
            {
                return this.CurrentContext.PasswordActionInfos.OrderByDescending(item => item.Id)
                    .FirstOrDefault(item => item.UserId == userId && item.PasswordActionType == actionType);
            }
            catch
            {
                throw;
            }
        }
    }

}
