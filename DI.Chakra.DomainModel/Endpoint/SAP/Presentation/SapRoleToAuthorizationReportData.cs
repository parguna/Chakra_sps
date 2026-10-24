namespace DI.Chakra.DomainModel.Presentation
{
    public class SapRoleToAuthorizationReportData
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string ObjectCode { get; set; }
        public string AuthCode { get; set; }
        public string FieldCode { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set; }
    }
}
