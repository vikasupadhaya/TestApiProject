using System;

namespace Entities.DataTransferObjects
{
    public class AccountDto
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string AccountType { get; set; }
    }
}
