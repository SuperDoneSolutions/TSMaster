using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TotalSquashNext.Controllers;
using TotalSquashNext.Models;

namespace TotalSquashNext.Tests.Controllers
{
    [TestClass]
    class UserTest
    {
        [TestClass]
    public class UserTests
    {
        #region methods to create a sample user object and delete them from the db
        PrimarySquashDBContext db = new PrimarySquashDBContext();

        private User GenericUser()
        {
            User user = new User();
            user.username = "jSmith";
            user.skillId = 1;
            user.password = "jimsmith12345";
            user.firstName = "Jim";
            user.lastName = "Smith";
            user.streetAddress = "55 William Street East";
            user.city = "Waterloo";
            user.provinceId = "ON";
            user.countryId = 1;
            user.phoneNumber = "519-998-4868";
            user.emailAddress = "jim@gmail.com";
            user.gender = "M";
            user.birthDate = DateTime.Today.AddDays(-1000);
            user.accountId = 1;
            user.organizationId = 1;

            return user;

        }
        private void PurgeUser(UsersController UsersController, string userName)
        {
            try
            {
                if (UsersController != null) UsersController.Dispose();

            }
            catch (Exception ex) { }
            PurgeUser(userName);

        }

        private void PurgeFarm(string userName)
        {
            try
            {
                var users = db.Users.Where(a => a.username == userName);
                foreach (User user in users)
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            { }     // ignore error            
        }// ******************************************************************************
        #endregion methods to create sample user object and delete them from database
        [TestMethod]
        public void CleanUserShouldCreate()
        {
            User user = GenericUser();
            UsersController usersController = new UsersController();

            ActionResult result = usersController.Create(user);

            Assert.IsTrue(result.GetType().Name != "ViewResult");
            List<User> newUser = db.Users.Where(a => a.username == user.username).ToList();
            Assert.IsTrue(newUser.Count != 0);
        }

        [TestMethod]
        public void NullUsernameShouldErrorOut()
        {
            User user = GenericUser();
            user.username = null;
            UsersController usersController = new UsersController();

            //Act
            ActionResult result = usersController.Create(user);

            //Assert
            Assert.IsTrue(result.GetType().Name == "ViewResult");
        }

        [TestMethod]
        public void NullSkillIdShouldErrorOut()
        {
            User user = GenericUser();
            user.skillId = 0;
            UsersController usersController = new UsersController();

            //Act
            ActionResult result = usersController.Create(user);

            //Assert
            Assert.IsTrue(result.GetType().Name == "ViewResult");
        }

        [TestMethod]
        public void NullPasswordShouldErrorOut()
        {
            User user = GenericUser();
            user.password = null;
            UsersController usersController = new UsersController();

            //Act
            ActionResult result = usersController.Create(user);

            //Assert
            Assert.IsTrue(result.GetType().Name == "ViewResult");
        }

        [TestMethod]
        public void NullFirstNameShouldErrorOut()
        {
            User user = GenericUser();
            user.firstName = null;
            UsersController usersController = new UsersController();

            //Act
            ActionResult result = usersController.Create(user);

            //Assert
            Assert.IsTrue(result.GetType().Name == "ViewResult");
        }

        [TestMethod]
        public void NullLastNameShouldErrorOut()
        {
            User user = GenericUser();
            user.lastName = null;
            UsersController usersController = new UsersController();

            //Act
            ActionResult result = usersController.Create(user);

            //Assert
            Assert.IsTrue(result.GetType().Name == "ViewResult");
        }

        [TestMethod]
        public void NullStreetAddressShouldErrorOut()
        {
            User user = GenericUser();
            user.streetAddress = null;
            UsersController usersController = new UsersController();

            //Act
            ActionResult result = usersController.Create(user);

            //Assert
            Assert.IsTrue(result.GetType().Name == "ViewResult");
        }

        [TestMethod]
        public void NullCityShouldErrorOut()
        {
            User user = GenericUser();
            user.city = null;
            UsersController usersController = new UsersController();

            //Act
            ActionResult result = usersController.Create(user);

            //Assert
            Assert.IsTrue(result.GetType().Name == "ViewResult");
        }

        [TestMethod]
        public void NullProvinceIdShouldErrorOut()
        {
            User user = GenericUser();
            user.provinceId = null;
            UsersController usersController = new UsersController();

            //Act
            ActionResult result = usersController.Create(user);

            //Assert
            Assert.IsTrue(result.GetType().Name == "ViewResult");
        }



        [TestMethod]
        public void NullCountryIdShouldErrorOut()
        {
            User user = GenericUser();
            user.countryId = null;
            UsersController usersController = new UsersController();

            //Act
            ActionResult result = usersController.Create(user);

            //Assert
            Assert.IsTrue(result.GetType().Name == "ViewResult");
        }

        [TestMethod]
        public void NullEmailShouldErrorOut()
        {
            User user = GenericUser();
            user.emailAddress = null;
            UsersController usersController = new UsersController();

            //Act
            ActionResult result = usersController.Create(user);

            //Assert
            Assert.IsTrue(result.GetType().Name == "ViewResult");
        }

        [TestMethod]
        public void NullGenderShouldErrorOut()
        {
            User user = GenericUser();
            user.gender = null;
            UsersController usersController = new UsersController();

            //Act
            ActionResult result = usersController.Create(user);

            //Assert
            Assert.IsTrue(result.GetType().Name == "ViewResult");
        }
        [TestMethod]
        public void NullBirthDateShouldErrorOut()
        {
            User user = GenericUser();
            user.birthDate = null;
            UsersController usersController = new UsersController();

            //Act
            ActionResult result = usersController.Create(user);

            //Assert
            Assert.IsTrue(result.GetType().Name == "ViewResult");
        }

        [TestMethod]
        public void Phone10DigitsAccepted()
        {
            //Arrange
            User user = GenericUser();
            user.phoneNumber = "( 123 ) - 456 -- 7890";
            UsersController usersController = new UsersController();

            //Act
            ActionResult result = usersController.Create(user);

            //Assert
            Assert.IsTrue(result.GetType().Name != "ViewResult");
        }

        [TestMethod]
        public void PhoneCorrectlyFormatted()
        {
            //Arrange
            User user = GenericUser();
            user.phoneNumber = "1234567890";
            UsersController usersController = new UsersController();

            //Act
            ActionResult result = usersController.Create(user);

            //Assert
            List<User> newUser = db.Users.Where(a => a.username == user.username).ToList();
            Assert.IsTrue(newUser.Count > 0 && newUser[0].phoneNumber == "123-456-7890");
        }



    }
}
    }

