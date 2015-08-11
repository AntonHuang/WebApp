using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.DomainModels.Member
{
    public class Member : IMember<Member>
    {
        public Member()
        {
            Candidates = new HashSet<Member>();
        }

        public string MemberID { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public string Address { get; set; }


        public string TransactionPassword { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime LastModifyDate { get; set; }
        public Member RegisterBy { get; set; }

        public Member Reference { get; set; }
        public ICollection<Member> Candidates { get; set; }
    }

    public interface IMember<T>
    {
        T Reference { get; set; }
        ICollection<T> Candidates { get; set; }
    }

    public enum Sex
    {
        Female,
        Male
    }

    public class PersonalMember : Member
    {
        public Sex? Sex { get; set; }
        public string IDCard { get; set; }
    }


}
