
namespace DI.Chakra.Web
{
    public class CommentAddedEventArgs
    {
        private long _keyId;
        private string _commandName;
        private string _comments;

        public CommentAddedEventArgs(long keyId, string commandName, string comments)
        {
            this._keyId = keyId;
            this._commandName = commandName;
            this._comments = comments;
        }

        public long KeyId
        {
            get { return _keyId; }
        }

        public string CommandName
        {
            get { return _commandName; }
        }

        public string Comments
        {
            get { return _comments; }
        }

    }
}