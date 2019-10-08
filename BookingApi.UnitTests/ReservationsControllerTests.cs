using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Ploeh.Samples.BookingApi.UnitTests
{
    public class ReservationsControllerTests
    {
        [Fact]
        public void PostInvalidDto()
        {
            var dto = new ReservationDto { };
            var sut = new ReservationsController(
                new Mock<IMapper>().Object,
                new Mock<IReservationsRepository>().Object,
                10);

            var actual = sut.Post(dto);

            var br = Assert.IsAssignableFrom<BadRequestObjectResult>(actual);
            Assert.Equal($"Invalid date: {dto.Date}.", br.Value);
        }

        [Fact]
        public void PostValidDtoWhenNoPriorReservationsExist()
        {
            var dto = new ReservationDto { Date = "2020-01-01 12:00" };
            var r = new Reservation { Date = new DateTime() };
            var mapperTD = new Mock<IMapper>();
            mapperTD.Setup(m => m.Map(dto)).Returns(r);
            var repositoryTD = new Mock<IReservationsRepository>();
            repositoryTD.Setup(repo => repo.ReadReservations(r.Date)).Returns(new List<Reservation>());
            repositoryTD.Setup(repo => repo.Create(r)).Returns(1337);
            var sut = new ReservationsController(
                mapperTD.Object,
                repositoryTD.Object,
                10);

            var actual = sut.Post(dto);

            var ok = Assert.IsAssignableFrom<OkObjectResult>(actual);
            Assert.Equal(1337, ok.Value);
        }

        [Fact]
        public void PostValidDtoWhenSoldOut()
        {

            var dto = new ReservationDto { Date = "2020-01-01 12:00" };

            var r = new Reservation { Date = new DateTime(), Quantity = 2 };
            var mapperTD = new Mock<IMapper>();
            var repositoryTD = new Mock<IReservationsRepository>();
            repositoryTD.Setup(repo => repo.ReadReservations(r.Date)).Returns(new List<Reservation>());
            mapperTD.Setup(m => m.Map(dto)).Returns(r);
            var sut = new ReservationsController(
                mapperTD.Object,
                repositoryTD.Object,
                1);

            var actual = sut.Post(dto);

            var c = Assert.IsAssignableFrom<ObjectResult>(actual);
            Assert.Equal(StatusCodes.Status500InternalServerError, c.StatusCode);
        }
    }
}
