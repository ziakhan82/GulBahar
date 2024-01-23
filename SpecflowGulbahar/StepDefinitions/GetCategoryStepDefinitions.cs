using AutoMapper;
using GulBahar_Business_Lib.Mapper;
using GulBahar_Business_Lib.Repository;
using GulBahar_Business_Lib.Repository.IRepository;
using GulBahar_DataAcess_Lib;
using GulBahar_DataAcess_Lib.Data;
using GulBahar_Models_Lib;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace SpecflowGulbahar.StepDefinitions
{
    [Binding]
    public class GetCategoryStepDefinitions
    {
        private readonly Mock<ApplicationDbContext> _mockDbContext = new Mock<ApplicationDbContext>();
        private readonly IMapper _mapper;
		private readonly CategoryRepository _categoryRepository;
		private int _categoryId;
        private CategoryDTO _result;

        public GetCategoryStepDefinitions()
        {
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            _mapper = mapperConfig.CreateMapper();
			_categoryRepository = new CategoryRepository(_mockDbContext.Object, _mapper);
		}

        [Given(@"the database contains a category with ID (.*)")]
        public void GivenTheDatabaseContainsACategoryWithID(int categoryId)
        {
            // Set up the mock DbContext to use an in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var dbContext = new ApplicationDbContext(options);
            var category = new Category { Id = 1, Name = "Test Category" };
            dbContext.Categories.Add(category);
            dbContext.SaveChanges();
            _categoryId = category.Id;
            _mockDbContext.Setup(x => x.Categories).Returns(dbContext.Categories);
        }
        [Given(@"the database does not contain a category with ID (.*)")]
        public void GivenTheDatabaseDoesNotContainACategoryWithID(int categoryId)
        {
            // Set up the mock DbContext to return an empty DbSet<Category>
            var categories = new List<Category>(); // Your list of categories
            var mockDbSet = new Mock<DbSet<Category>>();
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categories.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categories.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(() => categories.AsQueryable().GetEnumerator());

            _mockDbContext.Setup(x => x.Categories).Returns(mockDbSet.Object);
        }

        [When(@"I retrieve the category by ID (.*)")]
        public void WhenIRetrieveTheCategoryByID(int categoryId)
        {
            _categoryId = categoryId;
            var categoryRepository = new CategoryRepository(_mockDbContext.Object, _mapper);
            _result =  categoryRepository.GetById(categoryId);
        }

        [Then(@"the result should be a CategoryDTO with ID (.*)")]
        public void ThenTheResultShouldBeACategoryDTOWithID(int expectedId)
        {
            Assert.NotNull(_result);
            Assert.Equal(expectedId, _result.Id);
        }

        [Then(@"the result should be an empty CategoryDTO")]
        public void ThenTheResultShouldBeAnEmptyCategoryDTO()
        {
            Assert.NotNull(_result);
            Assert.Equal(0, _result.Id); // Assuming you set Id to 0 for an empty CategoryDTO
        }

        //[AfterScenario]
        //public void AfterScenario()
        //{
        //    if (_mockDbContext != null)
        //    {
        //        // Dispose or clean up the in-memory database
        //        _mockDbContext.Object.Database.EnsureDeleted();
        //        _mockDbContext.Object.Dispose();
        //    }
        }
		// ... other step definitions ...
	

}