using System;

namespace Transactions.API.Application.DTOs;

public class PeriodFilterDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
