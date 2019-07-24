/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using System;

namespace SAGESharp.IO
{
    class CastBinarySerializerTests
    {
        private readonly IBinaryReader reader;

        private readonly IBinarySerializer<byte> innerSerializer;

        private readonly IBinarySerializer<TestEnum> serializer;

        public CastBinarySerializerTests()
        {
            reader = Substitute.For<IBinaryReader>();
            innerSerializer = Substitute.For<IBinarySerializer<byte>>();
            serializer = new CastBinarySerializer<TestEnum, byte>(innerSerializer);
        }

        [SetUp]
        public void Setup()
        {
            reader.ClearSubstitute();
            innerSerializer.ClearSubstitute();
        }

        [Test]
        public void Test_Reading_An_Enum_With_CastSerializer()
        {
            innerSerializer.Read(reader).Returns((byte)TestEnum.A);

            new CastBinarySerializer<TestEnum, byte>(innerSerializer)
                .Read(reader)
                .Should()
                .Be(TestEnum.A);

            innerSerializer.Received().Read(reader);
        }

        enum TestEnum : byte
        {
            A = 0xAB,
            B = 0xBB
        }

        [Test]
        public void Test_Read_From_A_Null_Reader()
        {
            Action action = () => new CastBinarySerializer<TestEnum, byte>(null);

            action
                .Should()
                .Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("innerSerializer"));
        }

        [Test]
        public void Test_Read_To_A_Null_Object()
        {
            Action action = () => serializer.Read(null);

            action
                .Should()
                .Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("binaryReader"));
        }
    }
}