namespace hongWenAPP.Models.ViewStatement.DTOs
{
    public class AccountStatementModel
    {
        public long ID { get; set; }
        public string NameLatin { get; set; }
        public string NameKhmer { get; set; }
        public string Address { get; set; }
        public string AddressKhmer { get; set; } // Add new field for Khmer address to display in the report
        public string Phone { get; set; }
        public string Branch { get; set; }
        public string AccountNo { get; set; }
        public string Currency { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Deposit { get; set; }
        public string Withdraw { get; set; }
        public string Balance { get; set; }
        public string TransRefNo { get; set; }
        public string ActualTxnDate { get; set; }
    }

    // Request model for the service
    public class AccountStatementRequest
    {
        public string AccountNo { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string CompanyName { get; set; }
    }

    // Response model for the service
    public class AccountStatementResponse
    {
        public List<AccountStatementModel> Data { get; set; } = new();
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
