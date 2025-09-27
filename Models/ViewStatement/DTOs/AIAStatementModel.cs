using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace hongWenAPP.Models.ViewStatement.DTOs
{
    public class AIAStatementModel
    {
        [DisplayName("Account Number")]
        public string? AccountNo { get; set; }

        [DisplayName("Period Date")]
        [Required]
        public string PeriodDate { get; set; } = string.Empty;

        [DisplayName("Transaction Type")]
        [Required]
        public string TransactionType { get; set; } = "Transaction";
    }

    // Dynamic model to handle any API response structure
    public class DynamicAIAData
    {
        public Dictionary<string, object> Properties { get; set; } = new();
        
        // Constructor to create from API response
        public DynamicAIAData(Dictionary<string, object> apiData)
        {
            Properties = apiData ?? new Dictionary<string, object>();
        }

        // Method to get property value safely
        public object GetValue(string propertyName)
        {
            return Properties.ContainsKey(propertyName) ? Properties[propertyName] : null;
        }

        // Method to get string value safely
        public string GetStringValue(string propertyName)
        {
            var value = GetValue(propertyName);
            return value?.ToString() ?? string.Empty;
        }
    }

    public class AIAStatementResponse
    {
        public List<DynamicAIAData> Data { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
} 