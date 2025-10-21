using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Honse.Global.Extensions;

namespace Honse.Tests
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Address Address { get; set; }
        public List<Person> Children { get; set; } = new List<Person>();

        public Person() { }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public Address() { }

        public Address(string street, string city, string country)
        {
            Street = street;
            City = city;
            Country = country;
        }
    }

    // Different class with similar properties
    public class PersonDto
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public AddressDto Address { get; set; }
        public ICollection<PersonDto> Children { get; set; } = new List<PersonDto>();
    }

    public class AddressDto
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }

    public class DeepCopyToTests
    {
        [Fact]
        public void DeepCopyTo_SimpleObject_CopiesAllProperties()
        {
            // Arrange
            var source = new Person
            {
                Name = "John",
                Age = 30,
                Address = new Address { Street = "123 Main St", City = "NYC", Country = "USA" }
            };

            // Act
            var result = source.DeepCopyTo<Person>();

            // Assert
            Assert.NotSame(source, result);
            Assert.Equal(source.Name, result.Name);
            Assert.Equal(source.Age, result.Age);
            Assert.NotSame(source.Address, result.Address);
            Assert.Equal(source.Address.Street, result.Address.Street);
            Assert.Equal(source.Address.City, result.Address.City);
            Assert.Equal(source.Address.Country, result.Address.Country);
        }

        [Fact]
        public void DeepCopyTo_DifferentButCompatibleTypes_CopiesMatchingProperties()
        {
            // Arrange
            var source = new Person
            {
                Name = "Jane",
                Age = 25,
                Address = new Address { Street = "456 Oak Ave", City = "LA", Country = "USA" }
            };

            // Act
            var result = source.DeepCopyTo<PersonDto>();

            // Assert
            Assert.Equal(source.Name, result.Name);
            Assert.Equal(source.Age, result.Age);
            Assert.Equal(source.Address.Street, result.Address.Street);
            Assert.Equal(source.Address.City, result.Address.City);
            Assert.Equal(source.Address.Country, result.Address.Country);
        }

        [Fact]
        public void DeepCopyTo_WithCollections_CopiesAllElements()
        {
            // Arrange
            var source = new Person
            {
                Name = "Parent",
                Children = new List<Person>
            {
                new Person { Name = "Child1", Age = 10 },
                new Person { Name = "Child2", Age = 8 }
            }
            };

            // Act
            var result = source.DeepCopyTo<Person>();

            // Assert
            Assert.NotSame(source.Children, result.Children);
            Assert.Equal(source.Children.Count, result.Children.Count);

            for (int i = 0; i < source.Children.Count; i++)
            {
                Assert.NotSame(source.Children[i], result.Children[i]);
                Assert.Equal(source.Children[i].Name, result.Children[i].Name);
                Assert.Equal(source.Children[i].Age, result.Children[i].Age);
            }
        }

        [Fact]
        public void DeepCopyTo_WithNullProperties_HandlesNullCorrectly()
        {
            // Arrange
            var source = new Person
            {
                Name = "NullPerson",
                Address = null,
                Children = null
            };

            // Act
            var result = source.DeepCopyTo<Person>();

            // Assert
            Assert.Equal(source.Name, result.Name);
            Assert.Null(result.Address);
            Assert.Null(result.Children);
        }

        [Fact]
        public void DeepCopyTo_ValueTypes_CopiesCorrectly()
        {
            // Arrange
            var source = new { Name = "Test", Age = 42, Score = 95.5m };

            // Act
            var result = source.DeepCopyTo<Person>();

            // Assert
            Assert.Equal(source.Name, result.Name);
            Assert.Equal(source.Age, result.Age);
        }

        [Fact]
        public void DeepCopyTo_WithArrays_CopiesArrayContents()
        {
            // Arrange
            var source = new { Tags = new[] { "tag1", "tag2", "tag3" } };

            // Act
            var result = source.DeepCopyTo<TestClassWithArray>();

            // Assert
            Assert.NotSame(source.Tags, result.Tags);
            Assert.Equal(source.Tags, result.Tags);
        }

        [Fact]
        public void DeepCopyTo_CircularReference_ThrowsException()
        {
            // Arrange
            var parent = new Person { Name = "Parent" };
            var child = new Person { Name = "Child" };
            parent.Children.Add(child);
            child.Children.Add(parent); // Circular reference

            // Act & Assert
            Assert.Throws<System.Reflection.TargetInvocationException>(() => parent.DeepCopyTo<Person>());
        }

        [Fact]
        public void DeepCopyTo_Performance_ReusesCompiledDelegates()
        {
            // Arrange
            var source = new Person
            {
                Name = "Test",
                Age = 30,
                Address = new Address { Street = "Test St", City = "Test City" }
            };

            // Act - multiple copies should use cached delegates
            var watch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < 1000; i++)
            {
                var copy = source.DeepCopyTo<Person>();
            }

            watch.Stop();

            // Assert - should complete in reasonable time (adjust threshold as needed)
            Assert.True(watch.ElapsedMilliseconds < 1000, "Deep copy operations took too long");
        }

        [Fact]
        public void DeepCopyTo_WithFields_CopiesFieldValues()
        {
            // Arrange
            var source = new ClassWithFields(12)
            {
                PublicField = "Public",
                InternalField = 42
            };

            // Act
            var result = source.DeepCopyTo<ClassWithFields>();

            // Assert
            Assert.Equal(source.PublicField, result.PublicField);
            Assert.Equal(source.InternalField, result.GetInternalField()); // Use the getter method
            //Assert.Equal(source.GetPrivateField(), result.GetPrivateField()); // Use the getter method
        }
    }

    // Test helper classes
    public class TestClassWithArray
    {
        public string[] Tags { get; set; }
    }

    public class ClassWithFields
    {
        public string PublicField;
        internal int InternalField;
        //private decimal PrivateField;

        public ClassWithFields(decimal field)
        {
            //PrivateField = field;
        }


        // Add a method to verify the internal field was set
        public int GetInternalField() => InternalField;
        //public decimal GetPrivateField() => PrivateField;
    }
}
