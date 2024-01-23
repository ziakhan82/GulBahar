namespace SpecflowGulbahar.StepDefinitions
{
    [Binding]
    public sealed class CalculatorStepDefinitions
    {
		// For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

		private int _firstNumber;
		private int _secondNumber;
		private int _result;
		[Given("the first number is (.*)")]
        public void GivenTheFirstNumberIs(int number)
        {
		
			_firstNumber = number;

        }

        [Given("the second number is (.*)")]
        public void GivenTheSecondNumberIs(int number)
        {
			//TODO: implement arrange (precondition) logic

			_secondNumber = number;
		}

        [When("the two numbers are added")]
        public void WhenTheTwoNumbersAreAdded()
        {
			//TODO: implement act (action) logic

			_result = _firstNumber + _secondNumber;
		}

        [Then("the result should be (.*)")]
        public void ThenTheResultShouldBe(int result)
        {
			//TODO: implement assert (verification) logic
			Assert.Equal(result, _result);
		}
    }
}