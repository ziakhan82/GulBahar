    using GulBahar_Business_Lib.Repository.IRepository;
using GulBahar_DataAcess_Lib;
using GulBahar_DataAcess_Lib.Data;
using GulBahar_Models_Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace GulBahar_Business_Lib.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
		//the CategoryRepository class has two dependencies: ApplicationDbContext and IMapper. These dependencies are injected through the constructor,
        //and the class does not create instances of these dependencies internally.
		private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public CategoryRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<CategoryDTO> Create(CategoryDTO objDTO)
        {
            // Check if _mapper or _db is null
            if (_mapper == null)
            {
                throw new InvalidOperationException("_mapper is null");
            }

            if (_db == null)
            {
                throw new InvalidOperationException("_db is null");
            }

            var obj = _mapper.Map<CategoryDTO, Category>(objDTO);

            // Check if obj is null
            if (obj == null)
            {
                throw new InvalidOperationException("Mapped object (obj) is null");
            }

            // Continue with the rest of the method
            try
            {
                // Instead of using addedObj, directly use the mapped obj
                _db.Categories.Add(obj);

                await _db.SaveChangesAsync();

                // Return the mapped obj after saving changes
                return _mapper.Map<Category, CategoryDTO>(obj);
            }
            catch (Exception ex)
            {
                // Handle exceptions specific to the database interaction
                // For example, catch DbUpdateException and log or rethrow as needed
                throw new InvalidOperationException("Error saving changes to the database", ex);
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            var obj = await _db.Categories.FirstAsync(u => u.Id == id);
            if (obj != null)
            {
                _db.Categories.Remove(obj);
                return await _db.SaveChangesAsync();
            }
            return 0;
        }


        public async Task<CategoryDTO> Get(int id)
        {
			// Check if _mapper or _db is null
			if (_mapper == null)
			{
				throw new InvalidOperationException("_mapper is null");
			}

			if (_db == null)
			{
				throw new InvalidOperationException("_db is null");
			}
			var obj = await _db.Categories.FirstOrDefaultAsync(u => u.Id == id);

            if (obj != null)
            {
               return _mapper.Map<Category, CategoryDTO>(obj);
            }
            return new CategoryDTO(); 
        }

		public CategoryDTO GetById(int id)
		{
			// Check if _mapper or _db is null
			if (_mapper == null)
			{
				throw new InvalidOperationException("_mapper is null");
			}

			if (_db == null)
			{
				throw new InvalidOperationException("_db is null");
			}
			var obj = _db.Categories.FirstOrDefault(u => u.Id == id);

			if (obj != null)
			{
				return _mapper.Map<Category, CategoryDTO>(obj);
			}

			return new CategoryDTO();
		}

		public async Task <IEnumerable<CategoryDTO>> GetAll()
        {
            return _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryDTO>>(_db.Categories);
        }

        public async Task <CategoryDTO> Update(CategoryDTO objDTO)
        {
            var objFromDb = await _db.Categories.FirstOrDefaultAsync(u => u.Id == objDTO.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = objDTO.Name;
                _db.Categories.Update(objFromDb);
                await _db.SaveChangesAsync();
                return _mapper.Map<Category, CategoryDTO>(objFromDb);
            }
            return objDTO;
        }


    }
}
