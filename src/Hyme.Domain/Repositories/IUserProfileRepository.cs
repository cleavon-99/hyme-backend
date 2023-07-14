﻿using Hyme.Domain.Entities;
using Hyme.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyme.Domain.Repositories
{
    public interface IUserProfileRepository
    {
        Task<User?> GetByIdAsync(UserId id);
        Task<User?> GetByWalletAddress(WalletAddress walletAddress);
        Task AddAsync(User userProfile);
    }
}
