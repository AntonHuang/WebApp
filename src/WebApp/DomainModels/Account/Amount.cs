using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.DomainModels.Account
{
    public class Amount
    {
        public string Type
        {
            get;
        }

        public decimal ValueOfNumber
        {
            get;
        }

        public Amount(string type)
        {
            Type = type;
        }

        public Amount(string type, decimal valueOfNumb)
        {
            Type = type;
            ValueOfNumber = valueOfNumb;
        }

        public static Amount operator -(Amount old)
        {
            return new Amount(old.Type, -old.ValueOfNumber);
        }

        public static Amount operator +(Amount left, Amount right)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            if (left.Type.Equals(right.Type) == false)
            {
                throw new ArgumentException("Must be same Amount Type.");
            }

            return new Amount(left.Type, left.ValueOfNumber + right.ValueOfNumber);
        }

        public static Amount operator -(Amount left, Amount right)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            if (left.Type.Equals(right.Type) == false)
            {
                throw new ArgumentException("Must be same Amount Type.");
            }

            return new Amount(left.Type, left.ValueOfNumber - right.ValueOfNumber);
        }
    }
}
