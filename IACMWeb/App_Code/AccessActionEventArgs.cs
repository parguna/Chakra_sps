
using DI.Chakra.Infrastructure.Enums;
namespace DI.Chakra.Web
{
    public class AccessActionEventArgs
    {
        private ACLAction _actionType;

        public AccessActionEventArgs(ACLAction actionType)
        {
            this._actionType = actionType;
        }
        public ACLAction ActionType
        {
            get { return _actionType; }
        }

    }
}