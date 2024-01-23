using AutoMapper;
using GulBahar_Business_Lib.Mapper;
using GulBahar_Business_Lib.Repository;
using GulBahar_DataAcess_Lib;
using GulBahar_DataAcess_Lib.Data;
using GulBahar_Models_Lib;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Moq;
namespace GulBaharTests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Create_ShouldCreateCategoryAndReturnDTO()
        {
            // Arrange


            // Configures AutoMapper with a mapping profile. This is necessary for mapping between the CategoryDTO and Category objects.
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));

            // Creates an instance of the AutoMapper Mapper class using the configured mapping profile.
            var mapper = new Mapper(mapperConfig);

            // Create DbContextOptions for the in-memory database. This is used for setting up the mock ApplicationDbContext.
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            // Pass the DbContextOptions as a constructor argument to the mock
            // Creates a mock of the ApplicationDbContext using the in-memory database options.
            var mockDbContext = new Mock<ApplicationDbContext>(dbContextOptions);

            //Creates a mock for the DbSet<Category>. This is used to simulate the behavior of the database set for the Category entity.
            var mockDbSet = new Mock<DbSet<Category>>(); // Creating a mock for DbSet<Category>


            //Sets up the mock Categories property of the ApplicationDbContext to return the mock database set.
            //This is to ensure that when the repository accesses the Categories property, it gets the mock set.
            mockDbContext.Setup(x => x.Categories).Returns(mockDbSet.Object); // Setting up Categories property

            // Creates an instance of the CategoryRepository using the mock ApplicationDbContext and the AutoMapper instance.
            var categoryRepository = new CategoryRepository(mockDbContext.Object, mapper);

            // Act

            // Creates a sample CategoryDTO object.
            var categoryDTO = new CategoryDTO { Id = 1, Name = "TestCategory" };

            // Calls the Create method of the repository, attempting to create a new category.
            var result = await categoryRepository.Create(categoryDTO);

            // Assert
            //Asserts that the result returned from the Create method is not null. This checks whether the method successfully created a category.
            Assert.NotNull(result);

            // Verify that the DbSet's Add method was called with the correct parameter

            //Verifies that the Add method of the mock DbSet<Category> was called exactly once with any Category object.
            //This ensures that the repository attempted to add a category to the database.
            mockDbSet.Verify(x => x.Add(It.IsAny<Category>()), Times.Once);

            //Verifies that the SaveChangesAsync method of the mock ApplicationDbContext was called exactly once.This ensures
            //  that changes were attempted to be saved to the in-memory database.
            mockDbContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }


        [Fact]
        public async Task Delete_ShouldDeleteCategoryAndReturnNumberOfAffectedRowsAsync()
        {
            // Arrange
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            var mapper = new Mapper(mapperConfig);

            // Create DbContextOptions for the in-memory database
            //You create DbContextOptions for an in-memory database. This allows you to simulate database operations
            //without actually hitting a real database. The in-memory database is useful for testing as it's fast and isolated.
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            // Use the actual DbContext with in-memory database
            using (var dbContext = new ApplicationDbContext(dbContextOptions))
            {
                // Seed the in-memory database with the appropriate data
                dbContext.Categories.Add(new Category { Id = 1, Name = "TestCategory" });
                dbContext.SaveChanges();
            }

            // Create a new instance of DbContext for the repository
            using (var dbContext = new ApplicationDbContext(dbContextOptions))
            {
                //create an instance of the CategoryRepository using the in-memory database context and the AutoMapper
                //instance. This is the repository you will test.
                var categoryRepository = new CategoryRepository(dbContext, mapper);

                // Act

                //call the DeleteAsync method on the repository, simulating a delete operation.
                var result = await categoryRepository.DeleteAsync(1);

                // Assert
                //You assert that the result of the delete operation is as expected.
                //In this case, you expect that one row has been affected (deleted).
                Assert.Equal(1, result); // Verify that the result is 1 (number of affected rows)


            }
        }

      
        [Fact]
        public void Get_ShouldReturnCategoryDTO_IfExists()
        {
            // Arrange

            //This section sets up the necessary dependencies for the test. It configures and creates an instance of the AutoMapper Mapper class,
            //which will be used later for mapping entities to DTOs.
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            var mapper = new Mapper(mapperConfig);


            //A list of Category entities is created and converted to an IQueryable. This will serve as the data source for the mocked DbSet
            var categories = new List<Category>
    {
                //This IQueryable<Category> is then used to set up the behavior of the DbSet<Category> in the mock using Moq
        new Category { Id = 1, Name = "TestCategory" }
    }.AsQueryable();

            //A mock of the DbSet<Category> is created. This will be used to simulate the behavior of the Entity Framework DbSet.
            var mockDbSet = new Mock<DbSet<Category>>();

            //These lines set up the necessary properties and methods of the mocked DbSet to mimic the behavior of an IQueryable.
            //It ensures that when methods like FirstOrDefaultAsync are called on this mock, it will use the data from the categories list.
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categories.Provider);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.Expression);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categories.ElementType);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categories.GetEnumerator());

            //A mock of the ApplicationDbContext is created, and its Categories property is set to the previously created mock DbSet.
            //This sets up the context with the mocked data.
            var mockDbContext = new Mock<ApplicationDbContext>();
            mockDbContext.Setup(x => x.Categories).Returns(mockDbSet.Object);

            var categoryRepository = new CategoryRepository(mockDbContext.Object, mapper);

            // Act

            //An instance of the CategoryRepository is created, passing the mocked DbContext and Mapper instances.
            //Then, the Get method of the repository is called with an ID.
            var result = categoryRepository.Get(1);

            // Assert
            Assert.NotNull(result);
            //The line Assert.Equal(1, result.Id) checks whether the Id property of the result matches the expected value of 1
            Assert.Equal(1, result.Id);


            // Additional assertions if needed
        }


        // if the doesnt exist 
        [Fact]
        public void Get_ShouldReturnEmptyCategoryDTO_IfNotExistssss()
        {
            // Arrange
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            var mapper = new Mapper(mapperConfig);

            var categories = new List<Category>().AsQueryable();

            var mockDbSet = new Mock<DbSet<Category>>();
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categories.Provider);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.Expression);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categories.ElementType);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categories.GetEnumerator());

            var mockDbContext = new Mock<ApplicationDbContext>();
            mockDbContext.Setup(x => x.Categories).Returns(mockDbSet.Object);

            var categoryRepository = new CategoryRepository(mockDbContext.Object, mapper);

            // Act
            var result = categoryRepository.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Id);
            // Additional assertions if needed
        }

        [Fact]
        public async Task GetAll_ShouldReturnCategoryDTOs()
        {
            // Arrange

            //These lines set up the AutoMapper configuration and create an instance of the mapper. AutoMapper is used to map between Category and CategoryDTO.
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            var mapper = new Mapper(mapperConfig);


            //This creates a sample list of Category objects. The list is then converted to an IQueryable to simulate a database queryable result.
            var categories = new List<Category>
    {
        new Category { Id = 1, Name = "TestCategory1" },
        new Category { Id = 2, Name = "TestCategory2" },
        new Category { Id = 3, Name = "TestCategory3" }
    }.AsQueryable();
            //sets up a mock DbSet<Category> using Moq. The As<IQueryable<Category>>() allows us to mock the IQueryable interface.
            //The subsequent Setup calls configure the behavior of the IQueryable properties and methods.
            var mockDbSet = new Mock<DbSet<Category>>();
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categories.Provider);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.Expression);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categories.ElementType);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categories.GetEnumerator());

            //sets up a mock ApplicationDbContext using Moq and configures it to return the mock DbSet when the Categories property is accessed.
            var mockDbContext = new Mock<ApplicationDbContext>();
            mockDbContext.Setup(x => x.Categories).Returns(mockDbSet.Object);

            //creates an instance of the CategoryRepository using the mocked database context and mapper.
            var categoryRepository = new CategoryRepository(mockDbContext.Object, mapper);

            // Act
            var result = await categoryRepository.GetAll();

            // Assert

            //creates an instance of the CategoryRepository using the mocked database context and mapper.
            Assert.NotNull(result);
            Assert.Equal(3, result.Count()); // Assuming there are three categories in the mocked data
                                             // Additional assertions if needed
        }

        [Fact]
        public async Task Update_ShouldUpdateCategory_IfExists()
        {
            // Arrange
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            var mapper = new Mapper(mapperConfig);


            // create a list of categories and convert it to an IQueryable<Category>. This will be used to set up the mock DbSet<Category>.
            var categories = new List<Category>
    {
        new Category { Id = 1, Name = "TestCategory" }
    }.AsQueryable();

            // create a mock DbSet<Category> using Moq. I set up various properties (Provider, Expression, ElementType) to match the properties
            // of the IQueryable<Category> created earlier.

            var mockDbSet = new Mock<DbSet<Category>>();
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categories.Provider);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.Expression);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categories.ElementType);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categories.GetEnumerator());

            //create options for an in-memory database. This will be used to set up the ApplicationDbContext.
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            //create an instance of the ApplicationDbContext using the in-memory database options. add a sample category to the database and save changes.
            var dbContext = new ApplicationDbContext(options);
            dbContext.Categories.Add(new Category { Id = 1, Name = "TestCategory" });
            dbContext.SaveChanges();

            // create an instance of the CategoryRepository using the mock ApplicationDbContext and the mapper.
            var categoryRepository = new CategoryRepository(dbContext, mapper);

            // Act

            // create a CategoryDTO to represent the updated data. We call the Update method of the repository, passing the CategoryDTO as an argument.
            var categoryDTOToUpdate = new CategoryDTO { Id = 1, Name = "UpdatedCategory" };
            var result = await categoryRepository.Update(categoryDTOToUpdate);

            // Assert

            // create a CategoryDTO to represent the updated data. We call the Update method of the repository, passing the CategoryDTO as an argument.
            Assert.NotNull(result);
            Assert.Equal("UpdatedCategory", result.Name);

            // Additional assertions if needed
        }

		[Fact]
		public async Task Get_ShouldReturnCategoryDTO_IfExistsInMemoryDataBaseP()
		{
			// Arrange
			var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
			var mapper = new Mapper(mapperConfig);

			// Create DbContextOptions for the in-memory database
			var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "InMemoryDatabase")
				.Options;

			// Use the actual DbContext with in-memory database
			//This block creates a new instance of the ApplicationDbContext using the previously defined options.
			//It adds a category with Id = 1 and Name = "TestCategory" to the in-memory database and saves changes.
			using (var dbContext = new ApplicationDbContext(dbContextOptions))
			{
				// Seed the in-memory database with the appropriate data
				dbContext.Categories.Add(new Category { Id = 1, Name = "TestCategory" });
				dbContext.SaveChanges();
			}

			// Create a new instance of DbContext for the repository
			//This block creates a new instance of the CategoryRepository using the in-memory DbContext and the mapper.
			using (var dbContext = new ApplicationDbContext(dbContextOptions))
			{
				var categoryRepository = new CategoryRepository(dbContext, mapper);

				// Act

				//It performs the action(Get(1)) and gets the result.
				var result = await categoryRepository.Get(1);

				// Assert
				//It uses Assert statements to verify that the result is not null and that the mapping of the result is successful.
				Assert.NotNull(result); // Verify that the result is not null

				Assert.Equal("TestCategory", result.Name); // Verify that the mapping was successful

				// Additional assertions if needed
			}
		}

		[Fact]
        public void GetById_ShouldReturnCategoryDTO_IfExists()
        {
            // Arrange
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            var mapper = new Mapper(mapperConfig);

            var categories = new List<Category>
    {
        new Category { Id = 1, Name = "TestCategory" }
    }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Category>>();
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categories.Provider);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.Expression);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categories.ElementType);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categories.GetEnumerator());

            var mockDbContext = new Mock<ApplicationDbContext>();
            mockDbContext.Setup(x => x.Categories).Returns(mockDbSet.Object);

            var categoryRepository = new CategoryRepository(mockDbContext.Object, mapper);

            // Act
            var result = categoryRepository.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestCategory", result.Name);
            // Additional assertions if needed
        }

		[Fact]
		public async Task Get_ShouldReturnEmptyCategoryDTO_IfNotExistss()
		{
			// Arrange
			var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
			var mapper = new Mapper(mapperConfig);

			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "InMemoryDatabase")
				.Options;

			using (var dbContext = new ApplicationDbContext(options))
			{
				var categoryRepository = new CategoryRepository(dbContext, mapper);

				// Act
				var result = await categoryRepository.Get(2);

				// Assert
				Assert.NotNull(result);
				Assert.Equal(0, result.Id);

				// Additional assertions if needed
			}


		}

		//[Fact]
  //      public async Task Get_ShouldReturnCategoryDTO_IfExistsUsingMoqF()
  //      {
  //          // Arrange
  //          var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
  //          var mapper = new Mapper(mapperConfig);

  //          var categories = new List<Category>
  //      {
  //          new Category { Id = 1, Name = "TestCategory" }
  //      }.AsQueryable();

  //          var mockDbSet = new Mock<DbSet<Category>>();
  //          mockDbSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categories.Provider);
  //          mockDbSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.Expression);
  //          mockDbSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categories.ElementType);
  //          mockDbSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categories.GetEnumerator());

  //          var mockDbContext = new Mock<ApplicationDbContext>();
  //          mockDbContext.Setup(x => x.Categories).Returns(mockDbSet.Object);

  //          //mockDbContext.Setup(x => x.Categories).Returns(mockDbSet.Object);

  //          var mockMapper = new Mock<IMapper>();
  //          mockMapper.Setup(m => m.Map<Category, CategoryDTO>(It.IsAny<Category>()))
  //              .Returns((Category source) => new CategoryDTO { Id = source.Id, Name = source.Name });

  //          var categoryRepository = new CategoryRepository(mockDbContext.Object, mockMapper.Object);

  //          // Act
  //          var result = await categoryRepository.Get(1);

  //          // Assert
  //          Assert.NotNull(result); // Verify that the result is not null
  //          /*   Assert.Equal("TestCategory", result.Name);*/ // Verify that the mapping was successful
  //          Assert.Equal(1, result.Id);
  //          // Additional assertions if needed
  //      }

		// if the id doesnt exist
		//[Fact]
		//public void Get_ShouldReturnEmptyCategoryDTO_IfNotExists()
		//{
		//	// Arrange
		//	var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
		//	var mapper = new Mapper(mapperConfig);

		//	var mockDbSet = new Mock<DbSet<Category>>();
		//	mockDbSet.Setup(x => x.FindAsync(It.IsAny<object[]>()))
		//		.Returns((object[] ids) => Task.FromResult<Category>(null));

		//	var mockDbContext = new Mock<ApplicationDbContext>();
		//	mockDbContext.Setup(x => x.Categories).Returns(mockDbSet.Object);

		//	var categoryRepository = new CategoryRepository(mockDbContext.Object, mapper);

		//	// Act
		//	var result = categoryRepository.Get(2);

		//	// Assert
		//	Assert.NotNull(result);
		//	Assert.Equal(0, result.Id);

		//	// Additional assertions if needed
		//}

	}
	}