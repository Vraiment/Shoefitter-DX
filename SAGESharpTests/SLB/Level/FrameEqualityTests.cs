/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.Testing;
using System;

using static SAGESharp.SLB.Level.Defaults;

namespace SAGESharp.SLB.Level
{
    class FrameEqualityTests : AbstractEqualityByRefTests<Frame>
    {
        protected override Frame GetDefault() => DefaultFrame();

        protected override bool EqualsOperator(Frame left, Frame right) => left == right;

        protected override bool NotEqualsOperator(Frame left, Frame right) => left != right;

        [TestCaseSource(nameof(FrameModifiers))]
        public void Test_Compare_Default_Object_With_Modified_Object(Action<Frame> frameModifier) =>
            TestCompareDefaultObjectWithModifiedObject(frameModifier);

        static object[] FrameModifiers() => new ParameterGroup<Action<Frame>>()
            .Parameters(frame => frame.ToaAnimation = 0)
            .Parameters(frame => frame.CharAnimation = 0)
            .Parameters(frame => frame.CameraPositionTarget = 0)
            .Parameters(frame => frame.CameraDistance = 0)
            .Parameters(frame => frame.StringIndex = 0)
            .Parameters(frame => frame.ConversationSounds = null)
            .Build();

        [TestCaseSource(nameof(DualFrameModifiers))]
        public void Test_Compare_Modified_Objects(Action<Frame> frameModifierA, Action<Frame> frameModifierB) =>
            TestCompareModifiedObjects(frameModifierA, frameModifierB);

        static object[] DualFrameModifiers() => new ParameterGroup<Action<Frame>, Action<Frame>>()
            .Parameters(frame => frame.ConversationSounds = null, frame => frame.ConversationSounds = "")
            .Build();
    }
}