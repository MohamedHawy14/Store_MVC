﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Repository.IReository
{
    public interface IUnitOfWork
    {
        ICategory category { get;  }
        void Save();

    }
}
