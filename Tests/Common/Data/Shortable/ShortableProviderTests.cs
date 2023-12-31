/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuantConnect.Configuration;
using QuantConnect.Data.Shortable;

namespace QuantConnect.Tests.Common.Data.Shortable
{
    [TestFixture]
    public class ShortableProviderTests
    {
        [SetUp]
        public void SetupConfig()
        {
            Config.Set("data-folder", "TestData");
            Globals.Reset();
        }

        [TearDown]
        public void ResetConfig()
        {
            Config.Reset();
            Globals.Reset();
        }

        [Test]
        public void LocalDiskShortableProviderGetsDataBySymbol()
        {
            var shortableProvider = new LocalDiskShortableProvider("testbrokerage");
            var symbols = new[]
            {
                new Symbol(SecurityIdentifier.GenerateEquity("AAPL", QuantConnect.Market.USA, mappingResolveDate: new DateTime(2021, 1, 4)), "AAPL"),
                new Symbol(SecurityIdentifier.GenerateEquity("GOOG", QuantConnect.Market.USA, mappingResolveDate: new DateTime(2021, 1, 4)), "GOOG"),
                new Symbol(SecurityIdentifier.GenerateEquity("BAC", QuantConnect.Market.USA, mappingResolveDate: new DateTime(2021, 1, 4)), "BAC")
            };
            var results = new[]
            {
                new Dictionary<Symbol, long?>
                {
                    { symbols[0], 2000 },
                    { symbols[1], 5000 },
                    { symbols[2], null } // we have no data for this symbol
                },
                new Dictionary<Symbol, long?>
                {
                    { symbols[0], 4000 },
                    { symbols[1], 10000 },
                    { symbols[2], null } // we have no data for this symbol
                }
            };

            var dates = new[]
            {
                new DateTime(2020, 12, 21),
                new DateTime(2020, 12, 22)
            };

            foreach (var symbol in symbols)
            {
                for (var i = 0; i < dates.Length; i++)
                {
                    var date = dates[i];
                    var shortableQuantity = shortableProvider.ShortableQuantity(symbol, date);

                    Assert.AreEqual(results[i][symbol], shortableQuantity);
                }
            }
        }

        [TestCase("AAPL", "nobrokerage")]
        [TestCase("SPY", "testbrokerage")]
        public void LocalDiskShortableProviderDefaultsToNullForMissingData(string ticker, string brokerage)
        {
            var provider = new LocalDiskShortableProvider(brokerage);
            var date = new DateTime(2020, 12, 21);
            var symbol = new Symbol(SecurityIdentifier.GenerateEquity(ticker, QuantConnect.Market.USA, mappingResolveDate: date), ticker);

            Assert.IsFalse(provider.ShortableQuantity(symbol, date).HasValue);
        }
    }
}
