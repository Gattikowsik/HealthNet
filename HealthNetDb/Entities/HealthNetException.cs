using System;

namespace HealthNetDb.Entities;

public class HealthNetException : Exception
{
    public HealthNetException(string errorMsg) : base(errorMsg)
    {
        
    }
}
