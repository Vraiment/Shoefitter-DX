﻿using NUnit.Framework;
using SAGESharp.SLB;
using SAGESharpTests.Util;
using System;

namespace SAGESharpTests.SLB
{
    class IdentifierEqualityTests : AbstractEqualityByValTests<Identifier>
    {
        protected override bool EqualsOperator(Identifier left, Identifier right) => left == right;

        protected override bool NotEqualsOperator(Identifier left, Identifier right) => left != right;

        [TestCaseSource(nameof(Modifiers))]
        public void Test_Compare_Default_Object_With_Modified_Object(Identifier value) =>
            TestCompareNonEqualsDefaultValueAndNonDefaultValue(value);

        static object[] Modifiers() => new ParameterGroup<Identifier>()
            .Parameters(new Identifier { B0 = 0x01 })
            .Parameters(new Identifier { B1 = 0x01 })
            .Parameters(new Identifier { B2 = 0x01 })
            .Parameters(new Identifier { B3 = 0x01 })
            .Parameters(new Identifier { C0 = 'A' })
            .Parameters(new Identifier { C1 = 'B' })
            .Parameters(new Identifier { C2 = 'C' })
            .Parameters(new Identifier { C3 = 'D' })
            .Parameters(new Identifier(1))
            .Parameters(new Identifier(new byte[] { 0x01, 0x02, 0x03, 0x04 }))
            .Parameters(new Identifier("DCBA"))
            .Build();

        [TestCaseSource(nameof(DualModifiers))]
        public void Test_Compare_Modified_Objects(Identifier a, Identifier b) =>
            TestCompareNonEqualsNonDefaultValues(a, b);

        static object[] DualModifiers() => new ParameterGroup<Identifier, Identifier>()
            .Parameters(new Identifier(0x11223344), new Identifier(0x11121314))
            .Build();
    }
}
