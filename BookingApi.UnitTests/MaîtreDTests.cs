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
        public void CanAcceptReturnsReservationInHappyPathScenario()
        {
            var reservation = new Reservation
            {
                Date = new DateTime(2018, 8, 30),
                Quantity = 4
            };
            var sut = new MaîtreD(capacity: 10);

            var actual = sut.CanAccept(new Reservation[0], reservation);

            Assert.True(actual);
        }

        [Fact]
        public void CanAcceptOnInsufficientCapacity()
        {
            var reservation = new Reservation
            {
                Date = new DateTime(2018, 8, 30),
                Quantity = 4
            };
            var sut = new MaîtreD(capacity: 10);

            var actual = sut.CanAccept(
                new[] { new Reservation { Quantity = 7 } },
                reservation);

            Assert.False(actual);
        }
    }
}
