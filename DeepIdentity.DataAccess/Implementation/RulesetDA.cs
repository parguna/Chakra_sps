using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepIdentity.DataAccess.Implementation
{
  public  class RulesetDA
    {
      public List<Ruleset> GetRulesetlist()
      {
          try
          {
              using (IACMDBContext _context = new IACMDBContext())
              {
                  return _context.Rulesets.ToList();
              }
          }
          catch
          {
              throw;
          }
      }
      public Ruleset GetRulesetbyId(int id)
      {
          try
          {
              using (IACMDBContext _context = new IACMDBContext())
              {
                  return _context.Rulesets.FirstOrDefault(item => item.ID==id);
              }
          }
          catch
          {
              throw;
          }
      }
    }
}
