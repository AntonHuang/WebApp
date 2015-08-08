using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.DomainModels.Member
{
    public class Member : IMember
    {
        public string MemberID { get; set; }
        public string Type { get; set; }
        public DateTime RegisterTime { get; set; }
        public string TransactionPassword { get; set; }


        protected virtual Member Reference { get; set; }
        protected virtual IList<IMember> Candidates { get; set; }

        public IList<IMember> GetCandidates()
        {
           return  Candidates.ToList();
        }

        public IMember GetReference()
        {
            return Reference;
        }
    }

    public interface IMember
    {
        IMember GetReference();
        IList<IMember> GetCandidates();
    }

    public enum Sex {
        Female,
        Male
    }

    public class PersonalMember : Member{
        public Sex? Sex { get; set; }
        public string IDCard { get; set; }
    }


}
