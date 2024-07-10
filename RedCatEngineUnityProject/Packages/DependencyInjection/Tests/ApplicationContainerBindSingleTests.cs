using NUnit.Framework;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Tests.SpecialSubClasses;

namespace RedCatEngine.DependencyInjection.Tests
{
    public class ApplicationContainerBindSingleTests
    {
	    private IApplicationContainer _applicationContainer;

	    [SetUp]
	    public void SetUp()
	    {
		    _applicationContainer = new ApplicationContainer();
	    }

        [Test]
        public void GivenApplicationContainer_WhenBindOnce_ThenAllCorrect()
        {
	        _applicationContainer.BindAsSingle(new SimpleDemoParentClass());
	        Assert.IsTrue(_applicationContainer.TryGetSingle<SimpleDemoParentClass>(out _), "Not found instance");
        }

        [Test]
        public void GivenApplicationContainer_WhenBindOnce_ThenGetCorrectInstance()
        {
	        const int checkValue = 42;
	        _applicationContainer.BindAsSingle(new SimpleDemoDataParentClass(checkValue));
	        var instance = _applicationContainer.GetSingle<SimpleDemoDataParentClass>();
	        Assert.AreEqual(instance.Value, checkValue, "Not correct instance");
        }

		[Test]
		public void GivenApplicationContainer_WhenBindChild_ThenCanGetByParentType()
		{
			_applicationContainer.BindAsSingle(new SimpleDemoChildFirstClass());
			Assert.IsTrue(_applicationContainer.TryGetSingle<SimpleDemoParentClass>(out _), "Not found instance");
		}
    }
}
