using System;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUnitOfWork
    {
        IOwnerRepository Owner { get; }
        IAccountRepository Account { get; }
        Task SaveAsync();
    }
}
