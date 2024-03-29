﻿using GulBahar_Models_Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GulBahar_Business_Lib.Repository.IRepository
{
    public interface ICategoryRepository
	{
        public Task<CategoryDTO> Create(CategoryDTO objDTO);
        public Task<CategoryDTO> Update(CategoryDTO objDTO);
        public Task<int> DeleteAsync(int id);
        public Task <CategoryDTO> Get(int id);
        public Task<IEnumerable<CategoryDTO>> GetAll();

        public CategoryDTO GetById(int id);
        


    }
}
