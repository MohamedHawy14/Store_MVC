﻿using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Repository.IReository
{
    public interface ICategory : Irepository<Category> 
    {
        void Update(Category category);
       
    }
}
