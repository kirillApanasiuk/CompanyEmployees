﻿using Entities.DataTransferObjects.User;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IAuthenticationManager
    {
        Task<bool> ValidateUser(UserForAuthenticationDto userForAuth);
        Task<string> CreateToken();
    }
}
