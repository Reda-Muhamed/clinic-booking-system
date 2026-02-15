using ClinicBookingSystem.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicBookingSystem.Application.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
