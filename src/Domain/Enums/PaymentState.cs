using System.ComponentModel;

namespace Domain.Enums;

public enum PaymentState
{
    [Description("Pending")]
    Pending = 10,

    [Description("Paid")]
    Paid = 11,

    [Description("Failed")]
    Failed = 12
}
