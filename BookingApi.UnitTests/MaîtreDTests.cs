﻿using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Ploeh.Samples.BookingApi.UnitTests
{
    public class MaîtreDTests
    {
        [Fact]
        public void TryAcceptReturnsReservationIdInHappyPathScenario()
        {
            var reservation = new Reservation
            {
                Date = new DateTime(2018, 8, 30),
                Quantity = 4
            };
            var td = new Mock<IReservationsRepository>();
            td.Setup(r => r.Create(reservation)).Returns(42);
            var sut = new MaîtreD(capacity: 10, td.Object);

            var actual = sut.TryAccept(new Reservation[0], reservation);

            Assert.Equal(42, actual);
        }

        [Fact]
        public void TryAcceptOnInsufficientCapacity()
        {
            var reservation = new Reservation
            {
                Date = new DateTime(2018, 8, 30),
                Quantity = 4
            };
            var td = new Mock<IReservationsRepository>();
            var sut = new MaîtreD(capacity: 10, td.Object);

            var actual = sut.TryAccept(
                new[] { new Reservation { Quantity = 7 } },
                reservation);

            Assert.Null(actual);
            td.Verify(r => r.Create(It.IsAny<Reservation>()), Times.Never);
        }
    }
}
