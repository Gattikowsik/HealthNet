using System;

namespace HealthNet.DTOs.ComplianceRecordDto;

public class ComplianceRecordFilterDto
{
    // filter by specific entity
    public int? EntityId { get; set; }    
    // filter by case/test/outbreak
    public string? Type { get; set; }    
    // filter by compliant/non compliant etc 
    public string? Result { get; set; }   
     // filter by specific date
    public DateTime? Date { get; set; }  
}
