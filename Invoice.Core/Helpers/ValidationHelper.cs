using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

namespace Invoice.Core.Helpers
{
    public static class ValidationHelper
    {
        /// <summary>
        /// يتحقق من أن القيمة رقم موجب صحيح
        /// </summary>
        /// <param name="amount">القيمة المدخلة</param>
        /// <param name="errorMessage">الناتج في حالة الخطأ</param>
        /// <returns>صحيح إذا كانت القيمة صالحة</returns>
        public static bool ValidatePositiveAmount(decimal amount, out string errorMessage)
        {
            if (amount <= 0)
            {
                errorMessage = "القيمة يجب أن تكون رقمًا موجبًا أكبر من الصفر.";
                return false;
            }

            if (decimal.MaxValue < amount)
            {
                errorMessage = "القيمة المدخلة كبيرة جدًا.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}
