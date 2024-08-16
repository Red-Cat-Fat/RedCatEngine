using NUnit.Framework;
using RedCatEngine.Conditions.Base;
using RedCatEngine.Conditions.Variants;
using RedCatEngine.Conditions.Variants.Logic;
using RedCatEngine.DependencyInjection.Containers;

namespace RedCatEngine.Conditions.Tests
{
    public class ConditionTests
    {
        [Test]
        public void CheckForceCondition()
        {
            var applicationContainer = new ApplicationContainer();
            Assert.IsTrue(ForceCondition.True.Check(applicationContainer));
            Assert.IsFalse(ForceCondition.False.Check(applicationContainer));
        }
        [Test]
        public void CheckNotCondition()
        {
            var applicationContainer = new ApplicationContainer();
            var notCondition = new NotCondition { Condition = ForceCondition.True };
            Assert.IsFalse(notCondition.Check(applicationContainer));
            notCondition.Condition = ForceCondition.False;
            Assert.IsTrue(notCondition.Check(applicationContainer));
        }
        
        [Test]
        public void CheckAndCondition()
        {
            bool CheckAnd(params ICondition[] args)
            {
                var applicationContainer = new ApplicationContainer();
                var andCondition = new AndCondition()
                {
                    Conditions = args
                };
                return andCondition.Check(applicationContainer);
            }
            
            Assert.IsTrue(CheckAnd(ForceCondition.True, ForceCondition.True));
            Assert.IsFalse(CheckAnd(ForceCondition.False, ForceCondition.True));
            Assert.IsFalse(CheckAnd(ForceCondition.True, ForceCondition.False));
            Assert.IsFalse(CheckAnd(ForceCondition.False, ForceCondition.False));
            Assert.IsFalse(CheckAnd(ForceCondition.False, ForceCondition.False, ForceCondition.True));
            Assert.IsTrue(CheckAnd(ForceCondition.True, ForceCondition.True, ForceCondition.True));
        }

        [Test]
        public void CheckOrCondition()
        {
            bool CheckOr(params ICondition[] args)
            {
                var applicationContainer = new ApplicationContainer();
                var andCondition = new OrCondition()
                {
                    Conditions = args
                };
                return andCondition.Check(applicationContainer);
            }
            
            Assert.IsTrue(CheckOr(ForceCondition.True, ForceCondition.True));
            Assert.IsTrue(CheckOr(ForceCondition.False, ForceCondition.True));
            Assert.IsTrue(CheckOr(ForceCondition.True, ForceCondition.False));
            Assert.IsFalse(CheckOr(ForceCondition.False, ForceCondition.False));
            Assert.IsTrue(CheckOr(ForceCondition.False, ForceCondition.False, ForceCondition.True));
            Assert.IsTrue(CheckOr(ForceCondition.True, ForceCondition.True, ForceCondition.True));
            Assert.IsFalse(CheckOr(ForceCondition.False, ForceCondition.False, ForceCondition.False));
        }
    }
}
