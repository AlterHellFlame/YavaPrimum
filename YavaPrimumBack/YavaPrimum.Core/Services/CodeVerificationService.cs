using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.Core.Services
{
    public class CodeVerificationService
    {
        private readonly Dictionary<string, string> _verificationCodes = new Dictionary<string, string>();

        public async Task<string> GenerateCode(string email)
        {
            var code = new Random().Next(100000, 999999).ToString();
            _verificationCodes[email] = code;
            return code;
        }

        public async Task<bool> VerifyCode(string email, string code)
        {
            bool isExit =  _verificationCodes.TryGetValue(email, out var storedCode);

            return (isExit && storedCode == code);
        }
    }

}
