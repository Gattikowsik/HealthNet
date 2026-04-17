using System;
using HealthNetDb.Data;
using HealthNetDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthNet.Utility;

public class ActionHelper
{
    private static readonly HealthNetContext _context = new HealthNetContext();
    // public ActionHelper(HealthNetContext context)
    // {
    //     _context = context;
    // }
    public static async Task<int> GetActionIdByNameAsync(string actionName)
    {
        try
        {
            int actionId = await (from a in _context.Actions where a.ActionName==actionName select a.ActionId).FirstAsync();
            return actionId;
        }
        catch(Exception ex)
        {
            throw new HealthNetException("An Error Occurred While Fetching the Action Id "+ex.Message);
        }
    }
}
